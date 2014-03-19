using UnityEngine;
using System.Collections;

public class UnitController : ControllableCharacter
{
    
    protected float basicAttackPower;
    protected float secondaryAttackPower;

    protected int attackSelected = 1;

    // the blood particles for when the unit has been hit
    public GameObject bloodParticles;

    protected enum State
    {
        Idle,	// reposo
        GoingTo,
        Attacking
    }
	protected State currentState = State.Idle;
    private State lastState = State.Idle;
	
    public float velocity = 3.5f;
    public float rotationVelocity = 10.0f;
    public Vector3 dirMovement = new Vector3();
    private Vector3 destiny = new Vector3();
    protected float destinyThreshold = 0.5f;

    // health bar
    public Texture2D progressBarEmpty, progressBarFull;

    // modelo del asset (el que contiene las animaciones)
    protected Transform model;

    // indicates if the CSelectable component of the unit is marked selected
    public bool isSelected = false;

    public virtual void Awake ()
    {
        model = transform.FindChild("Model");
    }

    // Use this for initialization
    public override void Start ()
    {
        base.Start();

        currentLife = maximunLife;
        GetComponent<NavMeshAgent>().speed = velocity;

        if (destiny == Vector3.zero)
        {
            destiny = transform.position;
            animation.Play("Idle01");
        }
        else
        {
            currentState = State.GoingTo;
            animation.Play("Walk");
        }
    }

    // Update is called once per frame
    public virtual void Update ()
    {
        switch (currentState)
        {
            case State.Idle:

                break;
            case State.GoingTo:
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
                    animation.CrossFade("Idle01");
                }
                break;
        }
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

        animation.CrossFade("Walk");
    }

    protected void StopMoving ()
    {
        destiny = transform.position;
        GetComponent<NavMeshAgent>().destination = destiny;
        currentState = State.Idle;

        animation.CrossFade("Idle01");
    }

    public virtual void RightClickOnSelected (Vector3 destiny, Transform destTransform)
    {
        GoTo(destiny);
    }

    // this method is called when a unit collides with the army base
    public virtual void ArrivedToBase ()
    {

    }

    public override bool Damage (float damage, char type)
    {
        base.Damage(damage,type);

        // blood!
        GameObject blood = (GameObject)Instantiate(bloodParticles,
            transform.position + transform.forward, transform.rotation);
        Destroy(blood, 0.4f);
        
        if (currentLife <= 0)
        {
            // the unit DIES, play the dead animation
            animation.CrossFade("Die");
            // and comunicate it to the army manager
            baseController.armyController.UnitDied(this.gameObject);

            return true;
        }
        else
            return false;
    }

}
