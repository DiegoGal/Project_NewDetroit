using UnityEngine;
using System.Collections;

public class SkillAttack : ParticleDamage {

	public bool damagePhysic = true;
	//------------------------
	private GameObject owner;
	private System.Collections.Generic.List<Collider> unitList = new System.Collections.Generic.List<Collider>();
	private char damageType = 'P';
	
	
	//--------------------------------------------------------------------------
	
	
	public void Awake()
	{
		if (damagePhysic)
			damageType = 'P';
		else
			damageType = 'M';
	}	
	
	void OnTriggerEnter(Collider other)
	{
		Debug.Log(other.tag);
		if (other.gameObject.name != owner.name)
		{
			if (other.tag == "Minion")
			{
				if (!unitList.Contains(other))
				{
					// For damage
					UnitController otherUC = other.GetComponent<UnitController>();
					
//					photonView.RPC("Damage", PhotonTargets.All, other.gameObject.name, totalDamage);
					photonView.RPC("AddNewUnitForce", PhotonTargets.All, other.gameObject.name);
					
					unitList.Add(other);
				}
			}
			else if (other.tag == "Player")
			{
				photonView.RPC("Damage", PhotonTargets.All, other.gameObject.name, totalDamage);
			}
		}
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
	
	
	//-------------------------------------------------------------
	
	
	[RPC]
	public void Damage(string sEnemy, int damage)	
	{
		GameObject enemy = GameObject.Find(sEnemy);
//		CBasicAttributesHero cbah = enemy.GetComponent<CBasicAttributesHero>();
//		if (damagePhysic)
//			damage -= cbah.getDeffensePhysic();
//		else
//			damage -= cbah.getDeffenseMagic();
//		damage = Mathf.Max(0, totalDamage);
		
		enemy.GetComponent<CLife>().Damage(damage, damageType);
	}
	
	[RPC]
	public void AddNewUnitForce (string otherName)
	{
		GameObject other = GameObject.Find(otherName);
		if (other.GetComponent<PhotonView>().isMine)
		{
			// For damage
			UnitController otherUC = other.GetComponent<UnitController>();
			float enemyDist = Vector3.Distance(transform.position, other.transform.position);
			otherUC.GetComponent<CLife>().Damage(GetDamage() / enemyDist, damageType);
			
			// For add a force to the minions so they can fly
			if (!other.rigidbody)
				other.gameObject.AddComponent<Rigidbody>();
			other.rigidbody.isKinematic = false;
			other.rigidbody.useGravity = true;
			
			if (other.GetComponent<NavMeshAgent>() && other.GetComponent<NavMeshAgent>().enabled)
				other.GetComponent<NavMeshAgent>().Stop(true);
			Vector3 dir = other.transform.position - transform.position;
			dir = dir.normalized;
			
			other.rigidbody.AddForce(new Vector3(dir.x * 2f,
			                                     5f,
			                                     dir.z * 2f),
			                         ForceMode.Impulse);
			otherUC.Fly();
		}
	}
	
	
	//--------------------------------------------------------------------------------------
	
	
	public void setOwner(GameObject owner) { this.owner = owner; }
	
}
