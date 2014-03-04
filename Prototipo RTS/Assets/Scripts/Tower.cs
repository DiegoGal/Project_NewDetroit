using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tower : MonoBehaviour {

    // the team number
    public int teamNumber = -1;

    public bool canBeConquered;

    protected float alertHitTimer = 1.0f;
    protected float alertHitTimerAux = 0.0f;

    // the vision radious of the tower
    protected float visionSphereRadious;

    // the attack cadence
    protected float attackCadenceAux = 0.0f;

    // frecuencia (en segundos) de ataque primario
    public float attackCadence = 1.0f;

    protected ControllableCharacter lastEnemyAttacked;

    public GameObject shotParticles;

    // health bar
    public Texture2D progressBarEmpty, progressBarFull;

    //attack power of the tower
    protected float attackPower = 10;

    //Constant for the life of the tower
    protected const float totalLife = 100.0f;

    //The currentLife of the Tower
    protected float currentLife = 0.0f;

    //the list of enemies inside the tower vision
    protected List<ControllableCharacter> enemiesInside = new List<ControllableCharacter>();

    //********************************************************************************
    // For engineers

    // the distance to conquest and repair
    public float distanceToWait = 2.0f;

    // the number of the engineers that can conquest and repair the tower
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
	protected virtual void Start () 
    {

        // setting of the distance to wait
        distanceToWait += transform.GetComponent<BoxCollider>().size.x + despPosition;

        // inicialization of the engineerPositions and engineerPosTaken arrays depending of tnumEngineerPositions
        engineerPositions = new Vector3[numEngineerPositions];
        engineerPosTaken = new bool[numEngineerPositions];

        cubes = new GameObject[numEngineerPositions];

        float twoPi = Mathf.PI * 2;
        Vector3 center = transform.position;
        for (int i = 0; i < numEngineerPositions; i++)
        {
            Vector3 pos = new Vector3
                (
                    center.x +
                    (transform.GetComponent<BoxCollider>().size.x + despPosition) * Mathf.Sin(i * (twoPi / numEngineerPositions)),
                    0,
                    center.z +
                    (transform.GetComponent<BoxCollider>().size.x + despPosition) * Mathf.Cos(i * (twoPi / numEngineerPositions))
                    );
            engineerPositions[i] = pos;
            engineerPosTaken[i] = false;

            cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubes[i].transform.position = pos;
            Destroy(cubes[i].GetComponent<BoxCollider>());
            cubes[i].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);
            cubes[i].transform.parent = this.transform;
        }

        // inicialization of the engineer queue
        engineerQueue = new List<UnitEngineer>();

        visionSphereRadious = transform.FindChild("TowerVisionSphere").GetComponent<SphereCollider>().radius;
	
	}
	

	// Update is called once per frame
    protected virtual void Update() 
    {
	    
	}

    // EnemyEntersInVisionSphere is called by the visions spheres
    public void EnemyEntersInVisionSphere(ControllableCharacter enemy)
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
    public void EnemyExitsInVisionSphere(ControllableCharacter enemy)
    {
        // Removal of the enemy in the array enemiesInside
        enemiesInside.Remove(enemy);
        Debug.Log("Enemy exits the TOWER");
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
            return true;
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

    void OnCollisionEnter(Collision collision)
    {
        UnitEngineer unit = collision.transform.GetComponent<UnitEngineer>();
        if (unit != null)
        {
            //Debug.Log("COLISION2");

            // esto es interesante para instanciar partículas cuando se esté recolectando:
            /*ContactPoint contact = collision.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;
            Instantiate(explosionPrefab, pos, rot) as Transform;
            Destroy(gameObject);*/

            //unit.StartChoping();
        }
    }

    public void LeaveQueue(UnitEngineer unit)
    {
        engineerQueue.Remove(unit);
    }

    public int GetTeamNumber()
    {
        return teamNumber;
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

    public void LeaveEngineerPositionConquest(int index)
    {
        engineerPosTaken[index] = false;
        cubes[index].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);
        if (engineerQueue.Count > 0)
        {
            UnitEngineer unit = engineerQueue[0];
            
            unit.FinishWaitingToConquest(engineerPositions[index], index);
            engineerQueue.RemoveAt(0);
            engineerPosTaken[index] = true;
            cubes[index].renderer.material.color = new Color(0.863f, 0.078f, 0.235f);
        }
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

    public void LeaveEngineerPositionConstruct(int index)
    {
        engineerPosTaken[index] = false;
        cubes[index].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);
        if (engineerQueue.Count > 0)
        {
            UnitEngineer unit = engineerQueue[0];
            unit.FinishWaitingToConstruct(engineerPositions[index], index);
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
}
