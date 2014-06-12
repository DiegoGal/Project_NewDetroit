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


	protected const float 	COOLDOWN_SKILL_1 = 	5,	COOLDOWN_SKILL_2 = 	10,	COOLDOWN_SKILL_3 = 	20;
	//-----------------------------------------------------------------------------------------------------------------


	//public GameObject leftArm, rightArm;
	public GameObject cubeColliderHand;
	//-----------------------------------------------------------------------------------------------------------------
	
	//Time counts
	private float timeCountLife = 0;
	
	private Transform head;
	//-----------------------------------------------------------------------------------------------------------------


	// PARTICLES
	// Snot particle
	public GameObject snot; 
	private bool snotActivated = false;
	private float snotCD = 1.7f;
	// Splash particle
	private GameObject spl;
	public GameObject splash; 
	private bool splashActivated = false;
	private float splashCD = 1.7f;
	// Smoke particle
	public GameObject smoke; 
	private bool smokeActivated = false;
	private float smokeCD = 1.7f;
	private GameObject smokeInst; // Smoke instantiation
	// BullStrike particle
	private bool bullStrikeActivated = false;
	// FireCircle Particle
	public GameObject fireCircle;
	private GameObject fireCircleInst;
	private bool fireCircleActivated; 
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
	
	
	//-----------------------------------------------------------------------------------------------------------------
	// MAIN
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
		
		//Set the type of heroe
		this.type = TypeHeroe.Orc;

		//Set the collider cubes in both hands
		//Right hand
		Transform hand = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand");
		GameObject cubeColliderInst = (GameObject) Instantiate(cubeColliderHand, hand.position + new Vector3(-0.25f, -0.5f, 0), hand.rotation);
		cubeColliderInst.transform.parent = hand;
		cubeColliderInst.GetComponent<OrcBasicAttack> ().owner = this.gameObject;

		//Left hand
		hand = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand");
		cubeColliderInst = (GameObject) Instantiate(cubeColliderHand, hand.position + new Vector3(0.25f, -0.5f, 0), hand.rotation);
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

		if (head == null)
			head = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head");

	}
	
	// Update is called once per frame
	public override void Update ()
	{
		base.Update ();
		updateManaAdren();
		UpdateAnimation();
		UpdateParticles (); // Update particles
		Counter();
		this.newLevel ();
	}
	
	// Cool Down for detecting less time the collision with particles
	private float CDParticleCollision; 
	//This is for the particles that collides with the orc
	void OnParticleCollision(GameObject other)
	{
		// get the particle system
		ParticleSystem particleSystem;
		particleSystem = other.GetComponent<ParticleSystem>();
		//If the particle is a Moco    
		if (particleSystem.tag == "Moco")
		{
			if (CDParticleCollision > 0)
				CDParticleCollision -= Time.deltaTime;
			else
			{
				life.Damage(particleSystem.GetComponent<ParticleDamage>().GetDamage(), 'M');
				CDParticleCollision = 0.1f; // 5 deltatime aprox
			}
		}
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
					transform.Translate(Vector3.forward * 2 + Vector3.up);
					GameObject snt = (GameObject)Instantiate(snot, transform.localPosition, transform.rotation);
					snt.GetComponent<ParticleDamage>().SetDamage(attackM);
					transform.Translate(Vector3.back * 2 + Vector3.down);
					Destroy(snt, 5f);
					snotActivated = true;
					//--------------------------
					extraSpeed = false;
				}
				else if (stateAttackSecond == AttackSecond.Attack2 && cooldown2 == cooldown2total)
				{
					cState.animationName = "FloorHit";
					cState.animationChanged = true;
					//------------------------------
					StartCoroutine(SecondSkilll(animation["FloorHit"].length * 0.25f));
					//--------------------------
					extraSpeed = false;
				}
				else if (stateAttackSecond == AttackSecond.Attack3 && cooldown3 == cooldown3total)
				{
					cState.animationName = "BullStrike";
					cState.animationChanged = true;
					//--------------------------------
					smokeInst = (GameObject)Instantiate(smoke, transform.localPosition, transform.rotation);
					Destroy(smokeInst, 5f);
					smokeActivated = true;

					fireCircleInst = (GameObject)Instantiate(fireCircle, head.position, transform.rotation);
					fireCircleInst.transform.parent = head;
					Destroy(fireCircleInst, 1.7f);
					//--------------------------------
					GetComponent<OrcBullStrikeAttack>().EnableSphereCollider();
					GetComponent<OrcBullStrikeAttack>().SetDamage(attackP + 100);
					this.gameObject.AddComponent<Rigidbody>();
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
				if (stateAttackSecond == AttackSecond.Attack3) GetComponent<OrcBullStrikeAttack>().DisableSphereCollider();
			}
		}
	}

	
	//--------------------------------------------------------------------------------------------
	//Particles
	private void UpdateParticles()
	{
		if (snotActivated)
		{
			if (snotCD <= 0)
			{
				snotCD = 1.7f;
				snotActivated = false;
			}
			else snotCD -= Time.deltaTime;
		}
		
		if (splashActivated)
		{
			if (splashCD <= 0)
			{
				splashCD = 1.7f;
				splashActivated = false;
			}
			else splashCD -= Time.deltaTime;
		}	
		
		if (smokeActivated)
		{
			if (smokeInst!=null)
			{
				smokeInst.transform.position= transform.position;
				smokeInst.transform.Translate(Vector3.down*2);
			}
			if (smokeCD <= 0)
			{
				smokeCD = 1.7f;
				smokeActivated = false;
			}
			else smokeCD -= Time.deltaTime;
		}

		if (bullStrikeActivated)
		{

		}
	}

	//Corrutines
	private IEnumerator SecondSkilll(float time)
	{
		yield return new WaitForSeconds(time);

		spl = (GameObject)Instantiate(splash, transform.position + new Vector3(0, -1.3f, 0), Quaternion.identity);
    	spl.AddComponent<Rigidbody>();
    	spl.GetComponent<Rigidbody>().useGravity = false;
		spl.GetComponent<OrcSplashAttack>().SetDamage(attackM + 40);
		spl.GetComponent<OrcSplashAttack>().setOwner(gameObject);
		Destroy(spl, 1.5f);
		splashActivated = true;
	}
}

