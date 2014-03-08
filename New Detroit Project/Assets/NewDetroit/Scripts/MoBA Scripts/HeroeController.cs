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
	private int counterAbility;
	
	
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
	public override bool Damage (float damage)
	{
		currentLife -= damage - defP;
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
	
	void OnGUI ()
	{
		// DEBUG
		Vector3 pos = Camera.main.WorldToScreenPoint (transform.position);
		GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y, 200, 50), "Vida: " + this.currentLife);
		GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 10, 200, 50), "Experience: " + this.experience);
		GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 20, 200, 50), "Level: " + this.level);
		GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 30, 200, 50), "Maxima vida: " + this.maximunLife);
		GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 40, 200, 50), "Ataque fisico: " + this.attackP);
		GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 50, 200, 50), "Ataque magico: " + this.attackM);
		GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 60, 200, 50), "Velocidad ataque: " + this.speedAtt);
		GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 70, 200, 50), "Defensa fisica: " + this.defP);
		GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 80, 200, 50), "Defensa magica: " + this.defM);
		GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 90, 200, 50), "Mana: " + this.currentMana);
		GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 100, 200, 50), "Maximo Mana: " + this.mana);
		GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 110, 200, 50), "Adrenalina: " + this.currentAdren);
		GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 120, 200, 50), "Maxima Adrenalina: " + this.adren);
		GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y + 130, 200, 50), "Velocidad mvto: " + this.speedMov);
		
		//-------------------------------------------------------------------------------------------------
		// Life, Mana and Adrenaline
		float distance = Vector3.Distance (transform.position, Camera.main.transform.position); // real distance from camera
		float lengthLife = this.GetComponent<ThirdPersonCamera> ().distance / distance; // percentage of the distance
		// Position of the life in the screen
		Vector3 posLifeScene = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		posLifeScene.y += 2f;
		Vector3 posLife = Camera.main.WorldToScreenPoint (posLifeScene);
		Vector3 posAdrenScene = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		posAdrenScene.y += 1.7f;
		Vector3 posAdren = Camera.main.WorldToScreenPoint (posAdrenScene);
		Vector3 posManaScene = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		posManaScene.y += 1.55f;
		Vector3 posMana = Camera.main.WorldToScreenPoint (posManaScene);

		// Life
		float positiveLife = (float) this.currentLife / this.maximunLife; // percentage of positive life
		float negativeLife = 1 - positiveLife; //percentage of negative life
		Rect rectanglePositiveLife = new Rect (posLife.x - 50 * lengthLife, Screen.height - posLife.y, 100 * positiveLife * lengthLife, 10 * lengthLife);
		Rect rectangleNegativeLife = new Rect (posLife.x - 50 * lengthLife + 100 * positiveLife * lengthLife, Screen.height - posLife.y,
		                                       100 * negativeLife * lengthLife, 10 * lengthLife);
		// Adrenaline
		float positiveAdren = (float) this.currentAdren / this.adren;
		float negativeAdren = 1 - positiveAdren;
		Rect rectanglePositiveAdren = new Rect (posAdren.x - 50 * lengthLife, Screen.height - posAdren.y, 100 * positiveAdren * lengthLife, 5 * lengthLife);
		Rect rectangleNegativeAdren = new Rect (posAdren.x - 50 * lengthLife + 100 * positiveAdren * lengthLife, Screen.height - posAdren.y, 
		                                        100 * negativeAdren * lengthLife, 10 * lengthLife);
		// Mana
		float positiveMana = (float) this.currentMana / this.mana;
		float negativeMana = 1 - positiveMana;
		Rect rectanglePositiveMana = new Rect (posMana.x - 50 * lengthLife, Screen.height - posMana.y, 100 * positiveMana * lengthLife, 5 * lengthLife);
		Rect rectangleNegativeMana = new Rect (posMana.x - 50 * lengthLife + 100 * positiveMana * lengthLife, Screen.height - posMana.y, 
		                                       100 * negativeMana * lengthLife, 10 * lengthLife);

		GUI.DrawTexture (rectanglePositiveLife, textureLifePositive);
		GUI.DrawTexture (rectangleNegativeLife, textureLifeNegative);
		GUI.DrawTexture (rectanglePositiveAdren, textureAdrenPositive);
		GUI.DrawTexture (rectangleNegativeAdren, textureAdrenNegative);
		GUI.DrawTexture (rectanglePositiveMana, textureManaPositive);
		GUI.DrawTexture (rectangleNegativeMana, textureManaNegative);
	}//OnGUI	
}
