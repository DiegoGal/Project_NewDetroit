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

	// Use this for initialization
	void Start ()
    {
        nextDestiny = transform.position;
        timeToSearchForNewDestAux = timeToSearchForNewDest - Random.Range(0.0f, 2.0f);

        timeToNextEatAux = timeToNextEat - Random.Range(0.0f, 5.0f);

        animation.Play("Digging");
        animation.CrossFadeQueued("Idle");
	}
	
	// Update is called once per frame
	void Update ()
    {
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
                distance = Vector3.Distance(transform.position, nextDestiny);
                if (distance <= 1.0f)
                {
                    GetComponent<NavMeshAgent>().Stop();

                    animation.CrossFade("Idle");

                    currentState = State.Idle;
                }
                break;
        }
        
	}
}
