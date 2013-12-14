using UnityEngine;
using System.Collections;

public class MovingProp : MonoBehaviour {
	
	public Transform pointA;
	public Transform pointB;
	
	public float clearance = 0.01f;
	
	Vector3 target;
	
	enum Travel { AtoB, BtoA };
	Travel travel = new Travel();
	
	public float speed = 1;
	
	// Use this for initialization
	void Start () 
	{
		travel = Travel.AtoB;
		target = pointB.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(travel == Travel.AtoB)
		{
			if(Mathf.Abs(Vector3.Distance(transform.position, pointB.position)) < clearance)
			{
				travel = Travel.BtoA;
				target = pointA.position;
			}
		}
		else if(travel == Travel.BtoA)
		{
			if(Mathf.Abs(Vector3.Distance(transform.position, pointA.position)) < clearance)
			{
				travel = Travel.AtoB;
				target = pointB.position;
			}
		}
		
		transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
	
	}
}
