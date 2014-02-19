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
		GoingToItem,
		Waiting, // espera hasta que halla hueco en el item
		GoingToConquestPosition,
		GoingToRepairPosition,
		Repairing, // construyendo, conquistando o reparando
		Conquering
	}
	private EngineerState currentEngineerState = EngineerState.None;
	
	// referencia al item que se está construyendo, conquistando o reparando
	private Transform currentItem;
	
	private Vector3 lastEngineerPos;
	private int lastEngineerIndex;

    public override void Start ()
    {
        base.Start();

        basicAttackPower = secondaryAttackPower = attackPower;
    }
	
	// Update is called once per frame
	public override void Update () 
	{

		switch (currentEngineerState)
		{
			case EngineerState.None:
				base.Update();
				break;
			case EngineerState.GoingToItem:
				// if the distance to the item is less than distanceToWait we ask if there is gap
				float distItem = Vector3.Distance(transform.position, currentItem.position);
				float distToWait = currentItem.GetComponent<NeutralTower>().distanceToWait;
				if (distItem < 10.0f)
				{
				if ( currentItem.GetComponent<NeutralTower>().GetEngineerPosition(
						ref lastEngineerPos,
						ref lastEngineerIndex,
						this) )
					{
						// there is a gap and we have the position
						if (currentItem.GetComponent<NeutralTower>().IsCurrentStateNeutral())
							currentEngineerState = EngineerState.GoingToConquestPosition;
						else
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
			case EngineerState.Repairing:
				actualEngineerTime += Time.deltaTime;
				bool repaired = false;
				if (actualEngineerTime >= engineerTime)
				{
					repaired = currentItem.GetComponent<NeutralTower>().Repair(amountPerAction);
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
					conquest = currentItem.GetComponent<NeutralTower>().Conquest(amountPerAction,teamNumber);
					// The item has been conquered
					if (conquest)
					{
						Debug.Log("Unidad Conquistada!");
						currentEngineerState = EngineerState.None;
					}
					actualEngineerTime = 0;
				}
				break;
		} // Switch
	} // Update

	public override void RightClickOnSelected(Vector3 destiny, Transform destTransform)
	{
		if (destTransform.name == "WorldFloor")
		{
			//Debug.Log("ojo suelo!");
			if (currentEngineerState == EngineerState.GoingToRepairPosition ||
			    currentEngineerState == EngineerState.Repairing ||
			    currentEngineerState == EngineerState.GoingToConquestPosition ||
			    currentEngineerState == EngineerState.Conquering)
				currentItem.GetComponent<NeutralTower>().LeaveEngineerPosition(lastEngineerIndex);
			else if (currentEngineerState == EngineerState.Waiting)
				currentItem.GetComponent<NeutralTower>().LeaveQueue(this);
			currentEngineerState = EngineerState.None;
			base.RightClickOnSelected(destiny, destTransform);
		}
		else if (destTransform.name == "NeutralTower")
		{
			// actualizar la referencia de la última mina seleccionada
			currentItem = destTransform;
			if (currentEngineerState == EngineerState.None)
			{
				if (currentItem.GetComponent<NeutralTower>().IsCurrentStateNeutral())
				{
					// Se va a la torre
					Debug.Log("vamos a conquistar copon!");
					currentEngineerState = EngineerState.GoingToItem;
					GoTo(destiny);
				}
				else if (currentItem.GetComponent<NeutralTower>().GetTeamNumber() == teamNumber)
				{
					// Se va a la torre
					Debug.Log("vamos a arreglar copon!");
					currentEngineerState = EngineerState.GoingToItem;
					GoTo(destiny);
				}
			}
		}

	} // RightClickOnSelected

	public void FinishWaitingToRepair (Vector3 chopPosition, int chopIndex)
	{
		lastEngineerPos = chopPosition;
		lastEngineerIndex = chopIndex;
		currentEngineerState = EngineerState.GoingToRepairPosition;
		base.GoTo(lastEngineerPos);
	}

	public void FinishWaitingToConquest (Vector3 chopPosition, int chopIndex)
	{
		lastEngineerPos = chopPosition;
		lastEngineerIndex = chopIndex;
		currentEngineerState = EngineerState.GoingToConquestPosition;
		base.GoTo(lastEngineerPos);
	}

	public void StartRepairing ()
	{
		// cuando llegue a la mina pasar el estado a Choping
		if (currentEngineerState == EngineerState.GoingToItem)
		{
			Debug.Log("comenzando la reparacion...");
			currentEngineerState = EngineerState.Repairing;
		}
	}

	public void StartConquering ()
	{
		// cuando llegue a la mina pasar el estado a Choping
		if (currentEngineerState == EngineerState.GoingToItem)
		{
			Debug.Log("comenzando la conquista...");
			currentEngineerState = EngineerState.Conquering;
		}
	}
}
