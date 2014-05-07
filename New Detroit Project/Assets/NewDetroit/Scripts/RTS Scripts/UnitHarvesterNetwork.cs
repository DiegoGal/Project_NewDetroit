using UnityEngine;
using System.Collections;

public class UnitHarvesterNetwork : Photon.MonoBehaviour {

	CSelectable selectableScript;
	UnitHarvester harvesterScript;
    UnitHarvesterRemote remoteScript;
	FogOfWarUnit fogOfWarScript;
	NavMeshAgent navMes;
	
	void Awake()
	{
		selectableScript = GetComponent<CSelectable>();
		harvesterScript = GetComponent<UnitHarvester>();
		fogOfWarScript	= GetComponent<FogOfWarUnit>();
		navMes			= GetComponent<NavMeshAgent>();
        remoteScript = GetComponent<UnitHarvesterRemote>();

		if (photonView.isMine)
		{
			//MINE: local player, simply enable the local scripts
			selectableScript.enabled = true;
			harvesterScript.enabled = true;
			fogOfWarScript.enabled = true;
			navMes.enabled = true;
            remoteScript.enabled = false;
		}
		else
		{           
			selectableScript.enabled = false;
			harvesterScript.enabled = false;
			fogOfWarScript.enabled = false;
			navMes.enabled = false;
            remoteScript.enabled = true;
		}
		
		gameObject.name = gameObject.name + photonView.viewID;
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			//We own this player: send the others our data
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
            UnitHarvester script = this.GetComponent<UnitHarvester>();
            stream.SendNext(script.currentHarvestState);
            stream.SendNext(script.currentState);
            stream.SendNext(script.getLife());
            stream.SendNext(script.loaded);
            // if we sent damage we reset damageStream for not sending it more times.

		}
		else
		{
			//Network player, receive data
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctPlayerRot = (Quaternion)stream.ReceiveNext();
            state = (UnitHarvester.HarvestState)stream.ReceiveNext();
            unitState = (UnitController.State)stream.ReceiveNext();
            currentLife = (float)stream.ReceiveNext();
            loaded = (bool)stream.ReceiveNext();
            
		}
	}
	
	private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this
    private UnitHarvester.HarvestState state; // new State of the HarvesterUnit
    private UnitHarvester.State unitState; // new State of Unit
    private float currentLife; // for damage
    private bool loaded; //to see if the unit is carring something
	
	void Update()
	{
		if (!photonView.isMine)
		{
			//Update remote player (smooth this, this looks good, at the cost of some accuracy)
			transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
            UnitHarvesterRemote script = GetComponent<UnitHarvesterRemote>();
            script.currentHarvestState = state;
            script.currentState = unitState;
            script.loaded = loaded;
		}
	}
	
}