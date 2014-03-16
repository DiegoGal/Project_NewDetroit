using UnityEngine;
using System.Collections;

//-----------------------------------------------------------------------------------------------------------------------

public enum CharacterState
{
    Idle = 0,
    Walking = 1,
    Trotting = 2,
    Running = 3,
    Jumping = 4,
}

/*public enum HeroeState
{
	Iddle,
	Walk,
	Run,
	Attack,
	SecondaryAttack
}*/

/*public enum ScondaryAttack
{
	None,
	Attack1,
	Attack2,
	Attack3,
}*/


//-----------------------------------------------------------------------------------------------------------------------


public class ThirdPersonController : MonoBehaviour
{

    public AnimationClip idleAnimation;
    public AnimationClip walkAnimation;
    public AnimationClip runAnimation;
    public AnimationClip jumpPoseAnimation;

    public float walkMaxAnimationSpeed = 0.75f;
    public float trotMaxAnimationSpeed = 1.0f;
    public float runMaxAnimationSpeed = 1.0f;
    public float jumpAnimationSpeed = 1.15f;
    public float landAnimationSpeed = 1.0f;

	//animation.
    private Animator animator;

	//current states of heroe.
	//private HeroeState currentHeroeState;
	private HeroeController.StateHeroe currentHeroeState;
    public CharacterState _characterState;

    // The speed when walking
    public float walkSpeed = 2.0f;
    // after trotAfterSeconds of walking we trot with trotSpeed
    public float trotSpeed = 4.0f;
    // when pressing "Fire3" button (cmd) we start running
    public float runSpeed = 6.0f;

    public float inAirControlAcceleration = 3.0f;

    // How high do we jump when pressing jump and letting go immediately
    public float jumpHeight = 0.5f;

    // The gravity for the character
    public float gravity = 20.0f;
    // The gravity in controlled descent mode
    public float speedSmoothing = 10.0f;
    public float rotateSpeed = 500.0f;
    public float trotAfterSeconds = 3.0f;

    public bool canJump = false;

    private float jumpRepeatTime = 0.05f;
    private float jumpTimeout = 0.15f;
    private float groundedTimeout = 0.25f;

    // The camera doesnt start following the target immediately but waits for a split second to avoid too much waving around.
    private float lockCameraTimer = 0.0f;

    // The current move direction in x-z
    private Vector3 moveDirection = Vector3.zero;
    // The current vertical speed
    private float verticalSpeed = 0.0f;
    // The current x-z move speed
    private float moveSpeed = 0.0f;

    // The last collision flags returned from controller.Move
    private CollisionFlags collisionFlags;

    // Are we jumping? (Initiated with jump button and not grounded yet)
    private bool jumping = false;
    private bool jumpingReachedApex = false;

    // Are we moving backwards (This locks the camera to not do a 180 degree spin)
    private bool movingBack = false;
    // Is the user pressing any keys?
    private bool isMoving = false;
    // When did the user start walking (Used for going into trot after a while)
    private float walkTimeStart = 0.0f;
    // Last time the jump button was clicked down
    private float lastJumpButtonTime = -10.0f;
    // Last time we performed a jump
    private float lastJumpTime = -1.0f;
    // the height we jumped from (Used to determine for how long to apply extra jump power after jumping.)
    //private float lastJumpStartHeight = 0.0f;
    private Vector3 inAirVelocity = Vector3.zero;

    private float lastGroundedTime = 0.0f;
    public bool isControllable = true;

	private Vector3 lastPos;

	//Secondary attack selected
	private HeroeController.AttackSecond secondAttack;
	private float timerSecondAttack;

	//Splash particle
	public GameObject splash; 
	private bool splashActivated=false;
	private float splashCD = 1.7f;

	//Snot particle
	public GameObject snot; 
	private bool snotActivated=false;
	private float snotCD = 1.7f;

	//Smoke particle
	public GameObject smoke; 
	private bool smokeActivated=false;
	private float smokeCD = 1.7f;
	private GameObject smokeInst; //Smoke instantation

    // this is done because of 2 controllers (orc and thirdPerson)
    private OrcController orcController;
	//-----------------------------------------------------------------------------------------------------------------------


    void Awake()
    {
        moveDirection = transform.TransformDirection(Vector3.forward);

        animator = GetComponent<Animator>();
        if (!animator)
            Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");

        /*
    public AnimationClip idleAnimation;
    public AnimationClip walkAnimation;
    public AnimationClip runAnimation;
    public AnimationClip jumpPoseAnimation;	
        */
        if (!idleAnimation)
        {
            animator = null;
            Debug.Log("No idle animation found. Turning off animations.");
        }
        if (!walkAnimation)
        {
            animator = null;
            Debug.Log("No walk animation found. Turning off animations.");
        }
        if (!runAnimation)
        {
            animator = null;
            Debug.Log("No run animation found. Turning off animations.");
        }
        if (!jumpPoseAnimation && canJump)
        {
            animator = null;
            Debug.Log("No jump animation found and the character has canJump enabled. Turning off animations.");
        }

		// Initialize the current heroe state.
		currentHeroeState = HeroeController.StateHeroe.Idle;

		// Initial secondary attack (no secondary attack in the beginning).
		secondAttack = HeroeController.AttackSecond.None;
		timerSecondAttack = 0;

        // Get the Orc Controller
        orcController = this.GetComponent<OrcController>();
    }

    void UpdateSmoothedMovementDirection()
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
        if (v < -0.2f)
            movingBack = true;
        else
            movingBack = false;

        bool wasMoving = isMoving;
        isMoving = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;

        // Target direction relative to the camera
        Vector3 targetDirection = h * right + v * forward;

        // Grounded controls
        if (grounded)
        {
            // Lock camera for short period when transitioning moving & standing still
            lockCameraTimer += Time.deltaTime;
            if (isMoving != wasMoving)
                lockCameraTimer = 0.0f;

            // We store speed and direction seperately,
            // so that when the character stands still we still have a valid forward direction
            // moveDirection is always normalized, and we only update it if there is user input.
			if (targetDirection != Vector3.zero && currentHeroeState != HeroeController.StateHeroe.AttackBasic && currentHeroeState != HeroeController.StateHeroe.AttackSecond) //!animator.GetBool("isAttacking"))
            {
                // If we are really slow, just snap to the target direction
                /*if (moveSpeed < walkSpeed * 0.9f && grounded)
                {
                    moveDirection = targetDirection.normalized;
                }
                // Otherwise smoothly turn towards it
                else
                {*/
                    moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);

                    moveDirection = moveDirection.normalized;
                //}
            }

            // Smooth the speed based on the current target direction
            float curSmooth = speedSmoothing * Time.deltaTime;

            // Choose target speed
            //* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
            float targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);

            //_characterState = CharacterState.Idle;

			// Pick speed modifier
			if (currentHeroeState == HeroeController.StateHeroe.Idle || 
			    currentHeroeState == HeroeController.StateHeroe.AttackBasic || 
			    (currentHeroeState == HeroeController.StateHeroe.AttackSecond && secondAttack != HeroeController.AttackSecond.Attack3))
				targetSpeed = 0;
			else if (currentHeroeState == HeroeController.StateHeroe.AttackSecond && secondAttack == HeroeController.AttackSecond.Attack3)
				targetSpeed = 20;
			else if (currentHeroeState == HeroeController.StateHeroe.Walk)
				targetSpeed = walkSpeed;
			else if (currentHeroeState == HeroeController.StateHeroe.Run)
				targetSpeed = runSpeed;

			/*else if ( Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift))
            {
                targetSpeed *= runSpeed;
                //_characterState = CharacterState.Running;
            }
            else if (Time.time - trotAfterSeconds > walkTimeStart)
            {
                targetSpeed *= trotSpeed;
                //_characterState = CharacterState.Trotting;
            }
            else
            {
                targetSpeed *= walkSpeed;
                //_characterState = CharacterState.Walking;
            }*/
        
            moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);

            // Reset walk time start when we slow down
            if (moveSpeed < walkSpeed * 0.3f)
                walkTimeStart = Time.time;
        }
        // In air controls
        else
        {
            // Lock camera while in air
            if (jumping)
                lockCameraTimer = 0.0f;

            if (isMoving)
                inAirVelocity += targetDirection.normalized * Time.deltaTime * inAirControlAcceleration;
        }



    }
    void ApplyJumping()
    {
        // Prevent jumping too fast after each other
        if (lastJumpTime + jumpRepeatTime > Time.time)
            return;

        if (IsGrounded())
        {
            // Jump
            // - Only when pressing the button down
            // - With a timeout so you can press the button slightly before landing		
            if (canJump && Time.time < lastJumpButtonTime + jumpTimeout)
            {
                verticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
                SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
    void ApplyGravity()
    {
        if (isControllable)	// don't move player at all if not controllable.
        {
            // Apply gravity
            //bool jumpButton = Input.GetButton("Jump");
            
            // When we reach the apex of the jump we send out a message
            if (jumping && !jumpingReachedApex && verticalSpeed <= 0.0f)
            {
                jumpingReachedApex = true;
                SendMessage("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
            }

            if (IsGrounded())
                verticalSpeed = 0.0f;
            else
                verticalSpeed -= gravity * Time.deltaTime;
        }
    }

    float CalculateJumpVerticalSpeed(float targetJumpHeight)
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * targetJumpHeight * gravity);
    }

    void DidJump()
    {
        jumping = true;
        jumpingReachedApex = false;
        lastJumpTime = Time.time;
        //lastJumpStartHeight = transform.position.y;
        lastJumpButtonTime = -10;

        _characterState = CharacterState.Jumping;
    }

	// Update the secondary attack
	void UpdateSecondAttack()
	{
		HeroeController hc = this.GetComponent<HeroeController> ();
		if (Input.GetKey(KeyCode.Alpha1) && hc.ability1)
		{
			secondAttack = HeroeController.AttackSecond.Attack1;
		}
		else if (Input.GetKey(KeyCode.Alpha2) && hc.ability2)
		{
			secondAttack = HeroeController.AttackSecond.Attack2;
		}
		else if (Input.GetKey(KeyCode.Alpha3) && hc.ability3)
		{
			secondAttack = HeroeController.AttackSecond.Attack3;
		}

		if (snotActivated)
		{
			if (snotCD <= 0)
			{
				snotCD = 1.7f;
				snotActivated = false;
			}
			else
			{
				snotCD -= Time.deltaTime;
			}
		}

		if (splashActivated)
		{
			if (splashCD <= 0)
			{
				splashCD = 1.7f;
				splashActivated = false;
			}
			else
			{
				splashCD -= Time.deltaTime;
			}
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
			else
			{
				smokeCD -= Time.deltaTime;
			}
		}
	}

    Vector3 velocity = Vector3.zero;

    void Update()
    {        
        if (isControllable)
        {
            /*if (Input.GetButtonDown("Jump"))
            {
                lastJumpButtonTime = Time.time;
            }*/

			//update the current heroe state.
			if (currentHeroeState == HeroeController.StateHeroe.AttackSecond)
			{
				timerSecondAttack += Time.deltaTime;
				if ((secondAttack == HeroeController.AttackSecond.Attack1 && timerSecondAttack >= 3.3) || 
				    (secondAttack == HeroeController.AttackSecond.Attack2 && timerSecondAttack >= 1.633) ||
				    (secondAttack == HeroeController.AttackSecond.Attack3 && timerSecondAttack >= 1.633))
				{
					secondAttack = HeroeController.AttackSecond.None;
					currentHeroeState = HeroeController.StateHeroe.Idle;
				}
			}
			else if (Input.GetKey(KeyCode.Space) && secondAttack != HeroeController.AttackSecond.None)
			{
				currentHeroeState = HeroeController.StateHeroe.AttackSecond;
				timerSecondAttack = 0;
				if (secondAttack == HeroeController.AttackSecond.Attack2)
				{
                    GameObject spl = (GameObject)Instantiate(splash, transform.position + new Vector3(0, -2, 0), Quaternion.identity);
                    spl.GetComponent<OrcSplashAttack>().setDamage(orcController.attackM + 40);
                    spl.GetComponent<OrcSplashAttack>().setOwner(gameObject);
					Destroy(spl,1.5f);
					splashActivated = true;
				}
				else if (secondAttack == HeroeController.AttackSecond.Attack1)
				{
                    transform.Translate(Vector3.forward * 2 + Vector3.up);                    
                    GameObject snt = (GameObject)Instantiate(snot, transform.localPosition, transform.rotation);
                    snt.GetComponent<ParticleDamage>().setDamage(orcController.attackM);
					transform.Translate(Vector3.back*2 + Vector3.down);
					Destroy(snt,5f);
					snotActivated = true;
				}
				else if (secondAttack == HeroeController.AttackSecond.Attack3)
				{
					transform.Translate(Vector3.down*2);
					smokeInst = (GameObject)Instantiate(smoke,transform.localPosition,transform.rotation);
					transform.Translate(Vector3.up*2);
					Destroy(smokeInst,5f);
					smokeActivated = true;
				}
			}
			else if (Input.GetMouseButton(0))
			{
				currentHeroeState = HeroeController.StateHeroe.AttackBasic;
			}
			else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
			{
				if (Input.GetKey(KeyCode.LeftShift))
					currentHeroeState = HeroeController.StateHeroe.Run;
				else
					currentHeroeState = HeroeController.StateHeroe.Walk;
			}
			else 
			{
				currentHeroeState = HeroeController.StateHeroe.Idle;
			}


			// Update secondary attack
			UpdateSecondAttack();

			//update the movement direction.
			UpdateSmoothedMovementDirection();

            // Apply gravity
            // - extra power jump modifies gravity
            // - controlledDescent mode modifies gravity
            ApplyGravity();

            // Apply jumping logic
            //ApplyJumping();


            // Calculate actual motion
            Vector3 movement = moveDirection * moveSpeed + new Vector3(0, verticalSpeed, 0) + inAirVelocity;
            movement *= Time.deltaTime;

            // Move the controller
            CharacterController controller = GetComponent<CharacterController>();
            collisionFlags = controller.Move(movement);
        }
        velocity = (transform.position - lastPos)*25;

        // ANIMATION sector
		if (isControllable)
			if(animator)
			{
				//get the current state
				AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
				
				//if we're in "Run" mode, respond to input for jump, and set the Jump parameter accordingly. 
				/*if(stateInfo.nameHash == Animator.StringToHash("Base Layer.RunBT"))
				{
					if(Input.GetButton("Fire1")) 
						_animation.SetBool("Jump", true );
				}
				else
				{
					_animation.SetBool("Jump", false);				
				}*/

				// Secondary attack
				/*if (Input.GetKey(KeyCode.Space) && secondAttack == HeroeController.AttackSecond.Attack1)
				{
					animator.SetBool("isSecondAttack1", true);
					animator.SetBool("isSecondAttack2", false);
					animator.SetBool("isSecondAttack3", false);
				}
				else if (Input.GetKey(KeyCode.Space) && secondAttack == HeroeController.AttackSecond.Attack2)
				{
					animator.SetBool("isSecondAttack2", true);
					animator.SetBool("isSecondAttack1", false);
					animator.SetBool("isSecondAttack3", false);

					//Comprobamos que no ha sido activado el poder para activarlo
					if (!splashActivated){
						Object spl = Instantiate(splash,transform.position + new Vector3(0,-2,0),Quaternion.identity);
						Destroy(spl,1.5f);
						splashActivated=true;
					}
				}
				else if (Input.GetKey(KeyCode.Space) && secondAttack == HeroeController.AttackSecond.Attack3)
				{
					animator.SetBool("isSecondAttack3", true);
					animator.SetBool("isSecondAttack1", false);
					animator.SetBool("isSecondAttack2", false);
				}
				else
				{
					animator.SetBool("isSecondAttack1", false);
					animator.SetBool("isSecondAttack2", false);
					animator.SetBool("isSecondAttack3", false);
				}*/

				// attacking
				/*if (Input.GetMouseButton(0))
				{
					animator.SetBool ("isAttacking", true);
				}
				else
				{
					animator.SetBool ("isAttacking", false);
				}*/

				//moving
				/*if (!animator.GetBool("isAttacking"))
			    {
					float v = Input.GetAxisRaw("Vertical");
					float h = Input.GetAxisRaw("Horizontal");
					//set event parameters based on user input
					animator.SetFloat("Speed", h*h+v*v);
				}
				else
				{
					animator.SetFloat("Speed", 0);
				}*/

				//animate the animator
				if (currentHeroeState == HeroeController.StateHeroe.Idle)
				{
					animator.SetBool ("isRunning", false);
					animator.SetFloat ("Speed", 0);
					animator.SetBool ("isAttacking", false);
					animator.SetBool ("isSecondAttack1", false);
					animator.SetBool ("isSecondAttack2", false);
					animator.SetBool ("isSecondAttack3", false);
				}
				else if (currentHeroeState == HeroeController.StateHeroe.AttackSecond)
				{
					if (secondAttack == HeroeController.AttackSecond.Attack1)
						animator.SetBool ("isSecondAttack1", true);
					else if (secondAttack == HeroeController.AttackSecond.Attack2)
						animator.SetBool ("isSecondAttack2", true);
					else
						animator.SetBool ("isSecondAttack3", true);
				}
				else if (currentHeroeState == HeroeController.StateHeroe.AttackBasic)
				{
					animator.SetFloat ("Speed", 0);
					animator.SetBool ("isRunning", false);
					animator.SetBool ("isAttacking", true);
					animator.SetBool ("isSecondAttack1", false);
					animator.SetBool ("isSecondAttack2", false);
					animator.SetBool ("isSecondAttack3", false);
				}
				else if (currentHeroeState == HeroeController.StateHeroe.Walk)
				{
					animator.SetFloat ("Speed", 1);
					animator.SetBool ("isRunning", false);
					animator.SetBool ("isAttacking", false);
					animator.SetBool ("isSecondAttack1", false);
					animator.SetBool ("isSecondAttack2", false);
					animator.SetBool ("isSecondAttack3", false);
				}
				else if (currentHeroeState == HeroeController.StateHeroe.Run)
				{
					animator.SetFloat ("Speed", 1);
					animator.SetBool ("isRunning", true);
					animator.SetBool ("isAttacking", false);
					animator.SetBool ("isSecondAttack1", false);
					animator.SetBool ("isSecondAttack2", false);
					animator.SetBool ("isSecondAttack3", false);
				}
				
				//_animation.SetFloat("Direction", h, DirectionDampTime, Time.deltaTime);
			}		

        /*if (_animation)
        {
            if (_characterState == CharacterState.Jumping)
            {
                if (!jumpingReachedApex)
                {
                    _animation.animation[jumpPoseAnimation.name].speed = jumpAnimationSpeed;
					_animation.animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					_animation.CrossFade(jumpPoseAnimation.name,0.3f);
                }
                else
                {
					_animation.animation[jumpPoseAnimation.name].speed = -landAnimationSpeed;
					_animation.animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					_animation.CrossFade(jumpPoseAnimation.name,0.3f);
                }
            }
            else
            {
                if (this.isControllable && velocity.sqrMagnitude < 0.001f)
                {
                    _characterState = CharacterState.Idle;
                }
                else
                {
                    if (_characterState == CharacterState.Idle)
                    {
						_animation.CrossFade(idleAnimation.name,0.3f);
                    }
                    else if (_characterState == CharacterState.Running)
                    {
						_animation.animation[runAnimation.name].speed = runMaxAnimationSpeed;
                        if (this.isControllable)
                        {
							_animation.animation[runAnimation.name].speed = Mathf.Clamp(velocity.magnitude, 0.0f, runMaxAnimationSpeed);
                        }
						_animation.CrossFade(runAnimation.name,0.3f);
                    }
                    else if (_characterState == CharacterState.Trotting)
                    {
						_animation.animation[walkAnimation.name].speed = trotMaxAnimationSpeed;
                        if (this.isControllable)
                        {
							_animation.animation[walkAnimation.name].speed = Mathf.Clamp(velocity.magnitude, 0.0f, trotMaxAnimationSpeed);
                        }
						_animation.CrossFade(walkAnimation.name,0.3f);
                    }
                    else if (_characterState == CharacterState.Walking)
                    {
						_animation.animation[walkAnimation.name].speed = walkMaxAnimationSpeed;
                        if (this.isControllable)
                        {
							_animation.animation[walkAnimation.name].speed = Mathf.Clamp(velocity.magnitude, 0.0f, walkMaxAnimationSpeed);
                        }
                        _animation.CrossFade(walkAnimation.name,0.3f);
                    }
                }
            }
        }*/
        // ANIMATION sector

        // Set rotation to the move direction
        if (IsGrounded())
        {

            transform.rotation = Quaternion.LookRotation(moveDirection);

        }
        else
        {
            Vector3 xzMove = velocity;
            xzMove.y = 0;
            if (xzMove.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(xzMove);
            }
        }

        // We are in jump mode but just became grounded
        if (IsGrounded())
        {
            lastGroundedTime = Time.time;
            inAirVelocity = Vector3.zero;
            if (jumping)
            {
                jumping = false;
                SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);
            }
        }

        lastPos = transform.position;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //	Debug.DrawRay(hit.point, hit.normal);
        if (hit.moveDirection.y > 0.01f)
            return;
    }

    public float GetSpeed()
    {
        return moveSpeed;
    }

    public bool IsJumping()
    {
        return jumping;
    }

    public bool IsGrounded()
    {
        return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
    }

    public Vector3 GetDirection()
    {
        return moveDirection;
    }

    public bool IsMovingBackwards()
    {
        return movingBack;
    }

    public float GetLockCameraTimer()
    {
        return lockCameraTimer;
    }

    public bool IsMoving()
    {
        return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5f;
    }

    public bool HasJumpReachedApex()
    {
        return jumpingReachedApex;
    }

    public bool IsGroundedWithTimeout()
    {
        return lastGroundedTime + groundedTimeout > Time.time;
    }

    public void Reset()
    {
        gameObject.tag = "Player";
    }


}