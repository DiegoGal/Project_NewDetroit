using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireballAttack : ParticleDamage
{
    public GameObject owner;
    SphereCollider sphereCollider;

    public bool thrown = false;
    public List<Collider> unitList = new List<Collider>();

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


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name != owner.name)
        {
            if (other.tag == "Player")
            {
                HeroeController script = other.GetComponent<HeroeController>();
                script.Damage(GetDamage(), 'P');
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
