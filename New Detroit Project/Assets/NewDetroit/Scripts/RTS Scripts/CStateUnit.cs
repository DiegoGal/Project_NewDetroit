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

    public bool animationChanged = false;
    public bool animationChangeQueued = false;
	public bool animationChangeQueued2 = false;

    public string animationName, animationNameQueued, animationNameQueued2;

    public bool animationSend = false, animationSendQeued = false, animationSendQueued2 = false;

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
		//It's used to the third basic attack of heroes
		if (animationChangeQueued2)
		{
			animation.CrossFadeQueued(animationNameQueued2);
			animationChangeQueued2 = false;
			animationSendQueued2 = true;
		}
    }

}
