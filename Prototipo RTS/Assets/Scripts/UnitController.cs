using UnityEngine;
using System.Collections;

public class UnitController : MonoBehaviour
{
    public int teamNumber;
    public int maximunLife = 100;
    protected int currentLife;

    protected int basicAttackPower;
    protected int secondaryAttackPower;

    protected int attackSelected = 1;

    protected enum State
    {
        Iddle,	// reposo
        GoingTo,
        /*Harvesting,*/
        Attacking
    }
	protected State currentState = State.Iddle;
    private State lastState = State.Iddle;
	
    public float velocity = 3.5f;
    public float rotationVelocity = 10.0f;
    public Vector3 dirMovement = new Vector3();
    private Vector3 destiny = new Vector3();
    protected float destinyThreshold = 1.0f;

    // referencia a la posición de la base de la unidad
    protected Vector3 basePosition = new Vector3();
    // referencia a la base
    public BaseController baseController;

    // health bar
    public Texture2D progressBarEmpty, progressBarFull;

    // Use this for initialization
    public virtual void Start ()
    {
        currentLife = maximunLife;
        GetComponent<NavMeshAgent>().speed = velocity;

        if (destiny == Vector3.zero)
            destiny = transform.position;
    }

    // Update is called once per frame
    public virtual void Update ()
    {
        switch (currentState)
        {
            case State.Iddle:

                break;
            case State.GoingTo:
                //Vector3 direction = destiny - transform.position;
                Vector3 direction = new Vector3(destiny.x - transform.position.x, 0,
                    destiny.z - transform.position.z);
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
                    currentState = State.Iddle;
                break;
        }
    }

    public virtual void OnGUI()
    {
        // http://answers.unity3d.com/questions/11892/how-would-you-make-an-energy-bar-loading-progress.html
        Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 size = new Vector2(200.0f, 30.0f);

        // draw the background:
        GUI.BeginGroup(new Rect(camPos.x, Screen.height - camPos.y, size.x, size.y));
            GUI.Box(new Rect(0, 0, size.x, size.y), progressBarEmpty);

            // draw the filled-in part:
            GUI.BeginGroup(new Rect(0, 0, size.x * (float)currentLife / (float)maximunLife, size.y));
                GUI.Box(new Rect(0, 0, size.x, size.y), progressBarFull);
            GUI.EndGroup();

        GUI.EndGroup();
    }

    public void SetBasePosition (Vector3 basePosition)
    {
        this.basePosition = basePosition;
    }

    public void SetArmyBase (BaseController baseController)
    {
        this.baseController = baseController;
    }

    public void GoTo (Vector3 destiny)
    {
        this.destiny = destiny;
        GetComponent<NavMeshAgent>().destination = destiny;
        currentState = State.GoingTo;
    }

    public virtual void RightClickOnSelected (Vector3 destiny, Transform destTransform)
    {
        GoTo(destiny);
    }

    // this method is called when a unit collides with the army base
    public virtual void ArrivedToBase ()
    {

    }

    public bool Damage (int damage)
    {
        //Debug.Log("damage");
        currentLife -= damage;
        if (currentLife <= 0)
        {
            // the unit DIES
            Destroy(this.gameObject, 0.5f);

            return true;
        }
        else
            return false;
    }

}
