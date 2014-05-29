using UnityEngine;
using System.Collections;

public class CStateUnit : CState
{

    public UnitController.State currentState;
    public UnitHarvester.HarvestState currentHarvestState;
    public UnitEngineer.EngineerState currentEngineerState;
    public UnitScout.ScoutState currentScoutState;
    public UnitArtillery.ArtilleryState currentArtilleryState;
    public UnitHeavyArtillery.DeployState currentDeployState;

    public bool attack2Selected;

    public enum UnitType
    {
        Harvester,
        Engineer,
        Scout,
        BasicArtillery,
        HeavyArtillery
    }
    public UnitType unitType;

}
