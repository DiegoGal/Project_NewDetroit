using UnityEngine;
using System.Collections;

public class TowerArmyNetwork : Photon.MonoBehaviour
{
	
	CLife lifeScript;
	
	void Awake()
	{
		lifeScript = GetComponent<CLife>();
        GetComponent<CTeamTowerArmy>().enabled = true;

		if (photonView.isMine)
		{
			lifeScript.enabled = true;
			GetComponent<TowerArmy>().enabled = true;
		}
		else if (PhotonNetwork.connected)
		{
			lifeScript.enabled = true;
			GetComponent<TowerArmy>().enabled = false;
			this.gameObject.SetActive(false);
			Destroy(GameObject.Find("Light"));
			Destroy(GameObject.Find("TowerSphereConstruct"));
			//renderer.material = (Material) Resources.Load("GoblinTower_Material",typeof(Material));
		}
		
		gameObject.name = gameObject.name + photonView.viewID;
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{

		if (stream.isWriting)
		{
			//We own this player: send the others our data
			stream.SendNext(lifeScript.getLife());
			
		}
		else
		{
			//Network player, receive data
			life = (float)stream.ReceiveNext();
		}
	}
	
	private float life = 0; // Resources
	
	void Update()
	{
        if (PhotonNetwork.connected)
        if (!photonView.isMine)
		{
			lifeScript.setLife(life);
		}
	}
}