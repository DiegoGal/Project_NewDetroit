using UnityEngine;
using System.Collections;

public class RobotController : HeroeController
{
	//==============================
	//=====     Attributes     =====
	//==============================
	
	private const int IN_LIFE = 200,
	IN_ATT_P = 10,
	IN_ATT_M = 15,
	IN_DEF_P = 5,
	IN_DEF_M = 5,
	IN_MANA_1_2 = 100, IN_MANA_2_3 = 150, IN_MANA_3_4 = 150,
	IN_ADREN = 75,
	IN_SPEED_MOV = 5;
	private const double IN_SPEED_ATT = 0.1f;
	
	//===========================
	//=====     Methods     =====
	//===========================
	
	//PRIVATE
	
	private void newLevel()
	{
		if (hasNewLevel) 
		{
            life.currentLife += IN_LIFE;
			attackP += IN_ATT_P;
			attackM += IN_ATT_M;
			speedAtt += IN_SPEED_ATT;
			defP += IN_DEF_P;
			defM += IN_DEF_M;
			if (level == 2) mana += IN_MANA_1_2;
			else if (level == 3) mana += IN_MANA_2_3;
			else mana += IN_MANA_3_4;
			adren += IN_ADREN;
			speedMov += IN_SPEED_MOV;
		}
	}
	
	//================================
	//=====     Main methods     =====
	//================================
	
	// Use this for initialization
	public override void Start ()
	{
		base.Start ();
        this.life.currentLife = 375;
		this.attackP = 30;
		this.attackM = 25;
		this.speedAtt = 0.9f;
		this.defP = 25;
		this.defM = 20;
		this.mana = 175;
		this.adren = 150;
		this.speedMov = 50;
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		base.Update ();
		this.newLevel ();
	}
}


