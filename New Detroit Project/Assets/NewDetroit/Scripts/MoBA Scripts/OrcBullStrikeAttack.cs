using UnityEngine;
using System.Collections;

public class OrcBullStrikeAttack : ParticleDamage
{
	private SphereCollider sphereCollider;
	//private GameObject owner;
	private System.Collections.Generic.List<Collider> unitList = new System.Collections.Generic.List<Collider>();

	//--------------------------------------------------------------------------


	// Update the owner
	/*public void setOwner(GameObject owner)
	{
		this.owner = owner;
	}*/

	//Enable the sphere collider
	public void EnableSphereCollider()
	{
		sphereCollider.enabled = true;
	}

	//Disable the sphere collider
	public void DisableSphereCollider()
	{
		sphereCollider.enabled = false;
		unitList.Clear ();
	}

	//--------------------------------------------------------------------------


	// Use this for initialization
	void Awake () 
	{
		sphereCollider = GetComponent<SphereCollider>();
		sphereCollider.enabled = false;
		sphereCollider.radius = 1f;
	}

	// Update is called once per frame
	void Update () 
	{
	}

	void OnTriggerEnter(Collider other)
	{
		//if (other.gameObject.name != owner.name)
		if (other.gameObject.name != name)
		{
			if (other.tag == "Player")
			{
				HeroeController script = other.GetComponent<HeroeController>();
				script.Damage(GetDamage(),'P');
			}
			else if (other.tag == "Minion")
			{
				if (!unitList.Contains(other))
				{
					// For damage
					UnitController otherUC = other.GetComponent<UnitController>();
					otherUC.Damage(GetDamage(), 'P');
					
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
}

