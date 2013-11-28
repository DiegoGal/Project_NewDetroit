using UnityEngine;
using System.Collections;

public class UnitController : MonoBehaviour
{

    private enum State
    {
        Iddle,	// reposo
        GoingTo
    }
	private State currentState = State.Iddle;
	
    public float velocity = 5.0f;
    public Vector3 dirMovement;
    private Vector3 destiny;
    private int destinyThreshold;

    // Use this for initialization
    void Start ()
    {
        currentState = State.Iddle;

        dirMovement = new Vector3();
        destiny = transform.position;
        destinyThreshold = 1;
    }

    // Update is called once per frame
    void Update ()
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

    public void GoTo (Vector3 destiny)
    {
        this.destiny = destiny;
        currentState = State.GoingTo;
    }
}
