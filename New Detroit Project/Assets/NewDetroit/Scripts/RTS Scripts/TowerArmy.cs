using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerArmy : Tower 
{

    private bool isActive = false;
    private RaycastHit myHit; // Structure used to get information back from a raycast.
    private Ray myRay;
    private int layerMask; // para obviar la capa de la niebla
    private Vector3 destiny;
    
    public Material activeMaterial;
    public Material constructMaterial;

    // enum for the four states of the tower
    private enum TowerState
    {
        Destroyed,
        Idle,
        Alert, // espera hasta que halla hueco en la mina
        ShootingEnemies
    }

    // the state of the tower
    private TowerState currentTowerState = TowerState.Idle;

	// Conts for Tower conquest
	public float contConstr;
	
	// Constant when the tower is conquered
	private const float finalCont = 100.0f;
	
    // The distance over the floor
	private const int delta = 7;

	private bool constructed = false;
    public bool canConstruct = true;

    private int costResources = 10;
    // Reference to the base
    public BaseController baseController;

    public float TowerArmyRadius = 3.5f;

    public bool createCubes = false;

    public override void Awake ()
	{
        base.Awake();
        GetComponent<CSelectable>().enabled = false;
	}

	// Use this for initialization
	public override void Start () 
    {
		base.Start();
        radius = TowerArmyRadius;

        myHit = new RaycastHit();
        // ejemplo Unity: http://docs.unity3d.com/Documentation/Components/Layers.html
        // Bit shift the index of the layer (9) to get a bit mask
        layerMask = 1 << 9;
	}

    public bool StartConstruct (Vector3 destiny, BaseController baseController)
    {
        if (canConstruct && baseController.GetResources() >= costResources)
        {
            canConstruct = false;
            this.baseController = baseController;
            baseController.DecreaseResources(costResources);
            Vector3 posN = transform.position;
            posN.y = 0;
            transform.position = posN;
            Vector3 vecCOll = GetComponent<CapsuleCollider>().transform.position;
            Vector3 vecnav = GetComponent<NavMeshObstacle>().transform.position;
            GetComponent<CSelectable>().enabled = true;
            this.GetComponent<NavMeshObstacle>().enabled = true;
            isActive = true;

            float twoPi = Mathf.PI * 2;
            Vector3 center = transform.position;
            for (int i = 0; i < numEngineerPositions; i++)
            {
                /*
                Vector3 pos = new Vector3
                (
                    center.x +
                        (transform.GetComponent<BoxCollider>().size.x + despPosition) * Mathf.Sin(i * (twoPi / numEngineerPositions)),
                    0,
                    center.z +
                        (transform.GetComponent<BoxCollider>().size.x + despPosition) * Mathf.Cos(i * (twoPi / numEngineerPositions))
                );*/
                Vector3 pos = new Vector3
                (
                    center.x +
                    (radius + despPosition) * Mathf.Sin(i * (twoPi / numEngineerPositions)),
                    0,
                    center.z +
                    (radius + despPosition) * Mathf.Cos(i * (twoPi / numEngineerPositions))
                );
                engineerPositions[i] = pos;
                engineerPosTaken[i] = false;

                cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cubes[i].transform.position = pos;
                Destroy(cubes[i].GetComponent<BoxCollider>());
                cubes[i].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);
                cubes[i].transform.parent = this.transform;

                cubes[i].SetActive(createCubes);
            }
            DestroyUnnecessaryGameobjects();
            if (!PhotonNetwork.offlineMode)
                photonView.RPC("Constructed", PhotonTargets.All, transform.position, transform.rotation);
            return true;
        }
        return false;
    }

    public void SetActiveMaterial ()
    {
        renderer.material = activeMaterial;
    }

	public void SetConstructMaterial ()
	{
		renderer.material = constructMaterial;
	}
	
    public void SetBaseController (BaseController baseController)
    {
        this.baseController = baseController;
    }
    
	// Update is called once per frame
	public override void Update () 
    {
        base.Update();

        if (GetComponent<CSelectable>().IsSelected() && Input.GetKey(KeyCode.Delete))
        {
            //TODO que ingenieros cambien de estado
            Destroy(gameObject);
            baseController.IncreaseResources(costResources);
        }
        else if (!isActive)
        {
            Light light = transform.FindChild("Light").light;
            if (canConstruct)
            {
                light.color = Color.green;
                renderer.material.SetColor("_AlphaColor", Color.green);
            }
            else
            {
                light.color = Color.red;
                renderer.material.SetColor("_AlphaColor", Color.red);
            }
            myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(myRay, out myHit, 1000f, layerMask))
            {
                destiny = new Vector3(myHit.point.x, myHit.point.y + delta, myHit.point.z);
                transform.position = destiny;
            }
        }
        else if (constructed)
        {
            switch (currentTowerState)
            {
                case TowerState.Destroyed:

                    break;

                case TowerState.Idle:

                    // we change the state to neutral if the tower dies
                    if (life.currentLife <= 0.0f)
                        currentTowerState = TowerState.Destroyed;
                    // we change the state to Alert if there are enemies inside vision
                    if (enemiesInside.Count > 0)
                        currentTowerState = TowerState.Alert;
                    break;

                case TowerState.Alert:

                    if (alertHitTimerAux <= 0)
                    {
                        // launch a ray for each enemy inside the vision sphere
                        int count = enemiesInside.Count;
                        for (int i = 0; i < count; i++)
                        {
                            Vector3 pos = new Vector3(transform.position.x, 10, transform.position.z);
                            Debug.DrawLine(pos, enemiesInside[i].transform.position, Color.yellow, 0.3f);
                            Vector3 fwd = enemiesInside[i].transform.position - pos;
                            //Debug.Log("origen: " + transform.position + ". destino: " + enemiesInside[i].transform.position + ". direccion: " + fwd);
                            fwd.Normalize();
                            Vector3 aux = pos + (fwd * (visionSphereRadious + 10));
                            //Vector3 aux = new Vector3(fwd.x * transform.position.x, fwd.y * transform.position.y, fwd.z * transform.position.z);
                            Debug.DrawLine(pos, aux /** visionSphereRadious*/, Color.blue, 0.3f);
                            RaycastHit myHit;
                            if (Physics.Raycast(pos, fwd, out myHit, aux.magnitude + 2))
                            {
                                // the ray has hit something
                                CTeam enemy = myHit.transform.GetComponent<CTeam>();
                                if ((enemy != null) && (enemy == enemiesInside[i]))
                                {
                                    // this "something" is the enemy we are looking for...
                                    //Debug.Log("LE HE DADO!!!");
                                    lastEnemyAttacked = enemy;
                                    alertHitTimerAux = alertHitTimer;
                                    currentTowerState = TowerState.ShootingEnemies;
                                }
                            }
                        }
                        // reset the timer
                        alertHitTimerAux = alertHitTimer;
                    }
                    else
                    {
                        alertHitTimerAux -= Time.deltaTime;
                    }
                    // we change the state to Iddle if there aren't enemies inside vision
                    if (enemiesInside.Count == 0)
                        currentTowerState = TowerState.Idle;
                    // we change the state to neutral if the tower dies
                    if (life.currentLife <= 0.0f)
                        currentTowerState = TowerState.Destroyed;
                    break;

                case TowerState.ShootingEnemies:

                    // if it can shoot
                    if (attackCadenceAux <= 0.0f)
                    {
                        // Attack!
                        Debug.DrawLine(transform.position, lastEnemyAttacked.transform.position, Color.red, 0.2f);
                        // emite some particles:
                        GameObject particles = (GameObject)Instantiate
                        (
                            shotParticles,
                            shotDummy.position,
                            transform.rotation
                        );
                        Destroy(particles, 0.4f);
                        // first we check if the enemy is now alive
                        //ControllableCharacter lastEnemyAtackedUC = (ControllableCharacter)lastEnemyAttacked;
                        //if (lastEnemyAttacked.Damage(attackPower, 'P'))
                        if (PhotonNetwork.connected)
                        	photonView.RPC("Kick", PhotonTargets.All, lastEnemyAttacked.name, attackPower);
                        else
                        	Kick(lastEnemyAttacked.name, attackPower);
                        if (lastEnemyAttacked.GetComponent<CLife>().currentLife <= 0.0f)
                        {
                            // the enemy died, time to reset the lastEnemyAttacked reference
                            enemiesInside.Remove(lastEnemyAttacked);
                            if (enemiesInside.Count == 0)
                            {
                                lastEnemyAttacked = null;
                                // no more enemies, change the state
                                currentTowerState = TowerState.Idle;
                            }
                            else
                            {
                                // there are more enemies, we have to see if can be shooted
                                currentTowerState = TowerState.Alert;
                            }
                        }
                        // reset the timer
                        attackCadenceAux = attackCadence;
                    }
                    else
                        attackCadenceAux -= Time.deltaTime;
                    // we change the state to neutral if the tower dies
                    if (life.currentLife <= 0.0f)
                        currentTowerState = TowerState.Destroyed;
                    // we change the state to Iddle if there aren't enemies inside vision
                    if (enemiesInside.Count == 0)
                        currentTowerState = TowerState.Idle;

                    break;
            }
        }

	}// Update

	public virtual void OnGUI ()
	{
        Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
        Rect rect1;
        Rect rect2;
        if (constructed)
        {
            rect1 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 100.0f, 120.0f, 4.0f);
            rect2 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 100.0f, 120.0f * (life.currentLife / life.maximunLife), 4.0f);
        }
        else
        {
            rect1 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 100.0f, 120.0f, 4.0f);
            rect2 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 100.0f, 120.0f * (contConstr / finalCont), 4.0f);
        }
        GUI.DrawTexture(rect1, progressBarEmpty);
        GUI.DrawTexture(rect2, progressBarFull);
	}

	/*private bool CanConstruct()
	{
        RaycastHit theHit; // Structure used to get information back from a raycast.
        Ray aRay; // the ray

        aRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        return true;
	}*/

    public void SetCanConstruct (bool b)
    {
        canConstruct = b;
    }

    public void DestroyUnnecessaryGameobjects ()
    {
        // Remove unnecessary GameObjects
        Destroy(transform.FindChild("TowerSphereConstruct").gameObject);
        Destroy(transform.FindChild("Light").light);
    }

	// Construct is called by the engineers
	public bool Construct (float sum)
	{
        // increasement of the towers life
        if (!constructed)
        {
            if (contConstr < finalCont)
            {
                contConstr += sum;
                if (finalCont < contConstr)
                    contConstr = finalCont;
            }
            if (contConstr == finalCont)
            {
                life.currentLife = 50.0f;
                UpdateEnemiesInside(team.teamNumber);
                RemoveEngineersInQueue();
                for (int i = 0; i < numEngineerPositions; i++)
                    cubes[i].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);
                constructed = true;           
                //else
                //    Constructed(transform.position, transform.rotation);
                // insert the tower in the DistanceMeasurerTool
                DistanceMeasurerTool.InsertUnit(team);
                return true;
            }
            else
                return false;
        }
        return true;
	}

	[RPC]
	public void Constructed(Vector3 position, Quaternion rotation)
	{
		this.transform.rotation = rotation;
		this.transform.position = position;
		this.gameObject.SetActive(true);
	}
	
	public bool HasATeam ()
	{
		return team.teamNumber != -1;
	}

	public bool IsConstructed ()
	{
		return constructed;
	}

    public int NumEngineerPosTaken()
    {
        int cont = 0;
        foreach(bool pos in engineerPosTaken)
        {
            if (pos)
                cont++;
        }
        return cont;
    }

    public void LeaveEngineerPositionConstruct (int index)
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

}
