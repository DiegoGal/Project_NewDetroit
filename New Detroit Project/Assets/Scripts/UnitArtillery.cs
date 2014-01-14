using UnityEngine;
using System.Collections;

public class UnitArtillery : UnitController
{
    protected enum Mode
    {
        Defensive,
        Ofensive
    }
    protected Mode mode = Mode.Defensive;

    protected enum ArtilleryState
    {
        None,
        Searching,
        Attacking,
        Chasing
    }
    protected ArtilleryState currentArtilleryState = ArtilleryState.None;
	
	// Update is called once per frame
    public override void Update ()
    {
	
	}

    void OnGUI()
    {
        Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);

        GUI.skin.label.fontSize = 10;

        GUI.Label(new Rect(camPos.x - 10, Screen.height - camPos.y - 25, 100, 50),
                  currentState.ToString());
        GUI.Label(new Rect(camPos.x - 10, Screen.height - camPos.y - 35, 100, 50),
                  currentArtilleryState.ToString());
    }

    public void EnemyEntersInVisionSphere (UnitController enemy)
    {
        Debug.Log("SEARCHING");
    }

} // class UnitArtillery
