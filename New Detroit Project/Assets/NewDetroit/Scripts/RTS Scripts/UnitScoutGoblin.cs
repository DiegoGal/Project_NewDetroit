using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitScoutGoblin : UnitScout
{

	// Explosion particles references
	public GameObject particlesExplosionSmoke;
    public GameObject particlesExplosionFire;
    public GameObject particlesExplosionPieces;
	
    // Explosion particles instances
	private GameObject explosionSmokeInst;
    private GameObject explosionFireInst;
	private GameObject explosionPiecesInst;

    public override void Awake()
    {
         base.Awake();

         if (!mount)
             mount = transform.FindChild("Box002").gameObject;

         type = TypeHeroe.Orc;
    }

    public void UnitDamageMessage ()
    {
        if ( !afire && getLife() <= (GetMaximunLife() * startAfire) )
        {
            // la vida es muy baja, instanciar el fuego
            fireMountInst = Instantiate
            (
                fireMount,
                mount.transform.position,
                mount.transform.rotation
            ) as GameObject;
            fireMountInst.transform.parent = mount.transform;

            afire = true;
        }
    }

    public override void UnitDiedMessage()
    {
        base.UnitDiedMessage();

        if (mount)
            Destroy(mount);

        explosionFireInst = Instantiate
        (
            particlesExplosionFire,
            transform.position,
            transform.rotation
        ) as GameObject;

        explosionSmokeInst = Instantiate
        (
            particlesExplosionSmoke,
            transform.position,
            new Quaternion(0f, 180f, 180f, 0f)
        ) as GameObject;

        explosionPiecesInst = Instantiate
        (
            particlesExplosionPieces,
            transform.position,
            new Quaternion(0f, 180f, 180f, 0f)
        ) as GameObject;

        Destroy(explosionFireInst, 2.5f);
        Destroy(explosionSmokeInst, 2.5f);
        Destroy(explosionPiecesInst, 2.5f);
    }

}
