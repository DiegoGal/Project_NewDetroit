using UnityEngine;
using System.Collections;

public class UnitBasicArtillery : UnitArtillery
{

    public float attackPower1 = 10.0f;
    public float attackPower2 = 20.0f;

    public override void Awake()
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
    }

	// Use this for initialization
    public override void Start ()
	{
        base.Start();

        basicAttackPower = attackPower1;
        secondaryAttackPower = attackPower2;
	}
	
	// Update is called once per frame
    public override void Update ()
	{
		base.Update();
	}

} // class UnitBasicArtillery
