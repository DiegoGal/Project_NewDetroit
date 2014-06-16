using UnityEngine;
using System.Collections;

public class GrenadeAndMissileShockwaveNetwork : BasicNetwork 
{
    SphereCollider collider;
    ObjectAttack grenadeAttack;

    public override void Awake()
    {
        base.Awake();

        collider = GetComponent<SphereCollider>();
        grenadeAttack = GetComponent<ObjectAttack>();

        if (photonView.isMine)
        {

        }
        else
        {
            collider.enabled = false;
            grenadeAttack.enabled = false;
        }
    }
	// Update is called once per frame
	void Update () 
    {
	
	}
}
