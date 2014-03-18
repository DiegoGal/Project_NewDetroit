using UnityEngine;
using System.Collections;

public class CResourceBuilding : BuildingController 
{
    // referencia al controlador del ejército
    public ArmyController armyController;

	// Use this for initialization
    public override void Start() 
    {
        base.Start();
    }
	
	// Update is called once per frame
    public override void Update() 
    {
        base.Update();
	}

    public void DownloadResources(int resources)
    {
        armyController.IncreaseResources(resources);
    }

}
