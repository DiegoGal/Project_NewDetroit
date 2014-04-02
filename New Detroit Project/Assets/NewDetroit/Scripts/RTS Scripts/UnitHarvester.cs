using UnityEngine;
using System.Collections;

public class UnitHarvester : UnitController
{
    public int attackPower = 1;

    // capacidad de transporte por viaje que la unidad es capaz de cargar
    public int harvestCapacity = 10;

    // cantidad de recursos almacenados en la unidad
    private int resourcesLoaded = 0;

    // tiempo en segundos que la unidad tarda en realizar una recolección
    public float harvestTime = 1.0f;
    private float actualHarvestTime = 0.0f;

    // cantidad de recurso por unidad de recolección
    public int amountOfResourcesPerHarvest = 1;

    // capacidad total de recursos recolectados por la unidad
    private int totalHarvest = 0;

	// referencia al pack de minerales
	public GameObject mineralPack;
    private GameObject actualMineralPack;

    // dummys donde se instanciará el pack de minerales y otros objetos
    public Transform dummyMineralPack;
    public Transform dummyHand;
    public Transform dummyGlasses;
    public Transform dummyHead;
    public Transform dummyBackPack;

    // references to the assets the unit can have
    private GameObject helmet;
    private GameObject backpack;
    private GameObject glasses;

    private enum HarvestState
    {
        None,
        GoingToMine,
        Waiting, // espera hasta que halla hueco en la mina
        GoingToChopPosition,
        Choping, // picando
        ReturningToBase,
        GoingToHealUnit,
        Healing  // curando a una unidad
    }
    private HarvestState currentHarvestState = HarvestState.None;
    private HarvestState nextHarvestState = HarvestState.None;

    // referencia a la mina que se está cosechando
    private Transform currentMine;

    private Vector3 lastHarvestPos;
    private int lastHarvestIndex;

    // última posición a donde se va a dejar los recursos
    // es el punto más cercano de la base a la mina de recursos actual
    private Vector3 lastBasePos = new Vector3();

    // referencia a la unidad que se está curando
    private ControllableCharacter currentCharacterHealed;

    // distancia mínima a la que la unidad es capaz de curar
    public float minDistanceToHeal = 5.0f;

    // tiempo en segundos que la unidad tarda en realizar una curación
    public float healTime = 1.0f;
    private float actualHealTime = 0.0f;

    // cantidad de curación por unidad de curación (healTime)
    public int amountOfLifePerHeal = 5;

    // capacidad total de vida curada por la unidad
    private int totalHealed = 0;

    public override void Awake ()
    {
        base.Awake();

        // Por si no se han establecido las referencias a los dummys del modelo
        // en el editor de Unity las buscamos ahora:
        if (dummyMineralPack == null)
            dummyMineralPack = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Dummy Mineral");
        if (dummyHand == null)
            dummyHand = transform.FindChild("Bip001 L Hand/Dummy Pico");
        if (dummyGlasses == null)
            dummyGlasses = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head/Dummy Gafas");
        if (dummyHead == null)
            dummyHead = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head/Dummy Sombrero");
        if (dummyBackPack == null)
            dummyBackPack = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Dummy Espalda");
    }

    public override void Start ()
    {
        base.Start();

        basicAttackPower = secondaryAttackPower = attackPower;

        // instanciamos un casco o un cono encima de la cabeza
        if (Random.value <= 0.5f)
            helmet = (GameObject)Instantiate
            (
                Resources.Load("Goblin Army/GoblinHarvesterHelmet"),
                dummyHead.transform.position,
                dummyHead.transform.rotation
            );
        else
            helmet = (GameObject)Instantiate
            (
                Resources.Load("Goblin Army/GoblinHarvesterCone"),
                dummyHead.transform.position,
                dummyHead.transform.rotation
            );
        helmet.transform.Rotate(90.0f, 0.0f, 0.0f);
        helmet.transform.parent = dummyHead;
        // instanciamos aleatóriamente una mochila detrás
        if (Random.value <= 0.5f)
        {
            backpack = (GameObject)Instantiate
            (
                Resources.Load("Goblin Army/GoblinHarvesterBackpack"),
                dummyBackPack.transform.position,
                new Quaternion()
            );
            backpack.transform.parent = dummyBackPack;
        }
        // instanciamos unas gafas
        glasses = (GameObject)Instantiate
        (
            Resources.Load("Goblin Army/GoblinHarvesterGlasses"),
            dummyGlasses.transform.position,
            new Quaternion()
            //dummyGlasses.transform.rotation
        );
        glasses.transform.parent = dummyGlasses;
        glasses.transform.Rotate(new Vector3(0.0f, transform.rotation.y, 0.0f));

        /*GameObject newPack = Instantiate
        (
            mineralPack,
            dummyPack.transform.position,
            new Quaternion()
        ) as GameObject;
        newPack.transform.name = "MineralPack";
        newPack.transform.parent = dummyPack;
        newPack.transform.Rotate(new Vector3(180.0f, 180.0f, 180.0f));*/
    }
    
    public override void Update ()
    {
        switch (currentHarvestState)
        {
            case HarvestState.None:
                base.Update();
                break;
            case HarvestState.GoingToMine:
                // si la distancia a la mina es menor que la distanceToWait preguntamos si hay hueco
				float distMine = Vector3.Distance(transform.position, currentMine.position);
				float distToWait = currentMine.GetComponent<CResources>().distanceToWait;
				if (distMine < distToWait)
                {
                    if ( currentMine.GetComponent<CResources>().GetHarvestPosition(
                            ref lastHarvestPos,
                            ref lastHarvestIndex,
                            this)
                        )
                    {
                        // hay hueco y tenemos la posicion
                        currentHarvestState = HarvestState.GoingToChopPosition;
                        base.GoTo(lastHarvestPos);
                    }
                    else
                    {
                        currentHarvestState = HarvestState.Waiting;
                        GetComponent<NavMeshAgent>().destination = transform.position;
                        //animation.CrossFade("Idle Wait");
                        animation.CrossFadeQueued("Idle01");
                    }
                }
                else
                    base.Update();
                break;
            case HarvestState.Waiting:

                break;
            case HarvestState.GoingToChopPosition:
                //if (Vector3.Distance(transform.position, lastHarvestPos) < destinyThreshold)
                if (currentState == State.Idle)
                {
                    // ha llegado a la posición de extracción
                    StartChoping();
                }
                else
                    base.Update();
                break;
            case HarvestState.Choping:
                actualHarvestTime += Time.deltaTime;
                if (actualHarvestTime >= harvestTime)
                {
                    resourcesLoaded +=
                        currentMine.GetComponent<CResources>().GetResources(amountOfResourcesPerHarvest);
                    //Debug.Log("Chop! " + resourcesLoaded);
                    if (resourcesLoaded >= harvestCapacity)
                    {
                        // la unidad se ha "llenado"
                        Debug.Log("Estoy lleno, vamos pa la base");
                        currentMine.GetComponent<CResources>().LeaveHarvestPosition(lastHarvestIndex);
                        currentHarvestState = HarvestState.ReturningToBase;
					    nextHarvestState = HarvestState.GoingToMine;

						// intanciamos un pack de minerales encima de la unidad
                        //Debug.Log("Dummy position: " + dummyMineralPack.transform.position);
                        actualMineralPack = Instantiate
                        (
                            mineralPack,
                            dummyMineralPack.transform.position,
                            new Quaternion()
                        ) as GameObject;
                        actualMineralPack.transform.name = "MineralPack";
                        actualMineralPack.transform.parent = dummyMineralPack;
                        actualMineralPack.transform.Rotate(new Vector3(180.0f, 180.0f, 180.0f));

                        CResourceBuilding resourceBuilding = baseController.GetArmyController().GetResourceBuilding(currentMine.GetComponent<CResources>());
                        float radious = resourceBuilding.GetRadious();
                        float alpha = Mathf.Atan2((currentMine.transform.position.z - resourceBuilding.transform.position.z),
                           (currentMine.transform.position.x - resourceBuilding.transform.position.x));
                        lastBasePos.x = resourceBuilding.transform.position.x + (Mathf.Cos(alpha) * radious);
                        lastBasePos.z = resourceBuilding.transform.position.z + (Mathf.Sin(alpha) * radious);

                        GoTo(lastBasePos);
                    }
                    actualHarvestTime = 0;
                }
                break;
            case HarvestState.ReturningToBase:
                if (currentState == State.Idle)
                    ArrivedToBase();
                else
                    base.Update();
                break;
            case HarvestState.GoingToHealUnit:
                float distItem = Vector3.Distance(transform.position, currentCharacterHealed.transform.position);
                //base.GoTo(currentItem.position);
                if (distItem <= minDistanceToHeal)
                {
                    currentHarvestState = HarvestState.Healing;

                    animation.CrossFade("Heal");
                }
                // si el currentState es Idle significa que la unidad ya ha llegado al herido
                if (currentState == State.Idle)
                {

                }
                else
                    base.Update();
                break;
            case HarvestState.Healing:
                /*actualHealTime += Time.deltaTime;
                distItem = Vector3.Distance(transform.position, currentItem.position);
                bool healed = false;
                if (distItem < 4.0f)
                {
                    base.GoTo(transform.position);
                }
                else if (distItem > 8.0f)
                {
                    currentHarvestState = HarvestState.None;
                    nextHarvestState = HarvestState.None;
                }
                if (actualHealTime >= healTime)
                {
                    healed = currentItem.GetComponent<ControllableCharacter>().Heal(amountOfLifePerHeal);
                    // The item has been repaired
                    if (healed)
                    {
                        Debug.Log("Unidad curada");
                        currentHarvestState = HarvestState.None;
                        nextHarvestState = HarvestState.None;
                    }
                    actualHealTime = 0;
                }*/
                break;
        }
    } // Update

    public override void OnGUI ()
    {
        if (currentState != State.AscendingToHeaven)
        {
            base.OnGUI();

            GUI.skin.label.fontSize = 10;

            GUI.Label(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 45, 100, 50),
                currentState.ToString());
            GUI.Label(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 55, 100, 50),
                currentHarvestState.ToString());
            GUI.Label(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 65, 100, 50),
                "resources: " + resourcesLoaded);
        }
    }

    public override void RightClickOnSelected (Vector3 destiny, Transform destTransform)
    {
        if (destTransform.name == "WorldFloor")
        {
            //Debug.Log("ojo suelo!");
            if (currentHarvestState == HarvestState.GoingToChopPosition ||
                currentHarvestState == HarvestState.Choping)
                currentMine.GetComponent<CResources>().LeaveHarvestPosition(lastHarvestIndex);
            else if (currentHarvestState == HarvestState.Waiting)
                currentMine.GetComponent<CResources>().LeaveQueue(this);
            nextHarvestState = HarvestState.None;
            currentHarvestState = HarvestState.None;
            base.RightClickOnSelected(destiny, destTransform);
        }
        else if ((destTransform.name == "Resources Mine") || (destTransform.name == "Metro") )
        {
            baseController.GetArmyController().UpdateMines(destTransform);
            // actualizar la referencia de la última mina seleccionada
            currentMine = destTransform;
            // actualizar la posición de la base o del almacén donde se dejarán los recursos
            CResourceBuilding resourceBuilding = baseController.GetArmyController().GetResourceBuilding(destTransform.GetComponent<CResources>());
            float radius = resourceBuilding.GetRadious();
            float alpha = Mathf.Atan2((currentMine.transform.position.z - resourceBuilding.transform.position.z),
                (currentMine.transform.position.x - resourceBuilding.transform.position.x));
            lastBasePos.x = resourceBuilding.transform.position.x + (Mathf.Cos(alpha) * radius);
            lastBasePos.z = resourceBuilding.transform.position.z + (Mathf.Sin(alpha) * radius);

            /*GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = lastBasePos;
            cube.renderer.material.color = Color.red;
            cube.transform.parent = baseController.transform;*/

            Debug.Log("vamos pa la mina!");
            if (currentHarvestState == HarvestState.None)
            {
                // actualizar el estado de cosecha
                if (resourcesLoaded == harvestCapacity)
                {
                    // si la unidad ya esta llena de recursos, vuelve a la base para dejarlos
                    currentHarvestState = HarvestState.ReturningToBase;
                    GoTo(lastBasePos);
                    nextHarvestState = HarvestState.None;
                }
                else
                {
                    // todavía tiene capacidad para más recursos, se le envía a la mina
                    currentHarvestState = HarvestState.GoingToMine;
                    GoTo(destiny);
                    nextHarvestState = HarvestState.Choping;
                }
            }
            else if (currentHarvestState == HarvestState.ReturningToBase)
            {
                // actualizar el estado de cosecha
                if (resourcesLoaded == harvestCapacity)
                {
                    // si la unidad ya esta llena de recursos, vuelve a la base para dejarlos
                    currentHarvestState = HarvestState.ReturningToBase;
                    GoTo(lastBasePos);
                    nextHarvestState = HarvestState.GoingToMine;
                }
                else
                {
                    // todavía tiene capacidad para más recursos, se le envía a la mina
                    currentHarvestState = HarvestState.GoingToMine;
                    GoTo(destiny);
                    nextHarvestState = HarvestState.Choping;
                }
            }
        }
        else if ((destTransform.name == "Army Base") || (destTransform.name == "The Stinky Squid"))
        {
            // vuelve a la base, si tiene recursos los deja
            if (resourcesLoaded > 0)
            {
                Debug.Log("vuelta a la base");
                currentHarvestState = HarvestState.ReturningToBase;
                GoTo(lastBasePos);
            }
            else
            {
                currentHarvestState = HarvestState.None;
                GoTo(basePosition);
            }
            nextHarvestState = HarvestState.None;
        }
        else
            base.RightClickOnSelected(destiny, destTransform);
    } // RightClickOnSelected

    public void FinishWaiting (Vector3 chopPosition, int chopIndex)
    {
        lastHarvestPos = chopPosition;
        lastHarvestIndex = chopIndex;
        currentHarvestState = HarvestState.GoingToChopPosition;
        base.GoTo(lastHarvestPos);
    }

    public void StartChoping ()
    {
        // cuando llegue a la mina pasar el estado a Choping
        Debug.Log("comenzando cosecha...");
        animation.CrossFade("Picar");
        currentHarvestState = HarvestState.Choping;
        nextHarvestState = HarvestState.ReturningToBase;
    }

    public override void ArrivedToBase ()
    {
        if (currentHarvestState == HarvestState.ReturningToBase)
        {
            Debug.Log("dejando la cosecha en la base...");
            baseController.IncreaseResources(resourcesLoaded);
            totalHarvest += resourcesLoaded;
            resourcesLoaded = 0;

			// eliminamos el pack de minerales
            if (actualMineralPack)
            {
                Destroy(actualMineralPack);
                actualMineralPack = null;
            }

            if (nextHarvestState == HarvestState.GoingToMine)
            {
                // si estaba cosechando, volvemos a la mina
                Debug.Log("volvemos a la mina");
                currentHarvestState = HarvestState.GoingToMine;
                GoTo(currentMine.position);
                nextHarvestState = HarvestState.Choping;
            }
            else
                PlayAnimationCrossFade("Idle01");
        }
    }

    protected override void PlayAnimationCrossFade (string animationName)
    {
        // si la unidad esta cargada de minerales cambian algunas animaciones
        if (actualMineralPack)
        {
            if (animationName == "Walk")
                animation.CrossFade("Walk Loaded");
            else if (animationName == "Idle01")
                animation.CrossFade("Idle Loaded");
            else if (animationName == "Idle Wait")
                animation.CrossFade("Idle01");
            else
                base.PlayAnimationCrossFade(animationName);
        }
        else
            base.PlayAnimationCrossFade(animationName);
    }

    protected override void PlayAnimationCrossFadeQueued (string animationName)
    {
        // si la unidad esta cargada de minerales cambian algunas animaciones
        if (actualMineralPack)
        {
            if (animationName == "Walk")
                animation.CrossFadeQueued("Walk Loaded");
            else if (animationName == "Idle01")
                animation.CrossFadeQueued("Idle Loaded");
            else if (animationName == "Idle Wait")
                animation.CrossFadeQueued("Idle01");
            else
                base.PlayAnimationCrossFadeQueued(animationName);
        }
        else
            base.PlayAnimationCrossFadeQueued(animationName);
    }

    public override int GetUnitType()
    {
        return 0;
    }

    protected override void RemoveAssetsFromModel ()
    {
        if (helmet)
            Destroy(helmet);
        if (backpack)
            Destroy(backpack);
        if (glasses)
            Destroy(glasses);
        if (actualMineralPack)
            Destroy(actualMineralPack);
    }

} // class UnitHarvester