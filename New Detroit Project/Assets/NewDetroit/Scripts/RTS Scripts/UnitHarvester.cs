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
    public int harvestTime = 1;
    private float actualHarvestTime = 0;

    // cantidad de recurso por unidad de recolección
    public int amountOfResourcesPerHarvest = 1;

    // capacidad total de recursos recolectados por la unidad
    private int totalHarvest = 0;

	// referencia al pack de minerales
	public GameObject mineralPack;

    // dummy donde se instanciará el pack de minerales
    public Transform dummyMineralPack;
    public Transform dummyHand;
    public Transform dummyGlasses;
    public Transform dummyHead;
    public Transform dummyBackPack;

    private enum HarvestState
    {
        None,
        GoingToMine,
        Waiting, // espera hasta que halla hueco en la mina
        GoingToChopPosition,
        Choping, // picando
        ReturningToBase
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
        GameObject helmet;
        if (Random.value <= 0.5f)
            helmet = (GameObject)Instantiate
            (
                Resources.Load("GoblinHarvesterHelmet"),
                dummyHead.transform.position,
                dummyHead.transform.rotation
            );
        else
            helmet = (GameObject)Instantiate
            (
                Resources.Load("GoblinHarvesterCone"),
                dummyHead.transform.position,
                dummyHead.transform.rotation
            );
        helmet.transform.Rotate(90.0f, 0.0f, 0.0f);
        helmet.transform.parent = dummyHead;
        if (Random.value <= 0.5f)
        {
            GameObject backpack = (GameObject)Instantiate
            (
                Resources.Load("GoblinHarvesterBackpack"),
                dummyBackPack.transform.position,
                new Quaternion()
            );
            backpack.transform.parent = dummyBackPack;
        }

        // instanciamos aleatóriamente una mochila detrás
        
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
                            this) )
                    {
                        // hay hueco y tenemos la posicion
                        currentHarvestState = HarvestState.GoingToChopPosition;
                        base.GoTo(lastHarvestPos);
                    }
                    else
                    {
                        currentHarvestState = HarvestState.Waiting;
                        GetComponent<NavMeshAgent>().destination = transform.position;
                        animation.CrossFade("Idle Wait");
                        animation.CrossFadeQueued("Idle01");
                        //animation.PlayQueued("Iddle01");
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
                        Debug.Log("Dummy position: " + dummyMineralPack.transform.position);
                        GameObject newPack = Instantiate
                        (
                            mineralPack,
                            dummyMineralPack.transform.position,
                            new Quaternion()
                        ) as GameObject;
                        newPack.transform.name = "MineralPack";
                        newPack.transform.parent = dummyMineralPack;
                        newPack.transform.Rotate(new Vector3(180.0f, 180.0f, 180.0f));

                        GoTo(lastBasePos);
                        animation.CrossFade("Walk Carga");
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
        }
    } // Update

    public override void OnGUI ()
    {
        base.OnGUI();

        Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);

		GUI.skin.label.fontSize = 10;
        
		/*GUI.Label(new Rect(camPos.x - 10, Screen.height - camPos.y - 45, 100, 50),
		    currentState.ToString());
        GUI.Label(new Rect(camPos.x - 10, Screen.height - camPos.y - 55, 100, 50),
            currentHarvestState.ToString());
        GUI.Label(new Rect(camPos.x - 10, Screen.height - camPos.y - 65, 100, 50),
            "resources: " + resourcesLoaded);*/
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
            // actualizar la referencia de la última mina seleccionada
            currentMine = destTransform;
            // actualizar la posición de la base donde se dejarán los recursos
            float alpha = Mathf.Atan((currentMine.transform.position.x - basePosition.x) /
                (currentMine.transform.position.z - basePosition.z));
            //lastBasePos.x = basePosition.x - (Mathf.Sin(alpha) * radius);
            //lastBasePos.z = basePosition.z - (Mathf.Cos(alpha) * radius);
            lastBasePos.x = baseController.transform.position.x + (Mathf.Cos(alpha) * baseController.GetRadious());
            lastBasePos.z = baseController.transform.position.z + (Mathf.Sin(alpha) * baseController.GetRadious());

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
            baseController.DownloadResources(resourcesLoaded);
            totalHarvest += resourcesLoaded;
            resourcesLoaded = 0;

			// eliminamos el pack de minerales
            Transform pack = dummyMineralPack.transform.FindChild("MineralPack");
            if (pack != null)
                GameObject.Destroy(pack.gameObject);

			if (nextHarvestState == HarvestState.GoingToMine)
            {
                // si estaba cosechando, volvemos a la mina
                Debug.Log("volvemos a la mina");;
                currentHarvestState = HarvestState.GoingToMine;
                GoTo(currentMine.position);
				nextHarvestState = HarvestState.Choping;
            }
        }
    }

} // class UnitHarvester