using UnityEngine;
using System.Collections;

public class BasicAttack : Photon.MonoBehaviour {
	public GameObject owner;

	public Vector3 movement; // Movement
	
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
	[RPC]
	public void Damage(string sEnemy, int damage)	
	{
		Debug.Log("Ataco con " + damage + " de fuerza");
		GameObject enemy = GameObject.Find(sEnemy);
		enemy.GetComponent<CLife>().Damage(damage, 'P');
	}
	
	//------------------------------------------------------------------------------------------
	//MAIN
	// Use this for initialization
	void Start () {
		GetComponent<MeshRenderer> ().enabled = false;
		collider.enabled = false;
	}


	void OnTriggerEnter (Collider collisionInfo){
		GameObject go = collisionInfo.gameObject;
		if (go.name != this.owner.name)
		{
			CLife goCLife = go.GetComponent<CLife>();
			if (goCLife == null) return;
			
			photonView.RPC("Damage", PhotonTargets.All, go.name, owner.GetComponent<HeroeController>().getAttackP());
//			if (goCLife.Damage(owner.GetComponent<HeroeController>().getAttackP(), 'P')) // Damage the enemy and check if it is dead
//			{
//				ControllableCharacter cchar = goCLife.GetComponent<ControllableCharacter>();
//				// TODO! only works in offline mode
//				if (cchar) owner.GetComponent<HeroeController>().experienceUp(cchar.experienceGived);
//			}
		}
	}
}
