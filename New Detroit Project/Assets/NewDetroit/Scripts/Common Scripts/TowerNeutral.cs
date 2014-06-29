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
		Idle,
		Alert, // espera hasta que halla hueco en la mina
		ShootingEnemies
	}
	// the state of the tower
	private TowerState currentTowerState = TowerState.Neutral;
	
	//Conts for Tower conquest
	public float[] contConq;

	//Constant when the tower is conquered
	private const float finalCont = 100.0f;

	// For decrease the contConq if it is not conquering
	private float[] actualTimeConquering;
	private float conqueringTime = 4;
	private float amountToSubstract = 1;

    // to show the conquer cubes
    public bool showCubes = true;
    
    private int contStream = 0;
    private int teamNumberEngineer = -1;
	// Use this for initialization
	public override void Start ()
    {
        base.Start();
        distanceToWait = 2;
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
				    (transform.GetComponent<CapsuleCollider>().radius + despPosition) *
                     Mathf.Sin(i * (twoPi / numEngineerPositions)),
				0,
				center.z +
                    (transform.GetComponent<CapsuleCollider>().radius + despPosition) *
                     Mathf.Cos(i * (twoPi / numEngineerPositions))
			);
			engineerPositions[i] = pos;
			engineerPosTaken[i] = false;
			
			cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cubes[i].transform.position = pos;
			Destroy(cubes[i].GetComponent<BoxCollider>());
			cubes[i].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);
			cubes[i].transform.parent = this.transform;

            cubes[i].active = showCubes;
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
                if (PhotonNetwork.connected && contStream >= 10 && teamNumberEngineer != -1 && contConq[teamNumberEngineer] > 0)
                {
                    photonView.RPC("UpdateContConq", PhotonTargets.All, teamNumberEngineer, contConq[teamNumberEngineer]);
                    contStream = 0;
                }
                else
                    contStream++;
                break;

            case TowerState.Idle:

                // we change the state to neutral if the tower dies
                if (life.currentLife <= 0.0f)
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
                            if ( enemy && (enemy == enemiesInside[i]))
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
                // we change the state to Idle if there aren't enemies inside vision
                if (enemiesInside.Count == 0)
                    currentTowerState = TowerState.Idle;
                // we change the state to neutral if the tower dies
                if (life.currentLife <= 0.0f)
                    currentTowerState = TowerState.Neutral;
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
                    else lastEnemyAttacked.Damage(attackPower, 'P');

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
                    currentTowerState = TowerState.Neutral;
                // we change the state to Idle if there aren't enemies inside vision
                if (enemiesInside.Count == 0)
                    currentTowerState = TowerState.Idle;
                break;
        }
	}

	// Conquest is called by the engineers
	public bool Conquest (float sum, int teamNumber)
	{
        actualTimeConquering[teamNumber] = 0;
        contConq[teamNumber] += sum;
        teamNumberEngineer = teamNumber;
        if (IsCurrentStateNeutral() && contConq[teamNumber] >= finalCont)
        {
            for (int i = 0; i < teamsPlaying; i++)
                contConq[i] = 0;
            life.currentLife = 80.0f;

            // set the team values
            team.teamNumber = teamNumber;

            // insert the tower in the DistanceMeasurerTool
            DistanceMeasurerTool.InsertUnit(team);

            // change the state to Idle
            //
            if (PhotonNetwork.connected) 
                photonView.RPC("NewConquest", PhotonTargets.All, life.currentLife);
            else currentTowerState = TowerState.Idle;

            UpdateEnemiesInside(teamNumber);
            RemoveEngineersInQueue();
            for (int i = 0; i < numEngineerPositions; i++)
                cubes[i].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);

            return true;
        }
        return false;
	}

    [RPC]
    public void NewConquest (float lif)
    {
        currentTowerState = TowerState.Idle;
        life.currentLife = lif;
    }

    [RPC]
    public void UpdateContConq(int teamN, float sum)
    {
        contConq[teamN] = sum;
    }

	// It's called when a team isn't repairing and the state is neutral
	private void LessConquest (int team)
	{
		if (contConq[team] > 0)
			contConq[team] -= amountToSubstract;
	}

    public bool IsCurrentStateNeutral ()
    {
        return currentTowerState == TowerState.Neutral;
    }

    //public virtual void OnGUI ()
    //{
    //    Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);

    //    // rectángulo donde se dibujará la barra de conquista
    //    if (currentTowerState == TowerState.Neutral)
    //    {
    //        GUI.skin.label.fontSize = 10;

    //        // Team 0
    //        GUI.Label(new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 130.0f, 120.0f, 50),
    //              "Team 0 conquered");
    //        Rect rect3 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 110.0f, 120.0f, 4.0f);
    //        GUI.DrawTexture(rect3, progressBarEmpty);
    //        Rect rect4 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 110.0f, 120.0f * (contConq[0] / finalCont), 4.0f);
    //        GUI.DrawTexture(rect4, progressBarFull);

    //        // Team 1
    //        GUI.Label(new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 100.0f, 120.0f, 50),
    //              "Team 1 conquered");
    //        Rect rect5 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 80.0f, 120.0f, 4.0f);
    //        GUI.DrawTexture(rect5, progressBarEmpty);
    //        Rect rect6 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 80.0f, 120.0f * (contConq[1] / finalCont), 4.0f);
    //        GUI.DrawTexture(rect6, progressBarFull);
    //    }
    //    else
    //    {
    //        // rectángulo donde se dibujará la barra de vida
    //        Rect rect1 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 200.0f, 120.0f, 4.0f);
    //        GUI.DrawTexture(rect1, progressBarEmpty);
    //        Rect rect2 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 200.0f, 120.0f * (life.currentLife / life.maximunLife), 4.0f);
    //        GUI.DrawTexture(rect2, progressBarFull);

    //    }
    //}

    public void LeaveEngineerPositionConquest (int index)
    {
        engineerPosTaken[index] = false;
        cubes[index].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);
        if (engineerQueue.Count > 0)
        {
            UnitEngineer unit = engineerQueue[0];
            unit.FinishWaitingToRepair(engineerPositions[index], index);
            engineerQueue.RemoveAt(0);
            //engineerPosTaken[index] = true;
            if (PhotonNetwork.connected) 
                photonView.RPC("LessPositionsTaken", PhotonTargets.All, index);
            else
                engineerPosTaken[index] = true;
            cubes[index].renderer.material.color = new Color(0.863f, 0.078f, 0.235f);
        }
    }
   
    [RPC]
    public void LessPositionsTaken(int i)
    {
        engineerPosTaken[i] = false;
        cubes[i].renderer.material.color = new Color(0.863f, 0.078f, 0.235f);
    }
}
