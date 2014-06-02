using UnityEngine;
using System.Collections;

public class UnitScoutRemote : ControllableCharacter
{

    // Explosion particles references
    public GameObject particlesExplosionSmoke;
    public GameObject particlesExplosionFire;
    public GameObject particlesExplosionPieces;

    // Explosion particles instances
    private GameObject explosionSmokeInst;
    private GameObject explosionFireInst;
    private GameObject explosionPiecesInst;

    // fire particles
    public GameObject fireMower;
    private GameObject fireMowerInst;

    // ardiendo
    public bool afire = false;
    private bool auxAfire = false;

    public UnitScout.ScoutState currentScoutState = UnitScout.ScoutState.None;
    public UnitController.State currentState = UnitController.State.Idle;

    // modelo del asset de la máquina cortacesped
    public GameObject mower;

    private float timeToNextWaitAnimation;

    private bool instantiateParticlesWhenDead = true;

    public void Awake()
    {
        if (!mower)
            mower = transform.FindChild("Box002").gameObject;
    }

    // Use this for initialization
    public void Start()
    {
        timeToNextWaitAnimation = Random.Range(5.0f, 15.0f);
    }

    // Update is called once per frame
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
        // la vida es muy baja, instanciar el fuego
        if (afire && !auxAfire)
        {
            auxAfire = true;
            fireMowerInst = Instantiate
            (
                fireMower,
                mower.transform.position,
                mower.transform.rotation
            ) as GameObject;
            fireMowerInst.transform.parent = mower.transform;
        }
        if (!afire)
            auxAfire = false;
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
        PlayAnimationCrossFade("Attack1");
    }

    private void UpdateFlying()
    {
        PlayAnimationCrossFade("Idle01");
    }

    private void UpdateDying()
    {
        PlayAnimationCrossFade("Die");

        if (instantiateParticlesWhenDead)
        {
            instantiateParticlesWhenDead = false;
            if (mower)
                Destroy(mower);

            explosionFireInst = Instantiate
            (
                particlesExplosionFire,
                transform.position,
                transform.rotation
            ) as GameObject;

            explosionSmokeInst = Instantiate
            (
                particlesExplosionSmoke,
                transform.position,
                new Quaternion(0f, 180f, 180f, 0f)
            ) as GameObject;

            explosionPiecesInst = Instantiate
            (
                particlesExplosionPieces,
                transform.position,
                new Quaternion(0f, 180f, 180f, 0f)
            ) as GameObject;

            Destroy(explosionFireInst, 2.5f);
            Destroy(explosionSmokeInst, 2.5f);
            Destroy(explosionPiecesInst, 2.5f);
        }
    }

    private void UpdateAscendingToHeaven()
    {
        RemoveAssetsFromModel();
    }

    protected void PlayAnimationCrossFade(string animationName)
    {
        // esta unidad no tiene animación de Idle Wait
        if (animationName != "Idle Wait")
            animation.CrossFade(animationName);
    }

    protected void PlayAnimationCrossFadeQueued(string animationName)
    {
        // esta unidad no tiene animación de Idle Wait
        if (animationName != "Idle Wait")
            animation.CrossFadeQueued(animationName);
    }

    public int GetUnitType()
    {
        return 4;
    }

    protected void RemoveAssetsFromModel()
    {
        if (mower)
            Destroy(mower);
    }


}
