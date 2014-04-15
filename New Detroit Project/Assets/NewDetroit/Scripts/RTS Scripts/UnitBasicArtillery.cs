using UnityEngine;
using System.Collections;

public class UnitBasicArtillery : UnitArtillery
{

    public float attackPower1 = 10.0f;
    public float attackPower2 = 20.0f;

    public float attack1Cadence = 1.0f;
    public float attack2Cadence = 0.67f;

    private GameObject leftWeapon, rightWeapon, baseballBat;
    public Transform dummyBat;

    public override void Awake ()
    {
        base.Awake();

        numberOfWeapons = 2;
        // Por si no se han establecido las referencias a los dummys del modelo
        // en el editor de Unity las buscamos ahora:
        if (dummyLeftWeapon == null)
            dummyLeftWeapon = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand/Mano IZQ/WeaponLeft");
        if (dummyRightWeapon == null)
            dummyRightWeapon = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Mano DER/WeaponRight");
        if (dummyLeftWeaponGunBarrel == null)
            dummyLeftWeaponGunBarrel = dummyLeftWeapon.FindChild("GunBarrelLeft");
        if (dummyRightWeaponGunBarrel == null)
            dummyRightWeaponGunBarrel = dummyRightWeapon.FindChild("GunBarrelRight");
        if (dummyBat == null)
            dummyBat = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Arma Blanca/Cylinder002");

        if (dummyLeftWeapon)
            leftWeapon = dummyLeftWeapon.gameObject;
        if (dummyRightWeapon)
            rightWeapon = dummyRightWeapon.gameObject;
        if (dummyBat)
            baseballBat = dummyBat.gameObject;
    }

	// Use this for initialization
    public override void Start ()
	{
        base.Start();

        basicAttackPower = attackPower1;
        secondaryAttackPower = attackPower2;

        primaryAttackCadence = attack1Cadence;
        secondaryAttackCadence = attack2Cadence;
	}
	
	// Update is called once per frame
    public override void Update ()
	{
		base.Update();

        if (isSelected && Input.GetKeyDown(KeyCode.D))
        {
            //Debug.Break();
            if (attack2Selected) // change to attack1
            {
                maxAttackDistance = visionSphereRadius;
                attackCadence = attack1Cadence;
                    
                attack2Selected = false;
            }
            else // change to attack2
            {
                maxAttackDistance = maxAttackDistance2;
                attackCadence = attack2Cadence;

                attack2Selected = true;
            }
        }
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
    // we call the base method, in the "distance-attack" case we have to check first if the 
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
                }
                else
                    currentArtilleryState = ArtilleryState.Alert;

                PlayAnimationCrossFade("Idle01");

                currentState = State.Idle;
            }
            else if (attackCadenceAux <= 0.0f)
            {
                // check that the enemy is in sight
                Debug.DrawLine(transform.position, enemySelected.transform.position, Color.yellow, 0.3f);

                Vector3 fwd = enemySelected.transform.position - this.transform.position;
                fwd.Normalize();
                Vector3 aux = transform.position + eyesPosition + (fwd * maxAttackDistance);
                Debug.DrawLine(transform.position + eyesPosition, aux, Color.blue, 0.2f);
                RaycastHit myHit;
                if (Physics.Raycast(transform.position + eyesPosition, fwd, out myHit, maxAttackDistance))
                {
                    ControllableCharacter enemy = myHit.transform.GetComponent<ControllableCharacter>();
                    if ((enemy != null) && (enemy == enemySelected))
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
                        GameObject particles2 = (GameObject)Instantiate(shotParticles,
                            dummyRightWeaponGunBarrel.transform.position,
                            transform.rotation);
                        Destroy(particles2, 0.4f);

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
                                currentArtilleryState = ArtilleryState.Alert;

                            PlayAnimationCrossFade("Idle01");
                            currentState = State.Idle;
                        }
                    }
                    else
                    {
                        // the enemy is NOT on sight
                        currentArtilleryState = ArtilleryState.Alert;
                        PlayAnimationCrossFade("Idle01");
                        currentState = State.Idle;
                    }
                }
                else
                {
                    // the enemy is NOT on sight
                    currentArtilleryState = ArtilleryState.Alert;
                    PlayAnimationCrossFade("Idle01");
                    currentState = State.Idle;
                }
                // reset the timer
                attackCadenceAux = primaryAttackCadence;
            }
            else
                attackCadenceAux -= Time.deltaTime;
        }
    }

    protected override void PlayAnimationCrossFade (string animationName)
    {
        if ( (animationName == "Attack1") && attack2Selected )
            animation.CrossFade("Attack2");
        else
            base.PlayAnimationCrossFade(animationName);
    }

    public override int GetUnitType()
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
