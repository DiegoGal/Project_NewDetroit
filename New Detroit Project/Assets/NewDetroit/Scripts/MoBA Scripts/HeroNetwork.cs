using UnityEngine;
using System.Collections;

public class HeroNetwork : BasicNetwork 
{

	public virtual void Awake()
	{
		base.Awake();

		GetComponent<CLife>().enabled = true;
		GetComponent<CStateUnit>().enabled = true;

		if (photonView.isMine)
		{
			GetComponent<CharacterController>().enabled = true;
			GetComponent<ThirdPersonCamera>().enabled = true;
			GetComponent<FogOfWarUnit>().enabled = true;
			GetComponent<NavMeshObstacle>().enabled = true;

			//Orc
			if (GetComponent<HeroeController>().getTypeHero() == HeroeController.TypeHeroe.Orc)
			{
				GetComponent<OrcController>().enabled = true;
				GetComponent<SphereCollider>().enabled = true;
				GetComponent<OrcBullStrikeAttack>().enabled = true;
			}
			//Robot
			else
			{
				GetComponent<RobotController>().enabled = true;
			}
		}
		else
		{
			GetComponent<CharacterController>().enabled = false;
			GetComponent<ThirdPersonCamera>().enabled = false;
			GetComponent<FogOfWarUnit>().enabled = false;
			GetComponent<NavMeshObstacle>().enabled = false;

			//Orc
			if (GetComponent<HeroeController>().getTypeHero() == HeroeController.TypeHeroe.Orc)
			{
				GetComponent<OrcController>().enabled = false;
				GetComponent<SphereCollider>().enabled = false;
				GetComponent<OrcBullStrikeAttack>().enabled = false;
			}
			//Robot
			else
			{
				GetComponent<RobotController>().enabled = false;
			}
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
