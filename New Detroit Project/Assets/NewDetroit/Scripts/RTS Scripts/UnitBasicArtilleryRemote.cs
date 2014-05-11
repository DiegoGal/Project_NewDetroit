using UnityEngine;
using System.Collections;

public class UnitBasicArtilleryRemote : ControllableCharacter
{

    public UnitArtillery.ArtilleryState currentArtilleryState = UnitArtillery.ArtilleryState.None;
    public UnitController.State currentState = UnitController.State.Idle;
    public UnitController.State lastState = UnitController.State.Idle;


    // indicates the time remaining until the next waiting animation
    private float timeToNextWaitAnimation;
    // indicates if the second attack is selected
    public bool attack2Selected = false;

    // dummys
    public Transform dummyLeftWeapon;
    public Transform dummyRightWeapon;
    public Transform dummyLeftWeaponGunBarrel;
    public Transform dummyRightWeaponGunBarrel;

    private GameObject leftWeapon, rightWeapon, baseballBat;
    public Transform dummyBat;

    public GameObject shotParticles;

    private float attackCadenceAux;
    private bool setPistols = true;

    public override void Awake ()
    {
        base.Awake();

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
    //TODO Animar según el estado actual
    public override void Start()
    {
        base.Start();

        network();
        timeToNextWaitAnimation = Random.Range(5.0f, 15.0f);
    }

    public void changeAttack()
    {
        attack2Selected = !attack2Selected;
    }

    public void Update()
    {
        switch (currentState)
        {
            case UnitController.State.Idle:              UpdateIdle(); break;
            case UnitController.State.GoingTo:           UpdateGoingTo(); break;
            case UnitController.State.GoingToAnEnemy:    UpdateGoingToAnEnemy(); break;
            case UnitController.State.Attacking:         UpdateAttacking(); break;
            case UnitController.State.Flying:            UpdateFlying(); break;
            case UnitController.State.Dying:             UpdateDying(); break;
            case UnitController.State.AscendingToHeaven: UpdateAscendingToHeaven(); break;
        }
    }

    private void UpdateIdle()
    {
        // plays the waiting idle animation
        timeToNextWaitAnimation -= Time.deltaTime;
        if (timeToNextWaitAnimation <= 0)
        {
            PlayAnimationCrossFade("Idle Wait");
            PlayAnimationCrossFadeQueued("Idle01");
            timeToNextWaitAnimation = Random.Range(5.0f, 15.0f);
        }
        else
        {
            if (!animation.IsPlaying("Idle Wait"))
                PlayAnimationCrossFade("Idle01");
        }
    }

    private void UpdateGoingTo()
    {
        PlayAnimationCrossFade("Walk");
    }

    private void UpdateGoingToAnEnemy()
    {
        PlayAnimationCrossFade("Walk");
    }

    private void UpdateAttacking()
    {
        if (attack2Selected == false)
        {
            if (attackCadenceAux <= 0.0f)
            {
                attackCadenceAux = 1.0f;
                animation.CrossFade("Attack1");
                GameObject particles1 = (GameObject)Instantiate(shotParticles,
                dummyLeftWeaponGunBarrel.transform.position,
                transform.rotation);
                Destroy(particles1, 0.4f);
                GameObject particles2 = (GameObject)Instantiate(shotParticles,
                dummyRightWeaponGunBarrel.transform.position,
                transform.rotation);
                Destroy(particles2, 0.4f);
            }
            else
            attackCadenceAux -= Time.deltaTime;
        }
        else
            if (attack2Selected == true)
            {
                if (attackCadenceAux <= 0.0f)
                {
                    attackCadenceAux = 1.0f;
                    animation.CrossFade("Attack2");
                }
                else
                    attackCadenceAux -= Time.deltaTime;
            }
    }

    private void UpdateFlying()
    {
        PlayAnimationCrossFade("Idle01");
    }

    private void UpdateDying()
    {
        PlayAnimationCrossFade("Die");
    }

    private void UpdateAscendingToHeaven()
    {
        RemoveAssetsFromModel();
    }

    protected void PlayAnimation(string animationName)
    {
        animation.Play(animationName);
    }

    protected void PlayAnimationQueued(string animationName)
    {
        animation.PlayQueued(animationName);
    }

    protected void PlayAnimationCrossFade(string animationName)
    {
        animation.CrossFade(animationName);
    }

    protected void PlayAnimationCrossFadeQueued(string animationName)
    {
        animation.CrossFadeQueued(animationName);
    }

    protected void RemoveAssetsFromModel()
    {
        if (leftWeapon)
            Destroy(leftWeapon);
        if (rightWeapon)
            Destroy(rightWeapon);
        if (baseballBat)
            Destroy(baseballBat);
    }

}
