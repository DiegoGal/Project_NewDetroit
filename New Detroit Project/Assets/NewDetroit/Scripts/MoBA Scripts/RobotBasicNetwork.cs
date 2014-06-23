using UnityEngine;
using System.Collections;

public class RobotBasicNetwork : BasicNetwork {

	public virtual void Awake()
	{
		base.Awake();
		
		GetComponent<MeshRenderer> ().enabled = false;
		
		if (PhotonNetwork.connected)
			if (photonView.isMine)
			{
				GetComponent<BoxCollider>().enabled = true;
				GetComponent<RobotBasicAttack>().enabled = true;
			}
			else
			{
				GetComponent<BoxCollider>().enabled = false;
				GetComponent<RobotBasicAttack>().enabled = false;
				Destroy(GetComponent<Rigidbody>());
			}
	}
}
