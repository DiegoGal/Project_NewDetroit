using UnityEngine;
using System.Collections.Generic;

public class UnitArtillery : UnitController
{
    protected enum Mode
    {
        Defensive,
        Ofensive
    }
    protected Mode mode = Mode.Defensive;

    protected enum ArtilleryState
    {
        None,
        Alert,
        Attacking1, // primary attack
        Attacking2, // secondary attack
        Chasing
    }
    protected ArtilleryState currentArtilleryState = ArtilleryState.None;

    protected List<UnitController> enemiesInside;
    protected UnitController lastEnemyAttacked;

    private float alertHitTimer = 1.0f;
    private float alertHitTimerAux = 0.0f;

    // the vision radious of the unit
    protected float visionSphereRadious;

    // frecuencia (en segundos) de ataque primario
    public float primaryAttackCadence = 1.0f;
    // frecuencia (en segundos) de ataque secundario
    public float secondaryAttackCadence = 1.0f;
    private float attackCadenceAux = 0.0f;

    public override void Start ()
    {
        base.Start();
        
        enemiesInside = new List<UnitController>();
        visionSphereRadious = transform.FindChild("VisionSphere").GetComponent<SphereCollider>().radius;
    }

	// Update is called once per frame
    public override void Update ()
    {
        switch (currentArtilleryState)
        {
            case ArtilleryState.None:

                break;

            case ArtilleryState.Alert:
                if (alertHitTimerAux <= 0)
                {
                    // launch a ray for each enemy inside the vision sphere
                    int count = enemiesInside.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Debug.DrawLine(transform.position, enemiesInside[i].transform.position, Color.yellow, 0.2f);
                        
                        Vector3 fwd = enemiesInside[i].transform.position - this.transform.position;
                        //Debug.Log("origen: " + transform.position + ". destino: " + enemiesInside[i].transform.position + ". direccion: " + fwd);
                        RaycastHit myHit;
                        if (Physics.Raycast(transform.position, fwd, out myHit, visionSphereRadious))
                        {
                            // the ray has hit something
                            UnitController enemy = myHit.transform.GetComponent<UnitController>();
                            if ( (enemy != null) && (enemy = enemiesInside[i]) )
                            {
                                // this "something" is the enemy we are looking for...
                                //Debug.Log("LE HE DADO!!!");
                                lastEnemyAttacked = enemy;
                                alertHitTimerAux = alertHitTimer;
                                currentArtilleryState = ArtilleryState.Attacking1;
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
                break;

            case ArtilleryState.Attacking1:
                if (attackCadenceAux <= 0.0f)
                {
                    // Attack!
                    Debug.DrawLine(transform.position, lastEnemyAttacked.transform.position, Color.red, 0.2f);
                    // first we check if the enemy is now alive
                    if (lastEnemyAttacked.Damage(basicAttackPower))
                    {
                        // the enemy died, time to reset the lastEnemyAttacked reference
                        enemiesInside.Remove(lastEnemyAttacked);
                        if (enemiesInside.Count == 0)
                        {
                            lastEnemyAttacked = null;
                            // no more enemies, change the state
                            currentArtilleryState = ArtilleryState.None;
                        }
                        else
                        {
                            currentArtilleryState = ArtilleryState.Alert;
                        }
                    }
                    // reset the timer
                    attackCadenceAux = primaryAttackCadence;
                }
                else
                    attackCadenceAux -= Time.deltaTime;
                break;

            case ArtilleryState.Attacking2:
                if (attackCadenceAux <= 0.0f)
                {

                }
                else
                    attackCadenceAux = secondaryAttackCadence;
                break;

            case ArtilleryState.Chasing:

                break;
        }
	}

    public override void OnGUI()
    {
        base.OnGUI();

        Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);

        GUI.skin.label.fontSize = 10;

        GUI.Label(new Rect(camPos.x - 10, Screen.height - camPos.y - 15, 100, 50),
            "life" + currentLife.ToString());
        GUI.Label(new Rect(camPos.x - 10, Screen.height - camPos.y - 25, 100, 50),
            currentState.ToString());
        GUI.Label(new Rect(camPos.x - 10, Screen.height - camPos.y - 35, 100, 50),
            currentArtilleryState.ToString());
    }

    public void EnemyEntersInVisionSphere (UnitController enemy)
    {
        //Debug.Log("ALERT");
        enemiesInside.Add(enemy);
        currentArtilleryState = ArtilleryState.Alert;
        alertHitTimerAux = 0.0f;
    }

    public void EnemyLeavesVisionSphere (UnitController enemy)
    {
        enemiesInside.Remove(enemy);
        if (enemiesInside.Count == 0)
        {
            //Debug.Log("NONE");
            currentArtilleryState = ArtilleryState.None;
            alertHitTimerAux = 0.0f;
        }
    }

} // class UnitArtillery
