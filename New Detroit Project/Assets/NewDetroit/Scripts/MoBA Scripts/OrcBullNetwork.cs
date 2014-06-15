using UnityEngine;
using System.Collections;

public class OrcBullNetwork : BasicNetwork {

	public virtual void Awake()
	{
		base.Awake();
		
		if (photonView.isMine)
		{
			GetComponent<OrcBullStrikeAttack>().enabled = true;
			GetComponent<SphereCollider>().enabled = true;
		}
		else
		{
			GetComponent<OrcBullStrikeAttack>().enabled = false;
			GetComponent<SphereCollider>().enabled = false;

			Destroy(GetComponent<Rigidbody>());
		}
	}
}
