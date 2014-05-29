using UnityEngine;
using System.Collections;

public class UnitHeavyArtilleryRemote : ControllableCharacter
{

    public UnitArtillery.ArtilleryState currentArtilleryState = UnitArtillery.ArtilleryState.None;
    public UnitController.State currentState = UnitController.State.Idle;
    public UnitController.State lastState = UnitController.State.Idle;
    public UnitHeavyArtillery.DeployState currentDeployState = UnitHeavyArtillery.DeployState.Undeployed;

    // indicates the time remaining until the next waiting animation
    private float timeToNextWaitAnimation;
    // indicates if the second attack is selected
    public bool attack2Selected = false;
    // dummys
    public Transform dummyLeftWeapon;
    public Transform dummyRightWeapon;
    public Transform dummyLeftWeaponGunBarrel;
    public Transform dummyRightWeaponGunBarrel;

    public GameObject frontWeapon, backWeapon;

    public bool launchRocket = false; // true if the rocket is launch
    public Vector3 rocketDir; //direction of the rocket
    public GameObject rocket;
    public float attackPower2 = 40.0f;
    private float attackCadenceAux = 0.0f;
    // Animar según el estado actual
    public void Awake()
    {
        // Por si no se han establecido las referencias a los dummys del modelo
        // en el editor de Unity las buscamos ahora:
        if (dummyLeftWeapon == null)
            dummyLeftWeapon = transform.FindChild("Bip002/Bip002 Pelvis/Bip002 Spine/Bip002 Spine1/Bip002 Neck/Bip002 R Clavicle/Bip002 R UpperArm/Bip002 R Forearm/Bip002 R Hand/arma mano derecha");
        if (dummyLeftWeaponGunBarrel == null)
            dummyLeftWeaponGunBarrel = dummyLeftWeapon.FindChild("GoblinHeavyArtilleryWeapon01_A/GunBarrelLeft");
        if (dummyRightWeapon == null)
            dummyRightWeapon = transform.FindChild("Bip002/Bip002 Pelvis/Bip002 Spine/mortero espalda");
        if (dummyLeftWeapon)
            frontWeapon = dummyLeftWeapon.FindChild("GoblinHeavyArtilleryWeapon01_A").gameObject;
        if (dummyRightWeapon)
            backWeapon = dummyRightWeapon.FindChild("GoblinHeavyArtilleryWeapon01_B").gameObject;
    }

    public void Start()
    {
        InsertToTheDistanceMeasurerTool();
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
            case UnitController.State.Idle: UpdateIdle(); break;
            case UnitController.State.GoingTo: UpdateGoingTo(); break;
            case UnitController.State.GoingToAnEnemy: UpdateGoingToAnEnemy(); break;
            case UnitController.State.Attacking: UpdateAttacking(); break;
            case UnitController.State.Flying: UpdateFlying(); break;
            case UnitController.State.Dying: UpdateDying(); break;
            case UnitController.State.AscendingToHeaven: UpdateAscendingToHeaven(); break;
        }

        switch (currentDeployState)
        {
            case UnitHeavyArtillery.DeployState.Undeployed:
                break;
            case UnitHeavyArtillery.DeployState.Deploying:
                animation.CrossFade("Deployment-prepare");
                break;
            case UnitHeavyArtillery.DeployState.Deployed:
                if (currentState != UnitController.State.Attacking)
                {
                    animation.CrossFadeQueued("Deployment-iddle");
                }
                else
                {
                    if (attackCadenceAux >0)
                        attackCadenceAux -= Time.deltaTime;
                    if (launchRocket)
                    {
                        
                        if (attackCadenceAux <= 0)
                        {
                            animation.Play("Deployment-Shot");

                            // Instanciate a new Rocket
                            GameObject newRocket = Instantiate
                            (
                                rocket,
                                dummyLeftWeaponGunBarrel.transform.position,
                                new Quaternion()
                            ) as GameObject;
                            newRocket.rigidbody.isKinematic = false;
                            newRocket.transform.name = "Rocket";
                            newRocket.transform.rotation = dummyLeftWeaponGunBarrel.transform.rotation;
                            newRocket.transform.FindChild("RocketVisionCapsule").GetComponent<CRocketVisionCapsule>().SetOwner(this.gameObject);
                            newRocket.transform.FindChild("RocketVisionCapsule").GetComponent<CRocketVisionCapsule>().SetDamage((int)attackPower2);
                            newRocket.transform.FindChild("RocketVisionCapsule").GetComponent<CRocketVisionCapsule>().SetDestroyTime(2.5f);

                            newRocket.transform.parent = null;
                            newRocket.rigidbody.AddForce
                            (
                                rocketDir,
                                ForceMode.Impulse
                            );
                            attackCadenceAux = 2.0f;
                        }
                    }
                }
                break;
            case UnitHeavyArtillery.DeployState.Undeploying:
                animation.CrossFade("Deployment-Up");
                animation.CrossFadeQueued("Idle01");
                break;
        }
    }

    private void UpdateIdle()
    {
        // plays the waiting idle animation
        if (currentDeployState == UnitHeavyArtillery.DeployState.Undeployed)
        {
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
            PlayAnimationCrossFade("Attack1");
        }
        else
            if (attack2Selected == true)
            {
                PlayAnimationCrossFade("Attack2");
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
        if (currentDeployState != UnitHeavyArtillery.DeployState.Undeployed)
        {
            if (animationName == "Idle01")
                animation.CrossFade("Deployment-iddle");
            /*else if (animationName == "Idle Wait")
                animation.CrossFade("Idle Wait Deployed");*/
            else if (animationName == "Attack2")
                animation.CrossFade("Deployment-Shot");
        }
        else
            animation.CrossFade(animationName);
    }

    protected void PlayAnimationCrossFadeQueued(string animationName)
    {
        if (currentDeployState != UnitHeavyArtillery.DeployState.Undeployed)
        {
            if (animationName == "Idle01")
                animation.CrossFadeQueued("Deployment-iddle");
            /*else if (animationName == "Idle Wait")
                animation.CrossFade("Idle Wait Deployed");*/
        }
        else
            animation.CrossFadeQueued(animationName);
    }
    protected void RemoveAssetsFromModel()
    {
        if (frontWeapon)
            Destroy(frontWeapon);
        if (backWeapon)
            Destroy(backWeapon);
    }

}
