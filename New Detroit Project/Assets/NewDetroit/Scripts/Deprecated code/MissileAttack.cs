using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissileAttack : ParticleDamage
{
    public GameObject owner;
    SphereCollider sphereCollider;

    public bool thrown = false;
    public List<Collider> unitList = new List<Collider>();

    private float destroyTimeAcumSplash = 0;

    // Use this for initialization
    void Awake ()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (GetComponent<ParticleSystem>() && GetComponent<ParticleSystem>().particleCount > 0)
        {
            if (sphereCollider.radius < 6.0f) 
                sphereCollider.radius += Time.deltaTime * 10;
        }

        destroyTimeAcumSplash += Time.deltaTime;
        if (destroyTimeAcumSplash >= 1.2f)
            PhotonNetwork.Destroy(this.gameObject);
    }

    public void SetOwner (GameObject owner)
    {
        this.owner = owner;
    }

    [RPC]
    public void AddNewUnitForce(string otherName)
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
            //Vector3 dir = new Vector3(1.0f, 1.0f, 1.0f);
            Vector3 dir = other.transform.position - transform.position;
            dir = dir.normalized;

            other.rigidbody.AddForce(new Vector3(dir.x * 5.0f,
                                                  9.0f,
                                                  dir.z * 5.0f),
                                                  ForceMode.Impulse);
            otherUC.Fly();
        }
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.name != owner.name)
        {
            if (other.tag == "Player")
            {
                CLife script = other.GetComponent<CLife>();
                script.Damage(GetDamage(), 'P');
            }
            else if (other.tag == "Minion")
            {
                if (!unitList.Contains(other) && 
                    owner.GetComponent<CTeam>().teamNumber != other.GetComponent<CTeam>().teamNumber)
                {
                    string name = other.name;
                    photonView.RPC("AddNewUnitForce", PhotonTargets.All, name);
                    unitList.Add(other);
                }
            }
        }
    }
}
