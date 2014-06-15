using UnityEngine;
using System.Collections;

public class RobotTurnNetwork : BasicNetwork {

	public virtual void Awake()
	{
		base.Awake();
		
		if (photonView.isMine)
		{
			GetComponent<RobotTurn>().enabled = true;
			GetComponent<SphereCollider>().enabled = true;
		}
		else
		{
			GetComponent<RobotTurn>().enabled = false;
			GetComponent<SphereCollider>().enabled = false;
			
			Destroy(GetComponent<Rigidbody>());
		}
	}
}
