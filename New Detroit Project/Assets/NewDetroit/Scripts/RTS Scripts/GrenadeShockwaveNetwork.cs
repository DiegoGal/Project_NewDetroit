using UnityEngine;
using System.Collections;

public class GrenadeShockwaveNetwork : BasicNetwork 
{
    SphereCollider collider;
    GrenadeAttack grenadeAttack;

    public override void Awake()
    {
        base.Awake();

        collider = GetComponent<SphereCollider>();
        grenadeAttack = GetComponent<GrenadeAttack>();

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
