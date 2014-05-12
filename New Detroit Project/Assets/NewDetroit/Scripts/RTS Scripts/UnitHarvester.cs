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
    protected GameObject actualMineralPack;
    public bool loaded = false;

    // dummys donde se instanciará el pack de minerales y otros objetos
    public Transform dummyMineralPack;
    public Transform dummyHand;
    public Transform dummyGlasses;
    public Transform dummyHead;
    public Transform dummyBackPack;

    // references to the assets the unit can have
    public GameObject peak;
    protected GameObject helmet;
    protected GameObject backpack;
    protected GameObject glasses;

    public enum HarvestState
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
    public HarvestState currentHarvestState = HarvestState.None;
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
    public float minDistanceToHeal = 2.0f;

    // tiempo en segundos que la unidad tarda en realizar una curación
    public float healTime = 1.0f;
    private float actualHealTime = 0.0f;

    // cantidad de curación por unidad de curación (healTime)
    public int amountOfLifePerHeal = 4;

    // capacidad total de vida curada por la unidad
    private int totalHealed = 0;

    public override void Start ()
    {
        base.Start();

        basicAttackPower = secondaryAttackPower = attackPower;

    }
    
    /*public override void Update ()
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

						// se muestra el pack de minerales
                        loaded = true;
                        ShowMineralPack(loaded);

                        CResourceBuilding resourceBuilding = baseController.GetArmyController().GetResourceBuilding(currentMine.GetComponent<CResources>());
                        float radious = resourceBuilding.GetRadius();
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
                float distItem = Vector3.Distance(transform.position, currentCharacterHealed.transform.position)
                    - currentCharacterHealed.GetRadius();
                if (distItem <= minDistanceToHeal)
                {
                    // se ha llegado al compañero a curar, se cambia el estado:
                    currentHarvestState = HarvestState.Healing;
                    // paramos el GoTo
                    StopMoving();
                    // escondemos el pico
                    peak.renderer.enabled = false;
                    // se reproduce la animación de curar
                    animation.CrossFade("Heal");
                }
                else
                    base.Update();
                break;

            case HarvestState.Healing:
                actualHealTime += Time.deltaTime;
                distItem = Vector3.Distance(transform.position, currentCharacterHealed.transform.position)
                    - currentCharacterHealed.GetRadius();

                if ( (actualHealTime >= healTime) &&
                     (currentCharacterHealed.getLife() < currentCharacterHealed.GetMaximunLife() ) )
                {
                    // primero se comprueba que la unidad a curar siga estando "a tiro"
                    if (distItem <= minDistanceToHeal + 1.0f)
                    {
                        transform.LookAt(currentCharacterHealed.transform);
                        if (currentCharacterHealed.Heal(amountOfLifePerHeal))
                        {
                            // el compañero ya ha sido curado
                            Debug.Log("Unidad curada");
                            currentHarvestState = HarvestState.None;
                            nextHarvestState = HarvestState.None;

                            currentCharacterHealed = null;

                            // se vuelve a mostrar el pico
                            peak.renderer.enabled = true;
                        }

                        // se actualiza el contador de "curación" propio
                        totalHealed += amountOfLifePerHeal;

                        actualHealTime = 0.0f;
                    }
                    else if (distItem < minDistanceToHeal * 2.0f)
                    {
                        // la unidad a curar se ha alejado pero todavía esta a "vista"
                        // cambia el estado para dirigirse a la unidad a curar
                        currentHarvestState = HarvestState.GoingToHealUnit;
                        GoTo(currentCharacterHealed.transform.position);
                    }
                    else
                    {
                        // la unidad a curar ha salido y no está a vista
                        StopMoving();
                        currentHarvestState = HarvestState.None;
                        nextHarvestState = HarvestState.None;

                        // se vuelve a mostrar el pico
                        peak.renderer.enabled = true;
                    }
                }               
                break;
        }
    } // Update*/

    protected override void UpdateIdle ()
    {
        base.UpdateIdle();

        switch (currentHarvestState)
        {
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

                        // se muestra el pack de minerales
                        loaded = true;
                        ShowMineralPack(loaded);

                        CResourceBuilding resourceBuilding = baseController.GetArmyController().GetResourceBuilding(currentMine.GetComponent<CResources>());
                        float radious = resourceBuilding.GetRadius();
                        float alpha = Mathf.Atan2((currentMine.transform.position.z - resourceBuilding.transform.position.z),
                           (currentMine.transform.position.x - resourceBuilding.transform.position.x));
                        lastBasePos.x = resourceBuilding.transform.position.x + (Mathf.Cos(alpha) * radious);
                        lastBasePos.z = resourceBuilding.transform.position.z + (Mathf.Sin(alpha) * radious);

                        /*GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = lastBasePos;
                        cube.renderer.material.color = Color.red;*/

                        GoTo(lastBasePos);
                    }
                    actualHarvestTime = 0;
                }
                break;

            case HarvestState.Healing:
                // first check if the unit to heal is still in the game
                if (currentCharacterHealed && currentCharacterHealed.IsAlive() )
                {
                    actualHealTime += Time.deltaTime;
                    float distItem = Vector3.Distance
                    (
                        transform.position,
                        currentCharacterHealed.transform.position
                    ) - currentCharacterHealed.GetRadius();

                    if ((actualHealTime >= healTime) &&
                         (currentCharacterHealed.getLife() < currentCharacterHealed.GetMaximunLife()))
                    {
                        // primero se comprueba que la unidad a curar siga estando "a tiro"
                        if (distItem <= minDistanceToHeal + 1.0f)
                        {
                            transform.LookAt(currentCharacterHealed.transform);
                            if (currentCharacterHealed.Heal(amountOfLifePerHeal))
                            {
                                // el compañero ya ha sido curado
                                Debug.Log("Unidad curada");
                                currentHarvestState = HarvestState.None;
                                nextHarvestState = HarvestState.None;

                                currentCharacterHealed = null;

                                // se vuelve a mostrar el pico
                                peak.renderer.enabled = true;
                            }

                            // se actualiza el contador de "curación" propio
                            totalHealed += amountOfLifePerHeal;

                            actualHealTime = 0.0f;
                        }
                        else if (distItem < visionSphereRadius)
                        {
                            // la unidad a curar se ha alejado pero todavía esta a "vista"
                            // cambia el estado para dirigirse a la unidad a curar
                            currentHarvestState = HarvestState.GoingToHealUnit;
                            GoTo(currentCharacterHealed.transform.position);
                        }
                        else
                        {
                            // la unidad a curar ha salido y no está a vista
                            StopMoving();
                            currentHarvestState = nextHarvestState = HarvestState.None;

                            // se vuelve a mostrar el pico
                            peak.renderer.enabled = true;
                        }
                    }
                }
                else
                {
                    currentHarvestState = nextHarvestState = HarvestState.None;

                    currentCharacterHealed = null;

                    // se vuelve a mostrar el pico
                    peak.renderer.enabled = true;
                }
                break;
        }

    } // UpdateIdle

    protected override void UpdateGoingTo ()
    {
        base.UpdateGoingTo();

        switch (currentHarvestState)
        {
            case HarvestState.GoingToMine:
                // si la distancia a la mina es menor que la distanceToWait preguntamos si hay hueco
                float distMine = Vector3.Distance(transform.position, currentMine.position);
                float distToWait = currentMine.GetComponent<CResources>().distanceToWait;
                if (distMine < distToWait)
                {
                    if (currentMine.GetComponent<CResources>().GetHarvestPosition(
                            ref lastHarvestPos,
                            ref lastHarvestIndex,
                            this)
                        )
                    {
                        // hay hueco y tenemos la posicion
                        currentHarvestState = HarvestState.GoingToChopPosition;
                        nextHarvestState = HarvestState.Choping;
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
                break;

            case HarvestState.GoingToChopPosition:
                if (currentState == State.Idle)
                    StartChoping();
                break;

            case HarvestState.ReturningToBase:
                if (currentState == State.Idle)
                    ArrivedToBase();
                break;

            case HarvestState.GoingToHealUnit:
                // first check if the unit to heal is still in the game
                if (currentCharacterHealed && currentCharacterHealed.IsAlive() )
                {
                    float distItem = Vector3.Distance
                    (
                        transform.position,
                        currentCharacterHealed.transform.position
                    ) - currentCharacterHealed.GetRadius();
                    // se ha llegado al sitio donde estaba el compañero
                    if (distItem <= minDistanceToHeal)
                    {
                        // se ha llegado al compañero a curar, se cambia el estado:
                        currentHarvestState = HarvestState.Healing;
                        // paramos el GoTo
                        StopMoving();
                        // escondemos el pico
                        peak.renderer.enabled = false;
                        // se reproduce la animación de curar
                        animation.CrossFade("Heal");
                    }
                    else if (distItem <= visionSphereRadius)
                        GoTo(currentCharacterHealed.transform.position);
                }
                else
                {
                    currentCharacterHealed = null;
                    currentHarvestState = nextHarvestState = HarvestState.None;
                }
                break;
        }

    } // UpdateGoingTo

    /*public override void OnGUI ()
    {
        if (currentState != State.AscendingToHeaven)
        {
            base.OnGUI();

            GUI.skin.label.fontSize = 10;

            GUI.Label(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 65, 200, 50),
                currentState.ToString() + "\n" +
                currentHarvestState.ToString() + " -> " + nextHarvestState.ToString() + "\n" +
                "resources: " + resourcesLoaded );
        }
    }*/

    /*public override void RightClickOnSelected (Vector3 destiny, Transform destTransform)
    {
        if (destTransform.name == "WorldFloor")
        {
            //Debug.Log("ojo suelo!");
            if (currentHarvestState == HarvestState.GoingToChopPosition ||
                currentHarvestState == HarvestState.Choping)
                currentMine.GetComponent<CResources>().LeaveHarvestPosition(lastHarvestIndex);
            else if (currentHarvestState == HarvestState.Waiting)
                currentMine.GetComponent<CResources>().LeaveQueue(this);
            else if (currentHarvestState == HarvestState.Healing)
                peak.renderer.enabled = true;

            nextHarvestState = HarvestState.None;
            currentHarvestState = HarvestState.None;
        }
        else if ( (destTransform.name == "Resources Mine") || (destTransform.name == "Metro") )
        {
            peak.renderer.enabled = true;

            baseController.GetArmyController().UpdateMines(destTransform);

            // actualizar la referencia de la última mina seleccionada
            currentMine = destTransform;

            // actualizar la posición de la base o del almacén donde se dejarán los recursos
            CResourceBuilding resourceBuilding = baseController.GetArmyController().GetResourceBuilding(destTransform.GetComponent<CResources>());
            float radius = resourceBuilding.GetRadius();
            float alpha = Mathf.Atan2((currentMine.transform.position.z - resourceBuilding.transform.position.z),
                (currentMine.transform.position.x - resourceBuilding.transform.position.x));
            lastBasePos.x = resourceBuilding.transform.position.x + (Mathf.Cos(alpha) * radius);
            lastBasePos.z = resourceBuilding.transform.position.z + (Mathf.Sin(alpha) * radius);

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = lastBasePos;
            cube.renderer.material.color = Color.red;
            cube.transform.parent = baseController.transform;

            Debug.Log("vamos pa la mina!");
            if (currentHarvestState == HarvestState.None)
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
        else if ( (destTransform.name == "Army Base") || (destTransform.name == "The Stinky Squid") )
        {
            peak.renderer.enabled = true;
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
        else if (destTransform.GetComponent<ControllableCharacter>())
        {
            // se ha hecho click derecho en una unidad
            ControllableCharacter unit = destTransform.GetComponent<ControllableCharacter>();
            // se comprueba que la unidad sea del mismo equipo y que no tenga la vida máxima
            if ( (unit.teamNumber == teamNumber) && (unit.getLife() < unit.GetMaximunLife()) )
            {
                // la unidad es del mismo equipo
                Debug.Log("¡A curar!");
                // se actualiza la referencia a la unidad que se está curando
                currentCharacterHealed = unit;
                // cambia el estado para dirigirse a la unidad a curar
                currentHarvestState = HarvestState.GoingToHealUnit;
                GoTo(destiny);
            }
        }
            base.RightClickOnSelected(destiny, destTransform);
    } // RightClickOnSelected*/

    public override void RightClickOnSelected (Vector3 destiny, Transform destTransform)
    {
        base.RightClickOnSelected(destiny, destTransform);

        if (destTransform.name == "WorldFloor" || destTransform.name == "Terrain")
        {
            if (currentHarvestState == HarvestState.GoingToChopPosition ||
                currentHarvestState == HarvestState.Choping)
                currentMine.GetComponent<CResources>().LeaveHarvestPosition(lastHarvestIndex);
            else if (currentHarvestState == HarvestState.Waiting)
                currentMine.GetComponent<CResources>().LeaveQueue(this);
            else if (currentHarvestState == HarvestState.Healing)
                peak.renderer.enabled = true;

            nextHarvestState = HarvestState.None;
            currentHarvestState = HarvestState.None;
        }
        else if ( (destTransform.name == "Resources Mine") || (destTransform.name == "Metro") )
        {
            baseController.GetArmyController().UpdateMines(destTransform);

            // actualizar la referencia de la última mina seleccionada
            currentMine = destTransform;

            // actualizar la posición de la base o del almacén donde se dejarán los recursos
            CResourceBuilding resourceBuilding = baseController.GetArmyController().GetResourceBuilding(destTransform.GetComponent<CResources>());
            float radius = resourceBuilding.GetRadius();
            float alpha = Mathf.Atan2((currentMine.transform.position.z - resourceBuilding.transform.position.z),
                (currentMine.transform.position.x - resourceBuilding.transform.position.x));
            lastBasePos.x = resourceBuilding.transform.position.x + (Mathf.Cos(alpha) * radius);
            lastBasePos.z = resourceBuilding.transform.position.z + (Mathf.Sin(alpha) * radius);

            /*GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = lastBasePos;
            cube.renderer.material.color = Color.red;
            cube.transform.parent = baseController.transform;*/

            if (currentHarvestState == HarvestState.None)
            {
                // actualizar el estado de cosecha
                if (resourcesLoaded == harvestCapacity)
                {
                    // si la unidad ya esta llena de recursos, vuelve a la base para dejarlos
                    Debug.Log("primero voy a dejar los recursos que tengo, y despues pa la mina!");
                    currentHarvestState = HarvestState.ReturningToBase;
                    GoTo(lastBasePos);
                    nextHarvestState = HarvestState.GoingToMine;
                }
                else
                {
                    // todavía tiene capacidad para más recursos, se le envía a la mina
                    Debug.Log("vamos pa la mina!");
                    currentHarvestState = HarvestState.GoingToMine;
                    GoTo(destiny);
                    nextHarvestState = HarvestState.GoingToChopPosition;
                }
            }
            else if (currentHarvestState == HarvestState.ReturningToBase)
            {
                // actualizar el estado de cosecha
                if (resourcesLoaded == harvestCapacity)
                {
                    // si la unidad ya esta llena de recursos, vuelve a la base para dejarlos
                    //currentHarvestState = HarvestState.ReturningToBase;
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
            if (currentHarvestState == HarvestState.Healing)
                peak.renderer.enabled = true;  
        }
        else if ( (destTransform.name == ("Army Base Team" + teamNumber)) || (destTransform.name == "The Stinky Squid") )
        {
             peak.renderer.enabled = true;
             // vuelve a la base, si tiene recursos los deja
             if (resourcesLoaded > 0)
             {
                 Debug.Log("vuelta a la base");
                 currentHarvestState = HarvestState.ReturningToBase;
             }
             else
                 currentHarvestState = HarvestState.None;

             nextHarvestState = HarvestState.None;

             GoTo(lastBasePos);
         }
         else if (destTransform.GetComponent<ControllableCharacter>())
         {
             // se ha hecho click derecho en una unidad
             ControllableCharacter unit = destTransform.GetComponent<ControllableCharacter>();
             // se comprueba que la unidad sea del mismo equipo y que no tenga la vida máxima
             if ( (unit.teamNumber == teamNumber) && (unit.getLife() < unit.GetMaximunLife()) )
             {
                 // la unidad es del mismo equipo
                 Debug.Log("A curar!");
                 // se actualiza la referencia a la unidad que se está curando
                 currentCharacterHealed = unit;
                 // cambia el estado para dirigirse a la unidad a curar
                 currentHarvestState = HarvestState.GoingToHealUnit;
                 nextHarvestState = HarvestState.Healing;
             }
         }
    } // RightClickOnSelected

    public void FinishWaiting (Vector3 chopPosition, int chopIndex)
    {
        lastHarvestPos = chopPosition;
        lastHarvestIndex = chopIndex;
        currentHarvestState = HarvestState.GoingToChopPosition;
        nextHarvestState = HarvestState.Choping;

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

			// escondemos el pack de minerales
            loaded = false;
            ShowMineralPack(false);

            if (nextHarvestState == HarvestState.GoingToMine)
            {
                // si estaba cosechando, volvemos a la mina
                Debug.Log("volvemos a la mina");
                currentHarvestState = HarvestState.GoingToMine;
                GoTo(currentMine.position);
                nextHarvestState = HarvestState.Choping;
            }
            else
            {
                PlayAnimationCrossFade("Idle01");
                currentHarvestState = HarvestState.None;
                nextHarvestState = HarvestState.None;
            }
        }
    }

    // esconde (o muestra) todos los objetos que componen el pack de minerales
    protected void ShowMineralPack (bool enable=true)
    {
        Renderer[] renderers = actualMineralPack.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
            r.enabled = enable;
    }

    public override bool Damage (float damage, char type)
    {
        if ( base.Damage(damage, type) )
        {
            // salir de las colas
            if (currentHarvestState == HarvestState.GoingToChopPosition ||
                currentHarvestState == HarvestState.Choping)
                currentMine.GetComponent<CResources>().LeaveHarvestPosition(lastHarvestIndex);
            else if (currentHarvestState == HarvestState.Waiting)
                currentMine.GetComponent<CResources>().LeaveQueue(this);

            return true;
        }
        else
            return false;
    }

    protected override void PlayAnimationCrossFade (string animationName)
    {
        // si la unidad esta cargada de minerales cambian algunas animaciones
        if (loaded)
        {
            if (animationName == "Walk")
                animation.CrossFade("Walk Loaded");
            else if (animationName == "Idle01")
                animation.CrossFade("Idle Loaded");
            else if (animationName == "Idle Wait")
                animation.CrossFade("Idle Loaded");
            else
                base.PlayAnimationCrossFade(animationName);
        }
        else
            base.PlayAnimationCrossFade(animationName);
    }

    protected override void PlayAnimationCrossFadeQueued (string animationName)
    {
        // si la unidad esta cargada de minerales cambian algunas animaciones
        if (loaded)
        {
            if (animationName == "Walk")
                animation.CrossFadeQueued("Walk Loaded");
            else if (animationName == "Idle01")
                animation.CrossFadeQueued("Idle Loaded");
            else if (animationName == "Idle Wait")
                animation.CrossFadeQueued("Idle Loaded");
            else
                base.PlayAnimationCrossFadeQueued(animationName);
        }
        else
            base.PlayAnimationCrossFadeQueued(animationName);
    }

    protected override void PlayIdleWaitAnimation()
    {
        // solo se reproduce la animación de espera si el estado es None
        if (currentHarvestState == HarvestState.None)
            base.PlayIdleWaitAnimation();
    }

    public override int GetUnitType ()
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
        if (peak)
            Destroy(peak);
    }

} // class UnitHarvester