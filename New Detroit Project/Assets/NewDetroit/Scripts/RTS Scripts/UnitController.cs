﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class UnitController : ControllableCharacter
{
    
    protected float basicAttackPower;
    protected float secondaryAttackPower;

    public enum State
    {
        Idle,	// reposo
        GoingTo,
        GoingToAnEnemy,
        Attacking,
        Flying,
        Dying, // the unit is falling death
        AscendingToHeaven
    }
	public State currentState = State.Idle;
    public State lastState = State.Idle;
	
    public float velocity = 3.5f;
    protected Vector3 destiny = new Vector3();
    public float destinyThreshold = 0.6f;
    
    // health bar
    public Texture2D progressBarEmpty, progressBarFull;

    // modelo del asset (el que contiene las animaciones)
    protected Transform model;

    // indicates if the CSelectable component of the unit is marked selected
    public bool isSelected = false;

    // indicates the time remaining until the next waiting animation
    private float timeToNextWaitAnimation;

    // frecuencia (en segundos) de ataque
    public float attackCadence = 1.0f;
    protected float attackCadenceAux = 0.5f;

    // a special material for when the unit has died
    public Material dyingMaterial;

    private float timeFallingWhenDying = 1.6f;
    private float ascendingAceleration = 1.055f;

    // atributes for the attack
    protected CTeam lastEnemyAttacked;
    protected CTeam enemySelected;

    // Cool Down for detecting less time the collision with particles
    private float CDParticleCollision;

    private float posY = -1.0f;
    private float lastPosY = -1.0f;
    private bool goingDown = false;
    private int contTrapped = 0;

    // Reference to the CStateUnit Component of the Unit
    public CStateUnit cState;

    // Audio
    public AudioClip sfxDead;
    public AudioClip sfxOrder;
    public AudioClip sfxAttack;

    public override void Awake ()
    {
        base.Awake();

        cState = GetComponent<CStateUnit>();

        model = transform.FindChild("Model");

        radius = 0.25f;

    }

    // Use this for initialization
    public override void Start ()
    {
        base.Start();

        timeToNextWaitAnimation = Random.Range(5.0f, 15.0f);

        GetComponent<NavMeshAgent>().speed = velocity;
        GetComponent<NavMeshAgent>().stoppingDistance = destinyThreshold;

        if (destiny == Vector3.zero)
        {
            destiny = transform.position;
            PlayAnimation("Idle01");
        }
        else
        {
            currentState = State.GoingTo;
            cState.currentState = currentState;

            PlayAnimation("Walk");
        }
       
    }

    // Update is called once per frame
    public override void Update ()
    {
        base.Update();

        switch (currentState)
        {
            case State.Idle:              UpdateIdle();              break;
            case State.GoingTo:           UpdateGoingTo();           break;
            case State.GoingToAnEnemy:    UpdateGoingToAnEnemy();    break;
            case State.Attacking:         UpdateAttacking();         break;
            case State.Flying:            UpdateFlying();            break;
            case State.Dying:             UpdateDying();             break;
            case State.AscendingToHeaven: UpdateAscendingToHeaven(); break;
        }
    }

    protected virtual void UpdateIdle ()
    {
        // plays the waiting idle animation
        timeToNextWaitAnimation -= Time.deltaTime;
        if (timeToNextWaitAnimation <= 0)
        {
            PlayIdleWaitAnimation();
            timeToNextWaitAnimation = Random.Range(5.0f, 15.0f);
        }
        else
        {
            PlayAnimationCrossFadeQueued("Idle01");
        }
    }

    protected virtual void UpdateGoingTo ()
    {
        //Vector3 direction = destiny - transform.position;
        /*Vector3 direction = destiny - transform.position;
        if (direction.magnitude >= destinyThreshold)
        {
            Quaternion qu = new Quaternion();
            qu.SetLookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, qu, Time.deltaTime * rotationVelocity);
            transform.position += direction.normalized *
                velocity * Time.deltaTime;

            //transform.position = new Vector3(transform.position.x, 1, transform.position.z);
            //transform.Translate(direction.normalized * velocity * Time.deltaTime);

            //GetComponent<NavMeshAgent>().destination = destiny;
        }
        else
        {
            currentState = State.Idle;
            PlayAnimationCrossFade("Idle01");
        }*/

        /*NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent.pathStatus == NavMeshPathStatus.PathComplete &&
            agent.remainingDistance <= destinyThreshold)*/

        Vector3 direction = destiny - transform.position;
        /*Vector3 direction = new Vector3
        (
            destiny.x - transform.position.x,
            0.0f,
            destiny.z - transform.position.z
        );*/

        if (direction.magnitude <= destinyThreshold)
        {
            currentState = State.Idle;
            cState.currentState = currentState;
            PlayAnimationCrossFade("Idle01");
        }
        else 
        {
            PlayAnimationCrossFade("Walk");
        }
    }

    protected virtual void UpdateGoingToAnEnemy ()
    {
        // primero comprobamos que el enemigo siga "vivo"
        if (enemySelected)
        {
            // 1- comprobamos si el enemigo está "a mano" y se le puede atacar
            float distToEnemy = Vector3.Distance(transform.position, enemySelected.transform.position);
            float enemyRaduis = enemySelected.GetComponent<ControllableCharacter>().radius;
            if (distToEnemy <= maxAttackDistance + enemyRaduis)
            {
                // change to Attack state
                currentState = State.Attacking;
                cState.currentState = currentState;
                PlayAnimationCrossFade("Attack1");
                GetComponent<NavMeshAgent>().Stop();

                transform.LookAt(enemySelected.transform);
            }
            // 2- comprobamos si el enemigo esta "a vista"
            else if (distToEnemy <= visionSphereRadius)
            {
                this.destiny = enemySelected.transform.position;
                GetComponent<NavMeshAgent>().SetDestination(destiny);
            }
            // 3- se ha llegado al destino y se ha perdido de vista al enemigo
            else if (Vector3.Distance(transform.position, destiny) <= destinyThreshold)
            {
                StopMoving();
            }
        }
        else
        {
            // el enemigo ha sido eliminado
            //GoTo(destiny);
            currentState = State.GoingTo;
            cState.currentState = currentState;
        }

    }

    protected virtual void UpdateAttacking ()
    {
        attackCadenceAux -= Time.deltaTime;

        float enemyDist = Vector3.Distance(transform.position, enemySelected.transform.position);
        if (enemySelected)
        {
            float enemyRaduis = enemySelected.GetComponent<ControllableCharacter>().radius;
            if (enemyDist <= maxAttackDistance + enemyRaduis)
            {
                if (attackCadenceAux <= 0)
                {
                    transform.LookAt(enemySelected.transform);

                    attackCadenceAux = attackCadence;
					if (PhotonNetwork.connected)
                    	photonView.RPC("Kick", PhotonTargets.All, enemySelected.name, basicAttackPower);
                    else
                        enemySelected.GetComponent<CLife>().Damage(basicAttackPower);
                    if (!enemySelected.GetComponent<CLife>().IsAlive())
                    //if (enemySelected.Damage(basicAttackPower))
                    {
                        // the enemy has die
                        enemySelected = null;
                        currentState = State.Idle;

                        PlayAnimationCrossFade("Idle01");
                        attackCadenceAux = 0.5f;
                    }
                }
            }
            else if (enemyDist <= visionSphereRadius)
            {
                currentState = State.GoingToAnEnemy;

                this.destiny = enemySelected.transform.position;
                if (GetComponent<NavMeshAgent>() != null)
                    GetComponent<NavMeshAgent>().SetDestination(destiny);

                PlayAnimationCrossFade("Walk");
                attackCadenceAux = 0.5f;
            }
            else
            {
                enemySelected = null;
                currentState = State.Idle;

                PlayAnimationCrossFade("Idle01");
            }
        }
        else // the enemy is no longer alive
        {
            enemySelected = null;
            currentState = State.Idle;

            PlayAnimationCrossFade("Idle01");
            attackCadenceAux = 0.5f;
        }
        cState.currentState = currentState;
    }

    protected virtual void UpdateFlying ()
    {
        if (currentState != State.Dying)
        {
            int maxTrapped = 25;
            if (!goingDown)
            {
                rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                contTrapped = 0;
                if (transform.position.y > lastPosY)
                {
                    lastPosY = transform.position.y;
                }
                else
                    goingDown = true;
            }
            else if ((transform.position.y == lastPosY)  && goingDown)
            {
                if (contTrapped == maxTrapped)
                {
                    if (GetComponent<NavMeshAgent>())
                    {
                        GetComponent<NavMeshAgent>().Resume();
                        GetComponent<NavMeshAgent>().ResetPath();
                    }
                    Destroy(this.rigidbody);
                    //currentState = State.Idle;

                    if (lastState == State.GoingTo)
                        GoTo(destiny);
                    else
                    {
                        currentState = lastState;
                        cState.currentState = currentState;
                    }
                    lastPosY = -1.0f;
                    goingDown = false;
                    contTrapped = 0;
                    
                }
                else
                    contTrapped++;
                lastPosY = transform.position.y;
            }
            else
                lastPosY = transform.position.y;
        }
    }

    [RPC]
    public void ChangeToDyingMaterial()
    {
        // update the Alpha Multiply Value of the material
        float alphaValue = model.renderer.material.GetFloat("_AlphaMultiplyValue");
        alphaValue *= 0.97f;
        alphaValue -= 0.006f;
        model.renderer.material.SetFloat("_AlphaMultiplyValue", alphaValue);
    }

    private void UpdateDying ()
    {
        timeFallingWhenDying -= Time.deltaTime;
        if (timeFallingWhenDying <= 0.0f)
        {
            // remove the assets of the model
            RemoveAssetsFromModel();

            // elevate a little the unit just in case the y is 0
            transform.position = new Vector3
            (
                transform.position.x,
                0.01f,
                transform.position.z
            );

            currentState = State.AscendingToHeaven;
            cState.currentState = currentState;
            if (PhotonNetwork.connected)
            	photonView.RPC("ChangeToDyingMaterial", PhotonTargets.All);
            else
            	ChangeToDyingMaterial();
        }
    }

    private void UpdateAscendingToHeaven ()
    {
        // elevate the model
        transform.position = new Vector3
        (
            transform.position.x,
            transform.position.y * ascendingAceleration + 0.01f,
            transform.position.z
        );
        // update the Alpha Multiply Value of the material
        float alphaValue = model.renderer.material.GetFloat("_AlphaMultiplyValue");
        alphaValue *= 0.97f;
        alphaValue -= 0.006f;
        model.renderer.material.SetFloat("_AlphaMultiplyValue", alphaValue);
        if (alphaValue <= 0.0f)
            //Destroy(this.gameObject);
			if (PhotonNetwork.connected)
            	PhotonNetwork.Destroy(gameObject);
			else
				Destroy(gameObject);
    }

    public virtual void OnGUI ()
    {
        /*Vector2 size = new Vector2(48.0f, 12.0f);

         draw the background:
        GUI.BeginGroup(new Rect(screenPosition.x, Screen.height - screenPosition.y, size.x, size.y));
            GUI.Box(new Rect(0, 0, size.x, size.y), progressBarEmpty);

             draw the filled-in part:
            GUI.BeginGroup(new Rect(0, 0, size.x * (float)currentLife / (float)maximunLife, size.y));
                GUI.Box(new Rect(0, 0, size.x, size.y), progressBarFull);
            GUI.EndGroup();

        GUI.EndGroup();*/

        // rectángulo donde se dibujará la barra
        if (currentState != State.AscendingToHeaven)
        {
            Rect rect1 = new Rect(screenPosition.x - 10.0f, Screen.height - screenPosition.y - 30.0f, 20.0f, 3.0f);
            GUI.DrawTexture(rect1, progressBarEmpty);
            Rect rect2 = new Rect(screenPosition.x - 10.0f, Screen.height - screenPosition.y - 30.0f, 20.0f * (life.currentLife / life.maximunLife), 3.0f);
            GUI.DrawTexture(rect2, progressBarFull);
        }
    }

    public void GoTo (Vector3 destiny)
    {
        if (currentState != State.Dying && currentState != State.AscendingToHeaven)
        {
            this.destiny = destiny;
            GetComponent<NavMeshAgent>().SetDestination(destiny);
            currentState = State.GoingTo;
            cState.currentState = currentState;

            PlayAnimationCrossFade("Walk");
        }
    }

    protected void StopMoving ()
    {
        destiny = transform.position;
        //GetComponent<NavMeshAgent>().destination = destiny;
        GetComponent<NavMeshAgent>().Stop();
        currentState = State.Idle;
        cState.currentState = currentState;

        PlayAnimationCrossFade("Idle01");
    }

    [RPC]
    public void Kick (string otherName, float damage)
    {
        GameObject other = GameObject.Find(otherName);
        CLife otherCL = other.GetComponent<CLife>();
        otherCL.Damage(damage);
    }

    public virtual void RightClickOnSelected (Transform destTransform)
    {
        RightClickOnSelected(destTransform.position, destTransform);
    }

    public virtual void RightClickOnSelected (Vector3 destiny, Transform destTransform)
    {
        CTeam unit = destTransform.transform.GetComponent<CTeam>();
        if (unit && teamNumber != unit.teamNumber)
        {
            // check if the unit is not attacking the selected enemy yet
            if (
                 currentState != State.Attacking ||
                 (currentState == State.Attacking && lastEnemyAttacked != unit)
               )
            {
                GoTo(destiny);
                enemySelected = unit;
                currentState = State.GoingToAnEnemy;
                cState.currentState = currentState;
            }
        }
        else
        {
            // play the order sfx
            audio.PlayOneShot(sfxOrder);

            enemySelected = null;
            GoTo(destiny);
        }
    }

    // this method is called when a unit collides with the army base
    public virtual void ArrivedToBase ()
    {

    }

    // this method is called from the CLife component when the life is <= 0
    public virtual void UnitDiedMessage ()
    {
        //Debug.Log("MUEROOOOOOOOOO");
        currentState = State.Dying;
        cState.currentState = currentState;
        // the unit DIES, set the special material
        setDyingMaterial();
        // play the dead animation             
        PlayAnimationCrossFade("Die");
        // and comunicate it to the army manager              
        if (baseController)
            baseController.armyController.UnitDied(this.gameObject);

        // play the dead sfx
        audio.Stop();
        audio.PlayOneShot(sfxDead);

        // delete the Nave Mesh Agent for elevate the model
        if (GetComponent<NavMeshAgent>())
            Destroy(GetComponent<NavMeshAgent>());

        Minimap.DeleteUnit(this);
    }

    public void setDyingMaterial()
    {
        if (dyingMaterial)
            model.renderer.material = dyingMaterial; 
    }

    protected virtual void RemoveAssetsFromModel () { }

    public virtual void ChangeAttack () { }

    protected virtual void PlayAnimation (string animationName)
    {
        //animation.Play(animationName);
        cState.animationName  = animationName;
        cState.animationChanged = true;
    }

    protected virtual void PlayAnimationQueued (string animationName)
    {
        //animation.PlayQueued(animationName);
        cState.animationNameQueued = animationName;
        cState.animationChangeQueued = true;
    }

    protected virtual void PlayAnimationCrossFade (string animationName)
    {
        //animation.CrossFade(animationName);
        cState.animationName = animationName;
        cState.animationChanged = true;
    }

    protected virtual void PlayAnimationCrossFadeQueued (string animationName)
    {
        //animation.CrossFadeQueued(animationName);
        cState.animationNameQueued = animationName;
        cState.animationChangeQueued = true;
    }

    protected virtual void PlayIdleWaitAnimation ()
    {
        PlayAnimationCrossFade("Idle Wait");
        PlayAnimationCrossFadeQueued("Idle01");
    }

    public virtual int GetUnitType ()
    {
        return -1;
    }

    //This is for the particles that collides with the orc
//    void OnParticleCollision (GameObject other)
//    {
//        // get the particle system
//        ParticleSystem particleSystem;
//        particleSystem = other.GetComponent<ParticleSystem>();
//        //If the particle is a Moco    
//        if (particleSystem.tag == "Moco")
//        {
//            if (CDParticleCollision > 0)
//                CDParticleCollision -= Time.deltaTime;
//            else
//            {
//                life.Damage(particleSystem.GetComponent<ParticleDamage>().GetDamage(), 'M');
//                CDParticleCollision = 0.1f; // 5 deltatime aprox
//            }
//        }
//    }

    public void Fly ()
    {
        if (currentState != State.Dying && currentState != State.Flying)
        {
            goingDown = false;
            lastState = currentState;
            currentState = State.Flying;
            cState.currentState = currentState;
            if (posY == -1.0f)
            {
                posY = transform.position.y;
            }
        }
    }

    public virtual void AttackMovement (Vector3 destiny)
    {
        GoTo(destiny);
    }


}
