using UnityEngine;
using System.Collections;

public class CStateUnit : CState
{

    public UnitController.State currentState;
    public UnitController.State lastState;
    public UnitHarvester.HarvestState currentHarvestState;
    public UnitEngineer.EngineerState currentEngineerState;
    public UnitScout.ScoutState currentScoutState;
    public UnitArtillery.ArtilleryState currentArtilleryState;
    public UnitHeavyArtillery.DeployState currentDeployState;

    public Vector3 destiny = new Vector3();

    public bool attack2Selected;

    public bool animationChanged = false;
    public bool animationChangeQueued = false;

    public string animationName, animationNameQueued;

    public bool animationSend = false, animationSendQeued = false;

    public enum UnitType
    {
        Harvester,
        Engineer,
        Scout,
        BasicArtillery,
        HeavyArtillery
    }
    public UnitType unitType;

    public void Update ()
    {
        if (animationChanged)
        {
            animation.CrossFade(animationName);
            animationChanged = false;
            animationSend = true;
        }
        if (animationChangeQueued)
        {
            animation.CrossFadeQueued(animationNameQueued);
            animationChangeQueued = false;
            animationSendQeued = true;
        }
    }

}
