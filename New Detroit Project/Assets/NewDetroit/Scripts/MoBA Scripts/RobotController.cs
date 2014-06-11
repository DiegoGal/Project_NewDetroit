using UnityEngine;
using System.Collections;

public class RobotController : HeroeController
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

	protected const float 	COOLDOWN_SKILL_1 = 	5,	COOLDOWN_SKILL_2 = 	10,	COOLDOWN_SKILL_3 = 	20;
	//-----------------------------------------------------------------------------------------------------------------


	//sphere sword
	public GameObject cubeColliderSword;
	public GameObject skelterShot;
	private GameObject fireShotInst;
	//second skill
	public GameObject skelterTurn;
	private GameObject turnInst;
	//Time counts
	private float timeCountLife = 0;

	private Transform gun;
	public GameObject fireBall;
	private GameObject fireCircleInst;
	private float timeCircleCD = 5f;
	private bool timeCircle=false;

	//-----------------------------------------------------------------------------------------------------------------


	// PRIVATE    
	private void newLevel()
	{
		if (this.hasNewLevel) 
		{
			int maxLife = (int)life.maximunLife,
			maxAdren = adren,
			maxMana = mana;
			switch (level)
			{
			case 2:
				this.life.maximunLife = LIFE_2;
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
				this.life.maximunLife = LIFE_3;
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
				this.life.maximunLife = LIFE_4;
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
			
			float percentage = (float)(life.maximunLife - maxLife) / maxLife;
			life.currentLife = (1 + percentage) * life.currentLife;
			
			percentage = (float)(adren - maxAdren) / maxAdren;
			currentAdren = (int) ((1 + percentage) * currentAdren);
			
			percentage = (float)(mana - maxMana) / maxMana;
			currentMana = (int) ((1 + percentage) * currentMana);
			
			hasNewLevel = false;
		}
	}
	
	//================================
	//=====     Main methods     =====
	//================================
	
	// Use this for initialization
	public override void Start ()
	{
		base.Start ();
		
		this.life.currentLife = LIFE_1;
		this.currentMana = MANA_1;
		this.currentAdren = ADREN_1;
		
		this.life.maximunLife = LIFE_1;
		this.attackP = ATT_P_1;
		this.attackM = ATT_M_1;
		this.speedAtt = ATT_SPEED_1;
		this.defP = DEF_P_1;
		this.defM = DEF_M_1;
		this.mana = MANA_1;
		this.adren = ADREN_1;
		this.speedMov = MOV_SPEED_1;

		//set owner to sphere collider sword
		//this.sphereSword.GetComponent<OrcBasicAttack> ().setOwner (this.gameObject);

		//Set the collider cubes in the sword

		Transform sword = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Cylinder002/cuchilla");
		GameObject cubeColliderInst = (GameObject) Instantiate(cubeColliderSword, sword.position + new Vector3(0.4f, 1, 0.1f), sword.rotation);
		cubeColliderInst.transform.parent = sword;
		cubeColliderInst.GetComponent<RobotBasicAttack> ().owner = this.gameObject;
		
		//Set the type of heroe
		this.type = TypeHeroe.Robot;

		//Initializes the cooldowns of skills
		cooldown1total = COOLDOWN_SKILL_1; cooldown2total = COOLDOWN_SKILL_2; cooldown3total = COOLDOWN_SKILL_3;
		cooldown1 = COOLDOWN_SKILL_1; cooldown2 = COOLDOWN_SKILL_2; cooldown3 = COOLDOWN_SKILL_3;
		
		//Mana and adrenaline for skills
		manaSkill1 = 50; manaSkill2 = -1; manaSkill3 = -1;
		adrenSkill1 = -1; adrenSkill2 = 75; adrenSkill3 = 150;
		
		//Initialize the animation
		animation.Play ("Idle01");

		if (gun == null)
			gun = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Cylinder002/cuchilla");
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		base.Update ();
		updateManaAdren();
		UpdateAnimation();
//		UpdateParticles (); // Update particles
		Counter();
		this.newLevel ();
	}

	//--------------------------------------------------------------------------------------------
	//Animation
	// Only can do an action if hero don't do a secondary attack
    public void UpdateAnimation()
    {
        if (!doingSecondaryAnim)
        {
            // Secondary attack
            if (state == StateHeroe.AttackSecond)
            {
                if (stateAttackSecond == AttackSecond.Attack1 && cooldown1 == cooldown1total)
                {
                    //animation.CrossFade("Attack1");
                    //----------------------------
                    Transform head = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head");
                    fireCircleInst = (GameObject)Instantiate(fireBall, head.position + Vector3.down * 0.5f, transform.rotation);
                    fireCircleInst.transform.parent = head;
                    timeCircle = true;
                    Destroy(fireCircleInst, 1.7f);
					//------------------------------------
					canRotate = true;
					canMove = true;
					doingSecondaryAnim = false;
                }
                else if (stateAttackSecond == AttackSecond.Attack2 && cooldown2 == cooldown2total)
                {
                    animation.CrossFade("Attack2");
                    //----------------------------
                    turnInst = (GameObject)Instantiate(skelterTurn, transform.position + Vector3.up, transform.rotation);
                    turnInst.GetComponent<RobotTurn>().SetDamage(75);
                    turnInst.GetComponent<RobotTurn>().setOwner(gameObject);
                    turnInst.GetComponent<RobotTurn>().setTimeToTurn(1f);
                    Destroy(turnInst, 5f);
					//------------------------------------
					canRotate = false;
					canMove = false;
					doingSecondaryAnim = true;
                }
                else if (stateAttackSecond == AttackSecond.Attack3 && cooldown3 == cooldown3total)
                {
                    animation.CrossFade("Attack3");
                    //----------------------------
                    if (fireShotInst != null)
                        Destroy(fireShotInst);
                    skelterShot.GetComponent<MeshRenderer>().enabled = false;
                    fireShotInst = (GameObject)Instantiate(skelterShot, gun.position, transform.rotation);
                    fireShotInst.GetComponent<RobotShot>().SetDamage(75);
                    fireShotInst.GetComponent<RobotShot>().setOwner(gameObject);
                    fireShotInst.GetComponent<RobotShot>().setSpeed(2);
                    fireShotInst.GetComponent<RobotShot>().setDirection(transform.forward);
                    fireShotInst.GetComponent<RobotShot>().setTimeToShot(1.7f);
                    fireShotInst.transform.parent = gun;
                    Destroy(fireShotInst, 2.5f);
					//------------------------------------
					canRotate = false;
					canMove = false;
					doingSecondaryAnim = true;
                }
				//------------------------------------
				extraSpeed = false;
            }
            // Basic attack
            else if (state == StateHeroe.AttackBasic)
            {
                if (!animation.IsPlaying("Attack1") && !animation.IsPlaying("Attack2") && !animation.IsPlaying("Attack3"))
                {
                    animation.CrossFade("Attack1");
                    animation["Attack1"].speed = 1.2f;
                    animation.CrossFadeQueued("Attack2").speed = 1.2f;
                    animation.CrossFadeQueued("Attack3").speed = 1.2f;
                    for (int i = 0; i < 20; i++)
                    {
                        animation.CrossFadeQueued("Attack1").speed = 1.2f;
                        animation.CrossFadeQueued("Attack2").speed = 1.2f;
                        animation.CrossFadeQueued("Attack3").speed = 1.2f;
                    }
                }

                if (animation.IsPlaying("Attack3") && fireShotInst == null)
                {
                    skelterShot.GetComponent<MeshRenderer>().enabled = false;
                    fireShotInst = (GameObject)Instantiate(skelterShot, gun.position, transform.rotation);
                    fireShotInst.GetComponent<RobotShot>().SetDamage(75);
                    fireShotInst.GetComponent<RobotShot>().setOwner(gameObject);
                    fireShotInst.GetComponent<RobotShot>().setSpeed(2);
                    fireShotInst.GetComponent<RobotShot>().setDirection(transform.forward);
                    fireShotInst.GetComponent<RobotShot>().setTimeToShot(1.7f);
                    fireShotInst.transform.parent = gun;
                    Destroy(fireShotInst, 2.5f);
                }
				//------------------------------------
				canRotate = false;
				canMove = false;
				extraSpeed = false;
            }
            // Movement
            else if (state == StateHeroe.Run)
            {
                animation.CrossFade("Run");
				//------------------------------------
				canRotate = true;
				canMove = true;
				extraSpeed = false;
            }
            else if (state == StateHeroe.Walk)
            {
                animation.CrossFade("Walk");
				//------------------------------------
				canRotate = true;
				canMove = true;
				extraSpeed = false;
            }
            else if (state == StateHeroe.Dead)
            {
                animation.CrossFade("Die");
                this.life.currentLife = 0;
                this.transform.position = this.initialPosition;
                isMine = false;
                //this.GetComponent<ThirdPersonController>().enabled = false;
				//------------------------------------
				canRotate = false;
				canMove = false;
				extraSpeed = false;
            }
            else if (this.state == StateHeroe.Recover)
            {
                if (this.timeCountLife < 1) this.timeCountLife += Time.deltaTime;
                else
                {
                    this.timeCountLife = 0;
                    this.life.currentLife += 20;
                    if (this.life.currentLife >= this.life.maximunLife)
                    {
                        this.life.currentLife = this.life.maximunLife;
                        this.state = StateHeroe.Idle;
                        isMine = true;
                    }
                }
				//------------------------------------
				canRotate = false;
				canMove = false;
				extraSpeed = false;
            }
            // Idle
            else
            {
                if (!animation.IsPlaying("Idle01"))
                {
                    animation.CrossFade("Idle01");
                }
				//------------------------------------
				canRotate = false;
				canMove = false;
				extraSpeed = false;
            }
        }
        else
        {
			if (!animation.isPlaying)
            {
                doingSecondaryAnim = false;
            }
        }

        if (timeCircle)
        {
            if (timeCircleCD <= 0)
            {
                timeCircleCD = 5f;
                timeCircle = false;
            }
            else
                timeCircleCD -= Time.deltaTime;
        }

        if (timeCircle)
        {
            this.defP = this.defP * 3;
            this.defM = this.defM * 3;
        }
        else
        {
            this.defP = this.defP;
            this.defM = this.defM;
        }
    }
}


