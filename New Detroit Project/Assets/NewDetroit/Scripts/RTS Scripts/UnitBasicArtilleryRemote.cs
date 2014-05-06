using UnityEngine;
using System.Collections;

public class UnitBasicArtilleryRemote : MonoBehaviour
{

    public UnitArtillery.ArtilleryState currentArtilleryState = UnitArtillery.ArtilleryState.None;
    public UnitController.State currentState = UnitController.State.Idle;
    public UnitController.State lastState = UnitController.State.Idle;


    // indicates the time remaining until the next waiting animation
    private float timeToNextWaitAnimation;
    // indicates if the second attack is selected
    public bool attack2Selected = false;

    //TODO Animar según el estado actual
    public void Start()
    {
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
    }

    private void UpdateGoingTo()
    {
        if (timeToNextWaitAnimation != 0)
            timeToNextWaitAnimation = 0;
        PlayAnimationCrossFade("Walk");
    }

    private void UpdateGoingToAnEnemy()
    {
        if (timeToNextWaitAnimation != 0)
            timeToNextWaitAnimation = 0;
        PlayAnimationCrossFade("Walk");
    }

    private void UpdateAttacking()
    {
        if (currentArtilleryState == UnitArtillery.ArtilleryState.Attacking1 && attack2Selected == false)
        {
            animation.CrossFade("Attack1");
        }
        else
            if (currentArtilleryState == UnitArtillery.ArtilleryState.Attacking2 && attack2Selected == true)
            {
                animation.CrossFade("Attack2");
            }
    }

    private void UpdateFlying()
    {
        PlayAnimationCrossFade("Idle01");
    }

    private void UpdateDying()
    {
        //Do nothing
    }

    private void UpdateAscendingToHeaven()
    {
        //Do nothing
    }

    protected virtual void PlayAnimation(string animationName)
    {
        animation.Play(animationName);
    }

    protected virtual void PlayAnimationQueued(string animationName)
    {
        animation.PlayQueued(animationName);
    }

    protected virtual void PlayAnimationCrossFade(string animationName)
    {
        animation.CrossFade(animationName);
    }

    protected virtual void PlayAnimationCrossFadeQueued(string animationName)
    {
        animation.CrossFadeQueued(animationName);
    }

}
