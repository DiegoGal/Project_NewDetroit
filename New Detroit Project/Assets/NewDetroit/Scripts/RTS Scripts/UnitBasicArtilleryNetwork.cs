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
			soldierScript.enabled = false;
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
            stream.SendNext(script.changedAttack);
            stream.SendNext(script.attack2Selected);
            stream.SendNext(script.getLife());
            stream.SendNext(script.attackedUnitViewID);
            stream.SendNext(script.lastAttackedUnitViewID);
		}
		else
		{
			//Network player, receive data
			correctPlayerPos =       (Vector3)stream.ReceiveNext();
			correctPlayerRot =       (Quaternion)stream.ReceiveNext();
            state =                  (UnitBasicArtillery.ArtilleryState)stream.ReceiveNext();
            unitState =              (UnitController.State)stream.ReceiveNext();
            changedAttack =          (bool)stream.ReceiveNext();
            attack2Selected =        (bool)stream.ReceiveNext();
            currentLife =            (float)stream.ReceiveNext();
            attackedUnitViewID =     (int)stream.ReceiveNext();
            lastAttackedUnitViewID = (int)stream.ReceiveNext();
		}
	}
	
	private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this
    private UnitBasicArtillery.ArtilleryState state; // new State of the ArtilleryState
    private UnitController.State unitState; // new State of Unit
    private bool changedAttack; //to see if the type of attack has changed
    private bool attack2Selected; //to change the current type of attack
    private float currentLife; // for damage
    private int attackedUnitViewID; // to see the unit we are attacking
    private int lastAttackedUnitViewID; //to see the last unit we are attacking
	
	void Update()
	{
		if (!photonView.isMine)
		{
			//Update remote player (smooth this, this looks good, at the cost of some accuracy)
			transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
            UnitBasicArtilleryRemote script = GetComponent<UnitBasicArtilleryRemote>();
            script.currentArtilleryState = state;
            script.changedAttack = changedAttack;
            script.attack2Selected = attack2Selected;
            script.currentState = unitState;
            script.attackedUnitViewID = attackedUnitViewID;
            script.lastAttackedUnitViewID = lastAttackedUnitViewID;
		}
	}
	
}