﻿using UnityEngine;
using System.Collections;

public class BaseController : Photon.MonoBehaviour
{
    public bool isOnline = false;

	public int teamNumber;

    // referencia al controlador del ejército
    public ArmyController armyController;

	//Donde van a aparecer las unidades
	private Vector3 spawnDestiny;
	//Donde van a aparecer las unidades
	private Vector3 spawnOrigin;

	private RaycastHit myHit; // Structure used to get information back from a raycast.
	private Ray myRay;

    public GameObject harvesterUnit;
	public GameObject basicArtilleryUnit;
	public GameObject heavyArtilleryUnit;
	public GameObject engineerUnit;
    public GameObject scoutUnit;

    private GameObject cubeSpawnDest; // cubo que representa el spawnDestiny

    public float radius = 14.0f;//6.0f;

	// Use this for initialization
	void Start ()
    {
        spawnOrigin = transform.FindChild("SpawnPoint").position;
        /*spawnDestiny = new Vector3(
            this.transform.position.x + 5.5f,
            //this.transform.position.y,
            0.0f,
            this.transform.position.z - 5.5f
        );*/
        spawnDestiny = new Vector3(spawnOrigin.x - 2.0f, 0.0f, spawnOrigin.z);

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

        if (isOnline)
        {
            switch (id)
            {
                case 0: // Harvester
                    newUnit = PhotonNetwork.Instantiate("UnitHarvester", spawnOrigin, new Quaternion(), 0)
                            as GameObject;
                    break;
                case 1: // Basic Artillery
                    newUnit = PhotonNetwork.Instantiate("UnitBasicArtillery", spawnOrigin, new Quaternion(), 0)
                            as GameObject;
                    break;
                case 2: // Heavy Artillery
                    newUnit = PhotonNetwork.Instantiate("UnitHeavyArtillery", spawnOrigin, new Quaternion(), 0)
                            as GameObject;
                    break;
                case 3: // Engineer
                    newUnit = PhotonNetwork.Instantiate("UnitEngineer", spawnOrigin, new Quaternion(), 0)
                        as GameObject;
                    break;
                case 4: // Scout
                    newUnit = PhotonNetwork.Instantiate("UnitScout", spawnOrigin, new Quaternion(), 0)
                        as GameObject;
                    break;
            }
        }
        else
        {
            switch (id)
            {
                case 0: // Harvester
                    newUnit = Instantiate(harvesterUnit, spawnOrigin, new Quaternion())
                        as GameObject;
                    break;
                case 1: // Basic Artillery
                    newUnit = Instantiate(basicArtilleryUnit, spawnOrigin, new Quaternion())
                        as GameObject;
                    break;
                case 2: // Heavy Artillery
                    newUnit = Instantiate(heavyArtilleryUnit, spawnOrigin, new Quaternion())
                        as GameObject;
                    break;
                case 3: // Engineer
                    newUnit = Instantiate(engineerUnit, spawnOrigin, new Quaternion())
                        as GameObject;
                    break;
                case 4: // Scout
                    newUnit = Instantiate(scoutUnit, spawnOrigin, new Quaternion())
                        as GameObject;
                    break;
            }
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

    public Vector3 GetSpawnOrigin ()
    {
        return spawnOrigin;
    }

    public float GetRadious ()
    {
        return radius;
    }

} // class BaseController
