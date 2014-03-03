using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerGoblin : Tower 
{

    private bool active = false;
    private RaycastHit myHit; // Structure used to get information back from a raycast.
    private Ray myRay;
    private int layerMask; // para obviar la capa de la niebla
    private Vector3 destiny;
    
    public Material activeMaterial;

    // enum for the four states of the tower
    private enum TowerState
    {
        Destroyed,
        Iddle,
        Alert, // espera hasta que halla hueco en la mina
        ShootingEnemies
    }

    // the state of the tower
    private TowerState currentTowerState = TowerState.Iddle;

   
	// Use this for initialization
	void Start () 
    {
       	
        myHit = new RaycastHit();
        // ejemplo Unity: http://docs.unity3d.com/Documentation/Components/Layers.html
        // Bit shift the index of the layer (8) to get a bit mask
        layerMask = 1 << 8 | 1 << 2;

        // This would cast rays only against colliders in layer 8 and 2.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

		base.Start();
	}

    public void Construct(Vector3 destiny)
    {
        active = true;
        renderer.material = activeMaterial;
		this.GetComponent<NavMeshObstacle>().enabled = true;
    }

	// Update is called once per frame
	void Update () 
    {
       
        if (!active)
        {
            myRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(myRay, out myHit, 1000f, layerMask))
            {
				destiny = new Vector3 (myHit.point.x, 0, myHit.point.z);
				transform.position = destiny;
            }
        }
		else
		{
			switch (currentTowerState) 
			{
			case TowerState.Destroyed:

				break;
				
			case TowerState.Iddle:
				
				// we change the state to neutral if the tower dies
				if (currentLife <= 0.0f)
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
						Debug.DrawLine(transform.position, enemiesInside[i].transform.position, Color.yellow, 0.2f);
						
						Vector3 fwd = enemiesInside[i].transform.position - this.transform.position;
						//Debug.Log("origen: " + transform.position + ". destino: " + enemiesInside[i].transform.position + ". direccion: " + fwd);
						fwd.Normalize();
						Vector3 aux = new Vector3(fwd.x * transform.position.x, fwd.y * transform.position.y, fwd.z * transform.position.z);
						Debug.DrawLine(transform.position, aux /** visionSphereRadious*/, Color.blue, 0.3f);
						RaycastHit myHit;
						if (Physics.Raycast(transform.position, fwd, out myHit))
						{
							// the ray has hit something
							ControllableCharacter enemy = myHit.transform.GetComponent<ControllableCharacter>();
							if ( (enemy != null) && (enemy == enemiesInside[i]) )
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
					currentTowerState = TowerState.Iddle;
				// we change the state to neutral if the tower dies
				if (currentLife <= 0.0f)
					currentTowerState = TowerState.Destroyed;
				break;
				
			case TowerState.ShootingEnemies:
				
				// if it can shoot
				if (attackCadenceAux <= 0.0f)
				{
					// Attack!
					Debug.DrawLine(transform.position, lastEnemyAttacked.transform.position, Color.red, 0.2f);
					// emite some particles:
					Vector3 auxPos = new Vector3(transform.position.x, 10.0f + transform.position.y,transform.position.z);
					GameObject particles = (GameObject)Instantiate(shotParticles,
					                                               auxPos,
					                                               transform.rotation);
					Destroy(particles, 0.4f);
					// first we check if the enemy is now alive
					//ControllableCharacter lastEnemyAtackedUC = (ControllableCharacter)lastEnemyAttacked;
					if (lastEnemyAttacked.Damage(attackPower))
					{
						// the enemy died, time to reset the lastEnemyAttacked reference
						enemiesInside.Remove(lastEnemyAttacked);
						if (enemiesInside.Count == 0)
						{
							lastEnemyAttacked = null;
							// no more enemies, change the state
							currentTowerState = TowerState.Iddle;
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
				if (currentLife <= 0.0f)
					currentTowerState = TowerState.Destroyed;
				// we change the state to Iddle if there aren't enemies inside vision
				if (enemiesInside.Count == 0)
					currentTowerState = TowerState.Iddle;
				break;	
			}
		}
	}// Update

	public virtual void OnGUI()
	{
		Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
			
		// rectángulo donde se dibujará la barra de vida
		Rect rect1 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 200.0f, 120.0f, 4.0f);
		GUI.DrawTexture(rect1, progressBarEmpty);
		Rect rect2 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 200.0f, 120.0f * (currentLife/totalLife), 4.0f);
		GUI.DrawTexture(rect2, progressBarFull);

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
	
	public void LeaveEngineerPosition(int index)
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

	private bool CanConstruct()
	{
        RaycastHit theHit; // Structure used to get information back from a raycast.
        Ray aRay; // the ray

        aRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        return true;
	}
}
