using UnityEngine;
using System.Collections;

public class UnitEngineer : UnitController {

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

    private bool newConstruct = false;
    Vector3 constructDestiny = new Vector3();


    public override void Start ()
    {
        base.Start();
        basicAttackPower = secondaryAttackPower = attackPower;
    }
	
	// Update is called once per frame
	public override void Update () 
	{
        // If this is selected and "C" is pulsed, a towerGoblin has to be instanciate with transparency
        if ( Input.anyKeyDown && newConstruct && !Input.GetMouseButtonDown(1) && (!Input.GetKeyDown(KeyCode.T) && (!Input.GetKeyDown(KeyCode.W))))
        {
            newConstruct = false;
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
                break;
            case EngineerState.Waiting:

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
                }
                else
                    base.Update();
                break;
			case EngineerState.GoingToConstructPosition:
				if (currentState == State.Idle)
				{
					// when it have arrived to the conquest position
					Debug.Log("comenzando construccion!!!!!!!!");
					currentEngineerState = EngineerState.Constructing;
				}
				else
					base.Update();
				break;
            case EngineerState.Repairing:
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
                    }
                    actualEngineerTime = 0;
                }
                break;
            case EngineerState.Conquering:
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
                    }
                    actualEngineerTime = 0;
                }
                break;
			case EngineerState.Constructing:
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

                            baseController.GetArmyController().addWarehouse(currentItem.GetComponent<CResourceBuilding>());
                        }
                        actualEngineerTime = 0;
                    }
                } 
				break;
        } // Switch
	} // Update

	public override void RightClickOnSelected(Vector3 destiny, Transform destTransform)
	{
        if (newConstruct)
        {
			if (currentEngineerState == EngineerState.GoingToConquestPosition ||
			    currentEngineerState == EngineerState.Conquering)
			{
				currentItem.GetComponent<TowerNeutral>().LeaveEngineerPositionConquest(lastEngineerIndex);	
			}
			else if (currentEngineerState == EngineerState.GoingToRepairPosition ||
			         currentEngineerState == EngineerState.Repairing)
			{
				currentItem.GetComponent<BuildingController>().LeaveEngineerPositionRepair(lastEngineerIndex);
				
			}
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
           
            if (towerGoblin != null)
            {    // It goes to the tower
                if (towerGoblin.transform.GetComponent<TowerGoblin>().StartConstruct(constructDestiny))
                {
                    Debug.Log("vamos a construir una Torreta copon!");
                    currentItem = destTransform;
                    currentEngineerState = EngineerState.GoingToConstructItem;
                    GoTo(new Vector3(destiny.x, 0, destiny.z));
                    constructDestiny = Input.mousePosition;
                    currentItem = towerGoblin.transform;
                    newConstruct = false;

                }
            }
            else
            {    // It goes to the warehouse
                if (warehouse.transform.GetComponent<Warehouse>().StartConstruct(constructDestiny))
                {
                    Debug.Log("vamos a construir un almacen copon!");
                    currentItem = destTransform;
                    currentEngineerState = EngineerState.GoingToConstructItem;
                    GoTo(new Vector3(destiny.x, 0, destiny.z));
                    constructDestiny = Input.mousePosition;
                    currentItem = warehouse.transform;
                    newConstruct = false;

                }
            }
        }// NewConstruct
        else if (destTransform.name == "TowerNeutral" || destTransform.name == "TowerGoblin" || destTransform.name == "Warehouse" || destTransform.name == "WorldFloor")
		{
            destiny.y = 0;
            // actualizar la referencia de la última torre seleccionada
            if (currentEngineerState == EngineerState.GoingToConquestPosition ||
              currentEngineerState == EngineerState.Conquering)
            {
                currentItem.GetComponent<TowerNeutral>().LeaveEngineerPositionConquest(lastEngineerIndex);

            }
            else if (currentEngineerState == EngineerState.GoingToRepairPosition ||
                currentEngineerState == EngineerState.Repairing)
            {
                currentItem.GetComponent<BuildingController>().LeaveEngineerPositionRepair(lastEngineerIndex);

            }
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
            if (destTransform.name == "WorldFloor")
            {
                currentEngineerState = EngineerState.None;
                base.RightClickOnSelected(destiny, destTransform);
            }

            currentItem = destTransform;
            if (currentItem.name == "TowerNeutral" || currentItem.name == "TowerGoblin"|| destTransform.name == "Warehouse") 
                // When it's a TowerNeutral and has to be conquered
                if ((currentItem.GetComponent<Tower>() != null) && currentItem.GetComponent<Tower>().canBeConquered && currentItem.GetComponent<TowerNeutral>().IsCurrentStateNeutral())
				{
					// Se va a la torre
					Debug.Log("vamos a conquistar copon!");
					currentEngineerState = EngineerState.GoingToConquerableItem;
					GoTo(destiny);
                }
                // When it's in the same team and has not to be conquered
				else if (currentItem.GetComponent<BuildingController>().GetTeamNumber() == teamNumber)
				{
					// If it's the TowerNeutral, it has to be repaired
					if ((currentItem.GetComponent<Tower>() != null) && currentItem.GetComponent<Tower>().canBeConquered)
					{
						// Se va a la torre
						Debug.Log("vamos a arreglar la TN copon!");
						currentEngineerState = EngineerState.GoingToRepairItem;
						GoTo(destiny);
					}
                    // If it's the TowerGoblin or the warehouse and is constructed, it has to be repaired
                    else if ((currentItem.GetComponent<TowerGoblin>() != null) && currentItem.GetComponent<TowerGoblin>().IsConstructed() ||
                             (currentItem.GetComponent<Warehouse>() != null) && currentItem.GetComponent<Warehouse>().IsConstructed()) // If it has to repair
					{
						// Se va a la torre
						Debug.Log("vamos a arreglar!");
						currentEngineerState = EngineerState.GoingToRepairItem;
						GoTo(new Vector3(destiny.x, 0, destiny.z));					
					}
					else // If it has to construct 
					{
						// Se va a la torre
						Debug.Log("vamos a construir copon!");
						currentEngineerState = EngineerState.GoingToConstructItem;
						GoTo(new Vector3(destiny.x, 0, destiny.z));	
					}
				}
		}
	} // RightClickOnSelected

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

    public void FinishWaitingToConstruct(Vector3 constructPosition, int chopIndex)
    {
        lastEngineerPos = constructPosition;
        lastEngineerIndex = chopIndex;
        currentEngineerState = EngineerState.GoingToConquestPosition;
        base.GoTo(lastEngineerPos);
    }

	public void StartRepairing ()
	{
		// cuando llegue a la mina pasar el estado a Choping
		if (currentEngineerState == EngineerState.GoingToRepairItem)
		{
			Debug.Log("comenzando la reparacion...");
			currentEngineerState = EngineerState.Repairing;
		}
	}

	public void StartConquering ()
	{
		// cuando llegue a la mina pasar el estado a Choping
		if (currentEngineerState == EngineerState.GoingToConquerableItem)
		{
			Debug.Log("comenzando la conquista...");
			currentEngineerState = EngineerState.Conquering;
		}
	}

	public bool IsNewConstructing()
	{
		return newConstruct;
	}

	public void SetCanConstruct(int item)
	{
		newConstruct = true;
        switch (item)
        {
            case 0:
                towerGoblin = Instantiate(towerGoblinPrefab, new Vector3(Input.mousePosition.x, 0, Input.mousePosition.z), new Quaternion(0, 0, 0, 0))
			        as GameObject; //rotation (-90, -180, 0)
		        towerGoblin.name = towerGoblin.name.Replace("(Clone)", "");
		        towerGoblin.GetComponent<TowerGoblin>().SetTeamNumber(this.teamNumber);
                break;
            case 1:
                warehouse = Instantiate(warehousePrefab, new Vector3(Input.mousePosition.x, 0, Input.mousePosition.z), new Quaternion(0, 0, 0, 0))
			        as GameObject; //rotation (-90, -180, 0)
                warehouse.name = warehouse.name.Replace("(Clone)", "");
                warehouse.GetComponent<Warehouse>().SetTeamNumber(this.teamNumber);
                break;
        }
        
	}
}
