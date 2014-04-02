using UnityEngine;
using System.Collections;


public class FireballAttack : ParticleDamage
{
    public GameObject owner;
    SphereCollider sphereCollider;

    public bool thrown = false;
    public int damage;

    // Use this for initialization
    void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<ParticleSystem>() && GetComponent<ParticleSystem>().particleCount > 0)
        {
            if (sphereCollider.radius < 4.0f) 
                sphereCollider.radius += Time.deltaTime * 10;
        }
    }

    public void SetOwner(GameObject owner)
    {
        this.owner = owner;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name != owner.name)
        {
            if (other.tag == "Player")
            {
                HeroeController script = other.GetComponent<HeroeController>();
                script.Damage(damage, 'P');
            }
            else if (other.tag == "Minion")
            {
                UnitController script = other.GetComponent<UnitController>();
                script.Damage(damage, 'P');
                
                // For add a force to the minions so they can fly
                other.gameObject.AddComponent<Rigidbody>();
                other.rigidbody.isKinematic = false;
                other.rigidbody.useGravity = true;

                other.GetComponent<NavMeshAgent>().Stop(true);
                Vector3 dir = new Vector3(1.0f, 1.0f, 1.0f);
                dir = dir.normalized;

                other.rigidbody.AddForce(new Vector3(dir.x * 0.0f,
                                                      dir.y * 7.0f,
                                                      dir.z * 0.0f), 
                                                      ForceMode.Impulse);
                //other.GetComponent<NavMeshAgent>().Resume(); 
            }
        }
    }
}
