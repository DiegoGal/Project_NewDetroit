using UnityEngine;
using System.Collections;

public class RobotShotNetwork : BasicNetwork {

	public virtual void Awake()
	{
		base.Awake();
		
		if (photonView.isMine)
		{
			GetComponent<RobotShot>().enabled = true;
			GetComponent<BoxCollider>().enabled = true;
		}
		else
		{
			GetComponent<RobotShot>().enabled = false;
			GetComponent<BoxCollider>().enabled = false;
			
			Destroy(GetComponent<Rigidbody>());
		}
	}
}
