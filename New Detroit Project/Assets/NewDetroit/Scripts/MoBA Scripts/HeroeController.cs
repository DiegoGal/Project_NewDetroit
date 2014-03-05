using UnityEngine;
using System.Collections;

public abstract class HeroeController : ControllableCharacter 
{
	// The state of the heroe
	public enum StateHeroe
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

	// The state of secondary attack
	public enum AttackSecond
	{
		None,
		Attack1,
		Attack2,
		Attack3,
	}

	// The type of the heroe
	public enum TypeHeroe
	{
		Orc
	}


	// ------------------------------------------------------------------------------------------------------


	public Texture2D 	textureLifePositive, 
						textureLifeNegative;

	protected bool attackInstantiate;

	private const int LEVEL_1_2 = 200, // experience to improve from leve 1 to 2
					LEVEL_2_3 = 600, // experience to improve from leve 2 to 3
					LEVEL_3_4 = 1000; // experience to improve from leve 3 to 4

	protected int attackP, attackM, defP, defM, mana, adren, speedMov, level, experience; //Attributes
	protected double speedAtt; // Attribute
	protected bool hasNewLevel; // Tell us if the heroe has evolved  or not

	protected Animator animator; //Animator

	protected bool isMine; // Tell us if that instance if ours or not

	protected Vector3 initialPosition; // The spawn position

	protected StateHeroe state; // The state of the heroe

	private double timeCount; // Time counter

	public TypeHeroe type;	// Type of heroe

	public bool ability1, ability2, ability3;	// Booleans to unlock the abilities of heroe when up the level
	private int counterAbility;


	// ------------------------------------------------------------------------------------------------------


	// Increment the level
	protected void levelUp() 
	{
		this.level ++;
		this.hasNewLevel = true;
		counterAbility ++;
	}

	// Increment the life
	protected void lifeUp(int life)
	{
		this.currentLife += life;
	}

	// Increment the physical attack
	protected void attackPUp(int attackP) 
	{
		this.attackP += attackP;
	}

	// Increment the magical attack
	protected void attackMUp(int attackM) 
	{
		this.attackM += attackM;
	}

	// Increment the speed attack
	protected void speedAttUp(double speedAtt)
	{
		this.speedAtt += speedAtt;
	}

	// Increment the physical defense
	protected void defPUp(int defP)
	{
		this.defP += defP;
	}

	// Increment the magical defense
	protected void defMUp(int defM)
	{
		this.defM += defM;
	}

	// Increment the mana
	protected void manaUp(int mana)
	{
		this.mana += mana;
	}

	// Increment the adrenalin
	protected void adrenUp(int adren)
	{
		this.adren += adren;
	}

	// Increment the movement speed
	protected void speedMovUp(int speedMov)
	{
		this.speedMov += speedMov;
	}

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

	// Check dead condition
	protected void checkDead()
	{
		// if is dead
		if (this.currentLife <= 0 && this.state != StateHeroe.Dead && this.state != StateHeroe.Recover) 
		{
			this.currentLife = 0;
			this.state = StateHeroe.Dead;
			this.transform.position = this.initialPosition;
			this.GetComponent<ThirdPersonController>().enabled = false;
		}
	}

	// Check recover condition
	protected void checkRecover()
	{
		if (this.state == StateHeroe.Dead)// && Input.GetKey(KeyCode.R))
		{
			this.state = StateHeroe.Recover;
		}
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

	// Check if we have to improve our level
	protected void checkLevel()
	{
		if (this.experience > LEVEL_3_4) this.experience = LEVEL_3_4;
		
		if (this.level == 1 && this.experience >= LEVEL_1_2) this.levelUp ();
		else if (this.level == 2 && this.experience >= LEVEL_2_3) this.levelUp ();
		else if (this.level == 3 && this.experience >= LEVEL_3_4) this.levelUp ();
	}


	// ------------------------------------------------------------------------------------------------------


	// Return if the heroe is attacking or not
	public bool isAttackBasic()
	{
		//return this.attackInstantiate;
		return this.state == StateHeroe.AttackBasic;
	}

	// Damage the heroe
	public void damage(int life)
	{
		this.currentLife -= life;
	}

	// Return the current life of the heroe
	/*public float getLife()
	{
		return this.currentLife;
	}*/

	// Set the lide of the heroe
	public void setLife(int life)
	{
		this.currentLife = life;
	}

	// Return if the heroe's instantiate is ours or not
	public bool getIsMine()
	{
		return this.isMine;
	}

	// Set isMine to tell us if the heroe's instantiate is ours or not
	public void setIsMine(bool isMine)
	{
		this.isMine = isMine;
	}

	// Returns the type of the heroe
	public TypeHeroe getType()
	{
		return this.type;
	}

	// Increment the experience
	public void experienceUp(int experience)
	{
		this.experience += experience;
		this.checkLevel ();
	}

	// Get the current experience
	public int getExperience()
	{
		return experience;
	}

	// Set the current experience and update the current level
	public void setExperience(int experience)
	{
		this.experience = experience;
		this.checkLevel ();
	}


	// ------------------------------------------------------------------------------------------------------


	// Use this for initialization
	virtual public void Start () {
		//Set initial values
		this.level = 1;
		this.experience = 0;
		this.hasNewLevel = false;
		this.attackInstantiate = true;
		// Get the animator
		animator = GetComponent<Animator> ();
		if (!animator)
			Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");
		//Set the initial position
		this.initialPosition = transform.position;
		//Set the initial state of the heroe
		this.state = StateHeroe.IdleWalkRun;
		//Set the initial value of timeCount
		this.timeCount = 0;
		//Experience that the heroe gives when he dies
		this.experienceGived = 200;
		// Initialize the booleans of abilities
		ability1 = ability2 = ability3 = false;
		counterAbility = 0;
	}
	
	// Update is called once per frame
	virtual public void Update () {
		//launchAttack();

		checkDead ();
		checkRecover ();

		// Test if we have abilites to unlock
		unlockAbilities ();
	}

	void OnGUI ()
	{
		// labels for debug
		Vector3 pos = Camera.main.WorldToScreenPoint (transform.position);
		//GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y, 100, 50), "Vida: " + this.currentLife);
		//GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 20, 100, 50), "Mine: " + this.isMine);
		//GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 40, 100, 50), "State: " + this.state);
		//GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 60, 100, 50), "Experience: " + this.experience);
		//GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 80, 100, 50), "Level: " + this.level);
		//GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 100, 100, 50), "Maxima vida: " + this.maximunLife);

		//-------------------------------------------------------------------------------------------------
		// Life
		float distance = Vector3.Distance (transform.position, Camera.main.transform.position); // real distance from camera
		float lengthLife = this.GetComponent<ThirdPersonCamera> ().distance / distance; // percentage of the distance
		// Position of the life in the screen
		Vector3 posLifeScene = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		posLifeScene.y += 2f;
		Vector3 posLife = Camera.main.WorldToScreenPoint (posLifeScene);

		float positiveLife = (float) this.currentLife / this.maximunLife; // percentage of positive life
		float negativeLife = 1 - positiveLife; //percentage of negative life
		
		Rect rectanglePositive = new Rect (posLife.x - 50 * lengthLife, Screen.height - posLife.y, 100 * positiveLife * lengthLife, 10 * lengthLife);
		Rect rectangleNegative = new Rect (posLife.x - 50 * lengthLife + 100 * positiveLife * lengthLife, Screen.height - posLife.y, 
		                                   100 * negativeLife * lengthLife, 10 * lengthLife);
		//GUI.DrawTexture (rectanglePositive, textureLifePositive);
		//GUI.DrawTexture (rectangleNegative, textureLifeNegative);
	}

}
