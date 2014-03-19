using UnityEngine;
using System.Collections;

public class BaseController : CResourceBuilding
{
    public bool isOnline = false;

    // indicates the color of the units, 0=green, 1=purple
    public int teamColor = 0;

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
    public override void Start ()
    {
        base.Start();

        spawnOrigin = transform.FindChild("SpawnPoint").position;
        /*spawnDestiny = new Vector3(
            this.transform.position.x + 5.5f,
            //this.transform.position.y,
            0.0f,
            this.transform.position.z - 5.5f
        );*/
        spawnDestiny = new Vector3(spawnOrigin.x, 0.0f, spawnOrigin.z - 5.0f);

        // colocamos una caja en el spawnDestiny
        cubeSpawnDest = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubeSpawnDest.renderer.material.color = Color.black;
        // eliminamos el colisionador del cubo:
        Destroy(cubeSpawnDest.GetComponent<BoxCollider>());
        cubeSpawnDest.transform.position = spawnDestiny;

        // inicialización de las posiciones de los ingenieros (para reparación)        
        float twoPi = Mathf.PI * 2;
        Vector3 center = transform.position;

        float colliderSize;
        if (transform.GetComponent<BoxCollider>())
            colliderSize = transform.GetComponent<BoxCollider>().size.x;
        else if (transform.GetComponent<CapsuleCollider>())
            colliderSize = transform.GetComponent<CapsuleCollider>().radius;
        else
            colliderSize = transform.GetComponent<SphereCollider>().radius;

        for (int i = 0; i < numEngineerPositions; i++)
        {
            Vector3 pos = new Vector3
            (
                center.x +
                    (colliderSize + despPosition) * Mathf.Sin(i * (twoPi / numEngineerPositions)),
                0,
                center.z +
                    (colliderSize + despPosition) * Mathf.Cos(i * (twoPi / numEngineerPositions))
            );
            engineerPositions[i] = pos;
            engineerPosTaken[i] = false;

            cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubes[i].transform.position = pos;
            Destroy(cubes[i].GetComponent<BoxCollider>());
            cubes[i].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);
            cubes[i].transform.parent = this.transform;
        }

        currentLife = totalLife;

	}
	
	// Update is called once per frame
    public override void Update ()
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

    public virtual void OnGUI()
    {
        Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
        Rect rect1 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 50.0f, 120.0f, 4.0f);
        Rect rect2 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 50.0f, 120.0f * (currentLife / totalLife), 4.0f); 

        GUI.DrawTexture(rect1, progressBarEmpty);
        GUI.DrawTexture(rect2, progressBarFull);
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
            // set the texture of the unit
            if (newUnit && teamColor != 0)
            {
                newUnit.GetComponent<CSelectable>().SetOutlineColor(new Color(0.0f, 0.7843f, 1.0f));
            }
        }

		newUnit.GetComponent<UnitController>().SetArmyBase(this);
		newUnit.GetComponent<UnitController>().SetBasePosition(transform.position);
		newUnit.GetComponent<UnitController>().teamNumber = this.teamNumber;
		newUnit.GetComponent<UnitController>().GoTo(spawnDestiny);

		return  newUnit;
	}

    public Vector3 GetSpawnOrigin ()
    {
        return spawnOrigin;
    }

    public float GetRadious ()
    {
        return radius;
    }

    public ArmyController GetArmyController ()
    {
        return armyController;
    }

} // class BaseController
