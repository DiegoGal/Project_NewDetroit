using UnityEngine;
using System.Collections;

public class WarehouseNetwork : Photon.MonoBehaviour
{
	
	CLife lifeScript;
	
	void Awake()
	{
		lifeScript = GetComponent<CLife>();
		
		if (photonView.isMine)
		{
			lifeScript.enabled = true;
			GetComponent<Warehouse>().enabled = true;
		}
		else
		{
			lifeScript.enabled = true;
			GetComponent<Warehouse>().enabled = false;
			this.gameObject.SetActive(false);
			Destroy(GameObject.Find("Light"));
			Destroy(GameObject.Find("WarehouseBoxConstruct"));
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
		if (!photonView.isMine)
		{
			lifeScript.setLife(life);
		}
	}
}