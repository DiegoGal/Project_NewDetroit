using UnityEngine;
using System.Collections;

public class SnotAttack : ParticleDamage {

	GameObject owner;
	
	public void SetOwner(GameObject owner)
	{
		this.owner = owner;
	}
	
	[RPC]
	public void Damage(string sEnemy, int damage)	
	{
		GameObject enemy = GameObject.Find(sEnemy);
		enemy.GetComponent<CLife>().Damage(damage, 'M');
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnParticleCollision(GameObject other)
	{
		if (owner.name != other.name)
		{
			CLife goCLife = other.GetComponent<CLife>();
			if (goCLife == null) return;
			
			photonView.RPC("Damage", PhotonTargets.All, other.name, totalDamage);
		}
	}
}
