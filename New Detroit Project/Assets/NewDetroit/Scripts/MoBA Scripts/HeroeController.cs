using UnityEngine;
using System.Collections;

public abstract class HeroeController : MonoBehaviour 
{
	//==============================
	//=====     Attributes     =====
	//==============================

	public GameObject leftArm, rightArm;

	private bool attackInstantiate;

	private const int LEVEL_1_2 = 200,
					LEVEL_2_3 = 600,
					LEVEL_3_4 = 1000;
	protected int life, attackP, attackM, defP, defM, mana, adren, speedMov, level, experience;
	protected double speedAtt;
	protected bool hasNewLevel;

	protected Animator animator; //Animator

	//===========================
	//=====     Methods     =====
	//===========================

	//PROTECTED

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

	//PUBLIC

	public bool getAttackInstantiate()
	{
		return this.attackInstantiate;
	}

	public void setAttackInstantiate(bool attackInstantiate)
	{
		this.attackInstantiate = attackInstantiate;
	}

	public void damage(int life)
	{
		this.life -= life;
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
	}
	
	// Update is called once per frame
	virtual public void Update () {
		// If the hero is attacking (may be, we should do that in OrcController)
		if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") ||
		    animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") ||
		    animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3")) &&
		    animator.GetBool("isAttacking"))
		{
			//launchAttack();
			if (!attackInstantiate)
			{
				this.attackInstantiate = true;
			}
		}
		// Else if the hero is not attacking
		else
		{
			this.attackInstantiate = false;
		}
	}

	void OnGUI ()
	{
		Vector3 pos = Camera.main.WorldToScreenPoint (transform.position);
		GUI.Label (new Rect (pos.x + 20, Screen.height - pos.y, 100, 50), "Vida: " + this.life);
		if (Input.GetMouseButton (1))
			GUI.Label (new Rect (0, 0, 100, 50), "Attack!!!");
	}
}
