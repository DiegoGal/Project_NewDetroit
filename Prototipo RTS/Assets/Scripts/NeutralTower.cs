using UnityEngine;
using System.Collections;

public class NeutralTower : MonoBehaviour {

	private enum TowerState
	{
		Neutral,
		Iddle,
		Searching, // espera hasta que halla hueco en la mina
		ShootingEnemies
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void EnemyEntersInVisionSphere (UnitController enemy)
	{
		Debug.Log("TOWER SEARCHING");
	}
}
