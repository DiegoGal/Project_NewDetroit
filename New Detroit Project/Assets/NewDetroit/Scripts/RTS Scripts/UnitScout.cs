using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitScout : UnitController
{

	public int attackPower = 2;

	private int nextPositionIndex = 0;

	public List<Vector3> patrolPositionsList = new List<Vector3>();

	private enum ScoutState
	{
		None,
		Patrolling
	}
	private ScoutState currentScoutState = ScoutState.None;

	// Use this for initialization
	public override void Start ()
	{
		base.Start();
		basicAttackPower = secondaryAttackPower = attackPower;
	}
	
	// Update is called once per frame
	public override void Update () 
	{
		base.Update();
        if ( (currentScoutState == ScoutState.Patrolling) && (currentState == State.Idle) )
		{
            nextPositionIndex = (nextPositionIndex + 1) % patrolPositionsList.Count;
            base.GoTo(patrolPositionsList[nextPositionIndex]);
        }
	}

	public override void OnGUI()
	{
		base.OnGUI();
		
		GUI.skin.label.fontSize = 10;

        GUI.Label(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 45, 100, 50),
            currentState.ToString());
        GUI.Label(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 55, 100, 50),
            currentScoutState.ToString());
        GUI.Label(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 65, 100, 50),
            "NextPatrolPoint: " + nextPositionIndex);
	}

	public void StartPatrol (List<Vector3> positionList)
	{
        // si solo se ha marcado un punto de patrulla se hace GoTo simple a ese punto
        if (positionList.Count == 1)
        {
            nextPositionIndex = 0;
            currentScoutState = ScoutState.None;
            GoTo(positionList[0]);
        }
        else if (positionList.Count > 1)
        {
            nextPositionIndex = 0;
            currentScoutState = ScoutState.Patrolling;
            patrolPositionsList = new List<Vector3>(positionList);
            Debug.Log("siguiente goto a: " + patrolPositionsList[0]);
            base.GoTo(patrolPositionsList[0]);
        }
	}

	public override void RightClickOnSelected (Vector3 destiny, Transform destTransform)
	{
		if (destTransform.name == "WorldFloor")
		{
			currentScoutState = ScoutState.None;
			base.RightClickOnSelected(destiny, destTransform);
		}
	}

}
