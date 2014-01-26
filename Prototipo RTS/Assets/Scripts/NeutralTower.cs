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

	private TowerState currentTowerState = TowerState.Iddle;

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
	private float contConq1 = 0.0f;
	private float contConq2 = 0.0f;

	//Constant when the tower is conquered
	private const float finalCont = 100.0f;

	//Constant for the life of the tower
	private const float totalLife = 100.0f;

	//The currentLife of the Tower
	private float currentLife = 100.0f;

	private List<UnitController> enemiesInside = new List<UnitController>();

	// Use this for initialization
	void Start () {

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

	public bool Repair (float sum)
	{
		if (currentLife < totalLife)
		{
			currentLife += sum;
			if (totalLife > currentLife)
				currentLife = totalLife;
		}

		if (currentLife == totalLife)
		{
			return false;
		}
		else
			return true;

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

	public virtual void OnGUI()
	{
		Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);

		// rectángulo donde se dibujará la barra
		Rect rect1 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 200.0f, 120.0f, 4.0f);
		GUI.DrawTexture(rect1, progressBarEmpty);
		Rect rect2 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 200.0f, 120.0f * (currentLife/totalLife), 4.0f);
		GUI.DrawTexture(rect2, progressBarFull);
	}
}
