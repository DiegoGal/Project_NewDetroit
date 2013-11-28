using UnityEngine;
using System.Collections;

public class BaseController : MonoBehaviour
{

	//Donde van a aparecer las unidades
	private Vector3 spawnDestiny;
	//Donde van a aparecer las unidades
	private Vector3 spawnOrigin;

	private RaycastHit myHit; // Structure used to get information back from a raycast.
	private Ray myRay;

	public GameObject basicUnit;

    private GameObject cubeSpawnDest; // cubo que representa el spawnDestiny

	// Use this for initialization
	void Start ()
    {
		spawnOrigin = new Vector3 (
			this.transform.position.x + 5.5f,
			this.transform.position.y,
            this.transform.position.z - 5.5f
		);

		spawnDestiny = spawnOrigin;

        // colocamos una caja en el spawnDestiny
        cubeSpawnDest = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubeSpawnDest.renderer.material.color = Color.black;
        // eliminamos el colisionador del cubo:
        Destroy(cubeSpawnDest.GetComponent<BoxCollider>());
        cubeSpawnDest.transform.position = spawnDestiny;
	}
	
	// Update is called once per frame
	void Update ()
    {
		// hacemos click derecho
		if (this.GetComponent<CSelectable>().IsSelected() && Input.GetMouseButtonDown(1))
		{
			// lanzamos rayo y recogemos donde choca
			myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(myRay, out myHit, 1000f))
			{
                // si se ha seleccionado la propia base, se recoloca el spawn destiny
                if ((BaseController)myHit.transform.GetComponent("BaseController") == this)
                    spawnDestiny = spawnOrigin;
                else
				    spawnDestiny = myHit.point;
                cubeSpawnDest.transform.position = spawnDestiny;
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
