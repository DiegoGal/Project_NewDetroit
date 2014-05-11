using UnityEngine;
using System.Collections;

public class UnitBasicArtilleryRobot : UnitBasicArtillery
{

    public override void Awake ()
    {
        base.Awake();

        numberOfWeapons = 1;
        // Por si no se han establecido las referencias a los dummys del modelo
        // en el editor de Unity las buscamos ahora:
        if (dummyLeftWeapon == null)
            dummyLeftWeapon = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Point001/Weapon");
        if (dummyLeftWeaponGunBarrel == null)
            dummyLeftWeaponGunBarrel = dummyLeftWeapon.FindChild("GunBarrel");
        if (dummyBat == null)
            dummyBat = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Arma Blanca/Cylinder002");

        if (dummyLeftWeapon)
            leftWeapon = dummyLeftWeapon.gameObject;
        if (dummyBat)
            baseballBat = dummyBat.gameObject;
    }

} // class UnitBasicArtillery
