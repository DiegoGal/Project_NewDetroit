using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FogOfWarUnit : MonoBehaviour {
	//==============================
	//=====     Attributes     =====
	//==============================


	//=====================================
	//=====     Private Functions     =====
	//=====================================



	//==================================
	//=====     Main Functions     =====
	//==================================
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		//update the collision with the fog plane.
		RaycastHit[] hits;
		hits = Physics.RaycastAll (transform.position, Vector3.up);
		for (int i = 0; i < hits.Length; i ++)
		{
			RaycastHit hit = hits[i];
			MeshFilter filter = hit.collider.GetComponent<MeshFilter>();
			Vector3 relativePoint;
			if (filter)
			{
				relativePoint = filter.transform.InverseTransformPoint(hit.point);
				FogOfWarPlane.positions.Add (relativePoint);
			}
		}
	}
}
