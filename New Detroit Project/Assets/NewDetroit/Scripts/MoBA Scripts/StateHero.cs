using UnityEngine;
using System.Collections;

public class StateHero : MonoBehaviour 
{
    // ENUMS
    public enum StateHeroEnum // The state of the heroe
    {
        Recover,		// After dead, he must be recovered
        Idle,			// When is rest
        Walk,			// When is walking
        Run,			// When is running
        AttackBasic,	// When he is attacking with his basic attack
        AttackSecond,	// When he is attacking with his secondary attack
        Dead			// When is dead
    }

    public enum AttackSecond // The state of secondary attack
    {
        None,
        Attack1,
        Attack2,
        Attack3,
    }


    //----------------------------------------------------------------------------------------------


    // ATTRIBUTES
    // Flags
    protected bool doingSecondaryAnim = false; // A flag to tell us if the orc is doing a secondary attack anim or not
    protected CollisionFlags collisionFlags; // The last collision flags returned from controller.Move
        
    // Attributes
    protected AttributesHero attributes;

    // Controller
    protected ControlHero control;
    protected CharacterController characterController;

    // Sates
    protected StateHeroEnum state = StateHeroEnum.Idle; // The state of the heroe
    protected AttackSecond stateAttackSecond = AttackSecond.None;	// The state of secondary attack

    // Timers
    protected float timeRecover = 0;    // Recover adren, mana and cooldown
    private float timeLevelUp = 0;  //DEBUG

    // Counters
    protected int countSkills = 0; // Unlock skills


    //----------------------------------------------------------------------------------------------


    // METHODS

	// Use this for initialization
	public virtual void Start () 
    {
	    attributes = GetComponent<AttributesHero>();
        control = GetComponent<ControlHero>();
        characterController = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	public virtual void Update () 
    {
        UpdateState(false, false, false);

        // Unlock skill
        if (attributes.getLevel() > 1 && attributes.getLevel() - 1 - countSkills > 0)
        {
            if (!attributes.getUseSkill1() && control.UnlockSkill() == 1)
            {
                attributes.setUseSkill1(true);
                countSkills++;
            }
            else if (!attributes.getUseSkill2() && control.UnlockSkill() == 2)
            {
                attributes.setUseSkill2(true);
                countSkills++;
            }
            else if (!attributes.getUseSkill3() && control.UnlockSkill() == 3)
            {
                attributes.setUseSkill3(true);
                countSkills++;
            }
        }

        // Apply gravity
        if (!characterController.isGrounded)
        {
            characterController.Move(new Vector3(0, -5 * Time.deltaTime, 0));
        }

        // Rotate
        if (!doingSecondaryAnim) transform.Rotate(new Vector3(0, control.ValueRotation(), 0));

        // Recover adren, mana and cooldown
        timeRecover += Time.deltaTime;
        if (timeRecover >= 1)
        {
            attributes.recoverAdren(10);
            attributes.recoverMana(10);
            attributes.recoverCooldown(1);
            timeRecover = 0;
        }

        // DEBUG
        // Experience up
        timeLevelUp += Time.deltaTime;
        if (Input.GetKey(KeyCode.L) && timeLevelUp >= 1)
        {
            if (GetComponent<AttributesOrc>() != null)
            {
                GetComponent<AttributesOrc>().GainExperience(100);
            }
            else
            {
                GetComponent<AttributesRobot>().GainExperience(100);
            }
            timeLevelUp = 0;
        }
        // Die
        if (Input.GetKey(KeyCode.Delete)) attributes.Die();
	}


    //----------------------------------------------------------------------------------------------


    public void UpdateState(bool useSkill1, bool useSkill2, bool useSkill3)
    {
        // Hero dead
        if (attributes.currentLife <= 0 && this.state != StateHeroEnum.Dead && this.state != StateHeroEnum.Recover)
        {
            // States
            this.state = StateHeroEnum.Dead;
            stateAttackSecond = AttackSecond.None;

            // Flags
            doingSecondaryAnim = false;
        }
        // Hero recover mode
        else if (this.state == StateHeroEnum.Dead)
        {
            this.state = StateHeroEnum.Recover;
        }
        // Hero iddle
        else if (state == StateHeroEnum.Recover)
        {
            if (attributes.currentLife >= attributes.maximunLife)
            {
                state = StateHeroEnum.Idle;
            }
        }
        // Only can do an action if hero don't do a secondary attack
        else if (!doingSecondaryAnim)
        {
            // Secondary attack
            if ((control.GetSkill() == 1 || useSkill1) && attributes.UseSkill1())
            {
                // States
                state = StateHeroEnum.AttackSecond;
                stateAttackSecond = AttackSecond.Attack1;

                // Flags
                doingSecondaryAnim = true;
            }
            else if ((control.GetSkill() == 2 || useSkill2) && attributes.UseSkill2())
            {
                // States
                state = StateHeroEnum.AttackSecond;
                stateAttackSecond = AttackSecond.Attack2;

                // Flags
                doingSecondaryAnim = true;
            }
            else if ((control.GetSkill() == 3 || useSkill3) && attributes.UseSkill3())
            {
                // States
                state = StateHeroEnum.AttackSecond;
                stateAttackSecond = AttackSecond.Attack3;

                // Flags
                doingSecondaryAnim = true;
            }
            // Basic attack
            else if (control.IsAttacking())
            {
                state = StateHeroEnum.AttackBasic;
                stateAttackSecond = AttackSecond.None;
            }
            // Movement
            else if (control.IsWalking())
            {
                // State
                state = StateHeroEnum.Walk;
                stateAttackSecond = AttackSecond.None;

                // Move
                characterController.Move(control.MoveDirection() * Time.deltaTime * attributes.getSpeedWalk());
            }
            else if (control.IsRunning())
            {
                // State
                state = StateHeroEnum.Run;
                stateAttackSecond = AttackSecond.None;

                // Move
                characterController.Move(control.MoveDirection() * Time.deltaTime * attributes.getSpeedRun());
            }
            // Idle
            else
            {
                // State
                state = StateHeroEnum.Idle;
                stateAttackSecond = AttackSecond.None;
            }
        }
    }//end UpdateState


    //----------------------------------------------------------------------------------------------


    public StateHeroEnum GetState() { return state; }
    public int GetCountSkills() { return countSkills; }
}
