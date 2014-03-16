using UnityEngine;
using System.Collections;

public abstract class HeroeController : ControllableCharacter 
{
	protected const int EXP_LEVEL_1_2 = 		200, 		EXP_LEVEL_2_3 = 	600, 	EXP_LEVEL_3_4 = 	1000;
	protected const int COLDOWN_LEVEL_1 = 		20, 		COLDOWN_LEVEL_2 = 	30, 	COLDOWN_LEVEL_3 = 	40, 	COLDOWN_LEVEL_4 = 50;
	protected const int EXPERIENCE_TO_GIVE = 	100;
	
	
	//------------------------------------------------------------------------------------------------------
	public enum StateHeroe // The state of the heroe
	{
		Dead,			// When is dead
		Recover,		// After dead, he must be recovered
		Idle,			// When is rest
		Walk,			// When is walking
		Run,			// When is running
		AttackBasic,	// When he is attacking with his basic attack
		AttackSecond	// When he is attacking with his secondary attack
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
	//---------------------------------------------------------------------------------------------
	// PARTICLES
	// Snot particle
	public GameObject snot; 
	private bool snotActivated = false;
	private float snotCD = 1.7f;
	// Splash particle
	public GameObject splash; 
	private bool splashActivated = false;
	private float splashCD = 1.7f;
	// Smoke particle
	public GameObject smoke; 
	private bool smokeActivated = false;
	private float smokeCD = 1.7f;
	private GameObject smokeInst; // Smoke instantiation
	//---------------------------------------------------------------------------------------------
	public Texture2D 	textureLifePositive, textureLifeNegative,
						textureAdrenPositive, textureAdrenNegative,
						textureManaPositive, textureManaNegative;
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
	public bool isMine; // Tell us if that instance if ours or not
	public TypeHeroe type;	// Type of heroe
	public StateHeroe state; // The state of the heroe
	public AttackSecond stateAttackSecond;	// The state of secondary attack
	public bool ability1, 
				ability2, 
				ability3;
	
	protected bool hasNewLevel; // Tell us if the heroe has evolved  or not
	protected Animator animator; //Animator
	protected Vector3 initialPosition; // The spawn position
	
	private double timeCount; // Time counter
	public int counterAbility;
    //This is for the particles that collides with the hero
    private ParticleSystem.CollisionEvent[] collisionEvents = new ParticleSystem.CollisionEvent[16];
    
	
	// ------------------------------------------------------------------------------------------------------
	// PRIVATE
	// Method that control the logic of the attack (now, it is not used)
	protected void launchAttack()
	{
		//if (this.tag == "Player")
		//{
		//Si el ataque no ha sido lanzado
		if (!attackInstantiate)
		{
			attackInstantiate = true;
			//Se halla la direccion en la que se mueve el heroe
			Vector3 direction = this.GetComponent<ThirdPersonController>().GetDirection ();
			//Se halla la posicion donde se lanzara el ataque
			Vector3 position = new Vector3(
				transform.position.x + direction.x*2,
				transform.position.y + direction.y,
				transform.position.z + direction.z*2);
			//Se instancia el ataque
			//GameObject go = (GameObject) Instantiate(rightArm, position, new Quaternion());
			//go.GetComponent<Attack>().setMovement(direction);
			//go.GetComponent<OrcBasicAttack>().setOwner(this.gameObject);
			
			//this.GetComponent<OrcBasicAttack> ().enable(false);
		}
		//}
	}
	
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
	// PUBLIC
    // if type == 'P' is phisical damage if type == 'M' is magical damage
	public override bool Damage (float damage,char type)
	{
        if (type == 'P')
		    currentLife -= damage - defP;
        else
            if (type == 'M')
                currentLife -= damage - defM;
		return (currentLife <= 0);
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
		this.level = 1;
		this.experience = 0;
		this.hasNewLevel = false;
		this.attackInstantiate = true;
		this.initialPosition = transform.position;	// Set the initial position
		this.timeCount = 0;							// Set the initial value of timeCount
		this.experienceGived = EXPERIENCE_TO_GIVE;	// Experience that the heroe gives when he dies
		// Get the animator
		animator = GetComponent<Animator> ();
		if (!animator) Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");
		// Initialize the booleans of abilities
		ability1 = ability2 = ability3 = false;
		counterAbility = 0;
		// Initialize the animation
		animation.Play ("Iddle01");
		state = StateHeroe.Idle;				// Set the initial state of the hero
		stateAttackSecond = AttackSecond.None;		// Set the initial state of secondary attack of hero
	}//Start
	
	// Update is called once per frame
	virtual public void Update () {
		// Heroe dead
		if (this.currentLife <= 0 && this.state != StateHeroe.Dead && this.state != StateHeroe.Recover) 
		{
			this.currentLife = 0;
			this.state = StateHeroe.Dead;
			this.transform.position = this.initialPosition;
			this.GetComponent<ThirdPersonController>().enabled = false;
		}
		// Recover heroe
		else if (this.state == StateHeroe.Dead) this.state = StateHeroe.Recover;
		else if (this.state == StateHeroe.Recover)
		{
			if (this.timeCount < 1)	this.timeCount += Time.deltaTime;
			else
			{
				this.timeCount = 0;
				this.currentLife += 20;
				if (this.currentLife >= this.maximunLife)
				{
					this.currentLife = this.maximunLife;
					this.state = StateHeroe.Idle;
					this.GetComponent<ThirdPersonController>().enabled = true;
				}
			}
		}
		
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
		UpdateState (); // Update state
		UpdateParticles (); // Update particles
	}//Update

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
            Damage(particleSystem.GetComponent<ParticleDamage>().getDamage(), 'M');
            CDParticleCollision = 0.1f; // 5 deltatime aprox
            }
        }
           
         
    }

    void OnGUI ()
	{
		// DEBUG
		//Vector3 pos = Camera.main.WorldToScreenPoint (transform.position);
		//GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y, 200, 50), "Vida: " + this.currentLife);
		//GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 10, 200, 50), "Experience: " + this.experience);
		//GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 20, 200, 50), "Level: " + this.level);
		//GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 30, 200, 50), "Maxima vida: " + this.maximunLife);
		//GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 40, 200, 50), "Ataque fisico: " + this.attackP);
		//GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 50, 200, 50), "Ataque magico: " + this.attackM);
		//GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 60, 200, 50), "Velocidad ataque: " + this.speedAtt);
		//GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 70, 200, 50), "Defensa fisica: " + this.defP);
		//GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 80, 200, 50), "Defensa magica: " + this.defM);
		//GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 90, 200, 50), "Mana: " + this.currentMana);
		//GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 100, 200, 50), "Maximo Mana: " + this.mana);
		//GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 110, 200, 50), "Adrenalina: " + this.currentAdren);
		//GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 120, 200, 50), "Maxima Adrenalina: " + this.adren);
		//GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 130, 200, 50), "Velocidad mvto: " + this.speedMov);
		
		//-------------------------------------------------------------------------------------------------
		// Position of the life, mana and adrenaline in the screen
		Vector3 posLifeScene = new Vector3 (transform.position.x, transform.position.y, transform.position.z),
				posLifeSceneEnd = new Vector3 (transform.position.x, transform.position.y, transform.position.z),
				posAdrenScene = new Vector3 (transform.position.x, transform.position.y, transform.position.z),
				posAdrenSceneEnd = new Vector3 (transform.position.x, transform.position.y, transform.position.z),
				posManaScene = new Vector3 (transform.position.x, transform.position.y, transform.position.z),
				posManaSceneEnd = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		posLifeScene.y += 2f;
		posLifeSceneEnd.y += 1.8f;
		posAdrenScene.y += 1.78f;
		posAdrenSceneEnd.y += 1.68f;
		posManaScene.y += 1.66f;
		posManaSceneEnd.y += 1.56f;
		Vector3 posLife = Camera.main.WorldToScreenPoint (posLifeScene),
				posLifeEnd = Camera.main.WorldToScreenPoint (posLifeSceneEnd),
				posAdren = Camera.main.WorldToScreenPoint (posAdrenScene),
				posAdrenEnd = Camera.main.WorldToScreenPoint (posAdrenSceneEnd),
				posMana = Camera.main.WorldToScreenPoint (posManaScene),
				posManaEnd = Camera.main.WorldToScreenPoint (posManaSceneEnd);
		// Life, Adrenaline and Mana
		float 	distance = Vector3.Distance (transform.position, Camera.main.transform.position), // real distance from camera
				lengthLifeAdrenMana = this.GetComponent<ThirdPersonCamera> ().distance / distance, // percentage of the distance
				heightPosLife = posLife.y - posLifeEnd.y,
				heightPosAdren = posAdren.y - posAdrenEnd.y,
				heightPosMana = posMana.y - posManaEnd.y,
				widthAll = Screen.width / 10,
				widthHalf = widthAll / 2,
				positiveLife = (float) this.currentLife / this.maximunLife, // percentage of positive life
				negativeLife = 1 - positiveLife, //percentage of negative life
				positiveAdren = (float) this.currentAdren / this.adren,
				negativeAdren = 1 - positiveAdren,
				positiveMana = (float) this.currentMana / this.mana,
				negativeMana = 1 - positiveMana,				
				beginWidthPositiveLife = posLife.x - widthHalf * lengthLifeAdrenMana,
				beginHeightLife = Screen.height - posLife.y,
				widthPositiveLife = widthAll * positiveLife * lengthLifeAdrenMana,
				heightLife = heightPosLife * lengthLifeAdrenMana,				
				beginWidthNegativeLife = posLife.x - widthHalf * lengthLifeAdrenMana + widthAll * positiveLife * lengthLifeAdrenMana,
				widthNegativeLife = widthAll * negativeLife * lengthLifeAdrenMana,				
				beginWidthPositiveAdren = posAdren.x - widthHalf * lengthLifeAdrenMana,
				beginHeightAdren = Screen.height - posAdren.y,
				widthPositiveAdren = widthAll * positiveAdren * lengthLifeAdrenMana,
				heightAdren = heightPosAdren * lengthLifeAdrenMana,
				beginWidthNegativeAdren = posAdren.x - widthHalf * lengthLifeAdrenMana + widthAll * positiveAdren * lengthLifeAdrenMana,
				widthNegativeAdren = widthAll * negativeAdren * lengthLifeAdrenMana,				
				beginWidthPositiveMana = posMana.x - widthHalf * lengthLifeAdrenMana,
				beginHeightMana = Screen.height - posMana.y,
				widthPositiveMana = widthAll * positiveMana * lengthLifeAdrenMana,
				heightMana = heightPosMana * lengthLifeAdrenMana,
				beginWidthNegativeMana = posMana.x - widthHalf * lengthLifeAdrenMana + widthAll * positiveMana * lengthLifeAdrenMana,
				widthNegativeMana = widthAll * negativeMana * lengthLifeAdrenMana;		
		// Life	
		Rect rectanglePositiveLife = new Rect (beginWidthPositiveLife, beginHeightLife, widthPositiveLife, heightLife);
		Rect rectangleNegativeLife = new Rect (beginWidthNegativeLife, beginHeightLife, widthNegativeLife, heightLife);
		// Adrenaline
		Rect rectanglePositiveAdren = new Rect (beginWidthPositiveAdren, beginHeightAdren, widthPositiveAdren, heightAdren);
		Rect rectangleNegativeAdren = new Rect (beginWidthNegativeAdren, beginHeightAdren, widthNegativeAdren, heightAdren);
		// Mana
		Rect rectanglePositiveMana = new Rect (beginWidthPositiveMana, beginHeightMana, widthPositiveMana, heightMana);
		Rect rectangleNegativeMana = new Rect (beginWidthNegativeMana, beginHeightMana, widthNegativeMana, heightMana);
		// Draw
		GUI.DrawTexture (rectanglePositiveLife, textureLifePositive);
		GUI.DrawTexture (rectangleNegativeLife, textureLifeNegative);
		GUI.DrawTexture (rectanglePositiveAdren, textureAdrenPositive);
		GUI.DrawTexture (rectangleNegativeAdren, textureAdrenNegative);
		GUI.DrawTexture (rectanglePositiveMana, textureManaPositive);
		GUI.DrawTexture (rectangleNegativeMana, textureManaNegative);
	}//OnGUI


	//----------------------------------------------------------------------------------------------------------------------------------------
	// CONTROL HERO
	protected void UpdateControl()
	{
		if (isControllable)
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
			bool isOrcAttacking = state == HeroeController.StateHeroe.AttackBasic || state == HeroeController.StateHeroe.AttackSecond;
			if (targetDirection != Vector3.zero)// && !isOrcAttacking)
			{
				moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
				moveDirection = moveDirection.normalized;
			}
			
			// Smooth the speed based on the current target direction
			float curSmooth = speedSmoothing * Time.deltaTime;
			
			// Choose target speed
			//* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
			float targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);
			
			// Pick speed modifier
			if (animation.IsPlaying("BullStrike")) targetSpeed = extraRunSpeed;
			else if (!animation.IsPlaying("FloorHit") && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W)))
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
	// STATE & ANIMATION
	protected void UpdateState()
	{
		// Only can do an action if hero don't do a secondary attack
		if (!animation.IsPlaying("Burp") && !animation.IsPlaying("FloorHit") && !animation.IsPlaying("BullStrike"))
		{
			// Secondary attack
			if (Input.GetKey (KeyCode.Alpha1) && ability1) 
			{
				state = StateHeroe.AttackSecond;
				stateAttackSecond = AttackSecond.Attack1;
				animation.CrossFade("Burp");
				//--------------------------
				transform.Translate(Vector3.forward*2 + Vector3.up);
				GameObject snt = (GameObject)Instantiate(snot, transform.localPosition, transform.rotation);
				snt.GetComponent<ParticleDamage>().setDamage(attackM);
				transform.Translate(Vector3.back*2 + Vector3.down);
				Destroy(snt, 5f);
				snotActivated = true;
			}
			else if (Input.GetKey (KeyCode.Alpha2) && ability2) 
			{
				state = StateHeroe.AttackSecond;
				stateAttackSecond = AttackSecond.Attack2;
				animation.CrossFade("FloorHit");
				//------------------------------
				Object spl = Instantiate(splash, transform.position + new Vector3(0,-2,0), Quaternion.identity);
				Destroy(spl, 1.5f);
				splashActivated = true;
			}
			else if (Input.GetKey (KeyCode.Alpha3) && ability3) 
			{
				state = StateHeroe.AttackSecond;
				stateAttackSecond = AttackSecond.Attack3;
				animation.CrossFade("BullStrike");
				//--------------------------------
				transform.Translate(Vector3.down*2);
				smokeInst = (GameObject)Instantiate(smoke, transform.localPosition,transform.rotation);
				transform.Translate(Vector3.up*2);
				Destroy(smokeInst, 5f);
				smokeActivated = true;
			}
			// Basic attack
			else if (Input.GetMouseButton(0))
			{
				state = StateHeroe.AttackBasic;
				stateAttackSecond = AttackSecond.None;
				if (!animation.IsPlaying("Attack01") && !animation.IsPlaying("Attack02") && !animation.IsPlaying("Attack03"))
				{
					animation.CrossFade("Attack01");
					animation.CrossFadeQueued("Attack02");
					animation.CrossFadeQueued("Attack03");
				}
			}
			// Movement
			else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W))
			{
				if (Input.GetKey(KeyCode.LeftShift)) 
				{
					state = StateHeroe.Run;
					animation.CrossFade("Run");
				}
				else 
				{
					state = StateHeroe.Walk;
					animation.CrossFade("Walk");
				}
				stateAttackSecond = AttackSecond.None;
			}
			// Idle
			else 
			{
				state = StateHeroe.Idle;
				stateAttackSecond = AttackSecond.None;
				if (!animation.IsPlaying("Iddle01") && !animation.IsPlaying("Iddle02"))
				{
					animation.CrossFade("Iddle01");
					animation.CrossFadeQueued("Iddle02");
				} 
				
			}
		}
	}
	
	protected void UpdateParticles()
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
	}
	//----------------------------------------------------------------------------------------------------------------------------------------
}
