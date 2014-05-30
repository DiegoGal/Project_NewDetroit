using UnityEngine;
using System.Collections;

public class UnitNetwork : Photon.MonoBehaviour
{

	CSelectable selectableScript;
    CStateUnit stateScript;           // contains the current states of the units and other flags
	UnitController unitScript;
    UnitAnimationsNetwork unitAnimationsNetwork;
	FogOfWarUnit fogOfWarScript;
	NavMeshAgent navMeshAgent;

    private Vector3 correctPlayerPos = Vector3.zero; // We lerp towards this
    private Quaternion correctPlayerRot = Quaternion.identity; // We lerp towards this
    //private UnitBasicArtillery.ArtilleryState state; // new State of the ArtilleryState
    //private UnitController.State unitState; // new State of Unit
    private UnitController.State lastState; // last State of Unit
    //private float currentLife; // for damage
    //private bool attack2Selected; // to see if the attack has changed
	
	void Awake ()
	{
		selectableScript      = GetComponent<CSelectable>();
        unitScript            = GetComponent<UnitController>();
		fogOfWarScript	      = GetComponent<FogOfWarUnit>();
        unitAnimationsNetwork = GetComponent<UnitAnimationsNetwork>();
        navMeshAgent          = GetComponent<NavMeshAgent>();
        stateScript           = GetComponent<CStateUnit>();
		
		if (photonView.isMine)
		{
			//MINE: local player, simply enable the local scripts
			selectableScript.enabled      = true;
            unitScript.enabled            = true;
			fogOfWarScript.enabled        = true;
            unitAnimationsNetwork.enabled = false;
			navMeshAgent.enabled          = true;

            Debug.Log("MINE");
		}
		else
		{           
			selectableScript.enabled      = false;
            unitScript.enabled            = false;
			fogOfWarScript.enabled        = false;
            unitAnimationsNetwork.enabled = true;
			navMeshAgent.enabled          = false;

            Debug.Log("not MINE");
		}
		
		gameObject.name = gameObject.name + "_" + photonView.viewID;
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			//We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);

            stream.SendNext(stateScript.currentState);

            //UnitBasicArtillery script = this.GetComponent<UnitBasicArtillery>();
            //stream.SendNext(script.currentArtilleryState);
            //stream.SendNext(script.currentState);
            //stream.SendNext(script.lastState);
            //stream.SendNext(script.getLife());
            //stream.SendNext(script.attack2Selected);
		}
		else
		{
			//Network player, receive data
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctPlayerRot = (Quaternion)stream.ReceiveNext();

            stateScript.currentState = (UnitController.State)stream.ReceiveNext();
            // comprobar el cambio

            //state =            (UnitBasicArtillery.ArtilleryState)stream.ReceiveNext();
            //unitState =        (UnitController.State)stream.ReceiveNext();
            //lastState =        (UnitController.State)stream.ReceiveNext();
            //currentLife =      (float)stream.ReceiveNext();
            //attack2Selected  = (bool)stream.ReceiveNext();
		}
	}
	

	
	void Update()
	{
		if (!photonView.isMine)
		{
			//Update remote player (smooth this, this looks good, at the cost of some accuracy)
			transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);

            if (lastState != stateScript.currentState)
            {
                // cambiar animaciones
                unitAnimationsNetwork.ChangeAnimation(stateScript.currentState);
            }
            lastState = stateScript.currentState;

            //UnitBasicArtilleryRemote script = GetComponent<UnitBasicArtilleryRemote>();
            //script.currentArtilleryState = state;
            //script.lastState = lastState;
            //script.currentState = unitState;
            //script.attack2Selected = attack2Selected;
		}
	}
	
}