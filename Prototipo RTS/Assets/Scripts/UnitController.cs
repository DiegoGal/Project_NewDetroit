using UnityEngine;
using System.Collections;

public class UnitController : MonoBehaviour
{

    private enum State
    {
        Iddle,	// reposo
        GoingTo,
        Harvesting,
        Attacking
    }
	private State currentState = State.Iddle;
	
    public float velocity = 5.0f;
    public Vector3 dirMovement = new Vector3();
    private Vector3 destiny = new Vector3();
    private int destinyThreshold = 1;

    // Use this for initialization
    void Start()
    {
        if (destiny == Vector3.zero)
            destiny = transform.position;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        switch (currentState)
        {
            case State.Iddle:

                break;
            case State.GoingTo:
                Vector3 direction = destiny - transform.position;
                if (direction.magnitude >= destinyThreshold)
                {
                    transform.position += direction.normalized *
                        velocity * Time.deltaTime;
                    transform.position = new Vector3(transform.position.x, 1, transform.position.z);
                }
                else
                    currentState = State.Iddle;
                break;
        }
    }

    public void GoTo(Vector3 destiny)
    {
        this.destiny = destiny;
        currentState = State.GoingTo;
    }

    public virtual void RightClickOnSelected(Vector3 destiny, Transform destTransform)
    {
        GoTo(destiny);
    }

}
