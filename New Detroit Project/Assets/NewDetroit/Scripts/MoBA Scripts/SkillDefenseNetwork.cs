using UnityEngine;
using System.Collections;

public class SkillDefenseNetwork : BasicNetwork {

	void Awake()
	{
	if (PhotonNetwork.connected)
		if (photonView.isMine)
		{
			GetComponent<SkillDefense>().enabled = true;
		}
		else
		{
			GetComponent<SkillDefense>().enabled = false;
		}
	}
	
}
