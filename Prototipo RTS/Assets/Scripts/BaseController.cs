﻿using UnityEngine;
using System.Collections;

public class BaseController : CResourceBuilding
{
	
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
    public override void Start()
    {
        base.Start();

        float twoPi = Mathf.PI * 2;
        Vector3 center = transform.position;
        for (int i = 0; i < numEngineerPositions; i++)
        {
            Vector3 pos = new Vector3
                (
                    center.x +
                    (transform.GetComponent<BoxCollider>().size.x + despPosition) * Mathf.Sin(i * (twoPi / numEngineerPositions)),
                    0,
                    center.z +
                    (transform.GetComponent<BoxCollider>().size.x + despPosition) * Mathf.Cos(i * (twoPi / numEngineerPositions))
                    );
            engineerPositions[i] = pos;
            engineerPosTaken[i] = false;

            cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubes[i].transform.position = pos;
            Destroy(cubes[i].GetComponent<BoxCollider>());
            cubes[i].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);
            cubes[i].transform.parent = this.transform;
        }

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
        currentLife = totalLife;
	}
	
	// Update is called once per frame
    public override void Update()
    {
        base.Update();

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

		newUnit.GetComponent<UnitController>().SetArmyBase(this);
		newUnit.GetComponent<UnitController>().SetBasePosition(transform.position);
		newUnit.GetComponent<UnitController>().teamNumber = this.teamNumber;
		newUnit.GetComponent<UnitController>().GoTo(spawnDestiny);

		return  newUnit;
	}

    public ArmyController GetArmyController()
    {
        return armyController;
    }

    public virtual void OnGUI()
    {
        Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
        Rect rect1;
        Rect rect2;

        rect1 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 50.0f, 120.0f, 4.0f);
        rect2 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 50.0f, 120.0f * (currentLife / totalLife), 4.0f);
        
        GUI.DrawTexture(rect1, progressBarEmpty);
        GUI.DrawTexture(rect2, progressBarFull);

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
