using UnityEngine;
using System.Collections;

public class BaseController : Photon.MonoBehaviour
{
	public int teamNumber;

    // referencia al controlador del ejército
    public ArmyController armyController;

	//Donde van a aparecer las unidades
	private Vector3 spawnDestiny;
	//Donde van a aparecer las unidades
	private Vector3 spawnOrigin;

	private RaycastHit myHit; // Structure used to get information back from a raycast.
	private Ray myRay;

	public GameObject basicUnit;
    public GameObject harvesterUnit;
	public GameObject basicArtilleryUnit;
	public GameObject heavyArtilleryUnit;

    private GameObject cubeSpawnDest; // cubo que representa el spawnDestiny

	// Use this for initialization
	void Start ()
    {
        spawnOrigin = transform.FindChild("SpawnPoint").position;

        spawnDestiny = new Vector3(
            this.transform.position.x + 5.5f,
            this.transform.position.y,
            this.transform.position.z - 5.5f
        );

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

    void OnCollisionEnter (Collision collision)
    {
        /*UnitController unit = collision.transform.GetComponent<UnitController>();
        if (unit != null)
        {
            unit.ArrivedToBase();
        }*/
    }

	public GameObject SpawnUnit (int id)
	{
		GameObject newUnit = null;

		switch (id)
		{
			case 0: // Harvester
			newUnit = PhotonNetwork.Instantiate(harvesterUnit.name, spawnOrigin, new Quaternion(),0)
					as GameObject;
			break;
			case 1: // Basic Artillery
			newUnit = PhotonNetwork.Instantiate(basicArtilleryUnit.name, spawnOrigin, new Quaternion(),0)
					as GameObject;
			break;
			case 2: // Heavy Artillery
			newUnit = PhotonNetwork.Instantiate(heavyArtilleryUnit.name, spawnOrigin, new Quaternion(),0)
					as GameObject;
			break;
		}

		newUnit.GetComponent<UnitController>().SetArmyBase(this);
		newUnit.GetComponent<UnitController>().SetBasePosition(transform.position);
		newUnit.GetComponent<UnitController>().teamNumber = this.teamNumber;
		newUnit.GetComponent<UnitController>().GoTo(spawnDestiny);

		return  newUnit;
	}

    public void DownloadResources(int resources)
    {
        armyController.IncreaseResources(resources);
    }

} // class BaseController
