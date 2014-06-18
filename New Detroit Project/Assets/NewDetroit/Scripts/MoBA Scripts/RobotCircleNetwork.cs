using UnityEngine;
using System.Collections;

public class RobotCircleNetwork : BasicNetwork {
	
	void Awake()
	{
		if (photonView.isMine)
		{
			GetComponent<RobotCircleSkill>().enabled = true;
		}
		else
		{
			GetComponent<RobotCircleSkill>().enabled = false;
		}
	}

}
