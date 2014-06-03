using UnityEngine;
using System.Collections;

public class OrcBasicAttack : MonoBehaviour {
	public Vector3 movement; // Movement
	
	//private int damage; // Damage
	public GameObject owner; // Owner
	private bool hasCollided; // Tell us if the attack has collided with something
	private string nameCollide; // Tell us the name of the collided object
	private int lifeCollide; // Tell us the life of the collided object
	
	
	//------------------------------------------------------------------------------------------
	// PRIVATE
	// To move the attack
	private void move()
	{
		Vector3 position = new Vector3 (
			this.transform.position.x + this.movement.x * Time.deltaTime,
			this.transform.position.y + this.movement.y * Time.deltaTime,
			this.transform.position.z + this.movement.z * Time.deltaTime);
		this.transform.position = position;
	}
	
	
	//------------------------------------------------------------------------------------------
	// PUBLIC
	// Update the movement vector
	public void setMovement(Vector3 movement)
	{
		this.movement = movement;
	}
	
	// Update the owner
	public void setOwner(GameObject owner)
	{
		this.owner = owner;
	}
	
	// Get hasCollided
	public bool getHasCollided()
	{
		return this.hasCollided;
	}
	
	// Get the name
	public string getNameCollide()
	{
		return this.nameCollide;
	}
	
	// Get the name that de object that has collided and put hasCollided to false
	public string getNameCollideOnce()
	{
		this.hasCollided = false;
		return this.nameCollide;
	}
	
	// Get the life of the collided object
	public int getLifeCollide()
	{
		return lifeCollide;
	}
	
	
	//------------------------------------------------------------------------------------------
	// MAIN
	// Use this for initialization
	void Awake () 
	{
		this.collider.enabled = true;
		this.hasCollided = false;
		nameCollide = "";
		lifeCollide = 0;
		//this.GetComponent<MeshRenderer> ().enabled = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//move();
		Debug.Log (collider.enabled);

		if (this.owner != null && this.owner.GetComponent<HeroeController> ().isMine)
		{
			if (owner.GetComponent<HeroeController>().getTypeHero() == HeroeController.TypeHeroe.Robot && owner.animation.IsPlaying("Attack3"))
			{
				//this.collider.enabled = false;
			}
			// If the orc is not in basic attack11
			else if (this.owner.GetComponent<HeroeController> ().getState() != HeroeController.StateHeroe.AttackBasic)
			{
				//this.collider.enabled = false; // The collider is activated
			}
			// Else if the orc is attacking
			else
			{
				//this.collider.enabled = true; // The collider is desactivated
			}
		}
	}
	
	//Detect all object that collide with this
	void OnTriggerEnter (Collider collisionInfo)
	{
		GameObject go = collisionInfo.gameObject;
		nameCollide = go.name;
		if (nameCollide != this.owner.name)
		{
            CLife goCLife = go.GetComponent<CLife>();
            if (goCLife == null) return;
            if (goCLife.Damage(this.owner.GetComponent<HeroeController>().getAttackP(), 'P')) // Damage the enemy and check if it is dead
			{
                ControllableCharacter cchar = goCLife.GetComponent<ControllableCharacter>();
                // TODO! esto solo recibe la experiencia en offline
                if (cchar)
                    owner.GetComponent<HeroeController>().experienceUp(cchar.experienceGived);
			}
			hasCollided = true;
            lifeCollide = (int)goCLife.getLife();
		}
		// Here we have to check if the collide object is a heroe or a unit from RTS game!!!!! <---------------
	}
}
