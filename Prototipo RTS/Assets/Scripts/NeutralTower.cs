﻿using UnityEngine;
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
				

				break;

			case TowerState.Searching:
			
				break;

			case TowerState.ShootingEnemies:
			break;
				
		}
	}
	
	public void EnemyEntersInVisionSphere (UnitController enemy)
	{

		if (currentTowerState == TowerState.Iddle) 
		{
			currentTowerState = TowerState.Searching;
			//unitsSearched.Add(enemy);
			Debug.Log ("Enemy entered in TOWER");
		}
		else
		{
		
		}
	}
}
