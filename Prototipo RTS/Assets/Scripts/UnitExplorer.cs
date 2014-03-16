using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitExplorer : UnitController {

	public int attackPower = 1;

	protected const float totalLife = 100.0f;

	private int nextPosition = 0;

	public List<Vector3> positionPatrolList = new List<Vector3>();

	private enum ExplorerState
	{
		None,
		assigningPositions,
		Patrolling,
	}
	private ExplorerState currentExplorerState = ExplorerState.None;

	// Use this for initialization
	public override void Start ()
	{
		base.Start();
		basicAttackPower = secondaryAttackPower = attackPower;
	}
	
	// Update is called once per frame
	public override void Update () 
	{
		//base.Update ();
		switch (currentExplorerState)
		{
		case ExplorerState.None:
			base.Update ();
			//Debug.Log("None everywhere");
			break;
		case ExplorerState.Patrolling:
			//Debug.Log(positionPatrolList.Count);
			/*int numberPositions = positionPatrolList.Count;
			float distance = Vector3.Distance(transform.position, positionPatrolList[nextPosition]);
			if (distance < 1.0)//Puede haber alguna otra unidad en esa posicion
			//if (transform.position == positionPatrolList[nextPosition])
			{
				if (nextPosition+1 == positionPatrolList.Count)
					nextPosition = 0;
				else
					nextPosition++;
				base.GoTo(positionPatrolList[nextPosition]);
			}
			else
				base.Update();*/
			base.Update();
			if (currentState == State.Iddle)
			{
				Debug.Log("Wat");
				nextPosition = (nextPosition + 1) % positionPatrolList.Count;
				base.GoTo (positionPatrolList[nextPosition]);
			}
			break;
		}
	}

	public override void OnGUI()
	{
		base.OnGUI();
		
		Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
		
		GUI.skin.label.fontSize = 10;
		
		GUI.Label(new Rect(camPos.x - 10, Screen.height - camPos.y - 25, 100, 50),
		          currentState.ToString());
		GUI.Label(new Rect(camPos.x - 10, Screen.height - camPos.y - 35, 100, 50),
		          currentExplorerState.ToString());
	}

	// Repair is called by the harvesters
	public bool Heal(float sum)
	{
		// increasement of the explorers life
		if (currentLife < totalLife)
		{
			currentLife += sum;
			if (totalLife < currentLife)
				currentLife = totalLife;
		}
		if (currentLife == totalLife)
		{
			return true;
		}
		else
			return false;
	}

	public void changeStateAndTakeList(List<Vector3> positionList)
	{
		nextPosition = 0;
		currentExplorerState = ExplorerState.Patrolling;
		positionPatrolList = new List<Vector3>(positionList);
		Debug.Log ("siguiente goto a: " + positionPatrolList [0]);
		base.GoTo (positionPatrolList[0]);
	}

	public override void RightClickOnSelected(Vector3 destiny, Transform destTransform)
	{
		if (destTransform.name == "WorldFloor")
		{
			currentExplorerState = ExplorerState.None;
			base.RightClickOnSelected(destiny, destTransform);
		}
	}

}
