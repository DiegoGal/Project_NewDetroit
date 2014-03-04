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
    // To instanciate a towerGoblin
    public GameObject towerGoblin;
    private GameObject tower;
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
        if (Input.GetKeyDown(KeyCode.C) && (this.GetComponent<CSelectable>().IsSelected()))
        {
            newConstruct = true;
            tower = Instantiate(towerGoblin, constructDestiny, new Quaternion(0, 0, 0, 0))
            as GameObject; //rotation (-90, -180, 0)
            
        }

        switch (currentEngineerState)
        {
            case EngineerState.None:
                base.Update();
                break;
            case EngineerState.GoingToRepairItem:
                // if the distance to the item is less than distanceToWait we ask if there is gap
                float distItem = Vector3.Distance(transform.position, currentItem.position);
                float distToWait = currentItem.GetComponent<Tower>().distanceToWait;
                if (distItem < 10.0f)
                {
                    if (currentItem.GetComponent<Tower>().GetEngineerPosition(
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
                distToWait = currentItem.GetComponent<Tower>().distanceToWait;
                if (distItem < 10.0f)
                {
                    if (currentItem.GetComponent<Tower>().GetEngineerPosition(
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
                distToWait = currentItem.GetComponent<Tower>().distanceToWait;
                if (distItem < 10.0f)
                {
                    if (currentItem.GetComponent<Tower>().GetEngineerPosition(
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
                if (currentState == State.Iddle)
                {
                    // when it have arrived to the repair position
                    Debug.Log("comenzando reparacion!!!!!!!");
                    currentEngineerState = EngineerState.Repairing;
                }
                else
                    base.Update();
                break;
            case EngineerState.GoingToConquestPosition:
                if (currentState == State.Iddle)
                {
                    // when it have arrived to the conquest position
                    Debug.Log("comenzando conquista!!!!!!!!");
                    currentEngineerState = EngineerState.Conquering;
                }
                else
                    base.Update();
                break;
			case EngineerState.GoingToConstructPosition:
				if (currentState == State.Iddle)
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
                    repaired = currentItem.GetComponent<Tower>().Repair(amountPerAction);
                    // The item has been repaired
                    if (repaired)
                    {
                        Debug.Log("Unidad Reparada");
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
                    if (conquest)
                    {
                        Debug.Log("Unidad Conquistada!");
                        currentEngineerState = EngineerState.None;
                    }
                    actualEngineerTime = 0;
                }
                break;
			case EngineerState.Constructing:
				actualEngineerTime += Time.deltaTime;
				if (actualEngineerTime >= engineerTime)
				{
					conquest = currentItem.GetComponent<TowerGoblin>().Construct(amountPerAction);
					// The item has been conquered
					if (conquest)
					{
						Debug.Log("Torre construida!");
						currentEngineerState = EngineerState.None;
						//currentItem.GetComponent<TowerGoblin>().SetActive(true);					
                    }
					actualEngineerTime = 0;
				}
				break;
        } // Switch
	} // Update

	public override void RightClickOnSelected(Vector3 destiny, Transform destTransform)
	{
        if (newConstruct)
        {
            // Se va a la torre
            Debug.Log("vamos a construir copon!");
			currentItem = destTransform;
            currentEngineerState = EngineerState.GoingToConstructItem;
            GoTo(new Vector3(destiny.x, 0, destiny.z));
            constructDestiny = Input.mousePosition;
            tower.transform.GetComponent<TowerGoblin>().Construct(constructDestiny);
            //tower.Construct(constructDestiny);
            newConstruct = false;
        }
        else if (destTransform.name == "WorldFloor")
		{
			//Debug.Log("ojo suelo!");
            if (currentEngineerState == EngineerState.GoingToConquestPosition ||
                currentEngineerState == EngineerState.Conquering)
            {
                currentItem.GetComponent<Tower>().LeaveEngineerPositionConquest(lastEngineerIndex);

            }
            else if (currentEngineerState == EngineerState.GoingToRepairPosition ||
                currentEngineerState == EngineerState.Repairing)
            {
                currentItem.GetComponent<Tower>().LeaveEngineerPositionRepair(lastEngineerIndex);

            }
            else if (currentEngineerState == EngineerState.GoingToConstructPosition ||
                currentEngineerState == EngineerState.Constructing)
            {
                currentItem.GetComponent<Tower>().LeaveEngineerPositionConstruct(lastEngineerIndex);

            }
            else if (currentEngineerState == EngineerState.Waiting)
                currentItem.GetComponent<TowerNeutral>().LeaveQueue(this);
			currentEngineerState = EngineerState.None;
			base.RightClickOnSelected(destiny, destTransform);
		}
        else if (destTransform.name == "TowerNeutral" || destTransform.name == "TowerNeutralGoblin")
		{
			// actualizar la referencia de la última torre seleccionada
			currentItem = destTransform;
			if (currentEngineerState == EngineerState.None)
			{
				if (currentItem.GetComponent<Tower>().canBeConquered && currentItem.GetComponent<TowerNeutral>().IsCurrentStateNeutral())
				{
					// Se va a la torre
					Debug.Log("vamos a conquistar copon!");
					currentEngineerState = EngineerState.GoingToConquerableItem;
					GoTo(destiny);
				}
				else if (currentItem.GetComponent<Tower>().GetTeamNumber() == teamNumber)
				{
					// Se va a la torre
					Debug.Log("vamos a arreglar copon!");
					currentEngineerState = EngineerState.GoingToRepairItem;
					GoTo(destiny);
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
}
