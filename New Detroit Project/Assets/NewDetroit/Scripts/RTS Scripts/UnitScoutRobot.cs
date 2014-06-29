using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitScoutRobot : UnitScout
{

	// Explosion particles references
	public GameObject particlesExplosionSmoke;
    public GameObject particlesExplosionFire;
    public GameObject particlesExplosionPieces;
	
    // Explosion particles instances
	private GameObject explosionSmokeInst;
    private GameObject explosionFireInst;
	private GameObject explosionPiecesInst;

    // referente to the normal fire particles
    private GameObject fireParticles;

    public override void Awake()
    {
        base.Awake();

        if (!mount)
            mount = transform.FindChild("Nave").gameObject;

        fireParticles = transform.FindChild("FireMower2").gameObject;

        type = TypeHeroe.Robot;
    }

    public void UnitDamageMessage()
    {
        if (!afire && getLife() <= (GetMaximunLife() * startAfire))
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

        /*if (mount)
            Destroy(mount);*/
        RemoveAssetsFromModel();
    }

    protected override void RemoveAssetsFromModel()
    {
        base.RemoveAssetsFromModel();

        if (fireParticles)
            Destroy(fireParticles);

        StartCoroutine(DestroyShip(1.25f));
    }

    private IEnumerator DestroyShip (float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (mount)
        {
            explosionFireInst = Instantiate
            (
                particlesExplosionFire,
                mount.transform.position,
                mount.transform.rotation
            ) as GameObject;

            explosionSmokeInst = Instantiate
            (
                particlesExplosionSmoke,
                mount.transform.position,
                new Quaternion(0f, 180f, 180f, 0f)
            ) as GameObject;

            explosionPiecesInst = Instantiate
            (
                particlesExplosionPieces,
                mount.transform.position,
                new Quaternion(0f, 180f, 180f, 0f)
            ) as GameObject;

            Destroy(explosionFireInst, 2.5f);
            Destroy(explosionSmokeInst, 2.5f);
            Destroy(explosionPiecesInst, 4.0f);

            Destroy(mount);
        }
    }

}
