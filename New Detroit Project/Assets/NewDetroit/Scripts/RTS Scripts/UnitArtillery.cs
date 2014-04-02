﻿using UnityEngine;
using System.Collections.Generic;

public class UnitArtillery : UnitController
{
    protected enum WaitMode
    {
        Pasive,
        Ofensive
    }
    protected WaitMode waitMode = WaitMode.Pasive;

    protected enum MoveMode
    {
        Idle,
        Moving,
        MovingToAnEnemy,
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

    // dummys
    public Transform dummyLeftWeapon;
    public Transform dummyRightWeapon;
    public Transform dummyLeftWeaponGunBarrel;
    public Transform dummyRightWeaponGunBarrel;
    protected short numberOfWeapons;

    public GameObject shotParticles;

    public List<ControllableCharacter> enemiesInside;

    private float alertHitTimer = 1.0f;
    private float alertHitTimerAux = 0.0f;

    // position where the rays to search for enemies are launched
    protected Vector3 eyesPosition = new Vector3(0.0f, 1.5f, 0.0f);

    // frecuencia (en segundos) de ataque primario
    public float primaryAttackCadence = 1.0f;
    // frecuencia (en segundos) de ataque secundario
    public float secondaryAttackCadence = 1.0f;

    protected bool attackSelected = false; // false = attack1; true = attack2

    public float maxAttackDistance1, maxAttackDistance2 = 2.0f;

    protected bool thereIsVisionSphere = false;

    public override void Awake ()
    {
        base.Awake();
    }

    public override void Start ()
    {
        base.Start();
        
		enemiesInside = new List<ControllableCharacter>();

        // if there is a VisionSphere attached to the unit we catch its radious here
        // if not, then we get it just from the prefab's attribute value.
        Transform sphereTransform = transform.FindChild("VisionSphere");
        if (sphereTransform)
        {
            SphereCollider sphere = sphereTransform.GetComponent<SphereCollider>();
            if (sphere)
            {
                visionSphereRadious = sphere.radius;
                thereIsVisionSphere = true;
            }
            else
                thereIsVisionSphere = false;
        }
        else
            thereIsVisionSphere = false;

        if (attackSelected)
            maxAttackDistance = maxAttackDistance2;
        else
            maxAttackDistance = visionSphereRadious;
    }

	// Update is called once per frame
    /*public override void Update ()
    {
        base.Update();

        switch (moveMode)
        {
            case MoveMode.Idle:
                UpdateModeIdle();
                break;

            case MoveMode.Moving:
                //base.Update();
                if (currentState == State.Idle)
                    moveMode = MoveMode.Idle;
                break;

            case MoveMode.MovingToAnEnemy:
                //base.Update();
                // si el estado ha pasado a Alert y el enemigo seleccionado
                // ha entrada en la burbuja
                if (
                     (currentArtilleryState == ArtilleryState.Alert) &&
                     (enemiesInside.Contains(enemySelected))
                   )
                {
                    if (alertHitTimerAux <= 0)
                    {
                        // launch a ray for the enemy searched
                        Debug.DrawLine(transform.position, enemySelected.transform.position, Color.yellow, 0.3f);
                        Vector3 fwd = enemySelected.transform.position - this.transform.position;
                        fwd.Normalize();
                        Vector3 aux = transform.position + eyesPosition + (fwd * visionSphereRadious);
                        Debug.DrawLine(transform.position + eyesPosition, aux, Color.blue, 0.2f);
                        RaycastHit myHit;
                        if (Physics.Raycast(transform.position + eyesPosition, fwd, out myHit, 100))
                        {
                            //Debug.Log(myHit.transform.name);
                            // the ray has hit something
                            ControllableCharacter enemy = myHit.transform.GetComponent<ControllableCharacter>();
                            if (enemy = enemySelected)
                            {
                                // this "something" is the enemy we are looking for...
                                //Debug.Log("LE HE DADO!!!");
                                transform.LookAt(enemy.transform.position);
                                lastEnemyAttacked = enemy;
                                alertHitTimerAux = alertHitTimer;
                                currentArtilleryState = ArtilleryState.Attacking1;
                                // the enemy stops moving
                                StopMoving();
                                moveMode = MoveMode.Idle;
                            }
                        }
                        // reset the timer
                        alertHitTimerAux = alertHitTimer;
                    }
                    else
                        alertHitTimerAux -= Time.deltaTime;
                }
                break;

            case MoveMode.MovingAttacking:
                //base.Update();
                break;

        } // switch (moveMode)

	} // Update*/

    protected override void UpdateIdle ()
    {
        switch (currentArtilleryState)
        {
            case ArtilleryState.None:
                base.UpdateIdle();
                break;

            case ArtilleryState.Alert:

                if (alertHitTimerAux <= 0)
                {
                    // launch a ray for each enemy inside the vision sphere
                    int count = enemiesInside.Count;
                    for (int i = 0; i < count; i++)
                    {
                        // check if the enemy is still in the game
                        if (enemiesInside[i])
                        {
                            Debug.DrawLine(transform.position, enemiesInside[i].transform.position, Color.yellow, 0.3f);

                            Vector3 fwd = enemiesInside[i].transform.position - this.transform.position;
                            fwd.Normalize();
                            Vector3 aux = transform.position + eyesPosition + (fwd * visionSphereRadious);
                            Debug.DrawLine(transform.position + eyesPosition, aux, Color.blue, 0.2f);
                            RaycastHit myHit;
                            if (Physics.Raycast(transform.position + eyesPosition, fwd, out myHit, visionSphereRadious))
                            {
                                //Debug.Log(myHit.transform.name);
                                // the ray has hit something
                                ControllableCharacter enemy = myHit.transform.GetComponent<ControllableCharacter>();
                                if ((enemy != null) && (enemy == enemiesInside[i]))
                                {
                                    // this "something" is the enemy we are looking for...
                                    //Debug.Log("LE HE DADO!!!");
                                    // rotate the unit in the enemy direction
                                    transform.LookAt(enemy.transform.position);
                                    lastEnemyAttacked = enemy;
                                    alertHitTimerAux = alertHitTimer;
                                    currentArtilleryState = ArtilleryState.Attacking1;
                                    // the enemy stops moving
                                    StopMoving();

                                    animation.CrossFade("Attack1");
                                }
                            }
                        }
                        else // remove the enemy
                        {
                            enemiesInside.RemoveAt(i);
                            count--;
                            i--;
                        }
                    }
                    // reset the timer
                    alertHitTimerAux = alertHitTimer;
                }
                else
                    alertHitTimerAux -= Time.deltaTime;

                break;

            case ArtilleryState.Attacking1:

                if (lastEnemyAttacked == null)
                {
                    if (enemiesInside.Count == 0)
                    {
                        lastEnemyAttacked = null;
                        // no more enemies, change the state
                        currentArtilleryState = ArtilleryState.None; 
                    }
                    else
                        currentArtilleryState = ArtilleryState.Alert;

                    PlayAnimationCrossFade("Idle01");
                }
                else if (attackCadenceAux <= 0.0f)
                {
                    // Attack!
                    Debug.DrawLine(transform.position, lastEnemyAttacked.transform.position, Color.red, 0.2f);
                    transform.LookAt(lastEnemyAttacked.transform);
                    // play the attack animation:
                    //animation.CrossFade("Attack1");
                    //animation.CrossFadeQueued("Idle01");
                    // emite some particles:
                    GameObject particles1 = (GameObject)Instantiate(shotParticles,
                        dummyLeftWeaponGunBarrel.transform.position,
                        transform.rotation);
                    Destroy(particles1, 0.4f);
                    if (dummyRightWeaponGunBarrel != null)
                    {
                        GameObject particles2 = (GameObject)Instantiate(shotParticles,
                            dummyRightWeaponGunBarrel.transform.position,
                            transform.rotation);
                        Destroy(particles2, 0.4f);
                    }
                    // first we check if the enemy is now alive
                    if (lastEnemyAttacked.Damage(basicAttackPower, 'P'))
                    {
                        // the enemy died, time to reset the lastEnemyAttacked reference
                        enemiesInside.Remove(lastEnemyAttacked);
                        if (enemiesInside.Count == 0)
                        {
                            lastEnemyAttacked = null;
                            // no more enemies, change the state
                            currentArtilleryState = ArtilleryState.None;

                            PlayAnimationCrossFade("Idle01");
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

        } // switch (currentArtilleryState)
    }

    /*protected override void UpdateGoingTo ()
    {
        base.UpdateGoingTo();

        switch (moveMode)
        {
            case MoveMode.MovingToAnEnemy:
                // si el estado ha pasado a Alert y el enemigo seleccionado
                // ha entrada en la burbuja
                if (
                     (currentArtilleryState == ArtilleryState.Alert) &&
                     (enemiesInside.Contains(enemySelected))
                   )
                {
                    if (alertHitTimerAux <= 0)
                    {
                        // launch a ray for the enemy searched
                        Debug.DrawLine(transform.position, enemySelected.transform.position, Color.yellow, 0.3f);
                        Vector3 fwd = enemySelected.transform.position - this.transform.position;
                        fwd.Normalize();
                        Vector3 aux = transform.position + eyesPosition + (fwd * visionSphereRadious);
                        Debug.DrawLine(transform.position + eyesPosition, aux, Color.blue, 0.2f);
                        RaycastHit myHit;
                        if (Physics.Raycast(transform.position + eyesPosition, fwd, out myHit, 100))
                        {
                            //Debug.Log(myHit.transform.name);
                            // the ray has hit something
                            ControllableCharacter enemy = myHit.transform.GetComponent<ControllableCharacter>();
                            if (enemy = enemySelected)
                            {
                                // this "something" is the enemy we are looking for...
                                //Debug.Log("LE HE DADO!!!");
                                transform.LookAt(enemy.transform.position);
                                lastEnemyAttacked = enemy;
                                alertHitTimerAux = alertHitTimer;
                                currentArtilleryState = ArtilleryState.Attacking1;
                                // the enemy stops moving
                                StopMoving(); // this also sets the currentState to Idle
                                moveMode = MoveMode.Idle;
                            }
                        }
                        // reset the timer
                        alertHitTimerAux = alertHitTimer;
                    }
                    else
                        alertHitTimerAux -= Time.deltaTime;
                }
                break;
        }
    }*/

    /*private void UpdateModeIdle ()
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
                        Debug.DrawLine(transform.position, enemiesInside[i].transform.position, Color.yellow, 0.3f);

                        Vector3 fwd = enemiesInside[i].transform.position - this.transform.position;
                        fwd.Normalize();
                        Vector3 aux = transform.position + eyesPosition + (fwd * visionSphereRadious);
                        Debug.DrawLine(transform.position + eyesPosition, aux, Color.blue, 0.2f);
                        RaycastHit myHit;
                        if (Physics.Raycast(transform.position + eyesPosition, fwd, out myHit, 100))
                        {
                            //Debug.Log(myHit.transform.name);
                            // the ray has hit something
                            ControllableCharacter enemy = myHit.transform.GetComponent<ControllableCharacter>();
                            if ((enemy != null) && (enemy = enemiesInside[i]))
                            {
                                // this "something" is the enemy we are looking for...
                                //Debug.Log("LE HE DADO!!!");
                                // rotate the unit in the enemy direction
                                transform.LookAt(enemy.transform.position);
                                lastEnemyAttacked = enemy;
                                alertHitTimerAux = alertHitTimer;
                                currentArtilleryState = ArtilleryState.Attacking1;
                                // the enemy stops moving
                                StopMoving();
                            }
                        }
                    }
                    // reset the timer
                    alertHitTimerAux = alertHitTimer;
                }
                else
                    alertHitTimerAux -= Time.deltaTime;

                break;

            case ArtilleryState.Attacking1:

                if (lastEnemyAttacked == null)
                    currentArtilleryState = ArtilleryState.None;
                else if (attackCadenceAux <= 0.0f)
                {
                    // Attack!
                    Debug.DrawLine(transform.position, lastEnemyAttacked.transform.position, Color.red, 0.2f);
                    // play the attack animation:
                    animation.CrossFade("Attack1");
                    animation.CrossFadeQueued("Idle01");
                    // emite some particles:
                    GameObject particles1 = (GameObject)Instantiate(shotParticles,
                        dummyLeftWeaponGunBarrel.transform.position,
                        transform.rotation);
                    Destroy(particles1, 0.4f);
                    if (dummyRightWeaponGunBarrel != null)
                    {
                        GameObject particles2 = (GameObject)Instantiate(shotParticles,
                            dummyRightWeaponGunBarrel.transform.position,
                            transform.rotation);
                        Destroy(particles2, 0.4f);
                    }
                    // first we check if the enemy is now alive
                    if (lastEnemyAttacked.Damage(basicAttackPower,'P'))
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

        } // switch (currentArtilleryState)

    } // UpdateModeIdle*/

    public override void OnGUI ()
    {
        if (currentState != State.AscendingToHeaven)
        {
            base.OnGUI();

            GUI.skin.label.fontSize = 10;

            GUI.Label(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 45, 100, 50),
                currentState.ToString());
            GUI.Label(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 55, 100, 50),
                moveMode.ToString());
            GUI.Label(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 65, 100, 50),
                currentArtilleryState.ToString());
        }
    } // OnGUI

	public override void EnemyEntersInVisionSphere (ControllableCharacter enemy)
    {
        //if (!enemiesInside.Contains(enemy))
        //{
            //Debug.Log("enemigo a la vista!");
            enemiesInside.Add(enemy);
            if (currentArtilleryState == ArtilleryState.None)
            {
                currentArtilleryState = ArtilleryState.Alert;
                alertHitTimerAux = 0.0f;
            }
        //}
    }

    public override void EnemyLeavesVisionSphere (ControllableCharacter enemy)
    {
        enemiesInside.Remove(enemy);
        /*if (enemiesInside.Remove(enemy))
            Debug.Log("Enemy out");
        else
            Debug.Log("Enemy NOT removed");*/

        if (enemiesInside.Count == 0)
        {
            //Debug.Log("all enemies out");

            if (currentArtilleryState == ArtilleryState.Attacking1)
                PlayAnimationCrossFade("Idle01");

            currentArtilleryState = ArtilleryState.None;
            alertHitTimerAux = 0.0f;
        }
    }

    public override void RightClickOnSelected (Vector3 destiny, Transform destTransform)
    {
        base.RightClickOnSelected(destiny, destTransform);

        ControllableCharacter unit = destTransform.transform.GetComponent<ControllableCharacter>();
        if ((unit != null) && (teamNumber != unit.teamNumber))
        {
            enemySelected = unit;
            moveMode = MoveMode.MovingToAnEnemy;
        }
        else
        {
            enemySelected = null;
            if (currentState == State.GoingTo)
                moveMode = MoveMode.Moving;

            if (enemiesInside.Count > 0)
                currentArtilleryState = ArtilleryState.Alert;
            else
                currentArtilleryState = ArtilleryState.None;
        }

    } // RightClickOnSelected

} // class UnitArtillery
