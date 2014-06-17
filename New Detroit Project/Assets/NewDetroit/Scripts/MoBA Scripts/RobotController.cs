using UnityEngine;
using System.Collections;

public class RobotController : HeroeController
{
	public const int 		LIFE_1 = 		375, 	LIFE_2 = 		550, 	LIFE_3 = 		725, 	LIFE_4 = 		900,
							ATT_P_1 = 		30, 	ATT_P_2 = 		45, 	ATT_P_3 = 		60, 	ATT_P_4 = 		75,
							ATT_M_1 = 		25, 	ATT_M_2 = 		35, 	ATT_M_3 = 		45, 	ATT_M_4 = 		55;
	public const double 	ATT_SPEED_1 = 	0.9, 	ATT_SPEED_2 = 	1, 		ATT_SPEED_3 = 	1.1, 	ATT_SPEED_4 = 	1.2;
	public const int 		DEF_P_1 = 		25, 	DEF_P_2 = 		30, 	DEF_P_3 = 		35, 	DEF_P_4 = 		40,
							DEF_M_1 = 		20, 	DEF_M_2 = 		25, 	DEF_M_3 = 		30, 	DEF_M_4 = 		35,
							MANA_1 = 		175, 	MANA_2 = 		250, 	MANA_3 = 		350, 	MANA_4 = 		500,
							ADREN_1 = 		150, 	ADREN_2 = 		250, 	ADREN_3 = 		350, 	ADREN_4 = 		450,
							MOV_SPEED_1 = 	50, 	MOV_SPEED_2 = 	55, 	MOV_SPEED_3 = 	60, 	MOV_SPEED_4 = 	35;

	public const float 		COOLDOWN_SKILL_1 = 	5,	COOLDOWN_SKILL_2 = 	10,	COOLDOWN_SKILL_3 = 	20;
	//-----------------------------------------------------------------------------------------------------------------


	// Particles
	public GameObject fireBall;		// Skill 1
	public GameObject skelterTurn;	// Skill 2
	public GameObject skelterShot;	// Skill 3

	// Instances
	private GameObject fireCircleInst;	// Skill 1
	private GameObject turnInst;		// Skill 2
	private GameObject fireShotInst;	// Skill 3

	// Colliders
	public GameObject cubeColliderSword;	// Sword

	// Transforms
	private Transform gun;	// Gun
	private Transform head;	// Head

	// Time CD
	private float timeCountLife = 0;	//State recover

	// Flags
	private bool doneFirstSkill = false;	// Skill 1
	private bool isShot = false;			// Skill 3


	//-----------------------------------------------------------------------------------------------------------------


	// PRIVATE    
	private void newLevel()
	{
		if (this.hasNewLevel) 
		{
			int maxLife = (int)life.maximunLife,
			maxAdren = cBasicAttributes.getMaximunAdren(),
			maxMana = cBasicAttributes.getMaximunMana();
			switch (cBasicAttributes.getLevel())
			{
			case 2:
				this.life.maximunLife = LIFE_2;
				this.attackP = ATT_P_2;
				this.attackM = ATT_M_2;
				this.speedAtt = ATT_SPEED_2;
				cBasicAttributes.setDeffensePhysic(DEF_P_2);
				cBasicAttributes.setDeffenseMagic(DEF_M_2);
				cBasicAttributes.setMaximunMana(MANA_2);
				cBasicAttributes.setMaximunAdren(ADREN_2);
				this.speedMov = MOV_SPEED_2;
				break;
			case 3:
				this.life.maximunLife = LIFE_3;
				this.attackP = ATT_P_3;
				this.attackM = ATT_M_3;
				this.speedAtt = ATT_SPEED_3;
				cBasicAttributes.setDeffensePhysic(DEF_P_3);
				cBasicAttributes.setDeffenseMagic(DEF_M_3);
				cBasicAttributes.setMaximunMana(MANA_3);
				cBasicAttributes.setMaximunAdren(ADREN_3);
				this.speedMov = MOV_SPEED_3;
				break;
			case 4:
				this.life.maximunLife = LIFE_4;
				this.attackP = ATT_P_4;
				this.attackM = ATT_M_4;
				this.speedAtt = ATT_SPEED_4;
				cBasicAttributes.setDeffensePhysic(DEF_P_4);
				cBasicAttributes.setDeffenseMagic(DEF_M_4);
				cBasicAttributes.setMaximunMana(MANA_4);
				cBasicAttributes.setMaximunAdren(ADREN_4);
				this.speedMov = MOV_SPEED_4;
				break;
			}//end switch (level)
			
			float percentage = (float)(life.maximunLife - maxLife) / maxLife;
			life.currentLife = (1 + percentage) * life.currentLife;
			
			percentage = (float)(cBasicAttributes.getMaximunAdren() - maxAdren) / maxAdren;
			cBasicAttributes.setCurrentAdren((int) ((1 + percentage) * cBasicAttributes.getCurrentAdren()));
			
			percentage = (float)(cBasicAttributes.getMaximunMana() - maxMana) / maxMana;
			cBasicAttributes.setCurrentMana((int) ((1 + percentage) * cBasicAttributes.getCurrentMana()));
			
			hasNewLevel = false;
		}
	}
	
	//================================
	//=====     Main methods     =====
	//================================

	public virtual void Awake()
	{
		base.Awake();

		//Set the type of heroe
		this.type = TypeHeroe.Robot;
	}
	
	// Use this for initialization
	public override void Start ()
	{
		base.Start ();
		
		this.life.currentLife = LIFE_1;
//		cBasicAttributes.setCurrentMana(MANA_1);
//		cBasicAttributes.setCurrentAdren(ADREN_1);
		
		this.life.maximunLife = LIFE_1;
		this.attackP = ATT_P_1;
		this.attackM = ATT_M_1;
		this.speedAtt = ATT_SPEED_1;
		cBasicAttributes.setDeffensePhysic(DEF_P_1);
		cBasicAttributes.setDeffenseMagic(DEF_M_1);
//		cBasicAttributes.setMaximunMana(MANA_1);
//		cBasicAttributes.setMaximunAdren(ADREN_1);
		this.speedMov = MOV_SPEED_1;

		//Set the collider cubes in the sword
		Transform sword = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Cylinder002/cuchilla");
		GameObject cubeColliderInst = (GameObject) PhotonNetwork.Instantiate(cubeColliderSword.name, sword.position + new Vector3(0.4f, 1, 0.1f), sword.rotation, 0);
		cubeColliderInst.transform.parent = sword;
		cubeColliderInst.GetComponent<RobotBasicAttack> ().owner = this.gameObject;

		//Initializes the cooldowns of skills
		cooldown1total = COOLDOWN_SKILL_1; cooldown2total = COOLDOWN_SKILL_2; cooldown3total = COOLDOWN_SKILL_3;
		cooldown1 = COOLDOWN_SKILL_1; cooldown2 = COOLDOWN_SKILL_2; cooldown3 = COOLDOWN_SKILL_3;
		
		//Mana and adrenaline for skills
		manaSkill1 = 50; manaSkill2 = -1; manaSkill3 = -1;
		adrenSkill1 = -1; adrenSkill2 = 75; adrenSkill3 = 150;
		
		//Initialize the animation
		animation.Play ("Idle01");

		gun = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Cylinder002/cuchilla");
		head = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head");
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		base.Update ();
		updateManaAdren();
		UpdateAnimation();
		Counter();
		this.newLevel();
	}

	//--------------------------------------------------------------------------------------------
	//Animation
	// Only can do an action if hero don't do a secondary attack
    private void UpdateAnimation()
    {
        if (!doingSecondaryAnim)
        {
            // Secondary attack
            if (state == StateHeroe.AttackSecond)
            {
				if (stateAttackSecond == AttackSecond.Attack1 && cooldown1 == cooldown1total && !doneFirstSkill)
                {
					StartCoroutine(FirstSkill(0));
					//------------------------------------
					canRotate = true;
					canMove = true;
					doingSecondaryAnim = false;
					doneFirstSkill = true;
                }
                else if (stateAttackSecond == AttackSecond.Attack2 && cooldown2 == cooldown2total)
                {
					cState.animationName = "Attack2";
					cState.animationChanged = true;
                    //----------------------------
					StartCoroutine(SecondSkill(0));
					//------------------------------------
					canRotate = false;
					canMove = false;
					doingSecondaryAnim = true;
                }
                else if (stateAttackSecond == AttackSecond.Attack3 && cooldown3 == cooldown3total)
                {
					cState.animationName = "Attack3";
					cState.animationChanged = true;
                    //----------------------------
					StartCoroutine(ThirdSkill(0));
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
				// Do the animation
                if (!animation.IsPlaying("Attack1") && !animation.IsPlaying("Attack2") && !animation.IsPlaying("Attack3"))
                {
					cState.animationName = "Attack1";
					cState.animationNameQueued = "Attack2";
					cState.animationNameQueued2 = "Attack3";
					cState.animationChanged = true;
					cState.animationChangeQueued = true;
					cState.animationChangeQueued2 = true;
                }
				// Instantiate the shot
//				if (animation.IsPlaying("Attack3") && !isShot)
//                {
//					isShot = true;
//					StartCoroutine(ThirdBasicAttack(0));
//                }
				//------------------------------------
				canRotate = false;
				canMove = false;
				extraSpeed = false;
            }
            // Movement
            else if (state == StateHeroe.Run)
            {
				cState.animationName = "Run";
				cState.animationChanged = true;
				//------------------------------------
				canRotate = true;
				canMove = true;
				extraSpeed = false;
            }
            else if (state == StateHeroe.Walk)
            {
				cState.animationName = "Walk";
				cState.animationChanged = true;
				//------------------------------------
				canRotate = true;
				canMove = true;
				extraSpeed = false;
            }
            else if (state == StateHeroe.Dead)
            {
				cState.animationName = "Die";
				cState.animationChanged = true;
				//------------------------------------
                this.life.currentLife = 0;
                this.transform.position = this.initialPosition;
                isMine = false;
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
					cState.animationName = "Idle01";
					cState.animationChanged = true;
                }
				//------------------------------------
				canRotate = false;
				canMove = false;
				extraSpeed = false;
            }
        }
		//not secondary attack
        else
        {
			if (!animation.isPlaying)
            {
                doingSecondaryAnim = false;
            }
        }
    }//UpdateAnimation

	//-----------------------------------------------------------------------
	// Corutines
	private IEnumerator FirstSkill(float time)
	{
		yield return new WaitForSeconds(time);

		fireCircleInst = (GameObject)PhotonNetwork.Instantiate(fireBall.name, head.position + Vector3.down * 0.5f, transform.rotation, 0);
		RobotCircleSkill rc = fireCircleInst.GetComponent<RobotCircleSkill>();
		rc.setOwner(gameObject);
		rc.UpDef();
		fireCircleInst.transform.parent = head;

		yield return new WaitForSeconds(1.7f);

		rc.DownDef();
		PhotonNetwork.Destroy(fireCircleInst);

		doneFirstSkill = false;
	}

	private IEnumerator SecondSkill(float time)
	{
		yield return new WaitForSeconds(time);
		
		turnInst = (GameObject)PhotonNetwork.Instantiate(skelterTurn.name, transform.position + Vector3.up, transform.rotation, 0);
		RobotTurn rt = turnInst.GetComponent<RobotTurn>();
		rt.SetDamage(75);
		rt.setOwner(gameObject);
		rt.setTimeToTurn(1f);

		yield return new WaitForSeconds(5f);

		PhotonNetwork.Destroy(turnInst);
	}

	private IEnumerator ThirdSkill(float time)
	{
		yield return new WaitForSeconds(time);
		
		if (fireShotInst != null)
			PhotonNetwork.Destroy(fireShotInst);
		skelterShot.GetComponent<MeshRenderer>().enabled = false;
		fireShotInst = (GameObject)PhotonNetwork.Instantiate(skelterShot.name, gun.position, transform.rotation, 0);
		RobotShot rs = fireShotInst.GetComponent<RobotShot>();
		rs.SetDamage(75);
		rs.setOwner(gameObject);
		rs.setSpeed(2);
		rs.setDirection(transform.forward);
		rs.setTimeToShot(1.7f);
		fireShotInst.transform.parent = gun;

		yield return new WaitForSeconds(2.5f);
		
		PhotonNetwork.Destroy(fireShotInst);
	}

	private IEnumerator ThirdBasicAttack(float time)
	{
		yield return new WaitForSeconds(time);

		if (fireShotInst != null)
			PhotonNetwork.Destroy(fireShotInst);
		skelterShot.GetComponent<MeshRenderer>().enabled = false;
		fireShotInst = (GameObject)PhotonNetwork.Instantiate(skelterShot.name, gun.position, transform.rotation, 0);
		RobotShot rs = fireShotInst.GetComponent<RobotShot>();
		rs.SetDamage(75);
		rs.setOwner(gameObject);
		rs.setSpeed(2);
		rs.setDirection(transform.forward);
		rs.setTimeToShot(1.7f);
		fireShotInst.transform.parent = gun;

		yield return new WaitForSeconds(animation["Attack3"].length);

		PhotonNetwork.Destroy(fireShotInst);

		isShot = false;
	}
}


