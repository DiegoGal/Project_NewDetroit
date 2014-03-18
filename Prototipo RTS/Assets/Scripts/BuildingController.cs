﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingController : MonoBehaviour 
{

    // the team number
    public int teamNumber = -1;

    protected float alertHitTimer = 1.0f;
    protected float alertHitTimerAux = 0.0f;

    // health bar
    public Texture2D progressBarEmpty, progressBarFull;

    //Constant for the life of the tower
    protected const float totalLife = 100.0f;

    //The currentLife of the Tower
    protected float currentLife = 0.0f;

    public GameObject shotParticles;

    //********************************************************************************
    // For engineers

    // the distance to construct, conquest and repair
    public float distanceToWait = 2.0f;

    // the number of the engineers that can construct, conquest and repair the tower
    public int numEngineerPositions = 8;
    protected Vector3[] engineerPositions;
    protected bool[] engineerPosTaken;

    // displacement of the engineer positions
    public float despPosition = 1.4f;

    // Queue of units engineers which are waiting in the item
    protected List<UnitEngineer> engineerQueue;

    // for debugging
    protected GameObject[] cubes;

    //********************************************************************************

	// Use this for initialization
    public virtual void Start() 
    {
        // setting of the distance to wait
        distanceToWait += transform.GetComponent<BoxCollider>().size.x + despPosition;

        // inicialization of the engineerPositions and engineerPosTaken arrays depending of tnumEngineerPositions
        engineerPositions = new Vector3[numEngineerPositions];
        engineerPosTaken = new bool[numEngineerPositions];

        cubes = new GameObject[numEngineerPositions];
        // inicialization of the engineer queue
        engineerQueue = new List<UnitEngineer>();
	}
	
	// Update is called once per frame
    public virtual void Update() 
    {
	
	}

    // Repair is called by the engineers
    public bool Repair(float sum)
    {
        // increasement of the towers life
        if (currentLife < totalLife)
        {
            currentLife += sum;
            if (totalLife < currentLife)
                currentLife = totalLife;
        }
        if (currentLife == totalLife)
        {
            RemoveEngineersInQueue();
            for (int i = 0; i < numEngineerPositions; i++)
                cubes[i].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);
            return true;
        }
        else
            return false;
    }

    public void Damage(float damage)
    {
        //Debug.Log("damage");
        currentLife -= damage;
        // blood!
        GameObject blood = (GameObject)Instantiate(shotParticles,
                                                   transform.position + transform.forward, transform.rotation);
        Destroy(blood, 0.4f);

    }

    public void LeaveQueue(UnitEngineer unit)
    {
        engineerQueue.Remove(unit);
    }

    public int GetTeamNumber()
    {
        return teamNumber;
    }

    public void LeaveEngineerPositionRepair(int index)
    {
        engineerPosTaken[index] = false;
        cubes[index].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);
        if (engineerQueue.Count > 0)
        {
            UnitEngineer unit = engineerQueue[0];
            unit.FinishWaitingToRepair(engineerPositions[index], index);
            engineerQueue.RemoveAt(0);
            engineerPosTaken[index] = true;
            cubes[index].renderer.material.color = new Color(0.863f, 0.078f, 0.235f);
        }
    }

    public bool GetEngineerPosition(ref Vector3 pos, ref int index, UnitEngineer unit)
    {
        int i = 0; bool found = false;
        while (!found && (i < numEngineerPositions))
        {
            if (!engineerPosTaken[i])
            {
                pos = engineerPositions[i];
                index = i;
                engineerPosTaken[i] = true;
                cubes[i].renderer.material.color = new Color(0.863f, 0.078f, 0.235f);
                found = true;
            }
            else
                i++;
        }
        if (!found)
            engineerQueue.Add(unit);
        return found;
    }

    public void SetTeamNumber(int teamNumber)
    {
        this.teamNumber = teamNumber;
    }

    protected void RemoveEngineersInQueue()
    {
        engineerQueue.Clear();
        for (int i = 0; i < numEngineerPositions; i++)
            engineerPosTaken[i] = false;
    }

    public bool HasTotalLife()
    {
        return currentLife == totalLife;
    }
}