using UnityEngine;
using System.Collections;

public class GrenadeNetwork : BasicNetwork {

	SphereCollider sphereColl;
	Rigidbody rigidB;
	CGrenadeVisionSphere visionSphere;
	

	public override void Awake()
	{
		base.Awake();
		sphereColl = GetComponent<SphereCollider>();
		rigidB = GetComponent<Rigidbody>();
		visionSphere = GetComponent<CGrenadeVisionSphere>();
		
		if (photonView.isMine)
		{
			sphereColl.enabled = true;
			visionSphere.enabled = true;
		}
		else
		{
			sphereColl.enabled = false;
			Destroy (rigidB);
			visionSphere.enabled = false;
		}
	}
	
	// Update is called once per frame
//	void Update () {
//	
//	}
}
