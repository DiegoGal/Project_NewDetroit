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
		IdleWalkRun,	// When he is doing nothing but walking or running, or idle
		Iddle,
		Walk,
		Run,
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
	
	
	// ------------------------------------------------------------------------------------------------------
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
		this.state = StateHeroe.IdleWalkRun;		// Set the initial state of the heroe
		// Get the animator
		animator = GetComponent<Animator> ();
		if (!animator) Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");
		// Initialize the booleans of abilities
		ability1 = ability2 = ability3 = false;
		counterAbility = 0;
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
					this.state = StateHeroe.IdleWalkRun;
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
}
