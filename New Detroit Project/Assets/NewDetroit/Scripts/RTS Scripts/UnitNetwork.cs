using UnityEngine;
using System.Collections;

public class UnitNetwork : BasicNetwork
{

	private CSelectable selectableScript;
    protected CStateUnit stateScript; // contains the current states of the units and other flags
	private UnitController unitScript;
    private UnitAnimationsNetwork unitAnimationsNetwork;
	private FogOfWarUnit fogOfWarScript;
	private NavMeshAgent navMeshAgent;

    // modelo del asset (el que contiene las animaciones)
    protected Transform model;
	
	public virtual void Awake ()
	{
		base.Awake();

		//if (PhotonNetwork.connected)
        if (GetComponent<ControllableCharacter>())
        	if (GetComponent<ControllableCharacter>().getTypeHero() == ControllableCharacter.TypeHeroe.Orc) GetComponent<CTeam>().teamNumber = 0;
        	else GetComponent<CTeam>().teamNumber = 1;
	
		selectableScript      = GetComponent<CSelectable>();
        unitScript            = GetComponent<UnitController>();
		fogOfWarScript	      = GetComponent<FogOfWarUnit>();
        navMeshAgent          = GetComponent<NavMeshAgent>();
        stateScript           = GetComponent<CStateUnit>();
		
		if (PhotonNetwork.connected)
			if (photonView.isMine)
			{
				//MINE: local player, simply enable the local scripts
				selectableScript.enabled      = true;
	            unitScript.enabled            = true;
				fogOfWarScript.enabled        = true;
				navMeshAgent.enabled          = true;
	
	            //Debug.Log("MINE");
			}
			else
			{           
				selectableScript.enabled      = false;
	            unitScript.enabled            = false;
				fogOfWarScript.enabled        = false;
				navMeshAgent.enabled          = false;
	
	            //Debug.Log("not MINE");
			}
		else
			enabled = false;

        // se captura la referencia al modelo
        model = transform.FindChild("Model");
	}
	
	public virtual void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		base.OnPhotonSerializeView(stream, info);
		
		if (stream.isWriting)
		{
            stream.SendNext(stateScript.animationSend);
            stream.SendNext(stateScript.animationName);
            stream.SendNext(stateScript.animationSendQeued);
            stream.SendNext(stateScript.animationNameQueued);

			if (stateScript.animationSend) stateScript.animationSend = false;
			if (stateScript.animationSendQeued) stateScript.animationSendQeued = false;
		}
		else
		{            
            stateScript.animationChanged = (bool)stream.ReceiveNext();
            stateScript.animationName = (string)stream.ReceiveNext();
            stateScript.animationChangeQueued = (bool)stream.ReceiveNext();
            stateScript.animationNameQueued = (string)stream.ReceiveNext();
		}
	}
}