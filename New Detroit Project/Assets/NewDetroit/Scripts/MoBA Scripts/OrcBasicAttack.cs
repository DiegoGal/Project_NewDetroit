using UnityEngine;
using System.Collections;

public class OrcBasicAttack : MonoBehaviour {
	//==============================
	//=====     Attributes     =====
	//==============================

	//PUBLIC

	Vector3 movement; // Movement

	//PRIVATE

	private int damage; // Damage
	private GameObject owner; // Owner
	private Collider collideObject; // collide object

	//==========================
	//=====    Methods     =====
	//==========================

	//PRIVATE

	// To move the attack
	private void move()
	{
		Vector3 position = new Vector3 (
			this.transform.position.x + this.movement.x * Time.deltaTime,
			this.transform.position.y + this.movement.y * Time.deltaTime,
			this.transform.position.z + this.movement.z * Time.deltaTime);
		this.transform.position = position;
	}

	//PUBLIC

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

	// Get collide object
	public Collider getCollideObject()
	{
		return this.collideObject;
	}

	// Set collide object
	public void setCollideObject(Collider collideObject)
	{
		this.collideObject = collideObject;
	}

	// Get damage
	public int getDamage()
	{
		return this.damage;
	}

	//================================
	//=====     Main methods     =====
	//================================

	// Use this for initialization
	void Awake () 
	{
		this.damage = 50;
		// The collider is false in the begining
		this.collider.enabled = false;
		// Initialize collide object
		this.collideObject = null;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//move();
		if (owner != null)
			// If the orc is attacking
			if (!this.owner.GetComponent<HeroeController> ().getAttackInstantiate())
			{
				this.collider.enabled = false; // The collider is activated
			}
			// Else if the orc is not attacking
			else
			{
				this.collider.enabled = true; // The collider is desactivated
			}
	}

	//Detect all object that collide with this
	void OnTriggerStay(Collider collisionInfo) 
	{
		GameObject go = collisionInfo.gameObject;
		go.GetComponent<HeroeController> ().damage (this.damage); // Damage the enemy
		collideObject = collisionInfo;
		// Here we have to check that the enemy is not the owner!!!!! <---------------
	}
}
