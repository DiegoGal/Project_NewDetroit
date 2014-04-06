using UnityEngine;
using System.Collections;

public class UnitBasicArtillery : UnitArtillery
{

    public float attackPower1 = 10.0f;
    public float attackPower2 = 20.0f;

    public float attack1Cadence = 1.0f;
    public float attack2Cadence = 0.67f;

    private GameObject leftWeapon, rightWeapon, baseballBat;
    public Transform dummyBat;

    public override void Awake ()
    {
        base.Awake();

        numberOfWeapons = 2;
        // Por si no se han establecido las referencias a los dummys del modelo
        // en el editor de Unity las buscamos ahora:
        if (dummyLeftWeapon == null)
            dummyLeftWeapon = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand/Mano IZQ/WeaponLeft");
        if (dummyRightWeapon == null)
            dummyRightWeapon = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Mano DER/WeaponRight");
        if (dummyLeftWeaponGunBarrel == null)
            dummyLeftWeaponGunBarrel = dummyLeftWeapon.FindChild("GunBarrelLeft");
        if (dummyRightWeaponGunBarrel == null)
            dummyRightWeaponGunBarrel = dummyRightWeapon.FindChild("GunBarrelRight");
        if (dummyBat == null)
            dummyBat = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Arma Blanca/Cylinder002");

        if (dummyLeftWeapon)
            leftWeapon = dummyLeftWeapon.gameObject;
        if (dummyRightWeapon)
            rightWeapon = dummyRightWeapon.gameObject;
        if (dummyBat)
            baseballBat = dummyBat.gameObject;
    }

	// Use this for initialization
    public override void Start ()
	{
        base.Start();

        basicAttackPower = attackPower1;
        secondaryAttackPower = attackPower2;

        primaryAttackCadence = attack1Cadence;
        secondaryAttackCadence = attack2Cadence;
	}
	
	// Update is called once per frame
    public override void Update ()
	{
		base.Update();

        if (isSelected && Input.GetKeyDown(KeyCode.D))
        {
            //Debug.Break();
            if (attack2Selected) // change to attack1
            {
                maxAttackDistance = visionSphereRadious;
                attackCadence = attack1Cadence;
                    
                attack2Selected = false;
            }
            else // change to attack2
            {
                maxAttackDistance = maxAttackDistance2;
                attackCadence = attack2Cadence;

                attack2Selected = true;
            }
        }
	}

    protected override void UpdateGoingToAnEnemy ()
    {
        // si esta seleccionado el ataque con bate se llama a la clase base
        if (attack2Selected)
            base.UpdateGoingToAnEnemy();
        // si esta seleccionado el ataque a distancia miramos si la unidad
        // de verdad "ve" al enemigo seleccionado
        else if (currentArtilleryState == ArtilleryState.Alert)
        {
            if (alertHitTimerAux <= 0)
            {
                SearchForAnEnemy();
                // reset the timer
                alertHitTimerAux = alertHitTimer;
            }
            else
                alertHitTimerAux -= Time.deltaTime;
        }
    }

    protected override void PlayAnimationCrossFade (string animationName)
    {
        if ( (animationName == "Attack1") && attack2Selected )
            animation.CrossFade("Attack2");
        else
            base.PlayAnimationCrossFade(animationName);
    }

    public override int GetUnitType()
    {
        return 1;
    }

    protected override void RemoveAssetsFromModel ()
    {
        if (leftWeapon)
            Destroy(leftWeapon);
        if (rightWeapon)
            Destroy(rightWeapon);
        if (baseballBat)
            Destroy(baseballBat);
    }

} // class UnitBasicArtillery
