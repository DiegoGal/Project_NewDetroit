using UnityEngine;
using System.Collections;

public class UnitController : ControllableCharacter
{
    
    protected float basicAttackPower;
    protected float secondaryAttackPower;

    // the blood particles for when the unit has been hit
    public GameObject bloodParticles;

    protected enum State
    {
        Idle,	// reposo
        GoingTo,
        GoingToAnEnemy,
        Attacking,
        Flying,
        Dying, // the unit is falling death
        AscendingToHeaven
    }
	protected State currentState = State.Idle;
    private State lastState = State.Idle;
	
    public float velocity = 3.5f;
    public float rotationVelocity = 10.0f;
    public Vector3 dirMovement = new Vector3();
    protected Vector3 destiny = new Vector3();
    protected float destinyThreshold = 0.5f;

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
    private float ascendingAceleration = 1.045f;

    // atributes for the attack
    protected ControllableCharacter lastEnemyAttacked;
    protected ControllableCharacter enemySelected;

    // Cool Down for detecting less time the collision with particles
    private float CDParticleCollision;

    private float posY = -1.0f;
    private float lastPosY = -1.0f;
    private bool goingDown = false;
    private Quaternion desiredRotation;

    public virtual void Awake ()
    {
        model = transform.FindChild("Model");
    }

    // Use this for initialization
    public override void Start ()
    {
        base.Start();

        timeToNextWaitAnimation = Random.Range(5.0f, 15.0f);

        currentLife = maximunLife;
        GetComponent<NavMeshAgent>().speed = velocity;

        if (destiny == Vector3.zero)
        {
            destiny = transform.position;
            PlayAnimation("Idle01");
        }
        else
        {
            currentState = State.GoingTo;
            PlayAnimation("Walk");
        }
    }

    // Update is called once per frame
    public virtual void Update ()
    {
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
            PlayAnimationCrossFade("Idle Wait");
            PlayAnimationCrossFadeQueued("Idle01");
            timeToNextWaitAnimation = Random.Range(5.0f, 15.0f);
        }
    }

    protected virtual void UpdateGoingTo ()
    {
        //Vector3 direction = destiny - transform.position;
        Vector3 direction = destiny - transform.position;
        if (direction.magnitude >= destinyThreshold)
        {
            /*Quaternion qu = new Quaternion();
            qu.SetLookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, qu, Time.deltaTime * rotationVelocity);
            transform.position += direction.normalized *
                velocity * Time.deltaTime;*/

            //transform.position = new Vector3(transform.position.x, 1, transform.position.z);
            //transform.Translate(direction.normalized * velocity * Time.deltaTime);

            //GetComponent<NavMeshAgent>().destination = destiny;
        }
        else
        {
            currentState = State.Idle;
            PlayAnimationCrossFade("Idle01");
        }
    }

    protected virtual void UpdateGoingToAnEnemy ()
    {
        // 1- comprobamos si el enemigo está "a mano" y se le puede atacar
        float distToEnemy = Vector3.Distance(transform.position, enemySelected.transform.position);
        if (distToEnemy <= maxAttackDistance)
        {
            // change to Attack state
            currentState = State.Attacking;
            PlayAnimationCrossFade("Attack1");
            GetComponent<NavMeshAgent>().destination = transform.position;

            transform.LookAt(enemySelected.transform);
        }
        // 2- comprobamos si el enemigo esta "a vista"
        else if (distToEnemy <= visionSphereRadious)
        {
            this.destiny = enemySelected.transform.position;
            GetComponent<NavMeshAgent>().destination = destiny;
        }
        // 3- se ha llegado al destino y se ha perdido de vista al enemigo
        else if (Vector3.Distance(transform.position, destiny) <= destinyThreshold)
        {
            StopMoving();
        }
    }

    protected virtual void UpdateAttacking ()
    {
        attackCadenceAux -= Time.deltaTime;

        float enemyDist = Vector3.Distance(transform.position, enemySelected.transform.position);
        if (enemySelected)
        {
            if (enemyDist <= maxAttackDistance)
            {
                if (attackCadenceAux <= 0)
                {
                    transform.LookAt(enemySelected.transform);

                    attackCadenceAux = attackCadence;

                    if (enemySelected.Damage(basicAttackPower))
                    {
                        // the enemy has die
                        enemySelected = null;
                        currentState = State.Idle;

                        PlayAnimationCrossFade("Idle01");
                        attackCadenceAux = 0.5f;
                    }
                }
            }
            else if (enemyDist <= visionSphereRadious)
            {
                currentState = State.GoingToAnEnemy;

                this.destiny = enemySelected.transform.position;
                GetComponent<NavMeshAgent>().destination = destiny;

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
    }

    protected virtual void UpdateFlying()
    {
        float delta = 0.4f;
        if (!goingDown)
        {
            if (transform.position.y > lastPosY)
            {
                lastPosY = transform.position.y;
            }
            else
                goingDown = true;
        }
        else if (transform.position.y <= posY + delta)
        {
            GetComponent<NavMeshAgent>().Resume();
            Destroy(rigidbody);
            currentState = State.Idle;
            lastPosY = -1.0f;
            goingDown = false;
        }
    
    }

    private void UpdateDying ()
    {
        timeFallingWhenDying -= Time.deltaTime;
        if (timeFallingWhenDying <= 0.0f)
        {
            // remove the assets of the model
            RemoveAssetsFromModel();

            currentState = State.AscendingToHeaven;
        }
    }

    private void UpdateAscendingToHeaven ()
    {
        // elevate the model
        transform.position = new Vector3
        (
            transform.position.x,
            transform.position.y * ascendingAceleration,// + 0.01f,
            transform.position.z
        );
        // update the Alpha Multiply Value of the material
        float alphaValue = model.renderer.material.GetFloat("_AlphaMultiplyValue");
        alphaValue *= 0.97f;
        alphaValue -= 0.006f;
        model.renderer.material.SetFloat("_AlphaMultiplyValue", alphaValue);
        if (alphaValue <= 0.0f)
            Destroy(this.gameObject);
    }

    public override void OnGUI ()
    {
        base.OnGUI();
        /*Vector2 size = new Vector2(48.0f, 12.0f);

        // draw the background:
        GUI.BeginGroup(new Rect(screenPosition.x, Screen.height - screenPosition.y, size.x, size.y));
            GUI.Box(new Rect(0, 0, size.x, size.y), progressBarEmpty);

            // draw the filled-in part:
            GUI.BeginGroup(new Rect(0, 0, size.x * (float)currentLife / (float)maximunLife, size.y));
                GUI.Box(new Rect(0, 0, size.x, size.y), progressBarFull);
            GUI.EndGroup();

        GUI.EndGroup();*/

        // rectángulo donde se dibujará la barra
        Rect rect1 = new Rect(screenPosition.x - 10.0f, Screen.height - screenPosition.y - 30.0f, 20.0f, 3.0f);
        GUI.DrawTexture(rect1, progressBarEmpty);
        Rect rect2 = new Rect(screenPosition.x - 10.0f, Screen.height - screenPosition.y - 30.0f, 20.0f * (currentLife / maximunLife), 3.0f);
        GUI.DrawTexture(rect2, progressBarFull);
    }

    public void GoTo (Vector3 destiny)
    {
        this.destiny = destiny;
        GetComponent<NavMeshAgent>().destination = destiny;
        currentState = State.GoingTo;

        PlayAnimationCrossFade("Walk");
    }

    protected void StopMoving ()
    {
        destiny = transform.position;
        GetComponent<NavMeshAgent>().destination = destiny;
        currentState = State.Idle;

        PlayAnimationCrossFade("Idle01");
    }

    public virtual void RightClickOnSelected (Vector3 destiny, Transform destTransform)
    {
        GoTo(destiny);

        ControllableCharacter unit = destTransform.transform.GetComponent<ControllableCharacter>();
        if ( (unit != null) && (teamNumber != unit.teamNumber) )
        {
            enemySelected = unit;
            currentState = State.GoingToAnEnemy;
        }
        else
            enemySelected = null;
    }

    // this method is called when a unit collides with the army base
    public virtual void ArrivedToBase ()
    {

    }

    public override bool Damage (float damage, char type)
    {
        if (currentState != State.Dying && currentState != State.AscendingToHeaven)
        {
            base.Damage(damage, type);

            // blood!
            GameObject blood = (GameObject)Instantiate(bloodParticles,
                transform.position + transform.forward, transform.rotation);
            Destroy(blood, 0.4f);

            if (currentLife <= 0)
            {
                //Debug.Log("MUEROOOOOOOOOO");
                currentState = State.Dying;
                // the unit DIES, set the special material
                if (dyingMaterial)
                    model.renderer.material = dyingMaterial;
                // play the dead animation
                PlayAnimationCrossFade("Die");
                // and comunicate it to the army manager
                baseController.armyController.UnitDied(this.gameObject);

                // delete the Nave Mesh Agent for elevate the model
                Destroy(GetComponent<NavMeshAgent>());

                return true;
            }
            else
                return false;
        }
        else
            return true;
    }

    protected virtual void RemoveAssetsFromModel ()
    {
    }

    protected virtual void PlayAnimation (string animationName)
    {
        animation.Play(animationName);
    }

    protected virtual void PlayAnimationQueued (string animationName)
    {
        animation.PlayQueued(animationName);
    }

    protected virtual void PlayAnimationCrossFade (string animationName)
    {
        animation.CrossFade(animationName);
    }

    protected virtual void PlayAnimationCrossFadeQueued (string animationName)
    {
        animation.CrossFadeQueued(animationName);
    }

    public virtual int GetUnitType ()
    {
        return -1;
    }

    //This is for the particles that collides with the orc
    void OnParticleCollision (GameObject other)
    {
        // get the particle system
        ParticleSystem particleSystem;
        particleSystem = other.GetComponent<ParticleSystem>();
        //If the particle is a Moco    
        if (particleSystem.tag == "Moco")
        {
            if (CDParticleCollision > 0)
                CDParticleCollision -= Time.deltaTime;
            else
            {
                Damage(particleSystem.GetComponent<ParticleDamage>().getDamage(), 'M');
                CDParticleCollision = 0.1f; // 5 deltatime aprox
            }
        }
    }

    public void Fly()
    {
        currentState = State.Flying;
        if (posY == -1.0f)
        {
            posY = transform.position.y;
            desiredRotation = transform.rotation;
        }
    }

}
