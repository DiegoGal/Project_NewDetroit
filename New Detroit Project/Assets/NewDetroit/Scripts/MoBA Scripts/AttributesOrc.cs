using UnityEngine;
using System.Collections;

public class AttributesOrc : AttributesHero 
{
    // Constant values
    public const int    LIFE_1 = 375,       LIFE_2 = 550,       LIFE_3 = 725,       LIFE_4 = 900,
                        ATT_P_1 = 30,       ATT_P_2 = 45,       ATT_P_3 = 60,       ATT_P_4 = 75,
                        ATT_M_1 = 25,       ATT_M_2 = 35,       ATT_M_3 = 45,       ATT_M_4 = 55;
    public const float  ATT_SPEED_1 = 0.9f, ATT_SPEED_2 = 1,    ATT_SPEED_3 = 1.1f, ATT_SPEED_4 = 1.2f;
    public const int    DEF_P_1 = 25,       DEF_P_2 = 30,       DEF_P_3 = 35,       DEF_P_4 = 40,
                        DEF_M_1 = 20,       DEF_M_2 = 25,       DEF_M_3 = 30,       DEF_M_4 = 35,
                        MANA_1 = 175,       MANA_2 = 250,       MANA_3 = 350,       MANA_4 = 500,
                        ADREN_1 = 150,      ADREN_2 = 250,      ADREN_3 = 350,      ADREN_4 = 450,
                        WALK_SPEED_1 = 5,   WALK_SPEED_2 = 10,  WALK_SPEED_3 = 15,  WALK_SPEED_4 = 20,
                        RUN_SPEED_1 = 10,   RUN_SPEED_2 = 15,   RUN_SPEED_3 = 20,   RUN_SPEED_4 = 25;

    public const float  COOLDOWN_SKILL_1 = 5, COOLDOWN_SKILL_2 = 10, COOLDOWN_SKILL_3 = 20;

    protected const int EXP_LEVEL_1_2 = 200, EXP_LEVEL_2_3 = 600, EXP_LEVEL_3_4 = 1000;


    //----------------------------------------------------------------------------------------------


	// Use this for initialization
	public virtual void Start () 
    {
        base.Start();

        currentLife = LIFE_1;
        currentMana = MANA_1;
        currentAdren = ADREN_1;
        maximunLife = LIFE_1;
        attackPhysic = ATT_P_1;
        attackMagic = ATT_M_1;
        speedAttack = ATT_SPEED_1;
        deffensePhysic = DEF_P_1;
        deffenseMagic = DEF_M_1;
        maximunAdren = ADREN_1;
        maximunMana = MANA_1;
        speedWalk = WALK_SPEED_1;
        speedRun = RUN_SPEED_1;

        maximunCooldown1 = COOLDOWN_SKILL_1; maximunCooldown2 = COOLDOWN_SKILL_2; maximunCooldown3 = COOLDOWN_SKILL_3;
        currentCooldown1 = currentCooldown2 = currentCooldown3 = 0;

        //Mana and adrenaline for skills
        manaSkill1 = 50; manaSkill2 = -1; manaSkill3 = -1;
        adrenSkill1 = -1; adrenSkill2 = 75; adrenSkill3 = 150;
	}


    //----------------------------------------------------------------------------------------------


    public void GainExperience(int value)
    {
        if (level < 4)
        {
            experience += value;

            if (experience >= EXP_LEVEL_3_4)
            {
                level = 4;

                currentLife *= LIFE_4 / maximunLife;
                float val = currentAdren;
                val *= ADREN_4 / (float)maximunAdren;
                currentAdren = (int)val;
                val = currentMana;
                val *= MANA_4 / (float)maximunMana;
                currentMana = (int)val;

                maximunLife = LIFE_4;
                attackPhysic = ATT_P_4;
                attackMagic = ATT_M_4;
                speedAttack = ATT_SPEED_4;
                deffensePhysic = DEF_P_4;
                deffenseMagic = DEF_M_4;
                maximunAdren = ADREN_4;
                maximunMana = MANA_4;
                speedWalk = WALK_SPEED_4;
                speedRun = RUN_SPEED_4;
            }
            else if (experience >= EXP_LEVEL_2_3)
            {
                level = 3;

                currentLife *= LIFE_3 / maximunLife;
                float val = currentAdren;
                val *= ADREN_3 / (float)maximunAdren;
                currentAdren = (int)val;
                val = currentMana;
                val *= MANA_3 / (float)maximunMana;
                currentMana = (int)val;

                maximunLife = LIFE_3;
                attackPhysic = ATT_P_3;
                attackMagic = ATT_M_3;
                speedAttack = ATT_SPEED_3;
                deffensePhysic = DEF_P_3;
                deffenseMagic = DEF_M_3;
                maximunAdren = ADREN_3;
                maximunMana = MANA_3;
                speedWalk = WALK_SPEED_3;
                speedRun = RUN_SPEED_3;
            }
            else if (experience >= EXP_LEVEL_1_2)
            {
                level = 2;

                currentLife *= LIFE_2 / maximunLife;
                float val = currentAdren;
                val *= ADREN_2 / (float)maximunAdren;
                currentAdren = (int)val;
                val = currentMana;
                val *= MANA_2 / (float)maximunMana;
                currentMana = (int)val;

                maximunLife = LIFE_2;
                attackPhysic = ATT_P_2;
                attackMagic = ATT_M_2;
                speedAttack = ATT_SPEED_2;
                deffensePhysic = DEF_P_2;
                deffenseMagic = DEF_M_2;
                maximunAdren = ADREN_2;
                maximunMana = MANA_2;
                speedWalk = WALK_SPEED_2;
                speedRun = RUN_SPEED_2;
            }
        }
    }

}
