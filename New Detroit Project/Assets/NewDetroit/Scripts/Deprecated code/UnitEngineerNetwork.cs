using UnityEngine;
using System.Collections;

public class UnitEngineerNetwork : Photon.MonoBehaviour {
	
	CSelectable selectableScript;
	UnitEngineer engineerScript;
    UnitEngineerRemote remoteScript;
	FogOfWarUnit fogOfWarScript;
	NavMeshAgent navMes;
	
	void Awake()
	{
		selectableScript = GetComponent<CSelectable>();
		engineerScript   = GetComponent<UnitEngineer>();
        remoteScript     = GetComponent<UnitEngineerRemote>();
		fogOfWarScript	 = GetComponent<FogOfWarUnit>();
		navMes			 = GetComponent<NavMeshAgent>();
		
		if (photonView.isMine)
		{
			//MINE: local player, simply enable the local scripts
			selectableScript.enabled = true;
			engineerScript.enabled = true;
            remoteScript.enabled = false;
			fogOfWarScript.enabled = true;
			navMes.enabled = true;
		}
		else
		{           
			selectableScript.enabled = false;
			engineerScript.enabled = false;
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
            UnitEngineer script = this.GetComponent<UnitEngineer>();
            stream.SendNext(script.currentEngineerState);
            stream.SendNext(script.currentState);
            stream.SendNext(script.getLife());
            stream.SendNext(script.fireballDir);
		}
		else
		{
			//Network player, receive data
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctPlayerRot = (Quaternion)stream.ReceiveNext();
            state = (UnitEngineer.EngineerState)stream.ReceiveNext();
            unitState = (UnitController.State)stream.ReceiveNext();
            currentLife = (float)stream.ReceiveNext();
            fireballDir = (Vector3)stream.ReceiveNext();
		}
	}
	
	private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this
    private UnitEngineer.EngineerState state; // new State of the HarvesterUnit
    private UnitHarvester.State unitState; // new State of Unit
    private float currentLife; // for damage
    private Vector3 fireballDir; // direction of the fireball

	void Update()
	{
		if (!photonView.isMine)
		{
			//Update remote player (smooth this, this looks good, at the cost of some accuracy)
			transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
            UnitEngineerRemote script = GetComponent<UnitEngineerRemote>();
            script.currentEngineerState = state;
            script.currentState = unitState;
            script.fireballDir = fireballDir;
		}
	}
	
}