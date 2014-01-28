using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NeutralTower : MonoBehaviour {

	public int teamNumber;

	private enum TowerState
	{
		Neutral,
		Iddle,
		Alert, // espera hasta que halla hueco en la mina
		ShootingEnemies
	}

	private TowerState currentTowerState = TowerState.Neutral;

	private float alertHitTimer = 1.0f;
	private float alertHitTimerAux = 0.0f;

	// the vision radious of the tower
	protected float visionSphereRadious;

	private float attackCadenceAux = 0.0f;

	// frecuencia (en segundos) de ataque primario
	public float attackCadence = 1.0f;

	protected UnitController lastEnemyAttacked;

	public GameObject shotParticles;

	// health bar
	public Texture2D progressBarEmpty, progressBarFull;

	private float attackPower = 10;

	//Conts for Tower conquest
	private float[] contConq;

	//Constant when the tower is conquered
	private const float finalCont = 100.0f;

	//Constant for the life of the tower
	private const float totalLife = 100.0f;

	//The currentLife of the Tower
	private float currentLife = 0.0f;

	private List<UnitController> enemiesInside = new List<UnitController>();

	//********************************************************************************
	//For engineers
	public float distanceToWait = 2.0f;
	
	public int numEngineerPositions = 8;
	private Vector3[] engineerPositions;
	private bool[] engineerPosTaken;
	
	// desplazamiento de los harvest positions
	public float despPosition = 1.4f;

	// Queue of units harversters which are waiting in the mine
	private List<UnitEngineer> engineerQueue;

	// for debugging
	private GameObject[] cubes;

	//********************************************************************************

	// Use this for initialization
	void Start () {
	
		contConq = new float[2];;
		for (int i = 0; i < 2; i++)
			contConq[i] = 0;

		distanceToWait += transform.GetComponent<BoxCollider>().size.x + despPosition;

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

		engineerQueue = new List<UnitEngineer>();

		visionSphereRadious = transform.FindChild("TowerVisionSphere").GetComponent<SphereCollider>().radius;
	}
	
	// Update is called once per frame
	void Update () {
		
		switch (currentTowerState) 
		{
		case TowerState.Neutral:
			
			
			break;
			
		case TowerState.Iddle:

			if (currentLife <= 0.0f)
				currentTowerState = TowerState.Neutral;
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
						UnitController enemy = myHit.transform.GetComponent<UnitController>();
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
			if (enemiesInside.Count == 0)
				currentTowerState = TowerState.Iddle;
			if (currentLife <= 0.0f)
				currentTowerState = TowerState.Neutral;
			break;
			
		case TowerState.ShootingEnemies:

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
				//UnitController lastEnemyAtackedUC = (UnitController)lastEnemyAttacked;
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
			if (currentLife <= 0.0f)
				currentTowerState = TowerState.Neutral;
			if (enemiesInside.Count == 0)
				currentTowerState = TowerState.Iddle;
			break;	
		}
	}
	
	public void EnemyEntersInVisionSphere (UnitController enemy)
	{
		
		if (currentTowerState != TowerState.Neutral)
			if (enemiesInside.Count == 0) 
			{
				enemiesInside.Add(enemy);
				Debug.Log ("First Enemy entered in TOWER");
			}
			else
			{
				if (!enemiesInside.Contains(enemy))
				{
					enemiesInside.Add(enemy);
					Debug.Log ("New Enemy entered in TOWER");
				}
				else
				{
					Debug.Log ("Enemy already entered in TOWER");
				}
				
			}
	}
	
	public void EnemyExitsInVisionSphere (UnitController enemy)
	{
		enemiesInside.Remove(enemy);
		Debug.Log ("Enemy exits the TOWER");
	}

	public bool IsCurrentStateNeutral ()
	{
		return currentTowerState == TowerState.Neutral;
	}

	public bool Repair (float sum)
	{
		if (currentLife < totalLife)
		{
			currentLife += sum;
			if (totalLife < currentLife)
				currentLife = totalLife;
		}

		if (currentLife == totalLife)
		{
			return true;
		}
		else
			return false;

	}

	public bool Conquest (float sum, int team)
	{
		contConq[team] += sum;
		if (contConq[team] >= finalCont) 
		{
			for (int i = 0; i < 2; i++)
				contConq[i] = 0;
			currentLife = 80.0f;
			teamNumber = team;
			currentTowerState = TowerState.Iddle;
			return true;
		}
		return false;
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
				Rect rect3 = new Rect (camPos.x - 60.0f, Screen.height - camPos.y - 100.0f, 120.0f, 4.0f);
				GUI.DrawTexture (rect3, progressBarEmpty);
				Rect rect4 = new Rect (camPos.x - 60.0f, Screen.height - camPos.y - 100.0f, 120.0f * (contConq [0] / finalCont), 4.0f);
				GUI.DrawTexture (rect4, progressBarFull);

				// rectángulo donde se dibujará la barra de conquista
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

	public void LeaveEngineerPosition (int index)
	{
		engineerPosTaken[index] = false;
		cubes[index].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);
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
}
