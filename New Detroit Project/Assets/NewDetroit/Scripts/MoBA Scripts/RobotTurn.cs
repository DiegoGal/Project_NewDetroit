using UnityEngine;
using System.Collections;

public class RobotTurn : ParticleDamage {

	private GameObject owner;
	private System.Collections.Generic.List<Collider> unitList;
	private float timeToTurn = 1f;

	// Use this for initialization
	void Start () {
		unitList = new System.Collections.Generic.List<Collider>();
	}
	
	// Update is called once per frame
	void Update () {

		if (timeToTurn <= 0) 
		{
			if (GetComponent<SphereCollider>().radius <= 6.0f)
			{
				GetComponent<SphereCollider>().radius +=  Time.deltaTime * 4;
			}
		}
		else
			timeToTurn -= Time.deltaTime;


	}

	void OnTriggerEnter(Collider other)
	{
		if (owner != null && other.gameObject.name != owner.name)
		{
			if (other.tag == "Player")
			{
				CLife script = other.GetComponent<CLife>();
				script.Damage(GetDamage(),'M');
			}
			else if (other.tag == "Minion")
			{
				if (!unitList.Contains(other))
				{
					// For damage
					UnitController otherUC = other.GetComponent<UnitController>();
					otherUC.GetComponent<CLife>().Damage(GetDamage(), 'M');
					
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

	public void setOwner(GameObject owner) { this.owner = owner; }
	public void setTimeToTurn(float timeToTurn) { this.timeToTurn = timeToTurn; }
}
