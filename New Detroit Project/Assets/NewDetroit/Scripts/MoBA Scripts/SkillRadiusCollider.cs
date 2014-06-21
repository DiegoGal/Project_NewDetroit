using UnityEngine;
using System.Collections;

public class SkillRadiusCollider : MonoBehaviour 
{
	public float radius = 0.5f;
	public float radiusIncrement = 0.5f;
	public float radiusMax = 0.5f;
	public float timeWait = 0;
	//---------------------
	private SphereCollider sphereCollider;
	private float timeTotal = 0;


	//------------------------------------------


	public void Start()
	{
		sphereCollider = GetComponent<SphereCollider>();
		sphereCollider.radius = radius;
	}

	public void Update () 
	{
		if (timeTotal >= timeWait)
		{
			radius += radiusIncrement * Time.deltaTime;
			sphereCollider.radius = Mathf.Min(radiusMax, radius);
		}

		timeTotal += Time.deltaTime;
	}

}//End SkillRadiusCollider
