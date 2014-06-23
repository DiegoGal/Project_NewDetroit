using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectAttack : ParticleDamage
{
    public GameObject owner;
    SphereCollider sphereCollider;

    public bool thrown = false;
    public List<Collider> unitList = new List<Collider>();

    private float destroyTimeAcumSplash = 0;

    private float xForce, yForce, zForce;
    public float maxTime;

    public int typeOfObject; // 0 engineer, 1 heavyArtillery

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
            if (sphereCollider.radius < 4.0f) 
                sphereCollider.radius += Time.deltaTime * 10;
        }

        destroyTimeAcumSplash += Time.deltaTime;
        if (destroyTimeAcumSplash >= 1.2f)
            if (PhotonNetwork.connected) 
                PhotonNetwork.Destroy(this.gameObject);
            else
                Destroy(this.gameObject);
    }

    public void SetOwner (GameObject owner)
    {
        this.owner = owner;
    }

    [RPC]
    public void DamageHeroe(string otherName, int damage)
    {
        GameObject other = GameObject.Find(otherName);
        CLife otherCL = other.GetComponent<CLife>();
        float enemyDist = Vector3.Distance(transform.position, other.transform.position);
        otherCL.Damage(damage / enemyDist, 'P');
    }

    [RPC]
    public void AddNewUnitForce (string otherName, float xForce, float yForce, float zForce, int damage)
    {
        GameObject other = GameObject.Find(otherName);
        // For damage
        UnitController otherUC = other.GetComponent<UnitController>();
        float enemyDist = Vector3.Distance(transform.position, other.transform.position);
        otherUC.GetComponent<CLife>().Damage(damage / enemyDist, 'P');

        if (other.GetComponent<PhotonView>().isMine)
        {
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

    void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.name != owner.name)
        {
            string name = other.name;
            if (other.tag == "Player")
            {
                /*CLife script = other.GetComponent<CLife>();
                script.Damage(GetDamage(), 'P');*/
                if (PhotonNetwork.connected)
                    photonView.RPC("DamageHeroe", PhotonTargets.All, name, GetDamage());
                else
                {
                    CLife script = other.GetComponent<CLife>();
                    script.Damage(GetDamage(), 'P');
                }
            }
            else if (other.tag == "Minion")
            {
                if (!unitList.Contains(other) && 
                    owner.GetComponent<CTeam>().teamNumber != other.GetComponent<CTeam>().teamNumber)
                {                    
                    float xForce = 0, yForce = 0, zForce = 0;
                    switch (typeOfObject)
                    {
                        case 0:
                            xForce = 0.7f;
                            yForce = 3.5f;
                            zForce = 0.7f;
                            break;
                        case 1:
                            xForce = 5.0f;
                            yForce = 9.0f;
                            zForce = 5.0f;
                            break;

                    }
                    if (typeOfObject == 0)
                    {
                        xForce = 0.7f;
                        yForce = 3.5f;
                        zForce = 0.7f;
                    }
                    if (PhotonNetwork.connected)
                        photonView.RPC("AddNewUnitForce", PhotonTargets.All, name, xForce, yForce, zForce, GetDamage());
                    else
                    {
                        
                        // For damage
                        UnitController otherUC = other.GetComponent<UnitController>();
                        float enemyDist = Vector3.Distance(transform.position, other.transform.position);
                        otherUC.GetComponent<CLife>().Damage(GetDamage() / enemyDist, 'P');

                        if (other.GetComponent<PhotonView>().isMine || !(PhotonNetwork.connected))
                        {
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
                    unitList.Add(other);
                }
            }
        }
    }

}
