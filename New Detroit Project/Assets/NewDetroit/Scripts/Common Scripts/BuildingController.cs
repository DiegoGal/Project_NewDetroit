using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BuildingController : Photon.MonoBehaviour 
{

    // health bar
    public Texture2D progressBarEmpty, progressBarFull;

    // reference to the Life component of the building
    public CLife life;

    // reference to the Team component of the building
    public CTeam team;

    // the damage particles for when the building has been hit
    public GameObject damageParticles;

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

    public virtual void Awake()
    {
        life = GetComponent<CLife>();
        team = GetComponent<CTeam>();
    }

	// Use this for initialization
    public virtual void Start () 
    {
        // setting of the distance to wait
        if (transform.GetComponent<BoxCollider>())
            distanceToWait += transform.GetComponent<BoxCollider>().size.x + despPosition;
        else if (transform.GetComponent<CapsuleCollider>())
            distanceToWait += transform.GetComponent<CapsuleCollider>().radius + despPosition;
        else
            distanceToWait += distanceToWait;

        // inicialization of the engineerPositions and engineerPosTaken arrays depending of tnumEngineerPositions
        engineerPositions = new Vector3[numEngineerPositions];
        engineerPosTaken = new bool[numEngineerPositions];

        cubes = new GameObject[numEngineerPositions];
        // inicialization of the engineer queue
        engineerQueue = new List<UnitEngineer>();
	}
	
	// Update is called once per frame
    public virtual void Update () 
    {
	
	}

    // Repair is called by the engineers
    public bool Repair (float sum)
    {
        // increasement of the towers life
        if (life.currentLife < life.maximunLife)
        {
            life.currentLife += sum;
            if (life.maximunLife < life.currentLife)
                life.currentLife = life.maximunLife;
        }
        if (life.currentLife == life.maximunLife)
        {
            RemoveEngineersInQueue();
            for (int i = 0; i < numEngineerPositions; i++)
                cubes[i].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);
            return true;
        }
        else
            return false;
    }

    public void Damage (float damage)
    {
        //Debug.Log("damage");
        life.currentLife -= damage;
        // blood!
        GameObject particles = (GameObject)Instantiate
        (
            damageParticles,
            transform.position + transform.forward,
            transform.rotation
        );
        Destroy(particles, 0.4f);

    }

    public void LeaveQueue(UnitEngineer unit)
    {
        engineerQueue.Remove(unit);
    }

    public void LeaveEngineerPositionRepair (int index)
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

    public bool GetEngineerPosition (ref Vector3 pos, ref int index, UnitEngineer unit, bool conquest)
    {
        int i = 0; bool found = false;
        while (!found && (i < numEngineerPositions))
        {
            if (!engineerPosTaken[i])
            {
                pos = engineerPositions[i];
                index = i;
                //engineerPosTaken[i] = true;
                if (conquest && !PhotonNetwork.offlineMode)
                    photonView.RPC("MorePositionsTaken", PhotonTargets.All, i);
                else engineerPosTaken[i] = true;

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

    [RPC]
    public void MorePositionsTaken(int i)
    {
        engineerPosTaken[i] = true;
        cubes[i].renderer.material.color = new Color(0.863f, 0.078f, 0.235f);
    }

    public void SetTeamNumber (int teamNumber, int teamColorIndex)
    {
        team.teamNumber = teamNumber;

        CTeam cteam = GetComponent<CTeam>();
        cteam.teamNumber = teamNumber;
        cteam.teamColorIndex = teamColorIndex;

        CSelectable csel = GetComponent<CSelectable>();
        if (csel)
            csel.ResetTeamColor();
    }

    protected void RemoveEngineersInQueue ()
    {
        engineerQueue.Clear();
        for (int i = 0; i < numEngineerPositions; i++)
            engineerPosTaken[i] = false;
    }

    public bool HasTotalLife ()
    {
        return life.currentLife == life.maximunLife;
    }
}
