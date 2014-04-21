using UnityEngine;
using System.Collections;

public class UnitScoutNetwork: Photon.MonoBehaviour
{

    CSelectable selectableScript;
    UnitScout scoutScript;
    UnitScoutRemote remoteScript;
    FogOfWarUnit fogOfWarScript;
    NavMeshAgent navMes;

    void Awake()
    {
        selectableScript = GetComponent<CSelectable>();
        scoutScript = GetComponent<UnitScout>();
        remoteScript = GetComponent<UnitScoutRemote>();
        fogOfWarScript = GetComponent<FogOfWarUnit>();
        navMes = GetComponent<NavMeshAgent>();

        if (photonView.isMine)
        {
            //MINE: local player, simply enable the local scripts
            selectableScript.enabled = true;
            scoutScript.enabled = true;
            remoteScript.enabled = false;
            fogOfWarScript.enabled = true;
            navMes.enabled = true;
        }
        else
        {
            selectableScript.enabled = false;
            scoutScript.enabled = false;
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
            UnitScout script = this.GetComponent<UnitScout>();
            stream.SendNext(script.currentScoutState);
            stream.SendNext(script.currentState);
            stream.SendNext(script.getLife());
            stream.SendNext(script.attackedUnitViewID);
        }
        else
        {
            //Network player, receive data
            correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();
            state = (UnitScout.ScoutState)stream.ReceiveNext();
            unitState = (UnitController.State)stream.ReceiveNext();
            currentLife = (float)stream.ReceiveNext();
            attackedUnitViewID = (int)stream.ReceiveNext();
        }
    }

    private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
    private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this
    private UnitScout.ScoutState state; // new State of the HarvesterUnit
    private UnitHarvester.State unitState; // new State of Unit
    private float currentLife; // for damage
    private int attackedUnitViewID; // to see the unit we are attacking

    void Update()
    {
        if (!photonView.isMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
            UnitScoutRemote script = GetComponent<UnitScoutRemote>();
            script.currentScoutState = state;
            script.currentState = unitState;
            script.attackedUnitViewID = attackedUnitViewID;
        }
    }

}