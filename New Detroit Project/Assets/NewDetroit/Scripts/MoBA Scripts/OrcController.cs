using UnityEngine;
using System.Collections;

public class OrcController : HeroeController
{
	//==============================
	//=====     Attributes     =====
	//==============================
	private const int IN_LIFE = 175,
					IN_ATT_P = 15,
					IN_ATT_M = 10,
					IN_DEF_P = 5,
					IN_DEF_M = 5,
					IN_MANA_1_2 = 75, IN_MANA_2_3 = 100, IN_MANA_3_4 = 150,
					IN_ADREN = 100,
					IN_SPEED_MOV = 5;
	private const double IN_SPEED_ATT = 0.1f;


	//===========================
	//=====     Methods     =====
	//===========================

	//PRIVATE

	private void newLevel()
	{
		if (this.hasNewLevel) 
		{
			this.lifeUp(IN_LIFE);
			this.attackPUp(IN_ATT_P);
			this.attackMUp(IN_ATT_M);
			this.speedAttUp(IN_SPEED_ATT);
			this.defPUp(IN_DEF_P);
			this.defMUp(IN_DEF_M);
			if (this.level == 2) this.manaUp(IN_MANA_1_2);
			else if (this.level == 3) this.manaUp(IN_MANA_2_3);
			else this.manaUp(IN_MANA_3_4);
			this.adrenUp(IN_ADREN);
			this.speedMovUp(IN_SPEED_MOV);
		}
	}

	private void attack()
	{
		// if the orc is not dead and is not recovering after his dead
		if (this.state != StateHeroe.Dead && this.state != StateHeroe.Recover)
		{
			// If the hero is attacking
			if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") ||
			     animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") ||
			     animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3")) &&
			    animator.GetBool("isAttacking"))
			{
				/*if (!attackInstantiate)
				{
					this.attackInstantiate = true;
				}*/
				this.state = StateHeroe.AttackBasic;
			}
			// Else if the hero is not attacking
			else
			{
				//this.attackInstantiate = false;
				this.state = StateHeroe.IdleWalkRun;
			}
		}
	}

	//================================
	//=====     Main methods     =====
	//================================
	
	// Use this for initialization
	public override void Start ()
	{
		base.Start ();
		this.life = 375;
		this.attackP = 30;
		this.attackM = 25;
		this.speedAtt = 0.9f;
		this.defP = 25;
		this.defM = 20;
		this.mana = 175;
		this.adren = 150;
		this.speedMov = 50;
		this.totalLife = 375;
	}

	// Update is called once per frame
	public override void Update ()
	{
		base.Update ();
		this.attack ();

		//this.newLevel ();
	}
}

