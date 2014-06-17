using UnityEngine;
using System.Collections;

public class OrcSplashAttack : ParticleDamage
{

    private GameObject owner;
    public SphereCollider sphereCollider;
    public System.Collections.Generic.List<Collider> unitList = new System.Collections.Generic.List<Collider>();
    
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
    
	// Use this for initialization
	void Awake () 
    {
        sphereCollider = GetComponent<SphereCollider>();
	}

    // Update the owner
    public void setOwner(GameObject owner)
    {
        this.owner = owner;
    }

	// Update is called once per frame
	void Update () 
    {        
        if (GetComponent<ParticleSystem>().particleCount > 0)
        {
            if (sphereCollider.radius <= 10.0f)
            {
                sphereCollider.radius +=  Time.deltaTime * 10 ;
            }
        }
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
}
