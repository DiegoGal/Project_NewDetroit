using UnityEngine;
using System.Collections;

public class OrcController : HeroeController
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


	//Colliders
	public GameObject cubeColliderHand;	// Hand
	
	//Time counts
	private float timeCountLife = 0;	// State recover

	// Transforms
	private Transform head;		// Head
	private Transform pelvis;	// Pelvis

	// Particles
	public GameObject snot;				// Skill 1
	public GameObject splash;			// Skill 2
	public GameObject sphereThirdSkill;	// Skill 3
	public GameObject smoke;			// Smoke


	//-----------------------------------------------------------------------------------------------------------------
	// PRIVATE    
	private void newLevel()
	{
		if (this.hasNewLevel) 
		{
            int maxLife = (int)life.maximunLife,
				maxAdren = cBasicAttributes.getMaximunAdren(),
				maxMana =cBasicAttributes.getMaximunMana();
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
	
	
	//-----------------------------------------------------------------------------------------------------------------
	// MAIN
	public virtual void Awake()
	{
		base.Awake ();

		//Set the type of heroe
		this.type = TypeHeroe.Orc;
	}

	// Use this for initialization
	public override void Start ()
	{
		base.Start ();
		this.attackP = ATT_P_1;
		this.attackM = ATT_M_1;
		this.speedAtt = ATT_SPEED_1;
		cBasicAttributes.setDeffensePhysic(DEF_P_1);
		cBasicAttributes.setDeffenseMagic(DEF_M_1);
		this.speedMov = MOV_SPEED_1;

		//Set the collider cubes in both hands
		//Right hand	
		Transform hand = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand");
		GameObject cubeColliderInst = (GameObject) PhotonNetwork.Instantiate(cubeColliderHand.name, hand.position + new Vector3(-0.25f, -0.5f, 0), hand.rotation, 0);
		cubeColliderInst.transform.parent = hand;
		cubeColliderInst.GetComponent<OrcBasicAttack> ().owner = this.gameObject;
		
		//Left hand
		hand = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand");
		cubeColliderInst = (GameObject) PhotonNetwork.Instantiate(cubeColliderHand.name, hand.position + new Vector3(0.25f, -0.5f, 0), hand.rotation, 0);
		cubeColliderInst.transform.parent = hand;
		cubeColliderInst.GetComponent<OrcBasicAttack> ().owner = this.gameObject;
		
		//Initializes the cooldowns of skills
		cooldown1total = COOLDOWN_SKILL_1; cooldown2total = COOLDOWN_SKILL_2; cooldown3total = COOLDOWN_SKILL_3;
		cooldown1 = COOLDOWN_SKILL_1; cooldown2 = COOLDOWN_SKILL_2; cooldown3 = COOLDOWN_SKILL_3;

		//Mana and adrenaline for skills
		manaSkill1 = 50; manaSkill2 = -1; manaSkill3 = -1;
		adrenSkill1 = -1; adrenSkill2 = 75; adrenSkill3 = 150;
		
		//Initialize the animation
		animation.Play ("Iddle01");

		head = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head");
		pelvis = transform.FindChild("Bip001/Bip001 Pelvis");

	}
	
	// Update is called once per frame
	public override void Update ()
	{
		base.Update ();
		updateManaAdren();
		UpdateAnimation();
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
					cState.animationName = "Burp";
					cState.animationChanged = true;
					//--------------------------
					StartCoroutine(FirstSkill(animation["Burp"].length * 0.1f));
					//--------------------------
					extraSpeed = false;
				}
				else if (stateAttackSecond == AttackSecond.Attack2 && cooldown2 == cooldown2total)
				{
					cState.animationName = "FloorHit";
					cState.animationChanged = true;
					//------------------------------
					StartCoroutine(SecondSkill(animation["FloorHit"].length * 0.25f));
					//--------------------------
					extraSpeed = false;
				}
				else if (stateAttackSecond == AttackSecond.Attack3 && cooldown3 == cooldown3total)
				{
					cState.animationName = "BullStrike";
					cState.animationChanged = true;
					//--------------------------------
					//Smoke
					StartCoroutine(SmokeParticles(0));
					//--------------------------------
					//Shpere
					StartCoroutine(ThirdSkill(animation["BullStrike"].length * 0.2f));
					//--------------------------
					extraSpeed = true;
				}
				//-----------------------------------
				canMove = false;
				canRotate = false;
				doingSecondaryAnim= true;
			}
			// Basic attack
			else if (state == StateHeroe.AttackBasic)
			{
				if (!animation.IsPlaying("Attack01") && !animation.IsPlaying("Attack02") && !animation.IsPlaying("Attack03"))
				{
					cState.animationName = "Attack01";
					cState.animationNameQueued = "Attack02";
					cState.animationNameQueued2 = "Attack03";
					cState.animationChanged = cState.animationChangeQueued = cState.animationChangeQueued2 = true;
				}
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
                this.life.currentLife = 0;
				this.transform.position = this.initialPosition;
				//------------------------------------
				state = StateHeroe.Recover;
				//------------------------------------
				isMine = false;
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
//                    this.life.currentLife += 20;
//                    if (this.life.currentLife >= this.life.maximunLife)
//					{
//                        this.life.currentLife = this.life.maximunLife;
//						this.state = StateHeroe.Idle;
//						isMine = true;
//					}
					isMine = life.HealAlly(20);
					if (isMine) state = StateHeroe.Idle;
				}
				//------------------------------------
				canRotate = false;
				canMove = false;
				extraSpeed = false;
			}
			// Idle
			else
			{
				if (!animation.IsPlaying("Iddle01") && !animation.IsPlaying("Iddle02"))
				{
					cState.animationName = "Iddle01";
					cState.animationNameQueued = "Iddle02";
					cState.animationChanged = cState.animationChangeQueued = true;
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
	}

	
	//--------------------------------------------------------------------------------------------
	//Corrutines	
	private IEnumerator FirstSkill(float time)
	{
		yield return new WaitForSeconds(time);

		GameObject snt = (GameObject) PhotonNetwork.Instantiate(snot.name, transform.localPosition + transform.forward * 2 + Vector3.up, transform.rotation, 0);
		SkillAttack sa = snt.GetComponent<SkillAttack>();
		sa.SetDamage(1);
		sa.setOwner(gameObject);

		yield return new WaitForSeconds(3f);

		PhotonNetwork.Destroy(snt);
	}

	private IEnumerator SecondSkill(float time)
	{
		yield return new WaitForSeconds(time);

		GameObject spl = (GameObject) PhotonNetwork.Instantiate(splash.name, transform.position + new Vector3(0, -1.3f, 0), Quaternion.identity, 0);
		SkillAttack sa = spl.GetComponent<SkillAttack>();
		sa.SetDamage(attackM + 40);
		sa.setOwner(gameObject);

		yield return new WaitForSeconds(animation["FloorHit"].length * 0.75f);
		
		PhotonNetwork.Destroy(spl);
	}

	private IEnumerator ThirdSkill(float time)
	{
		yield return new WaitForSeconds(time);

		GameObject sphereThirdSkillInst = (GameObject) PhotonNetwork.Instantiate(sphereThirdSkill.name, head.position, transform.rotation, 0);
		SkillAttack sa = sphereThirdSkillInst.GetComponent<SkillAttack>();
		sa.setOwner(gameObject);
		sa.SetDamage(attackP + 100);
		sphereThirdSkillInst.transform.parent = pelvis;

		yield return new WaitForSeconds(animation["BullStrike"].length * 0.8f);

		PhotonNetwork.Destroy(sphereThirdSkillInst);
	}

	private IEnumerator SmokeParticles(float time)
	{
		yield return new WaitForSeconds(time);

		GameObject smokeInst = (GameObject)PhotonNetwork.Instantiate(smoke.name, transform.localPosition + Vector3.down*2, transform.rotation, 0);
		smokeInst.transform.parent = pelvis;

		yield return new WaitForSeconds(5f);
		
		PhotonNetwork.Destroy(smokeInst);
	}
}

