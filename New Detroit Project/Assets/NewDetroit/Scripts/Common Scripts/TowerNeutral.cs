using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerNeutral : Tower 
{

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
	
	//Conts for Tower conquest
	private float[] contConq;

	//Constant when the tower is conquered
	private const float finalCont = 100.0f;

	// For decrease the contConq if it is not conquering
	private float[] actualTimeConquering;
	private float conqueringTime = 4;
	private float amountToSubstract = 1;

	// Use this for initialization
	public override void Start ()
    {
        base.Start();

        // inicialization of the contConq and actualTimeConquering arrays depending of the number of teams playing
        contConq = new float[teamsPlaying];
        actualTimeConquering = new float[teamsPlaying];
        for (int i = 0; i < teamsPlaying; i++)
        {
            contConq[i] = 0;
            actualTimeConquering[i] = 0;
        }

		float twoPi = Mathf.PI * 2;
		Vector3 center = transform.position;
		for (int i = 0; i < numEngineerPositions; i++)
		{
			Vector3 pos = new Vector3
			(
				center.x +
				    (transform.GetComponent<BoxCollider>().size.x + despPosition) *
                     Mathf.Sin(i * (twoPi / numEngineerPositions)),
				0,
				center.z +
				    (transform.GetComponent<BoxCollider>().size.x + despPosition) *
                     Mathf.Cos(i * (twoPi / numEngineerPositions))
			);
			engineerPositions[i] = pos;
			engineerPosTaken[i] = false;
			
			cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cubes[i].transform.position = pos;
			Destroy(cubes[i].GetComponent<BoxCollider>());
			cubes[i].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);
			cubes[i].transform.parent = this.transform;
		}
	}
	
	// Update is called once per frame
	public override void Update ()
    {
        base.Update();

        switch (currentTowerState)
        {
            case TowerState.Neutral:

                // we have to see if there are teams that aren't conquering
                for (int i = 0; i < teamsPlaying; i++)
                {
                    actualTimeConquering[i] += Time.deltaTime;
                    if (actualTimeConquering[i] > conqueringTime)
                    {
                        LessConquest(i);
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
                    Vector3 auxPos = new Vector3(transform.position.x, 10.0f + transform.position.y, transform.position.z);
                    GameObject particles = (GameObject)Instantiate(shotParticles,
                                                                   auxPos,
                                                                   transform.rotation);
                    Destroy(particles, 0.4f);
                    // first we check if the enemy is now alive
                    //ControllableCharacter lastEnemyAtackedUC = (ControllableCharacter)lastEnemyAttacked;
                    if (lastEnemyAttacked.Damage(attackPower, 'P'))
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

	// Repair is called by the engineers
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
            RemoveEngineersInQueue();
            for (int i = 0; i < numEngineerPositions; i++)
                cubes[i].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);

            return true;
        }
        return false;
	}

	// It's called when a team isn't repairing and the state is neutral
	private void LessConquest (int team)
	{
		if (contConq[team] > 0)
			contConq[team] -= amountToSubstract;
	}

    public bool IsCurrentStateNeutral()
    {
        return currentTowerState == TowerState.Neutral;
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
            Rect rect3 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 110.0f, 120.0f, 4.0f);
            GUI.DrawTexture(rect3, progressBarEmpty);
            Rect rect4 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 110.0f, 120.0f * (contConq[0] / finalCont), 4.0f);
            GUI.DrawTexture(rect4, progressBarFull);

            // Team 1
            GUI.Label(new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 100.0f, 120.0f, 50),
                  "Team 1 conquered");
            Rect rect5 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 80.0f, 120.0f, 4.0f);
            GUI.DrawTexture(rect5, progressBarEmpty);
            Rect rect6 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 80.0f, 120.0f * (contConq[1] / finalCont), 4.0f);
            GUI.DrawTexture(rect6, progressBarFull);
        }
        else
        {
            // rectángulo donde se dibujará la barra de vida
            Rect rect1 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 200.0f, 120.0f, 4.0f);
            GUI.DrawTexture(rect1, progressBarEmpty);
            Rect rect2 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 200.0f, 120.0f * (currentLife / totalLife), 4.0f);
            GUI.DrawTexture(rect2, progressBarFull);

        }
    }

    public void LeaveEngineerPositionConquest (int index)
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

}
