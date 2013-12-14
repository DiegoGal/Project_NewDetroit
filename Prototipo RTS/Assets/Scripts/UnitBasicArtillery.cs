using UnityEngine;
using System.Collections;

public class UnitBasicArtillery : UnitController
{

	private enum ArtilleryState
	{
		None,
		Searching,
		Attacking,
		Chasing
	}
	private ArtilleryState currentArtilleryState = ArtilleryState.None;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		base.Update();
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

} // class UnitBasicArtillery
