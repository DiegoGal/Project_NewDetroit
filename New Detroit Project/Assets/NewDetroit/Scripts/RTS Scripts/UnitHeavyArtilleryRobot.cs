using UnityEngine;
using System.Collections;

public class UnitHeavyArtilleryRobot : UnitHeavyArtillery
{

    public GameObject dome;

    public override void Awake ()
    {
        base.Awake();

        numberOfWeapons = 1;
        // Por si no se han establecido las referencias a los dummys del modelo
        // en el editor de Unity las buscamos ahora:
        if (dummyLeftWeapon == null)
            dummyLeftWeapon = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand");
        if (dummyLeftWeaponGunBarrel == null)
            dummyLeftWeaponGunBarrel = dummyLeftWeapon.FindChild("GunBarrel");

        if (!dome)
            dome = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Xtra01").gameObject;

        type = TypeHeroe.Robot;
    }

    protected override void RemoveAssetsFromModel()
    {
        base.RemoveAssetsFromModel();

        if (dome)
            Destroy(dome);
    }
} // class UnitHeavyArtilleryRobot
