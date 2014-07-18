using UnityEngine;
using System.Collections;

public class StateRobot : StateHero
{
    // Particles
    public GameObject fireBall;		// Skill 1
    public GameObject skelterTurn;	// Skill 2
    public GameObject skelterShot;	// Skill 3

    // Instances
    private GameObject fireCircleInst;	// Skill 1
    private GameObject turnInst;		// Skill 2
    private GameObject fireShotInst;	// Skill 3

    // Colliders
    public GameObject cubeColliderSword;	// Sword

    // Transforms
    private Transform gun;	// Gun
    private Transform head;	// Head

    // Flags
    private bool doneFirstSkill = false;	// Skill 1
    private bool isShot = false;			// Skill 3

    //Component for animations
    private CStateUnit cState;


    //----------------------------------------------------------------------------------------------


    //================================
    //=====     Main methods     =====
    //================================

    // Use this for initialization
    public virtual void Start()
    {
        base.Start();

        cState = GetComponent<CStateUnit>();

        //Set the collider cubes in the sword
        Transform sword = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Cylinder002/cuchilla");
        GameObject cubeColliderInst;
        if (PhotonNetwork.connected)
            cubeColliderInst = (GameObject)PhotonNetwork.Instantiate(cubeColliderSword.name, sword.position + new Vector3(0.4f, 1, 0.1f), sword.rotation, 0);
        else
            cubeColliderInst = (GameObject)Instantiate(cubeColliderSword, sword.position + new Vector3(0.4f, 1, 0.1f), sword.rotation);
        cubeColliderInst.transform.parent = sword;
        cubeColliderInst.GetComponent<RobotBasicAttack>().owner = this.gameObject;

        //Initialize the animation
        animation.Play("Idle01");

        gun = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Cylinder002/cuchilla");
        head = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head");
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        UpdateLogic();
    }


    //----------------------------------------------------------------------------------------------


    private void UpdateLogic()
    {
        if (state == StateHeroEnum.Dead)
        {

        }
        else if (state == StateHeroEnum.Recover)
        {

        }
        else if (state == StateHeroEnum.AttackSecond)
        {
            if (stateAttackSecond == AttackSecond.Attack1)
            {
                // Particles
                StartCoroutine(FirstSkill(0));

                // Flags
                doingSecondaryAnim = false;

                // State
                stateAttackSecond = AttackSecond.None;
            }
            else if (stateAttackSecond == AttackSecond.Attack2)
            {
                if (!animation.IsPlaying("FloorHit"))
                {
                    // Animation
                    cState.animationName = "Attack2";
                    cState.animationChanged = true;

                    // Particles
                    StartCoroutine(SecondSkill(0));

                    // State
                    stateAttackSecond = AttackSecond.None;
                }
            }
            else if (stateAttackSecond == AttackSecond.Attack3)
            {
                if (!animation.IsPlaying("BullStrike"))
                {
                    // Animation
                    cState.animationName = "Attack3";
                    cState.animationChanged = true;

                    // Particles
                    // Shpere
                    StartCoroutine(ThirdSkill(0));

                    // State
                    stateAttackSecond = AttackSecond.None;
                }
            }
            else if (stateAttackSecond == AttackSecond.None)
            {
                // None
            }
        }
        else if (state == StateHeroEnum.AttackBasic)
        {
            if (!animation.IsPlaying("Attack1") && !animation.IsPlaying("Attack2") && !animation.IsPlaying("Attack3"))
            {
                // Animation
                cState.animationName = "Attack1";
                cState.animationNameQueued = "Attack2";
                cState.animationNameQueued2 = "Attack3";
                cState.animationChanged = cState.animationChangeQueued = cState.animationChangeQueued2 = true;
            }
        }
        else if (state == StateHeroEnum.Run)
        {
            // Animation
            cState.animationName = "Run";
            cState.animationChanged = true;

        }
        else if (state == StateHeroEnum.Walk)
        {
            // Animation
            cState.animationName = "Walk";
            cState.animationChanged = true;
        }
        else if (state == StateHeroEnum.Idle)
        {
            // Animation
            cState.animationName = "Idle01";
            cState.animationChanged = true;
        }
    } // End UpdateLogic


    //-----------------------------------------------------------------------


    // Corutines
    private IEnumerator FirstSkill(float time)
    {
        yield return new WaitForSeconds(time);

        if (!PhotonNetwork.offlineMode)
            fireCircleInst = (GameObject)PhotonNetwork.Instantiate(fireBall.name, head.position + Vector3.down * 0.5f, transform.rotation, 0);
        else
            fireCircleInst = (GameObject)Instantiate(fireBall, head.position + Vector3.down * 0.5f, transform.rotation);
        SkillDefense sd = fireCircleInst.GetComponent<SkillDefense>();
        sd.setOwner(gameObject);
        sd.UpDef();
        fireCircleInst.transform.parent = head;

        yield return new WaitForSeconds(1.7f);

        sd.DownDef();

        if (!PhotonNetwork.offlineMode)
            PhotonNetwork.Destroy(fireCircleInst);
        else
            Destroy(fireCircleInst);

        doneFirstSkill = false;
    }

    private IEnumerator SecondSkill(float time)
    {
        yield return new WaitForSeconds(time);

        if (!PhotonNetwork.offlineMode)
            turnInst = (GameObject)PhotonNetwork.Instantiate(skelterTurn.name, transform.position + Vector3.up, transform.rotation, 0);
        else
            turnInst = (GameObject)Instantiate(skelterTurn, transform.position + Vector3.up, transform.rotation);
        SkillAttack sa = turnInst.GetComponent<SkillAttack>();
        sa.SetDamage(75);
        sa.setOwner(gameObject);

        yield return new WaitForSeconds(animation["Attack2"].length);

        if (!PhotonNetwork.offlineMode)
            PhotonNetwork.Destroy(turnInst);
        else
            Destroy(turnInst);

        // Flags
        doingSecondaryAnim = false;
    }

    private IEnumerator ThirdSkill(float time)
    {
        yield return new WaitForSeconds(time);

        if (fireShotInst != null)
        {
            if (!PhotonNetwork.offlineMode)
                PhotonNetwork.Destroy(fireShotInst);
            else
                Destroy(fireShotInst);
        }
        skelterShot.GetComponent<MeshRenderer>().enabled = false;
        if (!PhotonNetwork.offlineMode)
            fireShotInst = (GameObject)PhotonNetwork.Instantiate(skelterShot.name, gun.position, transform.rotation, 0);
        else
            fireShotInst = (GameObject)Instantiate(skelterShot, gun.position, transform.rotation);
        SkillAttack sa = fireShotInst.GetComponent<SkillAttack>();
        sa.setOwner(gameObject);
        sa.SetDamage(75);
        SkillLaunch sl = fireShotInst.GetComponent<SkillLaunch>();
        sl.direction = transform.forward;
        fireShotInst.transform.parent = gun;

        yield return new WaitForSeconds(2.5f);

        if (!PhotonNetwork.offlineMode)
            PhotonNetwork.Destroy(fireShotInst);
        else
            Destroy(fireShotInst);

        // Flags
        doingSecondaryAnim = false;
    }

    private IEnumerator ThirdBasicAttack(float time)
    {
        yield return new WaitForSeconds(time);

        if (fireShotInst != null)
        {
            if (!PhotonNetwork.offlineMode)
                PhotonNetwork.Destroy(fireShotInst);
            else
                Destroy(fireShotInst);
        }
        skelterShot.GetComponent<MeshRenderer>().enabled = false;
        if (!PhotonNetwork.offlineMode)
            fireShotInst = (GameObject)PhotonNetwork.Instantiate(skelterShot.name, gun.position, transform.rotation, 0);
        else
            fireShotInst = (GameObject)Instantiate(skelterShot, gun.position, transform.rotation);
        SkillAttack sa = fireShotInst.GetComponent<SkillAttack>();
        sa.setOwner(gameObject);
        sa.SetDamage(20);
        SkillLaunch sl = fireShotInst.GetComponent<SkillLaunch>();
        sl.direction = transform.forward;
        fireShotInst.transform.parent = gun;

        yield return new WaitForSeconds(animation["Attack3"].length);

        if (!PhotonNetwork.offlineMode)
            PhotonNetwork.Destroy(fireShotInst);
        else
            Destroy(fireShotInst);

        isShot = false;
    }
}


