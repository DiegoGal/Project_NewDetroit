using UnityEngine;
using System.Collections;

public class BaseController : CResourceBuilding
{
    public bool isOnline = false;

	//Donde van a aparecer las unidades
	private Vector3 spawnDestiny;
	//Donde van a aparecer las unidades
	private Vector3 spawnOrigin;

	private RaycastHit myHit; // Structure used to get information back from a raycast.
	private Ray myRay;

    public GameObject harvesterUnit;
    public int harvesterUnitResourcesCost = 100,
               harvesterUnitEconomyCost = 0;
	public GameObject basicArtilleryUnit;
    public int basicArtilleryUnitResourcesCost = 200,
               basicArtilleryUnitEconomyCost = 0;
	public GameObject heavyArtilleryUnit;
    public int heavyArtilleryUnitResourcesCost = 300,
               heavyArtilleryUnitEconomyCost = 10;
	public GameObject engineerUnit;
    public int engineerUnitResourcesCost = 200,
               engineerUnitEconomyCost = 5;
    public GameObject scoutUnit;
    public int scoutUnitResourcesCost = 150,
               scoutUnitEconomyCost = 10;

    private GameObject cubeSpawnDest; // cubo que representa el spawnDestiny

    public float baseRadius = 12.0f;

    public bool createCubes = false;

	// Use this for initialization
    public override void Start ()
    {
        base.Start();
        //Minimap.SetBase(GetComponent<CResourceBuilding>());
        radius = baseRadius;

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
        float twoPi = Mathf.PI * 2.0f;
        Vector3 center = transform.position;

        float colliderSize;
        if (transform.GetComponent<BoxCollider>())
            colliderSize = transform.GetComponent<BoxCollider>().size.x;
        else if (transform.GetComponent<CapsuleCollider>())
            colliderSize = transform.GetComponent<CapsuleCollider>().radius;
        else if (transform.GetComponent<SphereCollider>())
            colliderSize = transform.GetComponent<SphereCollider>().radius;
        else
            colliderSize = 10.0f;

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
            cubes[i].SetActive(createCubes);
        }

        life.currentLife = life.maximunLife;

	}

    [RPC]
    public void EndGame()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(NetworkController.SceneNameMenu);
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
        if (life.currentLife <= 0)
        {
            photonView.RPC("EndGame",PhotonTargets.All);
        }
		
	}

    public virtual void OnGUI()
    {
        Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
        Rect rect1 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 50.0f, 120.0f, 4.0f);
        Rect rect2 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 50.0f, 120.0f * (life.currentLife / life.maximunLife), 4.0f); 

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

	public GameObject SpawnUnit (int id, ref int resources, ref int economy)
	{
		GameObject newUnit = null;

        if (isOnline)
        {        
            string unit;
            switch (id)
            {
                case 0: // Harvester
                    if (resources >= harvesterUnitResourcesCost && economy >= harvesterUnitEconomyCost)
                    {
                        if ((team.teamNumber % 2) == 0)
                            unit = "Goblin_Harvester_TeamA";
                        else
                            unit = "Robot_Harvester_TeamA";
                        // Instantiation with Photon
                        newUnit = PhotonNetwork.Instantiate(unit, spawnOrigin, new Quaternion(), 0); 
                        // Remove part of resources
                        resources -= harvesterUnitResourcesCost;
                        economy -= harvesterUnitEconomyCost;
                    }
                    break;
                case 1: // Basic Artillery
                    if (resources >= basicArtilleryUnitResourcesCost && economy >= basicArtilleryUnitEconomyCost)
                    {
                        if ((team.teamNumber % 2) == 0)
                            unit = "Goblin_ArtilleryBasic_TeamA";
                        else
                            unit = "Robot_ArtilleryBasic_TeamA";
                        // Instantiation with Photon
                        newUnit = PhotonNetwork.Instantiate(unit, spawnOrigin, new Quaternion(), 0); 
                        // Remove part of resources
                        resources -= basicArtilleryUnitResourcesCost;
                        economy -= basicArtilleryUnitEconomyCost;
                    }
                    break;
                case 2: // Heavy Artillery
                    if (resources >= heavyArtilleryUnitResourcesCost && economy >= heavyArtilleryUnitEconomyCost)
                    {
                        if ((team.teamNumber % 2) == 0)
                            unit = "Goblin_ArtilleryHeavy_TeamA";
                        else
                            unit = "Robot_ArtilleryHeavy_TeamA";
                        // Instantiation with Photon
                        newUnit = PhotonNetwork.Instantiate(unit, spawnOrigin, new Quaternion(), 0); 
                        // Remove part of resources
                        resources -= heavyArtilleryUnitResourcesCost;
                        economy -= heavyArtilleryUnitEconomyCost;
                    }
                    break;
                case 3: // Engineer
                    if (resources >= engineerUnitResourcesCost && economy >= engineerUnitEconomyCost)
                    {
                        if ((team.teamNumber % 2) == 0)
                            unit = "Goblin_Engineer_TeamA";
                        else
                            unit = "Goblin_Engineer_TeamB";
                        // Instantiation with Photon
                        newUnit = PhotonNetwork.Instantiate(unit, spawnOrigin, new Quaternion(), 0); 
                        // Remove part of resources
                        resources -= engineerUnitResourcesCost;
                        economy -= engineerUnitEconomyCost;
                    }
                    break;
                case 4: // Scout
                    if (resources >= scoutUnitResourcesCost && economy >= scoutUnitEconomyCost)
                    {
                        if ((team.teamNumber % 2) == 0)
                            unit = "Goblin_Scout_TeamA";
                        else
                            unit = "Robot_Scout_TeamA";
                        // Instantiation with Photon
                        newUnit = PhotonNetwork.Instantiate(unit, spawnOrigin, new Quaternion(), 0); 
                        // Remove part of resources
                        resources -= scoutUnitResourcesCost;
                        economy -= scoutUnitEconomyCost;
                    }
                    break;
            }
        }
        else
        {
            switch (id)
            {
                case 0: // Harvester
                    if (resources >= harvesterUnitResourcesCost && economy >= harvesterUnitEconomyCost)
                    {
                        newUnit = Instantiate(harvesterUnit, spawnOrigin, new Quaternion())
                            as GameObject;
                        resources -= harvesterUnitResourcesCost;
                        economy -= harvesterUnitEconomyCost;
                    }
                    break;
                case 1: // Basic Artillery
                    if (resources >= basicArtilleryUnitResourcesCost && economy >= basicArtilleryUnitEconomyCost)
                    {
                        newUnit = Instantiate(basicArtilleryUnit, spawnOrigin, new Quaternion())
                            as GameObject;
                        resources -= basicArtilleryUnitResourcesCost;
                        economy -= basicArtilleryUnitEconomyCost;
                    }
                    break;
                case 2: // Heavy Artillery
                    if (resources >= heavyArtilleryUnitResourcesCost && economy >= heavyArtilleryUnitEconomyCost)
                    {
                        newUnit = Instantiate(heavyArtilleryUnit, spawnOrigin, new Quaternion())
                            as GameObject;
                        resources -= heavyArtilleryUnitResourcesCost;
                        economy -= heavyArtilleryUnitEconomyCost;
                    }
                    break;
                case 3: // Engineer
                    if (resources >= engineerUnitResourcesCost && economy >= engineerUnitEconomyCost)
                    {
                        newUnit = Instantiate(engineerUnit, spawnOrigin, new Quaternion())
                            as GameObject;
                        resources -= engineerUnitResourcesCost;
                        economy -= engineerUnitEconomyCost;
                    }
                    break;
                case 4: // Scout
                    if (resources >= scoutUnitResourcesCost && economy >= scoutUnitEconomyCost)
                    {
                        newUnit = Instantiate(scoutUnit, spawnOrigin, new Quaternion())
                            as GameObject;
                        resources -= scoutUnitResourcesCost;
                        economy -= scoutUnitEconomyCost;
                    }
                    break;
            }
        }

        if (newUnit)
        {        		
            newUnit.GetComponent<ControllableCharacter>().isMine = true;
            newUnit.GetComponent<UnitController>().SetArmyBase(this);
            newUnit.GetComponent<UnitController>().SetBasePosition(transform.position);
            //newUnit.GetComponent<CTeam>().teamNumber = this.team.teamNumber;
            newUnit.GetComponent<CTeam>().teamColorIndex = GetComponent<CTeam>().teamColorIndex;
            newUnit.GetComponent<CSelectable>().ResetTeamColor();
            newUnit.GetComponent<UnitController>().GoTo(spawnDestiny);
        }

		return  newUnit;
	}

    public Vector3 GetSpawnOrigin ()
    {
        return spawnOrigin;
    }

    public ArmyController GetArmyController ()
    {
        return armyController;
    }

    // RPC function for manualy PhotonNetwork instantiation
//    [RPC]
//    void SpawnOnNetwork(Vector3 pos, Quaternion rot, int id1, PhotonPlayer np, GameObject prefab)
//    {
//        GameObject newUnit = Instantiate(prefab, pos, rot) as GameObject;
//
//        // We do this here because here has to go the instantiation of the unit
//        newUnit.GetComponent<UnitController>().SetArmyBase(this);
//        newUnit.GetComponent<UnitController>().SetBasePosition(transform.position);
//        //newUnit.GetComponent<UnitController>().teamNumber = this.teamNumber;
//		newUnit.GetComponent<CTeam>().teamNumber = GetComponent<CTeam>().teamNumber;
//        newUnit.GetComponent<UnitController>().GoTo(spawnDestiny);
//
//        // Set player's PhotonView
//        PhotonView[] nViews = newUnit.transform.GetComponentsInChildren<PhotonView>();
//        nViews[0].viewID = id1;
//    }

} // class BaseController
