﻿using UnityEngine;
using System.Collections;

public class UnitHarvesterNetwork : Photon.MonoBehaviour {

	CSelectable selectableScript;
	UnitHarvester harvesterScript;
	FogOfWarUnit fogOfWarScript;
	NavMeshAgent navMes;
	
	void Awake()
	{
		selectableScript = GetComponent<CSelectable>();
		harvesterScript = GetComponent<UnitHarvester>();
		fogOfWarScript	= GetComponent<FogOfWarUnit>();
		navMes			= GetComponent<NavMeshAgent>();

		if (photonView.isMine)
		{
			//MINE: local player, simply enable the local scripts
			selectableScript.enabled = true;
			harvesterScript.enabled = true;
			fogOfWarScript.enabled = true;
			navMes.enabled = true;
		}
		else
		{           
			selectableScript.enabled = false;
			harvesterScript.enabled = true;
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
            UnitHarvester script = this.GetComponent<UnitHarvester>();
            stream.SendNext(script.currentHarvestState);
            stream.SendNext(script.currentState);
            stream.SendNext(script.getLife());
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
            
		}
	}
	
	private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this
    private UnitHarvester.HarvestState state; // new State of the HarvesterUnit
    private UnitHarvester.State unitState; // new State of Unit
    private float currentLife; // for damage
	
	void Update()
	{
		if (!photonView.isMine)
		{
			//Update remote player (smooth this, this looks good, at the cost of some accuracy)
			transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
            UnitHarvester script = GetComponent<UnitHarvester>();
            script.currentHarvestState = state;
            script.currentState = unitState;
		}
	}
	
}