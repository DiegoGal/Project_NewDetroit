using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tower : BuildingController
{

    public bool canBeConquered;

    protected float alertHitTimer = 1.0f;
    protected float alertHitTimerAux = 0.0f;

    // the vision radious of the tower
    protected float visionSphereRadious;

    // the attack cadence
    protected float attackCadenceAux = 0.0f;

    // frecuency (in secs) of the primary attack
    public float attackCadence = 1.0f;

    protected ControllableCharacter lastEnemyAttacked;

    public GameObject shotParticles;

    //attack power of the tower
    protected float attackPower = 10;

    //the list of enemies inside the tower vision
    protected List<ControllableCharacter> enemiesInside = new List<ControllableCharacter>();

	// Use this for initialization
    public override void Start() 
    {
        base.Start();
        visionSphereRadious = transform.FindChild("TowerVisionSphere").GetComponent<SphereCollider>().radius;
	}
	

	// Update is called once per frame
	public override void Update() 
    {
        base.Update();
	}

    // EnemyEntersInVisionSphere is called by the visions spheres
    public void EnemyEntersInVisionSphere (ControllableCharacter enemy)
    {
        // Adition of the enemy in the array enemiesInside
        if (enemiesInside.Count == 0)
        {
            enemiesInside.Add(enemy);
            Debug.Log("First Enemy entered in TOWER");
        }
        else
            if (!enemiesInside.Contains(enemy))
            {
                enemiesInside.Add(enemy);
                Debug.Log("New Enemy entered in TOWER");
            }
            else
                Debug.Log("Enemy already entered in TOWER");
    }

    // EnemyEntersInVisionSphere is called by the visions spheres
    public void EnemyExitsInVisionSphere (ControllableCharacter enemy)
    {
        // Removal of the enemy in the array enemiesInside
        enemiesInside.Remove(enemy);
        Debug.Log("Enemy exits the TOWER");
    }

    // It is called when a team has conquered it. The units of this team have to leave the array enemiesInside
    protected void UpdateEnemiesInside(int team)
    {
        int max = enemiesInside.Count;
        int i = 0;
        int cont = 0;
        while (cont < max)
        {
            ControllableCharacter unit = enemiesInside[i].transform.GetComponent<ControllableCharacter>();
            int teamUnit = unit.teamNumber;
            if (teamUnit == team)
            {
                enemiesInside.Remove(unit);
                i--;
            }
            i++;
            cont++;
        }
    }

}
