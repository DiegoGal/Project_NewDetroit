using UnityEngine;
using System.Collections;

public abstract class HeroeController : MonoBehaviour 
{
	//==============================
	//=====     Attributes     =====
	//==============================

	// The state of the heroe
	public enum StateHeroe
	{
		Dead, // When is dead
		Recover, // After dead, he must be recovered
		IdleWalkRun, // When he is doing nothing but walking or running, or idle
		AttackBasic // When he is attacking with his basic attack 
	}

	public GameObject leftArm, rightArm;
	public Texture2D 	textureLifePositive, 
						textureLifeNegative;

	protected bool attackInstantiate;

	private const int LEVEL_1_2 = 200, // experience to improve from leve 1 to 2
					LEVEL_2_3 = 600, // experience to improve from leve 2 to 3
					LEVEL_3_4 = 1000; // experience to improve from leve 3 to 4

	protected int life, totalLife, attackP, attackM, defP, defM, mana, adren, speedMov, level, experience; //Attributes
	protected double speedAtt; // Attribute
	protected bool hasNewLevel; // Tell us if the heroe has evolved  or not

	protected Animator animator; //Animator

	protected bool isMine; // Tell us if that instance if ours or not

	protected Vector3 initialPosition; // The spawn position

	protected StateHeroe state; // The state of the heroe

	private double timeCount; // Time counter

	//===========================
	//=====     Methods     =====
	//===========================

	//=========================
	//====    PROTECTED    ====
	//=========================

	// Increment the level
	protected void levelUp() 
	{
		this.level ++;
		this.hasNewLevel = true;
	}

	// Increment the life
	protected void lifeUp(int life)
	{
		this.life += life;
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

	// Increment the experience
	protected void experienceUp(int experience)
	{
		this.experience += experience;
		if (this.experience > LEVEL_3_4) this.experience = LEVEL_3_4;

		if (this.level == 1 && this.experience >= LEVEL_1_2) this.levelUp ();
		else if (this.level == 2 && this.experience >= LEVEL_2_3) this.levelUp ();
		else if (this.level == 3 && this.experience >= LEVEL_3_4) this.levelUp ();
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
				GameObject go = (GameObject) Instantiate(rightArm, position, new Quaternion());
				//go.GetComponent<Attack>().setMovement(direction);
				go.GetComponent<OrcBasicAttack>().setOwner(this.gameObject);

				//this.GetComponent<OrcBasicAttack> ().enable(false);
			}
		//}
	}

	// Check dead condition
	protected void checkDead()
	{
		// if is dead
		if (this.life <= 0 && this.state != StateHeroe.Dead && this.state != StateHeroe.Recover) 
		{
			this.life = 0;
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
				this.life += 20;
				if (this.life >= this.totalLife)
				{
					this.life = this.totalLife;
					this.state = StateHeroe.IdleWalkRun;
					this.GetComponent<ThirdPersonController>().enabled = true;
				}
			}
		}
	}

	//======================
	//====    PUBLIC    ====
	//======================

	// Return if the heroe is attacking or not
	public bool isAttackBasic()
	{
		//return this.attackInstantiate;
		return this.state == StateHeroe.AttackBasic;
	}

	// Damage the heroe
	public void damage(int life)
	{
		this.life -= life;
	}

	// Return the current life of the heroe
	public int getLife()
	{
		return this.life;
	}

	// Set the lide of the heroe
	public void setLife(int life)
	{
		this.life = life;
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

	//================================
	//=====     Main methods     =====
	//================================

	// Use this for initialization
	virtual public void Start () {
		//Set the owner in the basic attack
		this.rightArm.GetComponent<OrcBasicAttack> ().setOwner (this.gameObject);
		this.leftArm.GetComponent<OrcBasicAttack> ().setOwner (this.gameObject);
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
	}
	
	// Update is called once per frame
	virtual public void Update () {
		//launchAttack();

		checkDead ();
		checkRecover ();
	}

	void OnGUI ()
	{
		Vector3 pos = Camera.main.WorldToScreenPoint (transform.position);
		GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y, 100, 50), "Vida: " + this.life);
		GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 20, 100, 50), "Mine: " + this.isMine);
		GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 40, 100, 50), "State: " + this.state);

		float positiveLife = (float) this.life / this.totalLife; // percentage of positive life
		float negativeLife = 1 - positiveLife; //percentage of negative life
		Rect rectanglePositive = new Rect (pos.x - 50, pos.y - 150, 100 * positiveLife, 10);
		Rect rectangleNegative = new Rect (pos.x - 50 + 100 * positiveLife, pos.y - 150, 100 * negativeLife, 10);
		GUI.DrawTexture (rectanglePositive, textureLifePositive); 
		GUI.DrawTexture (rectangleNegative, textureLifeNegative); 
	}
}
