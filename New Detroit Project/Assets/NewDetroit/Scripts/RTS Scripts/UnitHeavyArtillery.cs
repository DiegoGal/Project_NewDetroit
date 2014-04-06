using UnityEngine;
using System.Collections;

public class UnitHeavyArtillery : UnitArtillery
{

    public float attackPower1 = 20.0f;
    public float attackPower2 = 40.0f;

    // the vision radious of the unit
    // base: protected float visionSphereRadious;
    // extended vision radious (when its Deployed)
    // this is added to the original radious
    public float visionSphereRadiousExtended = 8.0f;

    // we need to save the original size of the vision sphere
    // in order to restore it after the deployed mode
    private float visionSphereRaiousOriginal;

    public GameObject frontWeapon, backWeapon;

    protected enum DeployState
    {
        Undeployed,	// sin desplegar
        Deploying,  // desplegando
        Deployed,   // desplegado
        Undeploying // anular despliegue
    }
    protected DeployState currentDeployState = DeployState.Undeployed;

    public override void Awake ()
    {
        base.Awake();

        numberOfWeapons = 1;
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

	// Use this for initialization
    public override void Start ()
	{
        base.Start();

        basicAttackPower = attackPower1;
        secondaryAttackPower = attackPower2;

        visionSphereRaiousOriginal = visionSphereRadious;
	}
	
	// Update is called once per frame
    public override void Update ()
	{
        if (currentDeployState == DeployState.Undeployed)
		    base.Update();

        if (isSelected && Input.GetKeyDown(KeyCode.D))
        {
            switch (currentDeployState)
            {
                case DeployState.Undeployed:
                    StopMoving();
                    animation.CrossFade("Deployment-prepare");
                    StartCoroutine(WaitAndCallback(animation["Deployment-prepare"].length));
                    animation.CrossFadeQueued("Deployment-iddle");
                    currentDeployState = DeployState.Deploying;
                    break;
                case DeployState.Deploying:
                    /*animation.CrossFade("Deployment-Up");
                    StartCoroutine(WaitAndCallback(animation["Deployment-Up"].length));
                    animation.CrossFadeQueued("Idle01");
                    currentDeployState = DeployState.Undeploying;*/
                    break;
                case DeployState.Deployed:
                    animation.CrossFade("Deployment-Up");
                    StartCoroutine(WaitAndCallback(animation["Deployment-Up"].length));
                    animation.CrossFadeQueued("Idle01");
                    currentDeployState = DeployState.Undeploying;
                    break;
                case DeployState.Undeploying:
                    /*animation.CrossFade("Deployment-prepare");
                    StartCoroutine(WaitAndCallback(animation["Deployment-prepare"].length));
                    animation.CrossFadeQueued("Deployment-iddle");
                    currentDeployState = DeployState.Deploying;*/
                    break;
            }
        }

	} // Update

    public override void RightClickOnSelected(Vector3 destiny, Transform destTransform)
    {
        if (currentDeployState == DeployState.Undeployed)
            base.RightClickOnSelected(destiny, destTransform);
    }

    private IEnumerator WaitAndCallback (float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        AnimationFinished();
    }

    private void AnimationFinished ()
    {
        switch (currentDeployState)
        {
            case DeployState.Undeployed:
                currentDeployState = DeployState.Deployed;
                attack2Selected = true;
                if (thereIsVisionSphere)
                    transform.FindChild("VisionSphere").GetComponent<SphereCollider>().radius =
                        visionSphereRadious + visionSphereRadiousExtended;
                else
                    visionSphereRadious = visionSphereRaiousOriginal + visionSphereRadiousExtended;
                break;

            case DeployState.Deploying:

                currentDeployState = DeployState.Deployed;
                attack2Selected = true;
                if (thereIsVisionSphere)
                    transform.FindChild("VisionSphere").GetComponent<SphereCollider>().radius =
                        visionSphereRadious + visionSphereRadiousExtended;
                else
                    visionSphereRadious = visionSphereRaiousOriginal + visionSphereRadiousExtended;

                break;

            case DeployState.Deployed:

                currentDeployState = DeployState.Undeployed;
                attack2Selected = false;
                if (thereIsVisionSphere)
                    transform.FindChild("VisionSphere").GetComponent<SphereCollider>().radius =
                        visionSphereRadious;
                else
                    visionSphereRadious = visionSphereRaiousOriginal;

                break;

            case DeployState.Undeploying:

                currentDeployState = DeployState.Undeployed;
                attack2Selected = false;
                if (thereIsVisionSphere)
                    transform.FindChild("VisionSphere").GetComponent<SphereCollider>().radius =
                        visionSphereRadious;
                else
                    visionSphereRadious = visionSphereRaiousOriginal;

                break;
        }
    }

    public override int GetUnitType ()
    {
        return 2;
    }

    protected override void RemoveAssetsFromModel()
    {
        if (frontWeapon)
            Destroy(frontWeapon);
        if (backWeapon)
            Destroy(backWeapon);
    }

} // class UnitHeavyArtillery
