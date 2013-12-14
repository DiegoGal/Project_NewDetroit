using UnityEngine;
using System.Collections;

public class PersonController : MonoBehaviour
{

    private RaycastHit myHit; // Structure used to get information back from a raycast.
    private Ray myRay;

	// Use this for initialization
	void Start ()
    {
        myHit = new RaycastHit();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //selecting = true;
            // lanzamos rayo y recogemos donde choca
            myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(myRay, out myHit, 1000f))
            {
                Vector3 destiny = myHit.point;

                GetComponent<NavMeshAgent>().destination = destiny;
            }
        }
    } // Update ()

}
