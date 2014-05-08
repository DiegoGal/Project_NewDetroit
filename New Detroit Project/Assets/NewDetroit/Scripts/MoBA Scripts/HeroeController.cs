﻿using UnityEngine;
using System.Collections;

public abstract class HeroeController : ControllableCharacter 
{
	protected const int EXP_LEVEL_1_2 = 		200, 		EXP_LEVEL_2_3 = 	600, 	EXP_LEVEL_3_4 = 	1000;
	protected const int COLDOWN_LEVEL_1 = 		20, 		COLDOWN_LEVEL_2 = 	30, 	COLDOWN_LEVEL_3 = 	40, 	COLDOWN_LEVEL_4 = 50;
	protected const int EXPERIENCE_TO_GIVE = 	100;
	
	
	//------------------------------------------------------------------------------------------------------
	public enum StateHeroe // The state of the heroe
	{
		Recover,		// After dead, he must be recovered
		Idle,			// When is rest
		Walk,			// When is walking
		Run,			// When is running
		AttackBasic,	// When he is attacking with his basic attack
		AttackSecond,	// When he is attacking with his secondary attack
		Dead			// When is dead
	}
	
	public enum AttackSecond // The state of secondary attack
	{
		None,
		Attack1,
		Attack2,
		Attack3,
	}
	
	public enum TypeHeroe // The type of the heroe
	{
		Orc
	}
	

	//---------------------------------------------------------------------------------------------
	// CONTROL HERO
	public bool isControllable = true;
	private Vector3 moveDirection = Vector3.zero; // The current move direction in x-z
	private float moveSpeed = 0.0f; // The current x-z move speed
	private float verticalSpeed = 0.0f; // The current vertical speed
	private Vector3 inAirVelocity = Vector3.zero;
	private CollisionFlags collisionFlags; // The last collision flags returned from controller.Move
	private Vector3 velocity = Vector3.zero;
	private Vector3 lastPos;
	private float lastGroundedTime = 0.0f;
	private bool movingBack = false; // Are we moving backwards (This locks the camera to not do a 180 degree spin)
	private bool isMoving = false; // Is the user pressing any keys?
	private float lockCameraTimer = 0.0f; // The camera doesnt start following the target immediately but waits for a split second to avoid too much waving around.
	private float rotateSpeed = 500.0f;
	private float speedSmoothing = 10.0f;
	private float extraRunSpeed = 20.0f;
	private float runSpeed = 12.0f; // when pressing "shift left" button (cmd) we start running
	private float walkSpeed = 6.0f; // The speed when walking
	private float walkTimeStart = 0.0f; // When did the user start walking (Used for going into trot after a while)
	private float inAirControlAcceleration = 3.0f;
	public float gravity = 20.0f; // The gravity for the character
	private bool jumping = false; // Are we jumping? (Initiated with jump button and not grounded yet)
	public float cooldown1, cooldown2, cooldown3, cooldown1total, cooldown2total, cooldown3total;
	protected bool doingSecondaryAnim = false; // A flag to tell us if the orc is doing a secondary attack anim or not
	//---------------------------------------------------------------------------------------------
	public Texture2D 	textureLifePositive, textureLifeNegative,
						textureAdrenPositive, textureAdrenNegative,
						textureManaPositive, textureManaNegative,
						textureBackground;
	public int 		attackP, 
					attackM, 
					defP, 
					defM, 
					mana, 
					adren, 
					speedMov, 
					level, 
					experience, 
					currentMana, 
					currentAdren;
	public double 	speedAtt;
	public bool attackInstantiate;	// Activate the spheres of arms
	public TypeHeroe type;	// Type of heroe
	public StateHeroe state = StateHeroe.Idle; // The state of the heroe
	public AttackSecond stateAttackSecond;	// The state of secondary attack
	public bool ability1, 
				ability2, 
				ability3;
	protected int manaSkill1, manaSkill2, manaSkill3, adrenSkill1, adrenSkill2, adrenSkill3; // mana and adrenalines for skills
	protected bool hasNewLevel; // Tell us if the heroe has evolved  or not
	protected Vector3 initialPosition; // The spawn position
	// Time counters
	private float timeCountMana = 0, timeCountAdren = 0;
	public int counterAbility;
    //This is for the particles that collides with the hero
    private ParticleSystem.CollisionEvent[] collisionEvents = new ParticleSystem.CollisionEvent[16];
	// ------------------------------------------------------------------------------------------------------
	// GUI
	private Rect 	rectanglePositiveLife,
					rectangleNegativeLife,
					rectanglePositiveAdren,
					rectangleNegativeAdren,
					rectanglePositiveMana,
					rectangleNegativeMana,
					rectangleLevel;
	
	// ------------------------------------------------------------------------------------------------------
	// PRIVATE	
	// Increment the level
	protected void levelUp() 
	{
		level ++;
		hasNewLevel = true;
		counterAbility ++;
	}
	
	// Check if we unlock some abilitie
	protected void unlockAbilities()
	{
		if (counterAbility > 0)
		{
			if (!ability3 && level == 4)
			{
				ability3 = true;
				counterAbility --;
			}
			else if (!ability1 && Input.GetKey(KeyCode.Alpha1))
			{
				ability1 = true;
				counterAbility --;
			}
			else if (!ability2 && Input.GetKey(KeyCode.Alpha2))
			{
				ability2 = true;
				counterAbility --;
			}
		}
	}
	// ------------------------------------------------------------------------------------------------------
	// GUI
	private void GUIRects()
	{
		float 	distance = Vector3.Distance (transform.position, Camera.main.transform.position), // real distance from camera
				lengthLifeAdrenMana = this.GetComponent<ThirdPersonCamera> ().distance / distance, // percentage of the distance
				widthAll = Screen.width / 10,
				widthHalf = widthAll / 2,
				positiveLife = (float) this.life.currentLife / this.life.maximunLife, // percentage of positive life
				positiveAdren = (float) this.currentAdren / this.adren, // percentage of positive adrenaline
				positiveMana = (float) this.currentMana / this.mana; // percentage of positive mana
		// Life
		Vector3 posScene = new Vector3 (transform.position.x, transform.position.y + 2f, transform.position.z),
				posSceneEnd = new Vector3 (transform.position.x, transform.position.y + 1.8f, transform.position.z),
				pos = Camera.main.WorldToScreenPoint (posScene),
				posEnd = Camera.main.WorldToScreenPoint (posSceneEnd);

		float 	x = pos.x - widthHalf * lengthLifeAdrenMana,
				y = Screen.height - pos.y,
				width = widthAll * positiveLife * lengthLifeAdrenMana,
				height = (pos.y - posEnd.y) * lengthLifeAdrenMana;	
		rectanglePositiveLife = new Rect (x, y, width, height);

		x = pos.x - widthHalf * lengthLifeAdrenMana + widthAll * positiveLife * lengthLifeAdrenMana;
		width = widthAll * (1 - positiveLife) * lengthLifeAdrenMana;
		rectangleNegativeLife = new Rect (x, y, width, height);
		// Adrenaline
		posScene = new Vector3 (transform.position.x, transform.position.y + 1.78f, transform.position.z);
		posSceneEnd = new Vector3 (transform.position.x, transform.position.y + 1.68f, transform.position.z);
		pos = Camera.main.WorldToScreenPoint (posScene);
		posEnd = Camera.main.WorldToScreenPoint (posSceneEnd);

		x = pos.x - widthHalf * lengthLifeAdrenMana;
		y = Screen.height - pos.y;
		width = widthAll * positiveAdren * lengthLifeAdrenMana;
		height = (pos.y - posEnd.y) * lengthLifeAdrenMana;
		rectanglePositiveAdren = new Rect (x, y, width, height);

		x = pos.x - widthHalf * lengthLifeAdrenMana + widthAll * positiveAdren * lengthLifeAdrenMana;
		width = widthAll * (1 - positiveAdren) * lengthLifeAdrenMana;
		rectangleNegativeAdren = new Rect (x, y, width, height);
		// Mana
		posScene = new Vector3 (transform.position.x, transform.position.y + 1.66f, transform.position.z);
		posSceneEnd = new Vector3 (transform.position.x, transform.position.y + 1.56f, transform.position.z);
		pos = Camera.main.WorldToScreenPoint (posScene);
		posEnd = Camera.main.WorldToScreenPoint (posSceneEnd);

		x = pos.x - widthHalf * lengthLifeAdrenMana;
		y = Screen.height - pos.y;
		width = widthAll * positiveMana * lengthLifeAdrenMana;
		height = (pos.y - posEnd.y) * lengthLifeAdrenMana;
		rectanglePositiveMana = new Rect (x, y, width, height);

		x = pos.x - widthHalf * lengthLifeAdrenMana + widthAll * positiveMana * lengthLifeAdrenMana;
		width = widthAll * (1 - positiveMana) * lengthLifeAdrenMana;
		rectangleNegativeMana = new Rect (x, y, width, height);
		// Level
		posScene = new Vector3 (transform.position.x, transform.position.y + 2f, transform.position.z);
		posSceneEnd = new Vector3 (transform.position.x, transform.position.y + 1.56f, transform.position.z);
		pos = Camera.main.WorldToScreenPoint (posScene);
		posEnd = Camera.main.WorldToScreenPoint (posSceneEnd);

		x = pos.x - (widthHalf + 22) * lengthLifeAdrenMana;
		y = rectanglePositiveLife.y;
		width = 20 * lengthLifeAdrenMana;
		height = (rectanglePositiveMana.y - rectanglePositiveLife.y) + rectanglePositiveMana.height;
		rectangleLevel = new Rect (x, y, width, height);
	}


	// ------------------------------------------------------------------------------------------------------
	// PUBLIC
    // if type == 'P' is phisical damage if type == 'M' is magical damage
	public override bool Damage (float damage,char type)
	{
        if (type == 'P')
		    life.currentLife -= damage - defP;
        else
            if (type == 'M')
                life.currentLife -= damage - defM;
        return (life.currentLife <= 0);
	}
	
	// Increment the experience
	public void experienceUp(int experience)
	{
		this.experience += experience;
		Mathf.Min (this.experience, EXP_LEVEL_3_4);
		
		if (this.level == 1 && this.experience >= EXP_LEVEL_1_2) this.levelUp ();
		else if (this.level == 2 && this.experience >= EXP_LEVEL_2_3) this.levelUp ();
		else if (this.level == 3 && this.experience >= EXP_LEVEL_3_4) this.levelUp ();
	}
	
	
	// ------------------------------------------------------------------------------------------------------
	// MAIN
	// Use this for initialization
	virtual public void Start () {

        base.Start();

        radius = transform.FindChild("Pivot").GetComponent<NavMeshObstacle>().radius * transform.localScale.x;

		this.level = 1;
		this.experience = 0;
		this.hasNewLevel = false;
		this.attackInstantiate = true;
		this.initialPosition = transform.position;	// Set the initial position
		this.experienceGived = EXPERIENCE_TO_GIVE;	// Experience that the heroe gives when he dies
		// Initialize the booleans of abilities
		ability1 = ability2 = ability3 = true;
		counterAbility = 0;
		// Initialize the animation
		state = StateHeroe.Idle;				// Set the initial state of the hero
		stateAttackSecond = AttackSecond.None;		// Set the initial state of secondary attack of hero
	}//Start
	
	// Update is called once per frame
	virtual public void Update () {
        base.Update();

		// Unlock abilities
		if (counterAbility > 0)
		{
			if (!ability3 && level == 4)
			{
				ability3 = true;
				counterAbility --;
			}
			else if (!ability1 && Input.GetKey(KeyCode.Alpha1))
			{
				ability1 = true;
				counterAbility --;
			}
			else if (!ability2 && Input.GetKey(KeyCode.Alpha2))
			{
				ability2 = true;
				counterAbility --;
			}
		}

		UpdateControl (); // Update control
		UpdateState (false, false, false); // Update state
		// GUI
		GUIRects();
	}//Update

    void OnGUI ()
	{
		// Debug
		Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
		GUI.Label(new Rect(pos.x + 20, Screen.height - pos.y, 200, 20), "State: " + state.ToString());
		//---------------------------------------------------------------
        GUI.DrawTexture(rectanglePositiveLife, textureLifePositive);
        GUI.DrawTexture(rectangleNegativeLife, textureLifeNegative);
        GUI.DrawTexture(rectanglePositiveAdren, textureAdrenPositive);
        GUI.DrawTexture(rectangleNegativeAdren, textureAdrenNegative);
        GUI.DrawTexture(rectanglePositiveMana, textureManaPositive);
        GUI.DrawTexture(rectangleNegativeMana, textureManaNegative);
        GUI.DrawTexture(rectangleLevel, textureBackground);

        FontStyle fs = GUI.skin.label.fontStyle;
        TextAnchor ta = GUI.skin.label.alignment;
        GUI.skin.label.fontStyle = FontStyle.Bold;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUI.Label(rectangleLevel, "" + level);
        GUI.skin.label.fontStyle = fs;
        GUI.skin.label.alignment = ta;
	}


	//----------------------------------------------------------------------------------------------------------------------------------------
	// CONTROL HERO
	protected void UpdateControl()
	{
		if (isMine)
		{			
			// Update the movement direction
			UpdateSmoothedMovementDirection();	
			
			// Apply gravity
			ApplyGravity();		
			
			// Calculate actual motion
			Vector3 movement = moveDirection * moveSpeed + new Vector3(0, verticalSpeed, 0) + inAirVelocity;
			movement *= Time.deltaTime;
			
			// Move the controller
			CharacterController controller = GetComponent<CharacterController>();
			collisionFlags = controller.Move(movement);
		}
		velocity = (transform.position - lastPos)*25;
		
		// Set rotation to the move direction
		if (IsGrounded()) transform.rotation = Quaternion.LookRotation(moveDirection);
		
		// We are in jump mode but just became grounded
		if (IsGrounded())
		{
			lastGroundedTime = Time.time;
			inAirVelocity = Vector3.zero;
		}	
		
		lastPos = transform.position;
	}
	
	protected void UpdateSmoothedMovementDirection()
	{
		Transform cameraTransform = Camera.main.transform;
		bool grounded = IsGrounded();
		
		// Forward vector relative to the camera along the x-z plane	
		Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;
		
		// Right vector relative to the camera
		// Always orthogonal to the forward vector
		Vector3 right = new Vector3(forward.z, 0, -forward.x);
		
		float v = Input.GetAxisRaw("Vertical");
		float h = Input.GetAxisRaw("Horizontal");
		
		// Are we moving backwards or looking backwards
		if (v < -0.2f) movingBack = true;
		else movingBack = false;
		
		bool wasMoving = isMoving;
		isMoving = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;
		
		// Target direction relative to the camera
		Vector3 targetDirection = h * right + v * forward;
		
		// Grounded controls
		if (grounded)
		{
			// Lock camera for short period when transitioning moving & standing still
			lockCameraTimer += Time.deltaTime;
			if (isMoving != wasMoving) lockCameraTimer = 0.0f;
			
			// We store speed and direction seperately,
			// so that when the character stands still we still have a valid forward direction
			// moveDirection is always normalized, and we only update it if there is user input.
			bool isOrcUsingAbility = state == HeroeController.StateHeroe.AttackSecond;
            if (targetDirection != Vector3.zero && !isOrcUsingAbility)
			{
				//moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
				moveDirection = targetDirection;
				moveDirection = moveDirection.normalized;
			}
			
			// Smooth the speed based on the current target direction
			float curSmooth = speedSmoothing * Time.deltaTime;
			
			// Choose target speed
			//* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
			float targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);
			
			// Pick speed modifier
			if (animation.IsPlaying("BullStrike")) targetSpeed = extraRunSpeed;
            else if (!animation.IsPlaying("FloorHit") && !animation.IsPlaying("Burp") && state != StateHeroe.AttackBasic &&
            			(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W)))
			{
				if (Input.GetKey(KeyCode.LeftShift)) targetSpeed = runSpeed;
				else targetSpeed = walkSpeed;
			}
			else targetSpeed = 0;
			
			moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);
			
			// Reset walk time start when we slow down
			if (moveSpeed < walkSpeed * 0.3f)
				walkTimeStart = Time.time;
		}
		// In air controls
		else if (isMoving) inAirVelocity += targetDirection.normalized * Time.deltaTime * inAirControlAcceleration;
	}
	
	protected  void ApplyGravity()
	{
		if (isControllable)	// don't move player at all if not controllable.
		{			
			if (IsGrounded()) verticalSpeed = 0.0f;
			else verticalSpeed -= gravity * Time.deltaTime;
		}
	}
	
	protected bool IsGrounded()
	{
		return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
	}
	
	public float GetLockCameraTimer()
	{
		return lockCameraTimer;
	}
	
	public bool IsMovingBackwards()
	{
		return movingBack;
	}
	
	public bool IsJumping()
	{
		return jumping;
	}
	//----------------------------------------------------------------------------------------------------------------------------------------
	// STATE 
	public void UpdateState(bool useSkill1, bool useSkill2, bool useSkill3)
	{
        if (isMine)
        {
            // Only can do an action if hero don't do a secondary attack
			if (!doingSecondaryAnim)			
            {
                // Secondary attack
                if (
					(Input.GetKey(KeyCode.Alpha1) || useSkill1) && ability1 && cooldown1 == cooldown1total &&
				    ((manaSkill1 != -1 && currentMana >= manaSkill1) || (adrenSkill1 != -1 && currentAdren >= adrenSkill1))
				    )
                {
                    state = StateHeroe.AttackSecond;
                    stateAttackSecond = AttackSecond.Attack1;
                }
                else if (
					(Input.GetKey(KeyCode.Alpha2) || useSkill2) && ability2 && cooldown2 == cooldown2total &&
					((manaSkill2 != -1 && currentMana >= manaSkill2) || (adrenSkill2 != -1 && currentAdren >= adrenSkill2))
					)
                {
                    state = StateHeroe.AttackSecond;
                    stateAttackSecond = AttackSecond.Attack2;
                }
                else if (
					(Input.GetKey(KeyCode.Alpha3) || useSkill3) && ability3 && cooldown3 == cooldown3total &&
					((manaSkill3 != -1 && currentMana >= manaSkill3) || (adrenSkill3 != -1 && currentAdren >= adrenSkill3))
					)
                {
                    state = StateHeroe.AttackSecond;
                    stateAttackSecond = AttackSecond.Attack3;
                }
                // Basic attack
                //else if (Input.GetMouseButton(0))
				else if (Input.GetButton("Fire1"))
                {
                    state = StateHeroe.AttackBasic;
                    stateAttackSecond = AttackSecond.None;
                }
                // Movement
                else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        state = StateHeroe.Run;
                    }
                    else
                    {
                        state = StateHeroe.Walk;
                    }
                    stateAttackSecond = AttackSecond.None;
                }
				// Heroe dead
                else if (this.life.currentLife <= 0 && this.state != StateHeroe.Dead && this.state != StateHeroe.Recover)
                {
                    this.state = StateHeroe.Dead;
                }
                // Recover heroe
                else if (this.state == StateHeroe.Dead)
                {
                    this.state = StateHeroe.Recover;
                }                
                // Idle
                else
                {
                    state = StateHeroe.Idle;
                    stateAttackSecond = AttackSecond.None;
                }
            }
        }
	}
	//----------------------------------------------------------------------------------------------------------------------------------------
	//Cooldown
	protected void Counter()
	{
		// Secondary attack
		if (cooldown1 < cooldown1total || state == StateHeroe.AttackSecond && stateAttackSecond == AttackSecond.Attack1) cooldown1 -= Time.deltaTime;
		if (cooldown1 <= 0) cooldown1 = cooldown1total;
		if (cooldown2 < cooldown2total || state == StateHeroe.AttackSecond && stateAttackSecond == AttackSecond.Attack2) cooldown2 -= Time.deltaTime;
		if (cooldown2 <= 0) cooldown2 = cooldown2total;
		//if (cooldown3 < cooldown3total || state == StateHeroe.AttackSecond && stateAttackSecond == AttackSecond.Attack3) cooldown3 -= Time.deltaTime;
		if (cooldown3 <= 0) cooldown3 = cooldown3total;
	}
	//----------------------------------------------------------------------------------------------------------------------------------------
	public void updateManaAdren()
	{
		bool useAdren = false;
		bool useMana = false;
		// If we are gonna do a skill
		if (state == StateHeroe.AttackSecond && !doingSecondaryAnim)
		{
			if (stateAttackSecond == AttackSecond.Attack1 && cooldown1 == cooldown1total) 
			{
				if (manaSkill1 != -1) 
				{
					useMana = true;
					currentMana -= manaSkill1;
				}
				else 
				{
					useAdren = true;
					currentAdren -= adrenSkill1;
				}
			}
			else if (stateAttackSecond == AttackSecond.Attack2 && cooldown2 == cooldown2total) 
			{
				if (manaSkill2 != -1) 
				{
					useMana = true;
					currentMana -= manaSkill2;
				}
				else 
				{
					useAdren = true;
					currentAdren -= adrenSkill2;
				}
			}
			else if (stateAttackSecond == AttackSecond.Attack3 && cooldown3 == cooldown3total)
			{
				if (manaSkill3 != -1) 
				{
					useMana = true;
					currentMana -= manaSkill3;
				}
				else 
				{
					useAdren = true;
					//currentAdren -= adrenSkill3;
				}
			}
		}
		// If we did not use an adren skill
		if (!useMana && currentMana != mana)
		{
			if (currentMana < mana)
			{
				if (timeCountMana >= 1)
				{
					timeCountMana = 0;
					currentMana += 5;
				}
				else timeCountMana += Time.deltaTime;
			}
			else
			{
				currentMana = mana;
			}
		}
		// If we did not use a mana skill
		if (!useAdren && currentAdren != adren)
		{
			if (currentAdren < adren)
			{
				if (timeCountAdren >= 1)
				{
					timeCountAdren = 0;
					currentAdren += 5;
				}
				else timeCountAdren += Time.deltaTime;
			}
			else
			{
				currentAdren = adren;
			}
		}
	}
}
