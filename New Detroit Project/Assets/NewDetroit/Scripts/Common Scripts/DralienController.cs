using UnityEngine;
using System.Collections;

public class DralienController : MonoBehaviour
{

    private Vector3 nextDestiny;

    public float timeToSearchForNewDest = 2.0f;
    private float timeToSearchForNewDestAux;

    public float distanceToNextDest = 6.0f;

	// Use this for initialization
	void Start ()
    {
        nextDestiny = transform.position;
        timeToSearchForNewDestAux = timeToSearchForNewDest;

        animation.Stop();
	}
	
	// Update is called once per frame
	void Update ()
    {
        timeToSearchForNewDestAux -= Time.deltaTime;
        if (timeToSearchForNewDestAux <= 0.0f)
        {
            nextDestiny = transform.position +
                new Vector3(Random.Range(-1.0f, 1.0f), transform.position.y, Random.Range(-1.0f, 1.0f)) * distanceToNextDest;
            GetComponent<NavMeshAgent>().SetDestination(nextDestiny);

            animation.PlayQueued("Walk");

            timeToSearchForNewDestAux = timeToSearchForNewDest + Random.Range(-0.5f, 0.5f);
        }
	}
}
