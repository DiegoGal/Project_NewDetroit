using UnityEngine;
using System.Collections;

public class MissileShockwaveNetwork : BasicNetwork
{
    SphereCollider collider;
    ObjectAttack missileAttack;

    public override void Awake()
    {
        base.Awake();

        collider = GetComponent<SphereCollider>();
        missileAttack = GetComponent<ObjectAttack>();

        if (photonView.isMine)
        {

        }
        else
        {
            collider.enabled = false;
            missileAttack.enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
