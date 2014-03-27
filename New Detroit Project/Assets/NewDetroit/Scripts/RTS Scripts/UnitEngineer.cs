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

    public GameObject laptopPrefab;
    public GameObject hamerPrefab;

    private bool newTGConstruct = false;
    private bool newWConstruct = false;
    private Vector3 constructDestiny = new Vector3();

    // dummy where is going to be instanciated a laptop
    public Transform dummyLaptop;
    public Transform dummyHand;

    // reference to the laptop
    public GameObject laptop;
    public GameObject hammer;

    public override void Awake()
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
    }
	
	// Update is called once per frame
	public override void Update () 
	{
        // If this is selected and "C" is pulsed, a towerGoblin has to be instanciate with transparency
        if (Input.anyKeyDown && (newTGConstruct || newWConstruct) && Input.GetMouseButtonDown(0) && 
            (currentEngineerState != EngineerState.GoingToConstructItem) && (currentEngineerState != EngineerState.GoingToConstructPosition))
        {
            newTGConstruct = newWConstruct = false;
            Destroy(towerGoblin);
            Destroy(warehouse);
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
                    Debug.Log("comenzando reparacion!!!!!!!");
                    currentEngineerState = EngineerState.Repairing;
                }
                else
                    base.Update();
                break;
            case EngineerState.GoingToConquestPosition:
                if (currentState == State.Idle)
                {
                    // when it have arrived to the conquest position
                    Debug.Log("comenzando conquista!!!!!!!!");
                    currentEngineerState = EngineerState.Conquering;

                    // intanciamos un laptop
                    Debug.Log("Dummy position: " + dummyLaptop.transform.position);
                    Debug.Log("Engineer position: " + transform.position);
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
                        // when it have arrived to the conquest position
                        Debug.Log("comenzando construccion!!!!!!!!");
                        currentEngineerState = EngineerState.Constructing;
                        newTGConstruct = newWConstruct = false;
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

                            baseController.GetArmyController().AddWarehouse(currentItem.GetComponent<CResourceBuilding>());
                        }
                        actualEngineerTime = 0;
                    }
                }
                break;
        } // Switch

	} // Update

    public override void RightClickOnSelected(Vector3 destiny, Transform destTransform)
    {
        destiny.y = 0;
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
        // He has to go to another position if he has to
        // destTransform.name == "TowerGoblin" || destTransform.name == "Goblin Warehouse" 
        if (destTransform.name == "WorldFloor")// If he has to go to another position of the worldfloor he goes
        {
            newTGConstruct = newWConstruct = false;
            currentEngineerState = EngineerState.None;
            // We destroy the Laptop
            Transform laptop1 = dummyLaptop.transform.FindChild("Laptop");
            if (laptop1 != null)
                GameObject.Destroy(laptop1.gameObject);

            base.RightClickOnSelected(destiny, destTransform);
        }
        else if (destTransform.name == "TowerNeutral")// If he has to go to a TowerNeutral
        {
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
                if (towerGoblin.transform.GetComponent<TowerGoblin>().StartConstruct(constructDestiny,baseController))
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
    }// RightClickOSelected
	

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

}
