using UnityEngine;
using System.Collections;

public class UnitHeavyArtillery : UnitArtillery
{

    public float attackPower1 = 20.0f;
    public float attackPower2 = 40.0f;

    // the vision radious of the unit
    //base: protected float visionSphereRadious;
    // extended vision radious (when its Deployed)
    public float visionSphereRadiousExtended = 10.0f;

    protected enum DeployState
    {
        Undeployed,	// sin desplegar
        Deploying,  // desplegando
        Deployed,   // desplegado
        Undeploying // anular despliegue
    }
    protected DeployState currentDeployState = DeployState.Undeployed;

    public override void Awake()
    {
        base.Awake();

        numberOfWeapons = 1;
        // Por si no se han establecido las referencias a los dummys del modelo
        // en el editor de Unity las buscamos ahora:
        if (dummyLeftWeapon == null)
            dummyLeftWeapon = transform.FindChild("Bip002/Bip002 Pelvis/Bip002 Spine/Bip002 Spine1/Bip002 Neck/Bip002 R Clavicle/Bip002 R UpperArm/Bip002 R Forearm/Bip002 R Hand/arma mano derecha");
        if (dummyLeftWeaponGunBarrel == null)
            dummyLeftWeaponGunBarrel = dummyLeftWeapon.FindChild("GoblinHeavyArtilleryWeapon01_A/GunBarrelLeft");
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

    public override void OnGUI ()
    {
        base.OnGUI();

        GUI.skin.label.fontSize = 10;

        GUI.Label(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 75, 100, 50),
            currentDeployState.ToString());
    } // OnGUI

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
                transform.FindChild("VisionSphere").GetComponent<SphereCollider>().radius =
                    visionSphereRadious + visionSphereRadiousExtended;
                break;
            case DeployState.Deploying:
                currentDeployState = DeployState.Deployed;
                transform.FindChild("VisionSphere").GetComponent<SphereCollider>().radius =
                    visionSphereRadious + visionSphereRadiousExtended;
                break;
            case DeployState.Deployed:
                currentDeployState = DeployState.Undeployed;
                transform.FindChild("VisionSphere").GetComponent<SphereCollider>().radius =
                    visionSphereRadious;
                break;
            case DeployState.Undeploying:
                currentDeployState = DeployState.Undeployed;
                transform.FindChild("VisionSphere").GetComponent<SphereCollider>().radius =
                    visionSphereRadious;
                break;
        }
    }

} // class UnitHeavyArtillery
