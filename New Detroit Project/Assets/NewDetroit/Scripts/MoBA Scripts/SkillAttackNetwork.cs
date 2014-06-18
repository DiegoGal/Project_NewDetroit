using UnityEngine;
using System.Collections;

public class SkillAttackNetwork : BasicNetwork {

	public bool isParticleDamage = false;
	
	
	//-------------------------------------------
	

	public virtual void Awake()
	{
		base.Awake();
		
		if (photonView.isMine)
		{
			GetComponent<SkillAttack>().enabled = true;
			if (!isParticleDamage)
				GetComponent<SphereCollider>().enabled = true;
		}
		else
		{
			GetComponent<SkillAttack>().enabled = false;			
			if (!isParticleDamage)
			{
				GetComponent<SphereCollider>().enabled = false;
				Destroy(GetComponent<Rigidbody>());
			}
		}
	}
	
}
