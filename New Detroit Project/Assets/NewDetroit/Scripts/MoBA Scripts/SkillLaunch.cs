using UnityEngine;
using System.Collections;

public class SkillLaunch : MonoBehaviour 
{

	public float speed = 1;
	public Vector3 direction = Vector3.forward;
	public float timeToLaunch = 1;
	//---------------------------
	private float timeElapsed = 0;
	
	
	//--------------------------------------------
	
	
	public void Update () 
	{
		if (timeElapsed >= timeToLaunch)
		{
			if (transform.parent != null) 
				transform.parent = null;
				
			transform.position += direction * speed;
		}
		
		timeElapsed += Time.deltaTime;
	}
	
}
