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
            }
        }
    }
}
