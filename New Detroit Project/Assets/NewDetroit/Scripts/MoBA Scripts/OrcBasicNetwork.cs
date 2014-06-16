using UnityEngine;
using System.Collections;

public class OrcBasicNetwork : BasicNetwork {

	public virtual void Awake()
	{
		base.Awake();
		
		GetComponent<MeshRenderer> ().enabled = false;
		
		if (photonView.isMine)
		{
			GetComponent<BoxCollider>().enabled = true;
			GetComponent<OrcBasicAttack>().enabled = true;
		}
		else
		{
			GetComponent<BoxCollider>().enabled = false;
			GetComponent<OrcBasicAttack>().enabled = false;
			Destroy(GetComponent<Rigidbody>());
		}
	}
	
//	public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//	{
//		base.OnPhotonSerializeView(stream, info);
//	
//		if (stream.isWriting)
//		{
//			//We own this player: send the others our data
//		}
//		else
//		{
//			//Network player, receive data
//		}
//	}
}
