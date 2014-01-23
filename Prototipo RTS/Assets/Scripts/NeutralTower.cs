using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NeutralTower : MonoBehaviour {

	public int teamNumber;

	private enum TowerState
	{
		Neutral,
		Iddle,
		Searching, // espera hasta que halla hueco en la mina
		ShootingEnemies
	}

	private TowerState currentTowerState = TowerState.Iddle;

	//Conts for Tower conquest
	private float contConq1 = 0.0f;
	private float contConq2 = 0.0f;

	//Constant when the tower is conquered
	private const float finalCont = 100.0f;

	//The life of the Tower
	private float life = 100.0f;

	private List<GameObject> unitsSearched = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		switch (currentTowerState) 
		{
		case TowerState.Neutral:
			
			
			break;
			
		case TowerState.Iddle:
			
			if (unitsSearched.Count > 0)
				currentTowerState = TowerState.Searching; 
			break;
			
		case TowerState.Searching:
			
			if (unitsSearched.Count == 0)
				currentTowerState = TowerState.Iddle;
			break;
			
		case TowerState.ShootingEnemies:
			
			if (unitsSearched.Count == 0)
				currentTowerState = TowerState.Iddle;
			break;	
		}
	}
	
	public void EnemyEntersInVisionSphere (GameObject enemy)
	{
		
		if (unitsSearched.Count == 0) 
		{
			currentTowerState = TowerState.Searching;
			unitsSearched.Add(enemy);
			Debug.Log ("First Enemy entered in TOWER");
		}
		else
		{
			if (!unitsSearched.Contains(enemy))
			{
				unitsSearched.Add(enemy);
				Debug.Log ("New Enemy entered in TOWER");
			}
			else
			{
				Debug.Log ("Enemy already entered in TOWER");
			}
			
		}
	}
	
	public void EnemyExitsInVisionSphere (GameObject enemy)
	{
		unitsSearched.Remove(enemy);
		Debug.Log ("Enemy exits the TOWER");
	}
}
