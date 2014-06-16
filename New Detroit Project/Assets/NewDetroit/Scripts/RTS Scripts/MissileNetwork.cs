using UnityEngine;
using System.Collections;

public class MissileNetwork : BasicNetwork
{

    CapsuleCollider capsuleColl;
    Rigidbody rigidB;
    CMissileVisionCapsule visionCapsule;


    public override void Awake()
    {
        base.Awake();
        capsuleColl = GetComponent<CapsuleCollider>();
        rigidB = GetComponent<Rigidbody>();
        visionCapsule = GetComponent<CMissileVisionCapsule>();

        if (photonView.isMine)
        {
            capsuleColl.enabled = true;
            visionCapsule.enabled = true;
        }
        else
        {
            capsuleColl.enabled = false;
            Destroy(rigidB);
            visionCapsule.enabled = false;
        }
    }

    // Update is called once per frame
    //	void Update () {
    //	
    //	}
}