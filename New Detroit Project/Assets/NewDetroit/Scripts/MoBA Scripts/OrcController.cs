using UnityEngine;
using System.Collections;

public class OrcController : HeroeController
{
	private const int 		LIFE_1 = 		375, 	LIFE_2 = 		550, 	LIFE_3 = 		725, 	LIFE_4 = 		900,
							ATT_P_1 = 		30, 	ATT_P_2 = 		45, 	ATT_P_3 = 		60, 	ATT_P_4 = 		75,
							ATT_M_1 = 		25, 	ATT_M_2 = 		35, 	ATT_M_3 = 		45, 	ATT_M_4 = 		55;
	private const double 	ATT_SPEED_1 = 	0.9, 	ATT_SPEED_2 = 	1, 		ATT_SPEED_3 = 	1.1, 	ATT_SPEED_4 = 	1.2;
	private const int 		DEF_P_1 = 		25, 	DEF_P_2 = 		30, 	DEF_P_3 = 		35, 	DEF_P_4 = 		40,
							DEF_M_1 = 		20, 	DEF_M_2 = 		25, 	DEF_M_3 = 		30, 	DEF_M_4 = 		35,
							MANA_1 = 		175, 	MANA_2 = 		250, 	MANA_3 = 		350, 	MANA_4 = 		500,
							ADREN_1 = 		150, 	ADREN_2 = 		250, 	ADREN_3 = 		350, 	ADREN_4 = 		450,
							MOV_SPEED_1 = 	50, 	MOV_SPEED_2 = 	55, 	MOV_SPEED_3 = 	60, 	MOV_SPEED_4 = 	35;
	
	
	//-----------------------------------------------------------------------------------------------------------------
	public GameObject leftArm, rightArm;
	
	
	//-----------------------------------------------------------------------------------------------------------------
	// PRIVATE
	private void newLevel()
	{
		if (this.hasNewLevel) 
		{
			int maxLife = (int) maximunLife,
			maxAdren = adren,
			maxMana = mana;
			switch (level)
			{
			case 2:
				this.maximunLife = LIFE_2;
				this.attackP = ATT_P_2;
				this.attackM = ATT_M_2;
				this.speedAtt = ATT_SPEED_2;
				this.defP = DEF_P_2;
				this.defM = DEF_M_2;
				this.mana = MANA_2;
				this.adren = ADREN_2;
				this.speedMov = MOV_SPEED_2;
				break;
			case 3:
				this.maximunLife = LIFE_3;
				this.attackP = ATT_P_3;
				this.attackM = ATT_M_3;
				this.speedAtt = ATT_SPEED_3;
				this.defP = DEF_P_3;
				this.defM = DEF_M_3;
				this.mana = MANA_3;
				this.adren = ADREN_3;
				this.speedMov = MOV_SPEED_3;
				break;
			case 4:
				this.maximunLife = LIFE_4;
				this.attackP = ATT_P_4;
				this.attackM = ATT_M_4;
				this.speedAtt = ATT_SPEED_4;
				this.defP = DEF_P_4;
				this.defM = DEF_M_4;
				this.mana = MANA_4;
				this.adren = ADREN_4;
				this.speedMov = MOV_SPEED_4;
				break;
			}//end switch (level)
			
			float percentage = (float)(maximunLife - maxLife) / maxLife;
			currentLife = (1 + percentage) * currentLife;
			
			percentage = (float)(adren - maxAdren) / maxAdren;
			currentAdren = (int) ((1 + percentage) * currentAdren);
			
			percentage = (float)(mana - maxMana) / maxMana;
			currentMana = (int) ((1 + percentage) * currentMana);
			
			hasNewLevel = false;
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
	
	
	//-----------------------------------------------------------------------------------------------------------------
	// MAIN
	// Use this for initialization
	public override void Start ()
	{
		base.Start ();
		
		this.currentLife = LIFE_1;
		this.currentMana = MANA_1;
		this.currentAdren = ADREN_1;
		
		this.maximunLife = LIFE_1;
		this.attackP = ATT_P_1;
		this.attackM = ATT_M_1;
		this.speedAtt = ATT_SPEED_1;
		this.defP = DEF_P_1;
		this.defM = DEF_M_1;
		this.mana = MANA_1;
		this.adren = ADREN_1;
		this.speedMov = MOV_SPEED_1;
		
		//Set the type of heroe
		this.type = TypeHeroe.Orc;
		
		//Set the owner in the basic attack
		this.rightArm.GetComponent<OrcBasicAttack> ().setOwner (this.gameObject);
		this.leftArm.GetComponent<OrcBasicAttack> ().setOwner (this.gameObject);
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		base.Update ();
		
		this.attack ();
		this.newLevel ();
	}
}

