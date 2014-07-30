using UnityEngine;
using System.Collections;

public abstract class HeroeController : ControllableCharacter 
{
	// CONST
    //protected const int EXP_LEVEL_1_2 = 		200, 		EXP_LEVEL_2_3 = 	600, 	EXP_LEVEL_3_4 = 	1000;
    //protected const int EXPERIENCE_TO_GIVE = 	100;	


	//-------------------------------------------------------------------------------------------------


	// CONTROL HERO
	private bool isControllable = true;
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
	private float gravity = 20.0f; // The gravity for the character
	private bool jumping = false; // Are we jumping? (Initiated with jump button and not grounded yet)-

    // Position
    protected Vector3 initialPosition; // The spawn position

    // FLAGS
    protected bool  canRotate = false,	//Flag to turn
                    canMove = false, 	//Flag to move with WASD
                    extraSpeed = false;	//Flag to apply extra speed


	//---------------------------------------------------------------------------------------------

    
    //protected AttributesHero attributes;
    //protected bool hasNewLevel; // Tell us if the heroe has evolved  or not
    //// Time counters
    //private float timeCountMana = 0, timeCountAdren = 0;
    //private int counterAbility;
    ////This is for the particles that collides with the hero
    //private ParticleSystem.CollisionEvent[] collisionEvents = new ParticleSystem.CollisionEvent[16];
    //protected bool doingSecondaryAnim = false; // A flag to tell us if the orc is doing a secondary attack anim or not
    ////-------------------------------------------------------------------------------------------------

    ////States
    //protected CStateUnit cState;


	// ------------------------------------------------------------------------------------------------
	// PRIVATE	
	// Increment the level
    //protected void levelUp() 
    //{
    //    attributes.levelUp();
    //    hasNewLevel = true;
    //    counterAbility ++;
    //}
	
	// Check if we unlock some abilitie
    //protected void unlockAbilities()
    //{
    //    if (counterAbility > 0)
    //    {
    //        if (!ability3 && attributes.getLevel() == 4)
    //        {
    //            ability3 = true;
    //            counterAbility --;
    //        }
    //        else if (!ability1 && Input.GetKey(KeyCode.Alpha1))
    //        {
    //            ability1 = true;
    //            counterAbility --;
    //        }
    //        else if (!ability2 && Input.GetKey(KeyCode.Alpha2))
    //        {
    //            ability2 = true;
    //            counterAbility --;
    //        }
    //    }
    //}


	// ------------------------------------------------------------------------------------------------

	
	// Increment the experience
    //public void experienceUp(int experience)
    //{
    //    attributes.gainExerience(experience);
    //    attributes.setExperience(Mathf.Min(attributes.getExperience(), EXP_LEVEL_3_4));

    //    if (attributes.getLevel() == 1 && attributes.getExperience() >= EXP_LEVEL_1_2) this.levelUp();
    //    else if (attributes.getLevel() == 2 && attributes.getExperience() >= EXP_LEVEL_2_3) this.levelUp();
    //    else if (attributes.getLevel() == 3 && attributes.getExperience() >= EXP_LEVEL_3_4) this.levelUp();
    //}
	
	
	// ------------------------------------------------------------------------------------------------


	// MAIN
	// Use this for initialization
    //public override void Start ()
    //{
    //    base.Start();

    //    this.initialPosition = transform.position;	// Set the initial position

    //    //this.hasNewLevel = false;
    //    //this.experienceGived = EXPERIENCE_TO_GIVE;	// Experience that the heroe gives when he dies
    //    //counterAbility = 0;
    //    //cState = GetComponent<CStateUnit>();
    //    //cState.animationChanged = cState.animationChangeQueued = cState.animationChangeQueued2 = false;
    //    //attributes = GetComponent<AttributesHero>();
    //}//Start
	
	// Update is called once per frame
    //public override void Update ()
    //{
    //    base.Update();

    //    UpdateControl (); // Update control
    //}//Update


	//-------------------------------------------------------------------------------------------------


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
			if (canRotate)
			{
				if (targetDirection != Vector3.zero)
				{
					moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
//					moveDirection = targetDirection;
					moveDirection = moveDirection.normalized;
				}
			}
			
			// Smooth the speed based on the current target direction
			float curSmooth = speedSmoothing * Time.deltaTime;
			
			// Choose target speed
			//* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
			float targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);
			
			// Pick speed modifier
			if (extraSpeed)  
				targetSpeed = extraRunSpeed;
			else if (canMove)
			{
				if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W))
				{
					if (Input.GetKey(KeyCode.LeftShift)) 
						targetSpeed = runSpeed;
					else 
						targetSpeed = walkSpeed;
				}
			}
			else 
				targetSpeed = 0;
			
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
	

    //----------------------------------------------------------------------------------------------


	//Cooldown
    //protected void Counter()
    //{
    //    // Secondary attack
    //    if (cooldown1 < cooldown1total || state == StateHeroe.AttackSecond && stateAttackSecond == AttackSecond.Attack1) cooldown1 -= Time.deltaTime;
    //    if (cooldown1 <= 0) cooldown1 = cooldown1total;
    //    if (cooldown2 < cooldown2total || state == StateHeroe.AttackSecond && stateAttackSecond == AttackSecond.Attack2) cooldown2 -= Time.deltaTime;
    //    if (cooldown2 <= 0) cooldown2 = cooldown2total;
    //    if (cooldown3 < cooldown3total || state == StateHeroe.AttackSecond && stateAttackSecond == AttackSecond.Attack3) cooldown3 -= Time.deltaTime;
    //    if (cooldown3 <= 0) cooldown3 = cooldown3total;
    //}


	//-------------------------------------------------------------------------------------------------


    //public void updateManaAdren()
    //{
    //    bool useAdren = false;
    //    bool useMana = false;
    //    // If we are gonna do a skill
    //    if (state == StateHeroe.AttackSecond && !doingSecondaryAnim)
    //    {
    //        if (stateAttackSecond == AttackSecond.Attack1 && cooldown1 == cooldown1total) 
    //        {
    //            if (manaSkill1 != -1) 
    //            {
    //                useMana = true;
    //            }
    //            else 
    //            {
    //                useAdren = true;
    //            }
    //        }
    //        else if (stateAttackSecond == AttackSecond.Attack2 && cooldown2 == cooldown2total) 
    //        {
    //            if (manaSkill2 != -1) 
    //            {
    //                useMana = true;
    //            }
    //            else 
    //            {
    //                useAdren = true;
    //            }
    //        }
    //        else if (stateAttackSecond == AttackSecond.Attack3 && cooldown3 == cooldown3total)
    //        {
    //            if (manaSkill3 != -1) 
    //            {
    //                useMana = true;
    //            }
    //            else 
    //            {
    //                useAdren = true;
    //            }
    //        }
    //    }
    //    // If we did not use an adren skill
    //    if (!useMana && timeCountMana >= 1)
    //    {
    //        timeCountMana = 0;
    //        attributes.recoverMana(5);
    //    }
    //    else if (!useMana)
    //    {
    //        timeCountMana += Time.deltaTime;
    //    }
    //    // If we did not use a mana skill
    //    if (!useAdren && timeCountAdren >= 1)
    //    {
    //        timeCountAdren = 0;
    //        attributes.recoverAdren(5);
    //    }
    //    else if (!useMana)
    //    {
    //        timeCountAdren += Time.deltaTime;
    //    }
    //}

}
