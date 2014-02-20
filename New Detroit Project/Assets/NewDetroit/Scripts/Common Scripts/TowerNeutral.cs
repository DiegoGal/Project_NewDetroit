using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerNeutral : MonoBehaviour {

	// the team number
	public int teamNumber = -1;

	// the number of teams playing
	public int teamsPlaying = 1;
	// enum for the four states of the tower
	private enum TowerState
	{
		Neutral,
		Iddle,
		Alert, // espera hasta que halla hueco en la mina
		ShootingEnemies
	}

	// the state of the tower
	private TowerState currentTowerState = TowerState.Neutral;
	
	private float alertHitTimer = 1.0f;
	private float alertHitTimerAux = 0.0f;

	// the vision radious of the tower
	protected float visionSphereRadious;

	// the attack cadence
	private float attackCadenceAux = 0.0f;

	// frecuencia (en segundos) de ataque primario
	public float attackCadence = 1.0f;
	
	protected ControllableCharacter lastEnemyAttacked;

	public GameObject shotParticles;

	// health bar
	public Texture2D progressBarEmpty, progressBarFull;

	//attack power of the tower
	private float attackPower = 10;

	//Conts for Tower conquest
	private float[] contConq;

	//Constant when the tower is conquered
	private const float finalCont = 100.0f;

	//Constant for the life of the tower
	private const float totalLife = 100.0f;

	//The currentLife of the Tower
	private float currentLife = 0.0f;

	//the list of enemies inside the tower vision
	private List<ControllableCharacter> enemiesInside = new List<ControllableCharacter>();

	//********************************************************************************
	// For engineers

	// the distance to conquest and repair
	public float distanceToWait = 2.0f;

	// the number of the engineers that can conquest and repair the tower
	public int numEngineerPositions = 8;
	private Vector3[] engineerPositions;
	private bool[] engineerPosTaken;
	
	// displacement of the engineer positions
	public float despPosition = 1.4f;

	// Queue of units engineers which are waiting in the item
	private List<UnitEngineer> engineerQueue;

	// for debugging
	private GameObject[] cubes;

	//********************************************************************************

	// For decrease the contConq if it is not conquering
	private float[] actualTimeConquering;
	private float conqueringTime = 4;
	private float amountToSubstract = 1;

	// Use this for initialization
	void Start () {
	
		// inicialization of the contConq and actualTimeConquering arrays depending of the number of teams playing
		contConq = new float[teamsPlaying];
		actualTimeConquering = new float[teamsPlaying];
		for (int i = 0; i < teamsPlaying; i++)
		{
			contConq[i] = 0;
			actualTimeConquering[i] = 0;
		}

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
					(transform.GetComponent<BoxCollider>().size.x + despPosition)*Mathf.Sin(i*(twoPi/numEngineerPositions)),
					0,
					center.z +
					(transform.GetComponent<BoxCollider>().size.x + despPosition)*Mathf.Cos(i*(twoPi/numEngineerPositions))
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
	void Update () {
		
		switch (currentTowerState) 
		{
		case TowerState.Neutral:

			// we have to see if there are teams that aren't conquering
			for (int i = 0; i < teamsPlaying; i++)
			{
				actualTimeConquering[i] += Time.deltaTime;
				if (actualTimeConquering[i] > conqueringTime)
				{
					lessConquest(i);
					actualTimeConquering[i] = 0;
				}
			}
			break;
			
		case TowerState.Iddle:

			// we change the state to neutral if the tower dies
			if (currentLife <= 0.0f)
				currentTowerState = TowerState.Neutral;
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
				currentTowerState = TowerState.Neutral;
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
				currentTowerState = TowerState.Neutral;
			// we change the state to Iddle if there aren't enemies inside vision
			if (enemiesInside.Count == 0)
				currentTowerState = TowerState.Iddle;
			break;	
		}
	}

	// EnemyEntersInVisionSphere is called by the visions spheres
	public void EnemyEntersInVisionSphere (ControllableCharacter enemy)
	{
		// Adition of the enemy in the array enemiesInside
		if (enemiesInside.Count == 0) 
		{
			enemiesInside.Add(enemy);
			Debug.Log ("First Enemy entered in TOWER");
		}
		else
			if (!enemiesInside.Contains(enemy))
			{
				enemiesInside.Add(enemy);
				Debug.Log ("New Enemy entered in TOWER");
			}
			else
				Debug.Log ("Enemy already entered in TOWER");
	}

	// EnemyEntersInVisionSphere is called by the visions spheres
	public void EnemyExitsInVisionSphere (ControllableCharacter enemy)
	{
		// Removal of the enemy in the array enemiesInside
		enemiesInside.Remove(enemy);
		Debug.Log ("Enemy exits the TOWER");
	}
	
	public bool IsCurrentStateNeutral ()
	{
		return currentTowerState == TowerState.Neutral;
	}

	// Repair is called by the engineers
	public bool Repair (float sum)
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
			LeaveAllEngineerPositions ();
			return true;
		}
		else
			return false;
	}

	// Conquest is called by the engineers
	public bool Conquest (float sum, int team)
	{
		actualTimeConquering[team] = 0;
		contConq[team] += sum;
		if (contConq[team] >= finalCont) 
		{
			for (int i = 0; i < teamsPlaying; i++)
				contConq[i] = 0;
			currentLife = 80.0f;
			teamNumber = team;
			currentTowerState = TowerState.Iddle;
			UpdateEnemiesInside(team);
			LeaveAllEngineerPositions();
			return true;
		}
		return false;
	}

	// It's called when a team isn't repairing and the state is neutral
	private void lessConquest (int team)
	{
		if (contConq[team] > 0)
			contConq[team] -= amountToSubstract;
	}

	public void Damage (float damage)
	{
		//Debug.Log("damage");
		currentLife -= damage;
		// blood!
		GameObject blood = (GameObject)Instantiate(shotParticles,
		                                           transform.position + transform.forward, transform.rotation);
		Destroy(blood, 0.4f);

	}

	void OnCollisionEnter (Collision collision)
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

	public virtual void OnGUI()
	{
		Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);

		// rectángulo donde se dibujará la barra de conquista
		if (currentTowerState == TowerState.Neutral) 
		{
				GUI.skin.label.fontSize = 10;

				// Team 0
				GUI.Label(new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 130.0f, 120.0f, 50),
			          "Team 0 conquered");
				Rect rect3 = new Rect (camPos.x - 60.0f, Screen.height - camPos.y - 110.0f, 120.0f, 4.0f);
				GUI.DrawTexture (rect3, progressBarEmpty);
				Rect rect4 = new Rect (camPos.x - 60.0f, Screen.height - camPos.y - 110.0f, 120.0f * (contConq [0] / finalCont), 4.0f);
				GUI.DrawTexture (rect4, progressBarFull);
				
				// Team 1
				GUI.Label(new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 100.0f, 120.0f, 50),
			          "Team 1 conquered");
				Rect rect5 = new Rect (camPos.x - 60.0f, Screen.height - camPos.y - 80.0f, 120.0f, 4.0f);
				GUI.DrawTexture (rect5, progressBarEmpty);
				Rect rect6 = new Rect (camPos.x - 60.0f, Screen.height - camPos.y - 80.0f, 120.0f * (contConq [1] / finalCont), 4.0f);
				GUI.DrawTexture (rect6, progressBarFull);
		}
		else
		{
			
			// rectángulo donde se dibujará la barra de vida
			Rect rect1 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 200.0f, 120.0f, 4.0f);
			GUI.DrawTexture(rect1, progressBarEmpty);
			Rect rect2 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 200.0f, 120.0f * (currentLife/totalLife), 4.0f);
			GUI.DrawTexture(rect2, progressBarFull);

		}
	}

	public bool GetEngineerPosition (ref Vector3 pos, ref int index, UnitEngineer unit)
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

	public void LeaveAllEngineerPositions ()
	{
		for (int i = 0; i < numEngineerPositions; i++)
		{
			LeaveEngineerPosition(i);
		}
	}

	public void LeaveEngineerPosition (int index)
	{
		engineerPosTaken[index] = false;
		cubes[index].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);
		// If there are more engineers waiting 
		if (engineerQueue.Count > 0)
		{
			UnitEngineer unit = engineerQueue[0];
			if (currentTowerState == TowerState.Neutral)
				unit.FinishWaitingToConquest(engineerPositions[index], index);
			else
				unit.FinishWaitingToRepair(engineerPositions[index], index);
			engineerQueue.RemoveAt(0);
			engineerPosTaken[index] = true;
			cubes[index].renderer.material.color = new Color(0.863f, 0.078f, 0.235f);
		}
	}
	
	public void LeaveQueue (UnitEngineer unit)
	{
		engineerQueue.Remove(unit);
	}

	public int GetTeamNumber()
	{
		return teamNumber;
	}

	// It is called when a team has conquered it. The units of this team have to leave the array enemiesInside
	private void UpdateEnemiesInside(int team)
	{
		int max = enemiesInside.Count;
		int i = 0;
		int cont = 0;
		while(cont < max)
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
