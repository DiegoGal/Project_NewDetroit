using UnityEngine;
using System.Collections;

public class OrcSplashAttack : ParticleDamage
{

    private GameObject owner;
    SphereCollider sphereCollider;
    public System.Collections.Generic.List<Collider> unitList = new System.Collections.Generic.List<Collider>();
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
}
