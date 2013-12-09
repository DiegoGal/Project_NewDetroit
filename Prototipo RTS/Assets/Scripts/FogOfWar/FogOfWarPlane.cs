using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FogOfWarPlane : MonoBehaviour {
	public static List<Vector3> positions = new List<Vector3> ();	//positions of allied units.



	//==============================
	//=====     Attributes     =====
	//==============================
	private bool[] visited;	//to show a transparent fog.
	private bool[] isVisiting;	//positions currently visited.



	//=====================================
	//=====     Private Functions     =====
	//=====================================
	private void FullMesh(float inRadius)
	{
		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		Vector3[] vertices =  mesh.vertices;
		float sqrRadius = inRadius * inRadius;
		Color[] colours = mesh.colors;
		//draw the fog for each unit.
		for (int num = 0; num < positions.Count; num ++) 
		{
			Vector3 position = positions[num];
			for (int i = 0; i < vertices.Length; i ++) 
			{
				float sqrMagnitude = (vertices [i] - position).sqrMagnitude;
				if (sqrMagnitude <= sqrRadius)
				{
					colours [i].a = 0;
					visited [i] = true;
					isVisiting [i] = true;
				}
				else
				{
					if (!isVisiting [i] && visited [i])
						colours [i].a = 0.3f;
					else if (!isVisiting [i])
						colours [i].a = 1;
				}
			}
		}
		mesh.colors = colours;
		//set isVisiting false to the next loop.
		for (int i = 0; i < vertices.Length; i++)
		{
			isVisiting [i] = false;
		}
		//clear the list of positions to the next loop.
		positions.Clear ();

	}



	//==================================
	//=====     Main Functions     =====
	//==================================
	// Use this for initialization
	void Start () {
		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		Vector3[] vertices = mesh.vertices;
		Color[] colors = new Color[vertices.Length];

		visited = new bool[vertices.Length];
		isVisiting = new bool[vertices.Length];

		//apply black color to mesh.
		for (int i = 0; i < vertices.Length; i++) 
		{
			colors[i] = Color.black;
			colors[i].a = 1;
			visited[i] = false;
			isVisiting [i] = false;
		}
		mesh.colors = colors;
	}
	
	// Update is called once per frame
	void Update () {
		FullMesh (3f);
	}
}
