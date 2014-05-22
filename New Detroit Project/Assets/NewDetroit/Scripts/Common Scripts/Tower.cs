using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tower : BuildingController
{

    public bool canBeConquered;

    protected float alertHitTimer = 1.0f;
    protected float alertHitTimerAux = 0.0f;

    // the vision radious of the tower
    public float visionSphereRadious = 10.0f;

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

    // reference to the dummy for the shot particles
    protected Transform shotDummy;

    protected float radius;

    public override void Awake ()
    {
        base.Awake();

        team.visionSphereRadius = visionSphereRadious;

        shotDummy = transform.FindChild("shot dummy");
    }

    // EnemyEntersInVisionSphere is called by the visions spheres
    public void EnemyEntersInVisionSphere (CTeam enemy)
    {
        ControllableCharacter enemyCC = enemy.GetComponent<ControllableCharacter>();
        if (enemyCC)
        {
            enemiesInside.Add(enemyCC);
        }

        /*// Adition of the enemy in the array enemiesInside
        if (enemiesInside.Count == 0)
        {
            enemiesInside.Add(enemy);
            //Debug.Log("First Enemy entered in TOWER");
        }
        else
            if (!enemiesInside.Contains(enemy))
            {
                enemiesInside.Add(enemy);
                //Debug.Log("New Enemy entered in TOWER");
            }
            //else
                //Debug.Log("Enemy already entered in TOWER");*/
    }

    // EnemyEntersInVisionSphere is called by the visions spheres
    public void EnemyExitsInVisionSphere(CTeam enemy)
    {
        // Removal of the enemy in the array enemiesInside
        ControllableCharacter enemyCC = enemy.GetComponent<ControllableCharacter>();
        if (enemyCC)
        {
            enemiesInside.Remove(enemyCC);
        }
        //Debug.Log("Enemy exits the TOWER");
    }

    // It is called when a team has conquered it. The units of this team have to leave the array enemiesInside
    protected void UpdateEnemiesInside (int team)
    {
        int max = enemiesInside.Count;
        int i = 0;
        int cont = 0;
        while (cont < max)
        {
            ControllableCharacter unit = enemiesInside[i].transform.GetComponent<ControllableCharacter>();
            int teamUnit = unit.GetTeamNumber();
            if (teamUnit == team)
            {
                enemiesInside.Remove(unit);
                i--;
            }
            i++;
            cont++;
        }
    }

    public float GetRadius()
    {
        return radius;
    }
}
