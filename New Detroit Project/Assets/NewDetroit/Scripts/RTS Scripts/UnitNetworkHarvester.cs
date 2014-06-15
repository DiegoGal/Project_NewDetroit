using UnityEngine;
using System.Collections;

public class UnitNetworkHarvester : UnitNetwork
{

    // last HarvestState of Unit
    private UnitHarvester.HarvestState lastHarvestState = UnitHarvester.HarvestState.None;
	
	public override void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
        base.OnPhotonSerializeView(stream, info);
		if (stream.isWriting)
		{
            stream.SendNext(stateScript.currentHarvestState);
		}
		else
		{
            stateScript.currentHarvestState = (UnitHarvester.HarvestState)stream.ReceiveNext();
		}
	}
	
	public override void Update ()
	{
        base.Update();

		if (!photonView.isMine)
		{
            if (lastHarvestState != stateScript.currentHarvestState)
            {
                // cambiar animaciones
                switch (stateScript.currentHarvestState)
                {
                    case UnitHarvester.HarvestState.None: animation.CrossFade("Idle01"); break;
                    case UnitHarvester.HarvestState.Healing: animation.CrossFade("Heal"); break;
                    case UnitHarvester.HarvestState.Choping: animation.CrossFade("Picar"); break;

                }
            }
//            lastState = stateScript.currentState;

            //UnitBasicArtilleryRemote script = GetComponent<UnitBasicArtilleryRemote>();
            //script.currentArtilleryState = state;
            //script.lastState = lastState;
            //script.currentState = unitState;
            //script.attack2Selected = attack2Selected;
		}
	}
	
}