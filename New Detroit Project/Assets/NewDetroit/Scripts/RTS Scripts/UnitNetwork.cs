using UnityEngine;
using System.Collections;

public class UnitNetwork : Photon.MonoBehaviour
{

	private CSelectable selectableScript;
    protected CStateUnit stateScript; // contains the current states of the units and other flags
	private UnitController unitScript;
    private UnitAnimationsNetwork unitAnimationsNetwork;
	private FogOfWarUnit fogOfWarScript;
	private NavMeshAgent navMeshAgent;

    private Vector3 correctPlayerPos = Vector3.zero; // We lerp towards this
    private Quaternion correctPlayerRot = Quaternion.identity; // We lerp towards this
    //private UnitBasicArtillery.ArtilleryState state; // new State of the ArtilleryState
    //private UnitController.State unitState; // new State of Unit
    protected UnitController.State lastState = UnitController.State.Idle; // last State of Unit
    //private float currentLife; // for damage
    //private bool attack2Selected; // to see if the attack has changed

    // modelo del asset (el que contiene las animaciones)
    protected Transform model;
	
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

            //Debug.Log("MINE");
		}
		else
		{           
			selectableScript.enabled      = false;
            unitScript.enabled            = false;
			fogOfWarScript.enabled        = false;
            unitAnimationsNetwork.enabled = true;
			navMeshAgent.enabled          = false;

            //Debug.Log("not MINE");
		}
		
		gameObject.name = gameObject.name + "_" + photonView.viewID;
        gameObject.name = gameObject.name.Replace("(Clone)", ""); 

        // se captura la referencia al modelo
        model = transform.FindChild("Model");
	}
	
	public virtual void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			//We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);

            // TODO! ñapa de pm, hay que hacerlo bien
            //stream.SendNext(GetComponent<CTeam>().teamNumber);

            stream.SendNext(stateScript.currentState);

            stream.SendNext(stateScript.animationSend);
            stream.SendNext(stateScript.animationName);
            stream.SendNext(stateScript.animationSendQeued);
            stream.SendNext(stateScript.animationNameQueued);

			if (stateScript.animationSend) stateScript.animationSend = false;
			if (stateScript.animationSendQeued) stateScript.animationSendQeued = false;

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

            // TODO! ñapa de pm, hay que hacerlo bien
            //GetComponent<CTeam>().teamNumber = (int)stream.ReceiveNext();

            stateScript.currentState = (UnitController.State)stream.ReceiveNext();

            stateScript.animationChanged = (bool)stream.ReceiveNext();
            stateScript.animationName = (string)stream.ReceiveNext();
            stateScript.animationChangeQueued = (bool)stream.ReceiveNext();
            stateScript.animationNameQueued = (string)stream.ReceiveNext();

            // comprobar el cambio

            //state =            (UnitBasicArtillery.ArtilleryState)stream.ReceiveNext();
            //unitState =        (UnitController.State)stream.ReceiveNext();
            //lastState =        (UnitController.State)stream.ReceiveNext();
            //currentLife =      (float)stream.ReceiveNext();
            //attack2Selected  = (bool)stream.ReceiveNext();
		}
	}
	
	public virtual void Update ()
	{
		if (!photonView.isMine)
		{
			//Update remote player (smooth this, this looks good, at the cost of some accuracy)
			transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);

            /*if (lastState != stateScript.currentState)
            {
                switch (stateScript.currentState)
                {
                    case UnitController.State.Idle: animation.CrossFade("Idle01"); break;
                    case UnitController.State.GoingTo: animation.CrossFade("Walk"); break;
                    case UnitController.State.GoingToAnEnemy: animation.CrossFade("Walk"); break;
                    case UnitController.State.Attacking: animation.CrossFade("Attack1"); break;
                    //case UnitController.State.Dying:          animation.CrossFade("Die");     break;
                    case UnitController.State.Flying: break;
                    case UnitController.State.AscendingToHeaven:
                        // TODO! cambiar el material de la unidad


                        break;
                }
            }*/
            lastState = stateScript.currentState;

            //UnitBasicArtilleryRemote script = GetComponent<UnitBasicArtilleryRemote>();
            //script.currentArtilleryState = state;
            //script.lastState = lastState;
            //script.currentState = unitState;
            //script.attack2Selected = attack2Selected;
		}
	}
	


}