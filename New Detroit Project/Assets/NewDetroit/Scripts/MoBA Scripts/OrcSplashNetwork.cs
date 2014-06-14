using UnityEngine;
using System.Collections;

public class OrcSplashNetwork : BasicNetwork 
{
	public virtual void Awake()
	{
		base.Awake();

		if (photonView.isMine)
		{
			GetComponent<OrcSplashAttack>().enabled = true;
			GetComponent<SphereCollider>().enabled = true;
		}
		else
		{
			GetComponent<OrcSplashAttack>().enabled = false;
			GetComponent<SphereCollider>().enabled = false;
		}
	}
}
