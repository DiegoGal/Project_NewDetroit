using UnityEngine;
using System.Collections;

public class OrcBullStrikeAttack : ParticleDamage
{
	private GameObject owner;
	private System.Collections.Generic.List<Collider> unitList = new System.Collections.Generic.List<Collider>();


	//--------------------------------------------------------------------------

	[RPC]
	public void Damage(string sEnemy, int damage)	
	{
		GameObject enemy = GameObject.Find(sEnemy);
		enemy.GetComponent<CLife>().Damage(damage, 'M');
	}
	
	[RPC]
	public void AddNewUnitForce (string otherName, float xForce, float yForce, float zForce)
	{
		GameObject other = GameObject.Find(otherName);
		if (other.GetComponent<PhotonView>().isMine)
		{
			// For damage
			UnitController otherUC = other.GetComponent<UnitController>();
			float enemyDist = Vector3.Distance(transform.position, other.transform.position);
			otherUC.GetComponent<CLife>().Damage(GetDamage() / enemyDist, 'P');
			
			// For add a force to the minions so they can fly
			if (!other.rigidbody)
				other.gameObject.AddComponent<Rigidbody>();
			other.rigidbody.isKinematic = false;
			other.rigidbody.useGravity = true;
			
			if (other.GetComponent<NavMeshAgent>() && other.GetComponent<NavMeshAgent>().enabled)
				other.GetComponent<NavMeshAgent>().Stop(true);
			Vector3 dir = other.transform.position - transform.position;
			dir = dir.normalized;
			
			other.rigidbody.AddForce(new Vector3(dir.x * xForce,
			                                     yForce,
			                                     dir.z * zForce),
			                         ForceMode.Impulse);
			otherUC.Fly();
		}
	}

	// Update is called once per frame
	void Update () 
	{

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
					
					photonView.RPC("Damage", PhotonTargets.All, other.gameObject.name, totalDamage);
					
					
					Vector3 dir = other.transform.position - transform.position;
					dir = dir.normalized;
					photonView.RPC("AddNewUnitForce", PhotonTargets.All, dir.x * 2.0f, 5.0f, dir.z * 2.0f);
				}
			}
			else
			{
				photonView.RPC("Damage", PhotonTargets.All, other.gameObject.name, totalDamage);
			}
		}
	}


	//--------------------------------------------------------------------------------------


	// Update the owner
	public void setOwner(GameObject owner)
	{
		this.owner = owner;
	}
}

