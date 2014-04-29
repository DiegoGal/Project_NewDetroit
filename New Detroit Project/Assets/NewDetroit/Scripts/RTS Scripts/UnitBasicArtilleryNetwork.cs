using UnityEngine;
using System.Collections;

public class UnitBasicArtilleryNetwork : Photon.MonoBehaviour {

	CSelectable selectableScript;
	UnitBasicArtillery soldierScript;
    UnitBasicArtilleryRemote remoteScript;
	FogOfWarUnit fogOfWarScript;
	NavMeshAgent navMes;
	
	void Awake()
	{
		selectableScript = GetComponent<CSelectable>();
		soldierScript    = GetComponent<UnitBasicArtillery>();
        remoteScript     = GetComponent<UnitBasicArtilleryRemote>();
		fogOfWarScript	 = GetComponent<FogOfWarUnit>();
		navMes			 = GetComponent<NavMeshAgent>();
		
		if (photonView.isMine)
		{
			//MINE: local player, simply enable the local scripts
			selectableScript.enabled = true;
			soldierScript.enabled = true;
            remoteScript.enabled = false;
			fogOfWarScript.enabled = true;
			navMes.enabled = true;
		}
		else
		{           
			selectableScript.enabled = false;
			soldierScript.enabled = true;
            remoteScript.enabled = true;
			fogOfWarScript.enabled = false;
			navMes.enabled = false;
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
            UnitBasicArtillery script = this.GetComponent<UnitBasicArtillery>();
            stream.SendNext(script.currentArtilleryState);
            stream.SendNext(script.currentState);
            stream.SendNext(script.lastState);
            stream.SendNext(script.getLife());
		}
		else
		{
			//Network player, receive data
			correctPlayerPos =       (Vector3)stream.ReceiveNext();
			correctPlayerRot =       (Quaternion)stream.ReceiveNext();
            state =                  (UnitBasicArtillery.ArtilleryState)stream.ReceiveNext();
            unitState =              (UnitController.State)stream.ReceiveNext();
            lastState =              (UnitController.State)stream.ReceiveNext();
            currentLife =            (float)stream.ReceiveNext();
		}
	}
	
	private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this
    private UnitBasicArtillery.ArtilleryState state; // new State of the ArtilleryState
    private UnitController.State unitState; // new State of Unit
    private UnitController.State lastState; // last State of Unit
    private float currentLife; // for damage
	
	void Update()
	{
		if (!photonView.isMine)
		{
			//Update remote player (smooth this, this looks good, at the cost of some accuracy)
			transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
            UnitBasicArtilleryRemote script = GetComponent<UnitBasicArtilleryRemote>();
            script.currentArtilleryState = state;
            script.lastState = lastState;
            script.currentState = unitState;                        
		}
	}
	
}