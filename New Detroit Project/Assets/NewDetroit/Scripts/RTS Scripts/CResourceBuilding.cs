using UnityEngine;
using System.Collections;

public abstract class CResourceBuilding : BuildingController 
{
    // referencia al controlador del ejército
    public ArmyController armyController;

    protected float radius;

    public void IncreaseResources (int resources)
    {
        armyController.IncreaseResources(resources);
    }

    public void DecreaseResources (int resources)
    {
        armyController.DecreaseResources(resources);
    }

    public int GetResources ()
    {
        return armyController.resources;
    }

    public float GetRadius ()
    {
        return radius;
    }

}
