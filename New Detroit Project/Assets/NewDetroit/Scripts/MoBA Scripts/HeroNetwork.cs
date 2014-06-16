using UnityEngine;
using System.Collections;

public class HeroNetwork : BasicNetwork 
{
	protected CStateUnit cState; // contains the current states of hero


	//-----------------------------------------------------
	public virtual void Awake()
	{
		base.Awake();

		//Initialize CBasicAttributesHero
		CBasicAttributesHero basicAttr = GetComponent<CBasicAttributesHero>();
		basicAttr.setLevel(1);
		if (GetComponent<HeroeController>().getTypeHero() == HeroeController.TypeHeroe.Orc)
		{
			basicAttr.currentLife = basicAttr.maximunLife = OrcController.LIFE_1;
			basicAttr.setCurrentMana(OrcController.MANA_1);
			basicAttr.setMaximunMana(OrcController.MANA_1);
			basicAttr.setCurrentAdren(OrcController.ADREN_1);
			basicAttr.setMaximunAdren(OrcController.ADREN_1);
		}
		else
		{
			basicAttr.currentLife = basicAttr.maximunLife = RobotController.LIFE_1;
			basicAttr.setCurrentMana(RobotController.MANA_1);
			basicAttr.setMaximunMana(RobotController.MANA_1);
			basicAttr.setCurrentAdren(RobotController.ADREN_1);
			basicAttr.setMaximunAdren(RobotController.ADREN_1);
		}

		//Initialize cState
		cState = GetComponent<CStateUnit>();

		// Enabled/Disabled scripts
		GetComponent<CBasicAttributesHero>().enabled = true;
		GetComponent<CharacterController>().enabled = true;
		cState.enabled = true;

		if (photonView.isMine)
		{
//			GetComponent<CharacterController>().enabled = true;
			GetComponent<ThirdPersonCamera>().enabled = true;
			GetComponent<FogOfWarUnit>().enabled = true;
			GetComponent<NavMeshObstacle>().enabled = true;

			//Orc
			if (GetComponent<HeroeController>().getTypeHero() == HeroeController.TypeHeroe.Orc)
			{
				GetComponent<OrcController>().enabled = true;
			}
			//Robot
			else
			{
				GetComponent<RobotController>().enabled = true;
			}
		}
		else
		{
//			GetComponent<CharacterController>().enabled = false;
			GetComponent<ThirdPersonCamera>().enabled = false;
			GetComponent<FogOfWarUnit>().enabled = false;
			GetComponent<NavMeshObstacle>().enabled = false;

			//Orc
			if (GetComponent<HeroeController>().getTypeHero() == HeroeController.TypeHeroe.Orc)
			{
				GetComponent<OrcController>().enabled = false;
			}
			//Robot
			else
			{
				GetComponent<RobotController>().enabled = false;
			}
		}
	}
	
	public virtual void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		base.OnPhotonSerializeView(stream, info);
		
		if (stream.isWriting)
		{
			stream.SendNext(cState.animationSend);
			stream.SendNext(cState.animationName);
			stream.SendNext(cState.animationSendQeued);
			stream.SendNext(cState.animationNameQueued);
			stream.SendNext(cState.animationSendQueued2);
			stream.SendNext(cState.animationNameQueued2);
			
			if (cState.animationSend) cState.animationSend = false;
			if (cState.animationSendQeued) cState.animationSendQeued = false;
			if (cState.animationSendQueued2) cState.animationSendQueued2 = false;
		}
		else
		{            
			cState.animationChanged = (bool)stream.ReceiveNext();
			cState.animationName = (string)stream.ReceiveNext();
			cState.animationChangeQueued = (bool)stream.ReceiveNext();
			cState.animationNameQueued = (string)stream.ReceiveNext();
			cState.animationChangeQueued2 = (bool)stream.ReceiveNext();
			cState.animationNameQueued2 = (string)stream.ReceiveNext();
		}
	}
}
