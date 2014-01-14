using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FogOfWarUnit : MonoBehaviour {
	//==============================
	//=====     Attributes     =====
	//==============================
	private float radius;	//radius vision.
	private float time;		//time to hit the fog plane
	private List<Vector3> positions = new List<Vector3> ();		//point to show the position in fog plane


	//=====================================
	//=====     Private Functions     =====
	//=====================================



	//==================================
	//=====     Main Functions     =====
	//==================================
	// Use this for initialization
	void Start () {
		radius = 2.0f;
		time = 0;
	}
	
	// Update is called once per frame
	void Update () {
		// Accumulated time since last update of collision points.
		time += Time.deltaTime;
		// Check the collision every second.
		if (time >= 1) 
		{
			// Reset time and points for updating them.
			time = 0;
			positions.Clear ();
			// Update the collision with the fog plane.
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
					// Save the collision points
					positions.Add(relativePoint);
				}
			}
		}
		else
		{
			for (int i = 0; i < positions.Count; i ++)
			{
				FogOfWarPlane.positions.Add (positions[i]);
			}
		}
	}
}
