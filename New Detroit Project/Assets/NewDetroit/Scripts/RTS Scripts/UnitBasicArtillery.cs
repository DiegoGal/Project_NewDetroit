using UnityEngine;
using System.Collections;

public class UnitBasicArtillery : UnitArtillery
{

    public float attackPower1 = 10.0f;
    public float attackPower2 = 20.0f;

    public float attack1Cadence = 1.0f;
    public float attack2Cadence = 0.67f;

    protected GameObject leftWeapon, rightWeapon, baseballBat;
    public Transform dummyBat;

	// Use this for initialization
    public override void Start ()
	{
        base.Start();

        basicAttackPower = attackPower1;
        secondaryAttackPower = attackPower2;

        primaryAttackCadence = attack1Cadence;
        secondaryAttackCadence = attack2Cadence;
	}

    protected override void UpdateGoingToAnEnemy ()
    {
        // si esta seleccionado el ataque con bate se llama a la clase base
        if (attack2Selected)
            base.UpdateGoingToAnEnemy();
        // si esta seleccionado el ataque a distancia miramos si la unidad
        // de verdad "ve" al enemigo seleccionado
        else if (currentArtilleryState == ArtilleryState.Alert)
        {
            if (alertHitTimerAux <= 0)
            {
                SearchForAnEnemy();
                // reset the timer
                alertHitTimerAux = alertHitTimer;
            }
            else
                alertHitTimerAux -= Time.deltaTime;
        }
    }

    // The parent class only check for the distance between this and the unit that is attacked
    // this artillery units attack with a bate and with distance weapons, in the first case
    // we call the base method, in the "distance-attack" becase we have to check first if the 
    // enemy is on sight launching rays
    protected override void UpdateAttacking ()
    {
        // si esta seleccionado el ataque con bate se llama a la clase base
        if (attack2Selected)
            base.UpdateAttacking();
        else
        {
            if (lastEnemyAttacked == null)
            {
                if (enemiesInside.Count == 0)
                {
                    lastEnemyAttacked = null;
                    // no more enemies, change the state
                    currentArtilleryState = ArtilleryState.None;
                    cState.currentArtilleryState = currentArtilleryState;
                }
                else
                {
                    currentArtilleryState = ArtilleryState.Alert;
                    cState.currentArtilleryState = currentArtilleryState;
                }

                PlayAnimationCrossFade("Idle01");

                currentState = State.Idle;
                cState.currentState = currentState;
            }
            else if (attackCadenceAux <= 0.0f)
            {
                // check that the enemy is in sight
                Debug.DrawLine(transform.position, enemySelected.transform.position, Color.yellow, 0.2f);

                Vector3 fwd = enemySelected.transform.position - this.transform.position; // -
                    //new Vector3(0.0f, eyesPosition.y * 0.5f, 0.0f);
                fwd.Normalize();
                Vector3 aux = transform.position + eyesPosition + (fwd * maxAttackDistance);
                Debug.DrawLine(transform.position + eyesPosition, aux, Color.blue, 0.2f);
                RaycastHit myHit;
                if (Physics.Raycast(transform.position + eyesPosition, fwd, out myHit, maxAttackDistance))
                {
                    CTeam enemy = myHit.transform.GetComponent<CTeam>();
                    if ( enemy && (enemy == enemySelected) )
                    {
                        // Attack!
                        lastEnemyAttacked = enemySelected;
                        Debug.DrawLine(transform.position, lastEnemyAttacked.transform.position, Color.red, 0.3f);
                        transform.LookAt(lastEnemyAttacked.transform);

                        // emite some particles:
                        GameObject particles1 = (GameObject)Instantiate(shotParticles,
                            dummyLeftWeaponGunBarrel.transform.position,
                            transform.rotation);
                        Destroy(particles1, 0.4f);
                        if (numberOfWeapons > 1)
                        {
                            GameObject particles2 = (GameObject)Instantiate(shotParticles,
                                dummyRightWeaponGunBarrel.transform.position,
                                transform.rotation);
                            Destroy(particles2, 0.4f);
                        }

                        ;
                        // first we check if the enemy is now alive
                        //if (lastEnemyAttacked.Damage(basicAttackPower))
                        if (PhotonNetwork.connected)
                        	photonView.RPC("Kick", PhotonTargets.All, lastEnemyAttacked.name, basicAttackPower);
                        else
                            lastEnemyAttacked.GetComponent<CLife>().Damage(basicAttackPower);
                        if (lastEnemyAttacked.GetComponent<CLife>().currentLife <= 0.0f)
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
                                currentArtilleryState = ArtilleryState.Alert;

                            cState.currentArtilleryState = currentArtilleryState;

                            PlayAnimationCrossFade("Idle01");
                            currentState = State.Idle;
                            cState.currentState = currentState;
                        }
                    }
                    else
                    {
                        // the enemy is NOT on sight
                        currentArtilleryState = ArtilleryState.Alert;
                        cState.currentArtilleryState = currentArtilleryState;

                        PlayAnimationCrossFade("Idle01");
                        currentState = State.Idle;
                        cState.currentState = currentState;
                    }
                }
                else
                {
                    // the enemy is NOT on sight
                    currentArtilleryState = ArtilleryState.Alert;
                    cState.currentArtilleryState = currentArtilleryState;

                    PlayAnimationCrossFade("Idle01");
                    currentState = State.Idle;
                    cState.currentState = currentState;
                }
                // reset the timer
                attackCadenceAux = primaryAttackCadence;
            }
            else
                attackCadenceAux -= Time.deltaTime;
        }
    }

    public override void ChangeAttack ()
    {
        if (attack2Selected) // change to attack1 (fire)
        {
            maxAttackDistance = maxAttackDistance1;
            attackCadence = attack1Cadence;

            attack2Selected = false;
            cState.attack2Selected = attack2Selected;
        }
        else // change to attack2 (bate)
        {
            maxAttackDistance = maxAttackDistance2;
            attackCadence = attack2Cadence;

            attack2Selected = true;
            cState.attack2Selected = attack2Selected;
        }
    }

    protected override void PlayAnimationCrossFade (string animationName)
    {
        if ( (animationName == "Attack1") && attack2Selected )
        {
            //animation.CrossFade("Attack2");
			cState.animationName = "Attack2";
			cState.animationChanged = true;
        }
        else
            base.PlayAnimationCrossFade(animationName);
    }

    public override int GetUnitType ()
    {
        return 1;
    }

    protected override void RemoveAssetsFromModel ()
    {
        if (leftWeapon)
            Destroy(leftWeapon);
        if (rightWeapon)
            Destroy(rightWeapon);
        if (baseballBat)
            Destroy(baseballBat);
    }

} // class UnitBasicArtillery
