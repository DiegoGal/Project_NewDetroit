using UnityEngine;
using System.Collections;

public class RobotTurn : ParticleDamage {

	private GameObject owner;
	private System.Collections.Generic.List<Collider> unitList;
	private float timeToTurn = 1f;


	//------------------------------------------------------------


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
				photonView.RPC("Damage", PhotonTargets.All, other.gameObject.name, totalDamage);
			}
			else if (other.tag == "Minion")
			{
				if (!unitList.Contains(other))
				{
					// For damage
					UnitController otherUC = other.GetComponent<UnitController>();

					photonView.RPC("Damage", PhotonTargets.All, other.gameObject.name, totalDamage);
					photonView.RPC("AddNewUnitForce", PhotonTargets.All, other.gameObject.name);

					unitList.Add(other);
				}
			}
		}
	}


	//-----------------------------------------------------------------


	[RPC]
	public void Damage(string sEnemy, int damage)	
	{
		GameObject enemy = GameObject.Find(sEnemy);
		enemy.GetComponent<CLife>().Damage(damage, 'M');
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
			
			other.rigidbody.AddForce(new Vector3(dir.x * 2f,
			                                     5f,
			                                     dir.z * 2f),
			                         ForceMode.Impulse);
			otherUC.Fly();
		}
	}


	//-----------------------------------------------------------------


	public void setOwner(GameObject owner) { this.owner = owner; }
	public void setTimeToTurn(float timeToTurn) { this.timeToTurn = timeToTurn; }
}
