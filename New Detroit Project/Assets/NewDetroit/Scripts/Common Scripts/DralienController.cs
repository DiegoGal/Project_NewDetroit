using UnityEngine;
using System.Collections;

public class DralienController : MonoBehaviour
{

    private Vector3 nextDestiny;

    public float timeToSearchForNewDest = 5.0f;
    private float timeToSearchForNewDestAux;

    public float distanceToNextDest = 6.0f;

    private enum State
    {
        Idle,
        GoingTo,
        Attacking,
        Dying
    };
    private State currentState = State.Idle;

    private float timeToNextEat = 8.0f;
    private float timeToNextEatAux;

    private float distance;

    private Vector3 prevPosition;
    private int positionRepeatCount = 0;
    private int maxPosRepeatCount = 50;

	// Use this for initialization
	void Start ()
    {
        nextDestiny = transform.position;
        timeToSearchForNewDestAux = timeToSearchForNewDest - Random.Range(0.0f, 2.0f);

        timeToNextEatAux = timeToNextEat - Random.Range(0.0f, 5.0f);

        animation.Play("Digging");
        animation.CrossFadeQueued("Idle");

        prevPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        prevPosition = transform.position;
        switch (currentState)
        {
            case State.Idle:
                timeToSearchForNewDestAux -= Time.deltaTime;
                if (timeToSearchForNewDestAux <= 0.0f)
                {
                    nextDestiny = transform.position +
                        new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)) * distanceToNextDest;
                    GetComponent<NavMeshAgent>().SetDestination(nextDestiny);

                    animation.CrossFade("Run");

                    timeToSearchForNewDestAux = timeToSearchForNewDest + Random.Range(-0.5f, 0.5f);

                    currentState = State.GoingTo;

                    positionRepeatCount = 0;

                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = nextDestiny;
                    cube.transform.localScale = new Vector3(0.5f, 2.0f, 0.5f);
                    cube.renderer.material.color = Color.red;
                    Destroy(cube, 2.0f);
                }
                else
                {
                    timeToNextEatAux -= Time.deltaTime;
                    if (timeToNextEatAux <= 0.0f)
                    {
                        animation.CrossFade("Eating");
                        animation.CrossFadeQueued("Idle");

                        timeToNextEatAux = timeToNextEat - Random.Range(0.0f, 5.0f);
                    }
                }
                break;

            case State.GoingTo:
                if (prevPosition == transform.position)
                {
                    positionRepeatCount++;
                    if (positionRepeatCount >= maxPosRepeatCount)
                    {
                        // se ha atascado
                        GetComponent<NavMeshAgent>().Stop();

                        animation.CrossFade("Idle");

                        currentState = State.Idle;

                        positionRepeatCount = 0;
                    }
                }
                else
                {
                    positionRepeatCount = 0;
                    prevPosition = transform.position;
                    //distance = Vector3.Distance(transform.position, nextDestiny);
                    // esto es más eficiente
                    float distX = transform.position.x - nextDestiny.x; distX = distX * distX;
                    float distZ = transform.position.z - nextDestiny.z; distZ = distZ * distZ;
                    distance = distX + distZ;
                    if (distance <= 1.4f)
                    {
                        GetComponent<NavMeshAgent>().Stop();

                        animation.CrossFade("Idle");

                        currentState = State.Idle;
                    }
                }
                break;
        }

	}
}
