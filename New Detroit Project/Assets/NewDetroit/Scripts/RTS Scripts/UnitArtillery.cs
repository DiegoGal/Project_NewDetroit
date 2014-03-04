using UnityEngine;
using System.Collections.Generic;

public class UnitArtillery : UnitController
{
    protected enum WaitMode
    {
        Defensive,
        Ofensive
    }
    protected WaitMode waitMode = WaitMode.Defensive;

    protected enum MoveMode
    {
        Idle,
        Moving,
        MovingAttacking
    }
    protected MoveMode moveMode = MoveMode.Idle;

    protected enum ArtilleryState
    {
        None,
        Alert,
        Attacking1, // primary attack
        Attacking2, // secondary attack
        Chasing
    }
    protected ArtilleryState currentArtilleryState = ArtilleryState.None;

    public GameObject shotParticles;

	protected List<ControllableCharacter> enemiesInside;
	protected ControllableCharacter lastEnemyAttacked;

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
        
		enemiesInside = new List<ControllableCharacter>();
        visionSphereRadious = transform.FindChild("VisionSphere").GetComponent<SphereCollider>().radius;
    }

	// Update is called once per frame
    public override void Update ()
    {
        base.Update();

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
                        fwd.Normalize();
                        Vector3 aux = new Vector3(fwd.x * transform.position.x, fwd.y * transform.position.y, fwd.z * transform.position.z);
                        Debug.DrawLine(transform.position, aux /** visionSphereRadious*/, Color.blue, 0.3f);
                        RaycastHit myHit;
                        if (Physics.Raycast(transform.position, fwd, out myHit, visionSphereRadious))
                        {
                            // the ray has hit something
							ControllableCharacter enemy = myHit.transform.GetComponent<ControllableCharacter>();
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
                    // emite some particles:
                    GameObject particles = (GameObject)Instantiate(shotParticles,
                        transform.FindChild("Weapon").FindChild("gun barrel").transform.position,
                        transform.rotation);
                    Destroy(particles, 0.4f);
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
	} // Update

    public override void OnGUI ()
    {
        base.OnGUI();

        Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);

        GUI.skin.label.fontSize = 10;

        GUI.Label(new Rect(camPos.x - 10, Screen.height - camPos.y - 45, 100, 50),
            currentState.ToString());
        GUI.Label(new Rect(camPos.x - 10, Screen.height - camPos.y - 55, 100, 50),
            currentArtilleryState.ToString());
    } // OnGUI

    public override void RightClickOnSelected (Vector3 destiny, Transform destTransform)
    {
        base.RightClickOnSelected(destiny, destTransform);
    } // RightClickOnSelected

	public void EnemyEntersInVisionSphere (ControllableCharacter enemy)
    {
        //Debug.Log("ALERT");
        enemiesInside.Add(enemy);
        currentArtilleryState = ArtilleryState.Alert;
        alertHitTimerAux = 0.0f;
    }

	public void EnemyLeavesVisionSphere (ControllableCharacter enemy)
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
