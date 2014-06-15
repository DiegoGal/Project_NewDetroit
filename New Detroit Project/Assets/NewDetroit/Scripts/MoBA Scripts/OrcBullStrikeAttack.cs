using UnityEngine;
using System.Collections;

public class OrcBullStrikeAttack : ParticleDamage
{
	private GameObject owner;
	private System.Collections.Generic.List<Collider> unitList = new System.Collections.Generic.List<Collider>();


	//--------------------------------------------------------------------------

	// Update is called once per frame
	void Update () 
	{

	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.name != owner.name)
		{
			if (other.tag == "Player")
			{
                CLife script = other.GetComponent<CLife>();
				script.Damage(GetDamage(),'P');
			}
			else if (other.tag == "Minion")
			{
				if (!unitList.Contains(other))
				{
					// For damage
					UnitController otherUC = other.GetComponent<UnitController>();
					otherUC.GetComponent<CLife>().Damage(GetDamage(), 'P');
					
					// For add a force to the minions so they can fly
					other.gameObject.AddComponent<Rigidbody>();
					other.rigidbody.isKinematic = false;
					other.rigidbody.useGravity = true;
					
					other.GetComponent<NavMeshAgent>().Stop(true);
					//Vector3 dir = new Vector3(1.0f, 1.0f, 1.0f);
					Vector3 dir = other.transform.position - transform.position;
					dir = dir.normalized;
					
					other.rigidbody.AddForce(new Vector3(dir.x * 2.0f,
					                                     5.0f,
					                                     dir.z * 2.0f),
					                         ForceMode.Impulse);
					otherUC.Fly();
					unitList.Add(other);
				}
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

