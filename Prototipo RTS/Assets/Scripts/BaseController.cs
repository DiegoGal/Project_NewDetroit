using UnityEngine;
using System.Collections;

public class BaseController : MonoBehaviour {

	//Donde van a aparecer las unidades
	private Vector3 spawnDestiny;
	//Donde van a aparecer las unidades
	private Vector3 spawnOrigin;

	private RaycastHit myHit; // Structure used to get information back from a raycast.
	private Ray myRay;

	public GameObject basicUnit;

	// Use this for initialization
	void Start () {
		spawnOrigin = new Vector3 (
			this.transform.position.x + 3,
			this.transform.position.y,
			this.transform.position.z + 3
		);

		spawnDestiny = spawnOrigin;
	}
	
	// Update is called once per frame
	void Update () {
		// hacemos click derecho
		if (this.GetComponent<CSelectable>().IsSelected() && Input.GetMouseButtonDown(1))
		{
			// lanzamos rayo y recogemos donde choca
			myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(myRay, out myHit, 1000f))
			{
				spawnDestiny = myHit.point;
			}
		}
		
	}

	public GameObject SpawnUnit ()
	{
		GameObject newUnit = Instantiate(basicUnit, spawnOrigin, new Quaternion()) as GameObject;
		newUnit.GetComponent<UnitController>().GoTo (spawnDestiny);
		return  newUnit;
	}
}
