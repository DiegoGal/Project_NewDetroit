using UnityEngine;
using System.Collections;

public class UnitHeavyArtillery : UnitArtillery
{

    public float attackPower1 = 20.0f;
    public float attackPower2 = 40.0f;

    // the vision radious of the unit
    // base: protected float visionSphereRadius;
    // extended vision radious (when its Deployed)
    // this is added to the original radious
    public float visionSphereRadiusExtended = 8.0f;

    // we need to save the original size of the vision sphere
    // in order to restore it after the deployed mode
    private float visionSphereRaiusOriginal;

    public GameObject frontWeapon, backWeapon;

    //For attacking1
    public GameObject rocket;
    private GameObject newRocket;
    bool zoneAttackMode = false;
    Vector3 zoneAttack;

    protected enum DeployState
    {
        Undeployed,	// sin desplegar
        Deploying,  // desplegando
        Deployed,   // desplegado
        Undeploying // anular despliegue
    }
    protected DeployState currentDeployState = DeployState.Undeployed;

    public override void Awake ()
    {
        base.Awake();

        numberOfWeapons = 1;
        // Por si no se han establecido las referencias a los dummys del modelo
        // en el editor de Unity las buscamos ahora:
        if (dummyLeftWeapon == null)
            dummyLeftWeapon = transform.FindChild("Bip002/Bip002 Pelvis/Bip002 Spine/Bip002 Spine1/Bip002 Neck/Bip002 R Clavicle/Bip002 R UpperArm/Bip002 R Forearm/Bip002 R Hand/arma mano derecha");
        if (dummyLeftWeaponGunBarrel == null)
            dummyLeftWeaponGunBarrel = dummyLeftWeapon.FindChild("GoblinHeavyArtilleryWeapon01_A/GunBarrelLeft");
        if (dummyRightWeapon == null)
            dummyRightWeapon = transform.FindChild("Bip002/Bip002 Pelvis/Bip002 Spine/mortero espalda");   
        if (dummyLeftWeapon)
            frontWeapon = dummyLeftWeapon.FindChild("GoblinHeavyArtilleryWeapon01_A").gameObject;
        if (dummyRightWeapon)
            backWeapon = dummyRightWeapon.FindChild("GoblinHeavyArtilleryWeapon01_B").gameObject;
    }

	// Use this for initialization
    public override void Start ()
	{
        base.Start();

        basicAttackPower = attackPower1;
        secondaryAttackPower = attackPower2;

        visionSphereRaiusOriginal = visionSphereRadius;

        maxAttackDistance1 = visionSphereRaiusOriginal;
        maxAttackDistance2 = visionSphereRaiusOriginal + visionSphereRadiusExtended;
        maxAttackDistance = maxAttackDistance1;

        attackCadenceAux = 0.0f;
        attackCadence = 2.0f;
	}
	
	// Update is called once per frame
    public override void Update ()
	{
        //if (currentDeployState == DeployState.Undeployed || currentDeployState == DeployState.Deployed)
		    base.Update();

        if (isSelected && Input.GetKeyDown(KeyCode.D))
        {
            switch (currentDeployState)
            {
                case DeployState.Undeployed:
                    StopMoving();
                    animation.CrossFade("Deployment-prepare");
                    StartCoroutine(WaitAndCallback(animation["Deployment-prepare"].length));
                    animation.CrossFadeQueued("Deployment-iddle");
                    currentDeployState = DeployState.Deploying;
                    break;
                case DeployState.Deploying:
                    /*animation.CrossFade("Deployment-Up");
                    StartCoroutine(WaitAndCallback(animation["Deployment-Up"].length));
                    animation.CrossFadeQueued("Idle01");
                    currentDeployState = DeployState.Undeploying;*/
                    break;
                case DeployState.Deployed:
                    if (currentState != State.Attacking)
                    {
                        animation.CrossFade("Deployment-Up");
                        StartCoroutine(WaitAndCallback(animation["Deployment-Up"].length));
                        animation.CrossFadeQueued("Idle01");
                        currentDeployState = DeployState.Undeploying;
                    }
                    break;
                case DeployState.Undeploying:
                    /*animation.CrossFade("Deployment-prepare");
                    StartCoroutine(WaitAndCallback(animation["Deployment-prepare"].length));
                    animation.CrossFadeQueued("Deployment-iddle");
                    currentDeployState = DeployState.Deploying;*/
                    break;
            }
        }

	} // Update

    protected override void UpdateGoingToAnEnemy ()
    {
        // si esta seleccionado el ataque normal (sin desplegar) miramos si la unidad
        // de verdad "ve" al enemigo seleccionado
        if (currentDeployState == DeployState.Undeployed)
        {
            if (currentArtilleryState == ArtilleryState.Alert)
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
        else if (currentDeployState == DeployState.Deployed)
        {

        }
    }

    protected override void UpdateAttacking ()
    {
        if (currentDeployState == DeployState.Undeployed) // if undeployed
            base.UpdateAttacking();
        else
        {
            attackCadenceAux -= Time.deltaTime;
            // depend on the type of attack
            float enemyDist;
            if (zoneAttackMode)
                enemyDist = Vector3.Distance(transform.position, zoneAttack);
            else
                enemyDist = Vector3.Distance(transform.position, enemySelected.transform.position);

            if (enemySelected || zoneAttackMode)
            {
                if ((enemySelected || zoneAttackMode) && (enemyDist <= maxAttackDistance))
                {
                    if (attackCadenceAux <= 0)
                    {
                        animation.Play("Deployment-Shot");
                         if (enemySelected)
                             transform.LookAt(enemySelected.transform);

                        // Instanciate a new Rocket
                        Debug.Log("Dummy position: " + dummyLeftWeaponGunBarrel.transform.position);
                        Debug.Log("HeavyArtillery position: " + transform.position);
                        newRocket = Instantiate
                        (
                            rocket,
                            dummyLeftWeaponGunBarrel.transform.position,
                            new Quaternion()
                        ) as GameObject;
                        newRocket.rigidbody.isKinematic = false;
                        newRocket.transform.name = "Rocket";
                        newRocket.transform.rotation = dummyLeftWeaponGunBarrel.transform.rotation;
                        newRocket.transform.FindChild("RocketVisionCapsule").GetComponent<CRocketVisionCapsule>().SetOwner(this.gameObject);
                        newRocket.transform.FindChild("RocketVisionCapsule").GetComponent<CRocketVisionCapsule>().SetDamage((int)attackPower2);
                        newRocket.transform.FindChild("RocketVisionCapsule").GetComponent<CRocketVisionCapsule>().SetDestroyTime(2.5f);


                        attackCadenceAux = attackCadence;
                        Vector3 dir;
                        if (zoneAttackMode)
                            dir = zoneAttack - newRocket.transform.position;
                        else
                            dir = enemySelected.transform.position - newRocket.transform.position;
                        dir = dir.normalized;
                        Vector3 dir1 = transform.forward.normalized;
                        newRocket.transform.parent = null;
                        newRocket.rigidbody.AddForce
                        (
                            new Vector3
                            (
                                dir.x * 8.0f * (enemyDist / maxAttackDistance),
                                11,
                                dir.z * 8.0f * (enemyDist / maxAttackDistance)
                            ),
                            ForceMode.Impulse
                        );

                        if (enemySelected && enemySelected.life.currentLife <= 0.0f)
                        {
                            // the enemy has die
                            enemySelected = null;
                            currentState = State.Idle;
                            //PlayAnimationCrossFade("Idle01");
                            attackCadenceAux = 2f;
                        }
                    }
                }
                else
                {
                    enemySelected = null;
                    currentState = State.Idle;
                    attackCadenceAux = 2f;
                    //PlayAnimationCrossFade("Idle01");
                }
            }
            else // the enemy is no longer alive
            {
                enemySelected = null;
                currentState = State.Idle;
                //PlayAnimationCrossFade("Idle01");
                attackCadenceAux = 2f;
            }
        }
    }

    public override void RightClickOnSelected (Vector3 destiny, Transform destTransform)
    {
        if (currentDeployState == DeployState.Deployed && Input.GetKey(KeyCode.E))
        {
            zoneAttackMode = true;
            zoneAttack = destiny;
            // comprobamos que el enemigo seleccionado este a "vista"
            Debug.DrawLine(transform.position, destiny, Color.yellow, 0.3f);

            Vector3 fwd = destiny - this.transform.position;
            fwd.Normalize();
            Vector3 aux = transform.position + eyesPosition + (fwd * maxAttackDistance);
            Debug.DrawLine(transform.position + eyesPosition, aux, Color.blue, 0.2f);
            RaycastHit myHit;
            
            // rotate the unit in the enemy direction
            transform.LookAt(destiny);
            alertHitTimerAux = alertHitTimer;
            // Change the state
            currentState = State.Attacking;
            
        }
        else if (currentDeployState == DeployState.Deployed) // if deployed
        {
            zoneAttackMode = false;
            ControllableCharacter unit = destTransform.transform.GetComponent<ControllableCharacter>();
            if (unit)
            {
                // check if the unit is not attacking the selected enemy yet
                if (
                        currentState != State.Attacking ||
                        (currentState == State.Attacking && lastEnemyAttacked != unit)
                    )
                {
                    if (teamNumber != unit.teamNumber)
                    {
                        //attackedUnitViewID = destTransform.GetComponent<PhotonView>().viewID;
                        //GoTo(destiny);
                        enemySelected = unit;
                        //currentState = State.GoingToAnEnemy;

                        // comprobamos que el enemigo seleccionado este a "vista"
                        Debug.DrawLine(transform.position, enemySelected.transform.position, Color.yellow, 0.3f);

                        Vector3 fwd = enemySelected.transform.position - this.transform.position;
                        fwd.Normalize();
                        Vector3 aux = transform.position + eyesPosition + (fwd * maxAttackDistance);
                        Debug.DrawLine(transform.position + eyesPosition, aux, Color.blue, 0.2f);
                        RaycastHit myHit;
                        if (Physics.Raycast(transform.position + eyesPosition, fwd, out myHit, maxAttackDistance))
                        {
                            //Debug.Log(myHit.transform.name);
                            // the ray has hit something
                            ControllableCharacter enemy = myHit.transform.GetComponent<ControllableCharacter>();
                            if ((enemy != null) && (enemy == enemySelected))
                            {
                                // this "something" is the enemy we are looking for...
                                //Debug.Log("LE HE DADO!!!");
                                // rotate the unit in the enemy direction
                                transform.LookAt(enemy.transform.position);
                                lastEnemyAttacked = enemy;
                                alertHitTimerAux = alertHitTimer;

                                currentState = State.Attacking;
                            }
                        }
                    }
                }
            }
            else
            {
                enemySelected = null;
                currentState = State.Idle;
                //PlayAnimationCrossFade("Idle01");
                attackCadenceAux = 2f;
            }
        }
        else if (currentDeployState == DeployState.Undeployed) // if undeployed
        {
            zoneAttackMode = false;
            base.RightClickOnSelected(destiny, destTransform);
        }
    }

    public override void AttackMovement (Vector3 destiny)
    {
        if (currentDeployState == DeployState.Undeployed)
            base.AttackMovement(destiny);
    }

    private IEnumerator WaitAndCallback (float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        AnimationFinished();
    }

    private void AnimationFinished ()
    {
        switch (currentDeployState)
        {
            case DeployState.Undeployed:
                currentDeployState = DeployState.Deployed;
                attack2Selected = true;
                if (thereIsVisionSphere)
                    transform.FindChild("VisionSphere").GetComponent<SphereCollider>().radius =
                        visionSphereRadius + visionSphereRadiusExtended;
                else
                {
                    visionSphereRadius = visionSphereRaiusOriginal + visionSphereRadiusExtended;
                    maxAttackDistance = maxAttackDistance2;
                }
                    
                break;

            case DeployState.Deploying:

                currentDeployState = DeployState.Deployed;
                attack2Selected = true;
                if (thereIsVisionSphere)
                    transform.FindChild("VisionSphere").GetComponent<SphereCollider>().radius =
                        visionSphereRadius + visionSphereRadiusExtended;
                else
                {
                    visionSphereRadius = visionSphereRaiusOriginal + visionSphereRadiusExtended;
                    maxAttackDistance = maxAttackDistance2;
                }

                break;

            case DeployState.Deployed:

                currentDeployState = DeployState.Undeployed;
                attack2Selected = false;
                if (thereIsVisionSphere)
                    transform.FindChild("VisionSphere").GetComponent<SphereCollider>().radius =
                        visionSphereRadius;
                else
                {
                    visionSphereRadius = visionSphereRaiusOriginal;
                    maxAttackDistance = maxAttackDistance1;
                }

                break;

            case DeployState.Undeploying:

                currentDeployState = DeployState.Undeployed;
                attack2Selected = false;
                if (thereIsVisionSphere)
                    transform.FindChild("VisionSphere").GetComponent<SphereCollider>().radius =
                        visionSphereRadius;
                else
                {
                    visionSphereRadius = visionSphereRaiusOriginal;
                    maxAttackDistance = maxAttackDistance1;
                }

                break;
        }
    }

    protected override void PlayAnimationCrossFade (string animationName)
    {
        if (currentDeployState != DeployState.Undeployed)
        {
            if (animationName == "Idle01")
                animation.CrossFade("Deployment-iddle");
            /*else if (animationName == "Idle Wait")
                animation.CrossFade("Idle Wait Deployed");*/
            else if (animationName == "Attack2")
                animation.CrossFade("Deployment-Shot");
        }
        else
            base.PlayAnimationCrossFade(animationName);
    }

    protected override void PlayAnimationCrossFadeQueued (string animationName)
    {
        if (currentDeployState != DeployState.Undeployed)
        {
            if (animationName == "Idle01")
                animation.CrossFadeQueued("Deployment-iddle");
            /*else if (animationName == "Idle Wait")
                animation.CrossFade("Idle Wait Deployed");*/
        }
        else
            base.PlayAnimationCrossFadeQueued(animationName);
    }

    public override int GetUnitType ()
    {
        return 2;
    }

    protected override void RemoveAssetsFromModel ()
    {
        if (frontWeapon)
            Destroy(frontWeapon);
        if (backWeapon)
            Destroy(backWeapon);
    }

} // class UnitHeavyArtillery
