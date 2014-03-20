using UnityEngine;
using System.Collections;

public class OrcSplashAttack : ParticleDamage
{

    private GameObject owner;
    SphereCollider sphereCollider;
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
            sphereCollider.radius += Time.deltaTime *10;
        }
	}

    void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.name != owner.name)
        {
            if (other.tag == "Player")
            {
                HeroeController script = other.GetComponent<HeroeController>();
                script.Damage(getDamage(),'M');
            }
            else if (other.tag == "Minion")
            {
                UnitController script = other.GetComponent<UnitController>();
                script.Damage(getDamage(),'M');
            }
        }
    }
}
