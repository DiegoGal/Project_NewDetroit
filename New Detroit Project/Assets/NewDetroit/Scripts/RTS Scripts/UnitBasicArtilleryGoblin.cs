using UnityEngine;
using System.Collections;

public class UnitBasicArtilleryGoblin : UnitBasicArtillery
{

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

} // class UnitBasicArtillery
