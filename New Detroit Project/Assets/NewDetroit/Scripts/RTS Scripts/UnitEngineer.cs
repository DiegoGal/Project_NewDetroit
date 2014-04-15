using UnityEngine;
using System.Collections;

public class UnitEngineer : UnitController
{

	public int attackPower = 1;
	
	// tiempo en segundos que la unidad tarda en realizar una construccion, conquista y/o reparacion
	public int engineerTime = 1;
	private float actualEngineerTime = 0;
	
	// cantidad de construccion, conquista y/o reparacion por unidad de recolección
	public int amountPerAction = 2;

    private enum EngineerState
    {
        None,
        GoingToRepairItem,
        GoingToConquerableItem,
        GoingToConstructItem,
        Waiting, // espera hasta que halla hueco en el item
        GoingToConquestPosition,
        GoingToRepairPosition,
		GoingToConstructPosition,
        Repairing, // construyendo, conquistando o reparando
        Conquering,
		Constructing
    }
	private EngineerState currentEngineerState = EngineerState.None;
	
	// referencia al item que se está construyendo, conquistando o reparando
	private Transform currentItem;
	
	private Vector3 lastEngineerPos;
	private int lastEngineerIndex;

    // To instanciate a towerGoblin and warehouse
    public GameObject towerGoblinPrefab;
    public GameObject warehousePrefab;
    // The items towerGoblin and warehouse
    private GameObject towerGoblin;
    private GameObject warehouse;

    private bool newTGConstruct = false;
    private bool newWConstruct = false;
    private Vector3 constructDestiny = new Vector3();

    // dummy where is going to be instanciated a laptop
    public Transform dummyLaptop;
    public Transform dummyHand;

    // reference to the laptop and hammer
    public GameObject laptop;
    public GameObject hammer;

    //For attacking1
    public GameObject fireball;
    private GameObject newFireball;

    public override void Awake ()
    {
        base.Awake();

        // Por si no se han establecido las referencias a los dummys del modelo
        // en el editor de Unity las buscamos ahora:
        if (dummyLaptop == null)
            dummyLaptop = transform.FindChild("Bip001/Bip001 Footsteps");
        if (dummyHand == null)
            dummyHand = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Mano Der");
    }

    public override void Start ()
    {
        base.Start();
        basicAttackPower = secondaryAttackPower = attackPower;
        attackCadenceAux = 2.5f;
        attackCadence = 3.2f;
    }
	
	// Update is called once per frame
	public override void Update () 
	{
        // If this is selected and "C" is pulsed, a towerGoblin has to be instanciate with transparency
        if (Input.anyKeyDown && (newTGConstruct || newWConstruct) && Input.GetMouseButtonDown(0) && 
            (currentEngineerState != EngineerState.GoingToConstructItem) && (currentEngineerState != EngineerState.GoingToConstructPosition) &&
            (currentState != State.Dying) && (currentState != State.AscendingToHeaven))
        {
            newTGConstruct = newWConstruct = false;
            Destroy(towerGoblin);
            Destroy(warehouse);
        }
        if (currentLife <= 0.0f)
        {
            LeaveQueues();
            currentEngineerState = EngineerState.None;

        }
        switch (currentEngineerState)
        {
            case EngineerState.None:
                base.Update();
                break;
            case EngineerState.GoingToRepairItem:
                // if the distance to the item is less than distanceToWait we ask if there is gap
                float distItem = Vector3.Distance(transform.position, currentItem.position);
                float distToWait = currentItem.GetComponent<BuildingController>().distanceToWait;
                if (distItem < 10.0f)
                {
                    if (currentItem.GetComponent<BuildingController>().GetEngineerPosition(
                            ref lastEngineerPos,
                            ref lastEngineerIndex,
                            this))
                    {
                        // there is a gap and we have the position
                        currentEngineerState = EngineerState.GoingToRepairPosition;
                        base.GoTo(lastEngineerPos);
                    }
                    else
                    {
                        currentEngineerState = EngineerState.Waiting;
                        GetComponent<NavMeshAgent>().destination = transform.position;
                    }
                }
                else
                    base.Update();
                break;
            case EngineerState.GoingToConquerableItem:
                // if the distance to the item is less than distanceToWait we ask if there is gap
                distItem = Vector3.Distance(transform.position, currentItem.position);
                distToWait = currentItem.GetComponent<BuildingController>().distanceToWait;
                if (distItem < 10.0f)
                {
                    if (currentItem.GetComponent<BuildingController>().GetEngineerPosition(
                        ref lastEngineerPos,
                        ref lastEngineerIndex,
                        this))
                    {
                        // there is a gap and we have the position
                        currentEngineerState = EngineerState.GoingToConquestPosition;
                        base.GoTo(lastEngineerPos);
                    }
                    else
                    {
                        currentEngineerState = EngineerState.Waiting;
                        GetComponent<NavMeshAgent>().destination = transform.position;
                    }
                }
                else
                    base.Update();
                break;
            case EngineerState.GoingToConstructItem:
                // if the distance to the item is less than distanceToWait we ask if there is gap
                if (currentItem != null)
                {
                    distItem = Vector3.Distance(transform.position, currentItem.position);
                    distToWait = currentItem.GetComponent<BuildingController>().distanceToWait;
                    if (distItem < 10.0f)
                    {
                        if (currentItem.GetComponent<BuildingController>().GetEngineerPosition(
                                ref lastEngineerPos,
                                ref lastEngineerIndex,
                                this))
                        {
                            // there is a gap and we have the position
                            currentEngineerState = EngineerState.GoingToConstructPosition;
                            base.GoTo(lastEngineerPos);
                        }
                        else
                        {
                            currentEngineerState = EngineerState.Waiting;
                            GetComponent<NavMeshAgent>().destination = transform.position;
                        }
                    }
                    else
                        base.Update();
                }
                else
                {
                    currentEngineerState = EngineerState.None;
                    animation.Play("Idle01");
                    newTGConstruct = newWConstruct = false;
                }
                break;
            case EngineerState.Waiting:
                animation.Play("Idle Wait");

                break;
            case EngineerState.GoingToRepairPosition:
                if (currentState == State.Idle)
                {
                    // when it have arrived to the repair position
                    currentEngineerState = EngineerState.Repairing;

                    // We instanciate a Hammer
                    GameObject newHammer = Instantiate
                    (
                        hammer,
                        dummyHand.transform.position,
                        new Quaternion()
                    ) as GameObject;
                    newHammer.transform.name = "Hammer";
                    newHammer.transform.parent = dummyHand;
                    newHammer.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
                }
                else
                    base.Update();
                break;
            case EngineerState.GoingToConquestPosition:
                if (currentState == State.Idle)
                {
                    // when it have arrived to the conquest position
                    currentEngineerState = EngineerState.Conquering;

                    // We instanciate a laptop
                    GameObject newLaptop = Instantiate
                    (
                        laptop,
                        dummyLaptop.transform.position,
                        new Quaternion()
                    ) as GameObject;
                    newLaptop.transform.name = "Laptop";
                    newLaptop.transform.parent = dummyLaptop;
                    newLaptop.transform.rotation = transform.rotation;
                }
                else
                    base.Update();
                break;
            case EngineerState.GoingToConstructPosition:
                if (currentItem != null)
                {
                    if (currentState == State.Idle)
                    {
                        // when it have arrived to the construct position
                        currentEngineerState = EngineerState.Constructing;
                        if (newTGConstruct)
                            Minimap.InsertTower(towerGoblin.GetComponent<Tower>());
                        else if (newWConstruct)
                            Minimap.InsertWarehouse(warehouse.GetComponent<Warehouse>());
                        newTGConstruct = newWConstruct = false;

                        // intanciamos un Hammer
                        GameObject newHammer = Instantiate
                        (
                            hammer,
                            dummyHand.transform.position,
                            new Quaternion()
                        ) as GameObject;
                        newHammer.transform.name = "Hammer";
                        newHammer.transform.parent = dummyHand;
                        newHammer.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
                    }
                    else
                        base.Update();
                }
                else
                {
                    currentEngineerState = EngineerState.None;
                    animation.Play("Idle01");
                    newTGConstruct = newWConstruct = false;
                }
                break;
            case EngineerState.Repairing:
                animation.Play("Build");
                actualEngineerTime += Time.deltaTime;
                bool repaired = false;
                if (actualEngineerTime >= engineerTime)
                {
                    repaired = currentItem.GetComponent<BuildingController>().Repair(amountPerAction);
                    // The item has been repaired
                    if (repaired || currentItem.GetComponent<BuildingController>().HasTotalLife())
                    {
                        Debug.Log("Torre Reparada");
                        currentEngineerState = EngineerState.None;
                        animation.Play("Idle01");

                        // We destroy the Hammer
                        Transform hammer1 = dummyHand.transform.FindChild("Hammer");
                        if (hammer1 != null)
                            GameObject.Destroy(hammer1.gameObject);
                    }
                    actualEngineerTime = 0;
                }
                break;
            case EngineerState.Conquering:
                animation.Play("Capture");
                actualEngineerTime += Time.deltaTime;
                bool conquest = false;
                if (actualEngineerTime >= engineerTime)
                {
                    conquest = currentItem.GetComponent<TowerNeutral>().Conquest(amountPerAction, teamNumber);
                    // The item has been conquered
                    if (conquest || !currentItem.GetComponent<TowerNeutral>().IsCurrentStateNeutral())
                    {
                        Debug.Log("Torre Conquistada!");
                        currentEngineerState = EngineerState.None;
                        animation.Play("Idle01");
                        Minimap.SetTowerNeutral(currentItem.GetComponent<TowerNeutral>());
                        // We destroy the Laptop
                        Transform laptop1 = dummyLaptop.transform.FindChild("Laptop");
                        if (laptop1 != null)
                            GameObject.Destroy(laptop1.gameObject);
                    }
                    actualEngineerTime = 0;
                }
                break;
            case EngineerState.Constructing:
                animation.Play("Build");
                actualEngineerTime += Time.deltaTime;
                bool construct = false;
                if (currentItem.GetComponent<TowerGoblin>() != null)
                {
                    if (actualEngineerTime >= engineerTime)
                    {
                        construct = currentItem.GetComponent<TowerGoblin>().Construct(amountPerAction);
                        // The item has been constructed
                        if (construct)
                        {
                            Debug.Log("Torre construida!");
                            currentEngineerState = EngineerState.None;
                            currentItem.GetComponent<TowerGoblin>().SetActiveMaterial();
                            animation.Play("Idle01");

                            // We destroy the Hammer
                            Transform hammer1 = dummyHand.transform.FindChild("Hammer");
                            if (hammer1 != null)
                                GameObject.Destroy(hammer1.gameObject);
                        }
                        actualEngineerTime = 0;
                    }
                }
                else if (currentItem.GetComponent<Warehouse>() != null)
                {
                    if (actualEngineerTime >= engineerTime)
                    {
                        construct = currentItem.GetComponent<Warehouse>().Construct(amountPerAction);
                        // The item has been constructed
                        if (construct)
                        {
                            Debug.Log("Almacen construido!");
                            currentEngineerState = EngineerState.None;
                            currentItem.GetComponent<Warehouse>().SetActiveMaterial();
                            animation.Play("Idle01");

                            // We destroy the Hammer
                            Transform hammer1 = dummyHand.transform.FindChild("Hammer");
                            if (hammer1 != null)
                                GameObject.Destroy(hammer1.gameObject);

                            baseController.GetArmyController().AddWarehouse(currentItem.GetComponent<CResourceBuilding>());
                        }
                        actualEngineerTime = 0;
                    }
                }
                break;
        } // Switch

	} // Update

    public override void RightClickOnSelected (Vector3 destiny, Transform destTransform)
    {
        // We destroy the Laptop if it exist
        Transform laptop1 = dummyLaptop.transform.FindChild("Laptop");
        if (laptop1 != null)
            GameObject.Destroy(laptop1.gameObject);
        // We destroy the Hammer if it exist
        Transform hammer1 = dummyHand.transform.FindChild("Hammer");
        if (hammer1 != null)
            GameObject.Destroy(hammer1.gameObject);
        destiny.y = 0;

        LeaveQueues();

        // He has to go to another position if he has to
        // destTransform.name == "TowerGoblin" || destTransform.name == "Goblin Warehouse" 
        if (destTransform.name == "WorldFloor")// If he has to go to another position of the worldfloor he goes
        {
            newTGConstruct = newWConstruct = false;
            currentEngineerState = EngineerState.None;

            base.RightClickOnSelected(destiny, destTransform);
            attackCadenceAux = 2.5f;
        }
        else if (destTransform.name == "TowerNeutral")// If he has to go to a TowerNeutral
        {
            attackCadenceAux = 2.5f;
            newTGConstruct = newWConstruct = false;
            currentItem = destTransform;
            if (currentItem.GetComponent<BuildingController>().GetTeamNumber() != teamNumber) // if it's not in the same team
            {
                if ((currentItem.GetComponent<Tower>() != null) && currentItem.GetComponent<Tower>().canBeConquered
                    && currentItem.GetComponent<TowerNeutral>().IsCurrentStateNeutral()) // If he has to conquest it
                {
                    // Se va a la torre
                    Debug.Log("vamos a conquistar la TN copon!");
                    currentEngineerState = EngineerState.GoingToConquerableItem;
                    GoTo(destiny);
                }
            }
            else // TN in the same team
            {
                // Se va a la torre
                Debug.Log("vamos a arreglar la TN copon!");
                currentEngineerState = EngineerState.GoingToRepairItem;
                GoTo(destiny);
            }
        }
        else if (destTransform.name == "TowerBoxConstruct" || destTransform.name == "TowerGoblin")// If he has to go to a TowerGoblin
        {
            attackCadenceAux = 2.5f;
            GameObject comp1 = null;
            GameObject comp2 = null;
            currentItem = destTransform;
            if (destTransform.name == "TowerBoxConstruct")
            {
                comp1 = destTransform.GetComponent<BoxConstruct>().gameObject;
                comp2 = towerGoblin.transform.GetComponent<TowerGoblin>().transform.FindChild("TowerBoxConstruct").gameObject;
            }
            else if (newTGConstruct)
            {
                comp1 = destTransform.GetComponent<TowerGoblin>().gameObject;
                comp2 = towerGoblin.transform.GetComponent<TowerGoblin>().gameObject;
            }
            if ((comp1 != comp2) || !newTGConstruct) // If it's not the same towerGoblin that is going To Construct
            {
                newTGConstruct = newWConstruct = false;
                // if it's in the same team he has to reapir it
                if (currentItem.GetComponent<BuildingController>().GetTeamNumber() == teamNumber)
                {
                    if (currentItem.GetComponent<TowerGoblin>().IsConstructed())
                    {
                        // Se va a la torre
                        Debug.Log("vamos a arreglar la TowerGoblin copon!");
                        currentEngineerState = EngineerState.GoingToRepairItem;
                        GoTo(destiny);
                    }
                    else
                    {
                        // Se va a la torre
                        Debug.Log("vamos a construir la TowerGoblin copon!");
                        currentEngineerState = EngineerState.GoingToConstructItem;
                        GoTo(destiny);
                    }

                }
            }
            else if (newTGConstruct)// If he is constructing a TG
            {
                // Construct the new TowerGoblin
                if (towerGoblin.transform.GetComponent<TowerGoblin>().StartConstruct(constructDestiny, baseController))
                {
                    Debug.Log("vamos a construir una Torreta copon!");
                    currentItem = destTransform;
                    currentEngineerState = EngineerState.GoingToConstructItem;
                    GoTo(new Vector3(destiny.x, 0, destiny.z));
                    constructDestiny = destiny;
                    currentItem = towerGoblin.transform;
                    //newConstruct = false;
                }
            }
        }
        else if (destTransform.name == "WarehouseBoxConstruct" || destTransform.name == "Goblin Warehouse")// If he has to go to a TowerGoblin
        {
            attackCadenceAux = 2.5f;
            currentItem = destTransform;
            GameObject comp1 = null;
            GameObject comp2 = null;
            if (destTransform.name == "WarehouseBoxConstruct")
            {
                comp1 = destTransform.GetComponent<BoxConstruct>().gameObject;
                comp2 = warehouse.transform.GetComponent<Warehouse>().transform.FindChild("WarehouseBoxConstruct").gameObject;
            }
            else if (newWConstruct)
            {
                comp1 = destTransform.GetComponent<Warehouse>().gameObject;
                comp2 = warehouse.transform.GetComponent<Warehouse>().gameObject;
            }

            if ((comp1 != comp2) || !newWConstruct) // If it's not the same towerGoblin that is going To Construct
            {
                newTGConstruct = newWConstruct = false;
                // if it's in the same team he has to reapir it
                if (currentItem.GetComponent<BuildingController>().GetTeamNumber() == teamNumber)
                {
                    if (currentItem.GetComponent<Warehouse>().IsConstructed())
                    {
                        // Se va a la Warehouse
                        Debug.Log("vamos a arreglar la Warehouse copon!");
                        currentEngineerState = EngineerState.GoingToRepairItem;
                        GoTo(destiny);
                    }
                    else
                    {
                        // Se va a la Warehouse
                        Debug.Log("vamos a construir la Warehouse copon!");
                        currentEngineerState = EngineerState.GoingToConstructItem;
                        GoTo(destiny);
                    }

                }
            }
            else if (newWConstruct)// If he is constructing a warehouse
            {
                // Construct the new warehouse
                if (warehouse.transform.GetComponent<Warehouse>().StartConstruct(constructDestiny, baseController))
                {
                    Debug.Log("vamos a construir una warehouse copon!");
                    currentItem = destTransform;
                    currentEngineerState = EngineerState.GoingToConstructItem;
                    GoTo(new Vector3(destiny.x, 0, destiny.z));
                    constructDestiny = destiny;
                    currentItem = warehouse.transform;
                    //newConstruct = false;
                }
            }
        }
        else
            base.RightClickOnSelected(destiny, destTransform);
    }// RightClickOSelected

    private void LeaveQueues()
    {
        // He has to leave the engineerPosition if he has to
        if (currentEngineerState == EngineerState.GoingToConquestPosition || currentEngineerState == EngineerState.Conquering)
            currentItem.GetComponent<TowerNeutral>().LeaveEngineerPositionConquest(lastEngineerIndex);
        else if (currentEngineerState == EngineerState.GoingToRepairPosition || currentEngineerState == EngineerState.Repairing)
            currentItem.GetComponent<BuildingController>().LeaveEngineerPositionRepair(lastEngineerIndex);
        else if (currentEngineerState == EngineerState.GoingToConstructPosition ||
                    currentEngineerState == EngineerState.Constructing)
        {
            if (currentItem.GetComponent<TowerGoblin>() != null)
                currentItem.GetComponent<TowerGoblin>().LeaveEngineerPositionConstruct(lastEngineerIndex);
            else if (currentItem.GetComponent<Warehouse>() != null)
                currentItem.GetComponent<Warehouse>().LeaveEngineerPositionConstruct(lastEngineerIndex);
        }
        else if (currentEngineerState == EngineerState.Waiting)
            currentItem.GetComponent<BuildingController>().LeaveQueue(this);
    }

    public void FinishWaitingToRepair (Vector3 repairPosition, int chopIndex)
    {
        lastEngineerPos = repairPosition;
        lastEngineerIndex = chopIndex;
        currentEngineerState = EngineerState.GoingToRepairPosition;
        base.GoTo(lastEngineerPos);
    }

    public void FinishWaitingToConquest (Vector3 conquestPosition, int chopIndex)
    {
        lastEngineerPos = conquestPosition;
        lastEngineerIndex = chopIndex;
        currentEngineerState = EngineerState.GoingToConquestPosition;
        base.GoTo(lastEngineerPos);
    }

    public void FinishWaitingToConstruct (Vector3 constructPosition, int chopIndex)
    {
        lastEngineerPos = constructPosition;
        lastEngineerIndex = chopIndex;
        currentEngineerState = EngineerState.GoingToConquestPosition;
        base.GoTo(lastEngineerPos);
    }

    public void StartRepairing ()
    {
        if (currentEngineerState == EngineerState.GoingToRepairItem)
        {
            Debug.Log("comenzando la reparacion...");
            currentEngineerState = EngineerState.Repairing;
        }
    }

	public void StartConquering ()
	{
        if (currentEngineerState == EngineerState.GoingToConquerableItem)
        {
            Debug.Log("comenzando la conquista...");
            currentEngineerState = EngineerState.Conquering;
        }
	}

	public bool IsNewConstructing ()
	{
		return newTGConstruct || newWConstruct;
	}

    public void SetCanConstruct (int item)
	{
        switch (item)
        {
            case 0:
                towerGoblin = Instantiate(towerGoblinPrefab, new Vector3(Input.mousePosition.x, 0, Input.mousePosition.z), new Quaternion(0, 0, 0, 0))
                    as GameObject; //rotation (-90, -180, 0)
                towerGoblin.name = towerGoblin.name.Replace("(Clone)", "");
                towerGoblin.GetComponent<TowerGoblin>().SetTeamNumber(this.teamNumber);
                newTGConstruct = true;
                break;
            case 1:
                warehouse = Instantiate(warehousePrefab, new Vector3(Input.mousePosition.x, 0, Input.mousePosition.z), new Quaternion(0, 0, 0, 0))
                    as GameObject; //rotation (-90, -180, 0)
                warehouse.name = warehouse.name.Replace("(Clone)", "");
                warehouse.GetComponent<Warehouse>().SetTeamNumber(this.teamNumber);
                newWConstruct = true;
                break;
        }
	}

    public override int GetUnitType()
    {
        return 3;
    }

    protected override void UpdateGoingToAnEnemy()
    {
        // 1- comprobamos si el enemigo está "a mano" y se le puede atacar
        float distToEnemy = Vector3.Distance(transform.position, enemySelected.transform.position);
        if (distToEnemy <= maxAttackDistance)
        {
            // change to Attack state
            currentState = State.Attacking;
            PlayAnimationCrossFade("Attack1");
            GetComponent<NavMeshAgent>().destination = transform.position;

            transform.LookAt(enemySelected.transform);

            
           /* if (newFireball)
                Destroy(newFireball.gameObject);*/
        }
        // 2- comprobamos si el enemigo esta "a vista"
        else if (distToEnemy <= visionSphereRadius)
        {
            this.destiny = enemySelected.transform.position;
            GetComponent<NavMeshAgent>().destination = destiny;
        }
        // 3- se ha llegado al destino y se ha perdido de vista al enemigo
        else if (Vector3.Distance(transform.position, destiny) <= destinyThreshold)
        {
            StopMoving();
        }
    }

    protected override void UpdateAttacking()
    {
        attackCadenceAux -= Time.deltaTime;

        float enemyDist = Vector3.Distance(transform.position, enemySelected.transform.position);
        if (enemySelected)
        {
            if (enemyDist <= maxAttackDistance)
            {
                if (attackCadenceAux <= 0)
                {
                    transform.LookAt(enemySelected.transform);

                    if (!newFireball)
                    {
                        // Instanciate a new Fireball
                        Debug.Log("Dummy position: " + dummyHand.transform.position);
                        Debug.Log("Engineer position: " + transform.position);
                        newFireball = Instantiate
                        (
                            fireball,
                            dummyHand.transform.position,
                            //new Vector3(transform.position.x + 3.0f, 1.0f, transform.position.z),
                            new Quaternion()
                        ) as GameObject;
                        newFireball.rigidbody.isKinematic = false;
                        newFireball.transform.name = "Fireball";
                        newFireball.transform.parent = dummyHand;
                        newFireball.transform.rotation = transform.rotation;
                        newFireball.transform.FindChild("FireballVisionSphere").GetComponent<CFireballVisionSphere>().SetOwner(this.gameObject);
                        newFireball.transform.FindChild("FireballVisionSphere").GetComponent<CFireballVisionSphere>().SetDamage((int)attackPower);
                        newFireball.transform.FindChild("FireballVisionSphere").GetComponent<CFireballVisionSphere>().SetDestroyTime(2.5f);
                    }

                    attackCadenceAux = attackCadence;
                    Vector3 dir = enemySelected.transform.position - newFireball.transform.position;
                    dir = dir.normalized;
                    Vector3 dir1 = transform.forward.normalized;
                    newFireball.transform.parent = null;
                    newFireball.rigidbody.AddForce(new Vector3(dir.x * 8.0f * (enemyDist / maxAttackDistance),
                                                                        7,
                                                                dir.z * 8.0f * (enemyDist / maxAttackDistance)), ForceMode.Impulse);

                    //newFireball.GetComponent<SphereCollider>().isTrigger = true;
                    
                    //newFireball.rigidbody.AddForce(fireball.transform.forward * 500);
                    if (enemySelected.currentLife <= 0.0f)
                    {
                        // the enemy has die
                        enemySelected = null;
                        currentState = State.Idle;

                        PlayAnimationCrossFade("Idle01");
                        attackCadenceAux = 2.5f;
                    }
                }
            }
            else if (enemyDist <= visionSphereRadius)
            {
                currentState = State.GoingToAnEnemy;

                this.destiny = enemySelected.transform.position;
                GetComponent<NavMeshAgent>().destination = destiny;

                PlayAnimationCrossFade("Walk");
                attackCadenceAux = 2.5f;
            }
            else
            {
                enemySelected = null;
                currentState = State.Idle;
                attackCadenceAux = 2.5f;
                PlayAnimationCrossFade("Idle01");
            }
        }
        else // the enemy is no longer alive
        {
            enemySelected = null;
            currentState = State.Idle;

            PlayAnimationCrossFade("Idle01");
            attackCadenceAux = 2.5f;
        }
    }

    protected override void RemoveAssetsFromModel()
    {
        // We destroy the Hammer
        Transform hammer1 = dummyHand.transform.FindChild("Hammer");
        if (hammer1 != null)
            GameObject.Destroy(hammer1.gameObject);
        // We destroy the Laptop
        Transform laptop1 = dummyLaptop.transform.FindChild("Laptop");
        if (laptop1 != null)
            GameObject.Destroy(laptop1.gameObject);
    }
}
