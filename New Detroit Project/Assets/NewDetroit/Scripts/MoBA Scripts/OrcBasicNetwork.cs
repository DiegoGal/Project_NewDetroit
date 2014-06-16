using UnityEngine;
using System.Collections;

public class OrcBasicNetwork : BasicNetwork {

	public virtual void Awake()
	{
		base.Awake();
		
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
}
