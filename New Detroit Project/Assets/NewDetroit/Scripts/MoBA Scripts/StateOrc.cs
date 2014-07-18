using UnityEngine;
using System.Collections;

public class StateOrc : StateHero
{
    //Colliders
	public GameObject cubeColliderHand;	// Hand

	// Transforms
	private Transform head;		// Head
	private Transform pelvis;	// Pelvis

	// Particles
	public GameObject snot;				// Skill 1
	public GameObject splash;			// Skill 2
	public GameObject sphereThirdSkill;	// Skill 3
	public GameObject smoke;			// Smoke

    //Component for animations
    private CStateUnit cState;


	//-------------------------------------------------------------------------------------------------


	// Use this for initialization
	public virtual void Start ()
	{
		base.Start();

        cState = GetComponent<CStateUnit>();

		//Set the collider cubes in both hands
		//Right hand	
		Transform hand = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand");
		GameObject cubeColliderInst;
		if (PhotonNetwork.connected) 
			cubeColliderInst = (GameObject) PhotonNetwork.Instantiate(cubeColliderHand.name, hand.position + new Vector3(-0.25f, -0.5f, 0), hand.rotation, 0);
		else 
			cubeColliderInst = (GameObject) Instantiate(cubeColliderHand, hand.position + new Vector3(-0.25f, -0.5f, 0), hand.rotation);
			
		cubeColliderInst.transform.parent = hand;
		cubeColliderInst.GetComponent<OrcBasicAttack> ().owner = this.gameObject;		
		//Left hand
		hand = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand");
		if (PhotonNetwork.connected)
			cubeColliderInst = (GameObject) PhotonNetwork.Instantiate(cubeColliderHand.name, hand.position + new Vector3(0.25f, -0.5f, 0), hand.rotation, 0);
		else
			cubeColliderInst = (GameObject) Instantiate(cubeColliderHand, hand.position + new Vector3(0.25f, -0.5f, 0), hand.rotation);
		cubeColliderInst.transform.parent = hand;
		cubeColliderInst.GetComponent<OrcBasicAttack> ().owner = this.gameObject;

        //Transforms
        head = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head");
        pelvis = transform.FindChild("Bip001/Bip001 Pelvis");

		//Initialize the animation
		animation.Play ("Iddle01");
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		base.Update ();

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
                if (!animation.IsPlaying("Burp"))
                {
                    // Animation
                    cState.animationName = "Burp";
                    cState.animationChanged = true;

                    // Particles
                    StartCoroutine(FirstSkill(animation["Burp"].length * 0.1f));

                    // State
                    stateAttackSecond = AttackSecond.None;
                }
            }
            else if (stateAttackSecond == AttackSecond.Attack2)
            {
                if (!animation.IsPlaying("FloorHit"))
                {
                    // Animation
                    cState.animationName = "FloorHit";
                    cState.animationChanged = true;

                    // Particles
                    StartCoroutine(SecondSkill(animation["FloorHit"].length * 0.25f));

                    // State
                    stateAttackSecond = AttackSecond.None;
                }
            }
            else if (stateAttackSecond == AttackSecond.Attack3)
            {
                if (!animation.IsPlaying("BullStrike"))
                {
                    // Animation
                    cState.animationName = "BullStrike";
                    cState.animationChanged = true;

                    // Particles
                    // Smoke
                    StartCoroutine(SmokeParticles(0));
                    // Shpere
                    StartCoroutine(ThirdSkill(animation["BullStrike"].length * 0.2f));

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
            if (!animation.IsPlaying("Attack01") && !animation.IsPlaying("Attack02") && !animation.IsPlaying("Attack03"))
            {
                // Animation
                cState.animationName = "Attack01";
                cState.animationNameQueued = "Attack02";
                cState.animationNameQueued2 = "Attack03";
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
            if (!animation.IsPlaying("Iddle01") && !animation.IsPlaying("Iddle02"))
            {
                // Animation
                cState.animationName = "Iddle01";
                cState.animationNameQueued = "Iddle02";
                cState.animationChanged = cState.animationChangeQueued = true;
            }
        }
    } // End UpdateLogic

	
	//--------------------------------------------------------------------------------------------


	//Corrutines	
	private IEnumerator FirstSkill(float time)
	{
		yield return new WaitForSeconds(time);

		GameObject snt;
		if (!PhotonNetwork.offlineMode)
			snt = (GameObject) PhotonNetwork.Instantiate(snot.name, transform.localPosition + transform.forward * 2 + Vector3.up, transform.rotation, 0);
		else
		{
			snt = (GameObject) Instantiate(snot, transform.localPosition + transform.forward * 2 + Vector3.up, transform.rotation);
			snt.GetComponent<SkillAttackNetwork>().enabled = false;
		}
		SkillAttack sa = snt.GetComponent<SkillAttack>();
		sa.SetDamage(1);
		sa.setOwner(gameObject);

		yield return new WaitForSeconds(3f);

        if (!PhotonNetwork.offlineMode)
			PhotonNetwork.Destroy(snt);
		else
			Destroy(snt);

        // Flags
        doingSecondaryAnim = false;
	}

	private IEnumerator SecondSkill(float time)
	{
		yield return new WaitForSeconds(time);

		GameObject spl;
        if (!PhotonNetwork.offlineMode) 
			 spl = (GameObject) PhotonNetwork.Instantiate(splash.name, transform.position + new Vector3(0, -1.3f, 0), Quaternion.identity, 0);
		else
			spl = (GameObject) Instantiate(splash, transform.position + new Vector3(0, -1.3f, 0), Quaternion.identity);
		SkillAttack sa = spl.GetComponent<SkillAttack>();
		sa.SetDamage(attributes.getAttackMagic() + 40);
		sa.setOwner(gameObject);

		yield return new WaitForSeconds(animation["FloorHit"].length * 0.75f);

        if (!PhotonNetwork.offlineMode)
			PhotonNetwork.Destroy(spl);
		else
			Destroy(spl);

        // Flags
        doingSecondaryAnim = false;
	}

	private IEnumerator ThirdSkill(float time)
	{
		yield return new WaitForSeconds(time);

		GameObject sphereThirdSkillInst;
        if (!PhotonNetwork.offlineMode) 
			sphereThirdSkillInst = (GameObject) PhotonNetwork.Instantiate(sphereThirdSkill.name, head.position, transform.rotation, 0);
		else
			sphereThirdSkillInst = (GameObject) Instantiate(sphereThirdSkill, head.position, transform.rotation);
		SkillAttack sa = sphereThirdSkillInst.GetComponent<SkillAttack>();
		sa.setOwner(gameObject);
		sa.SetDamage(attributes.getAttackPhysic() + 100);
		sphereThirdSkillInst.transform.parent = pelvis;

		yield return new WaitForSeconds(animation["BullStrike"].length * 0.8f);

        if (!PhotonNetwork.offlineMode)
			PhotonNetwork.Destroy(sphereThirdSkillInst);
		else
			Destroy(sphereThirdSkillInst);

        // Flags
        doingSecondaryAnim = false;
	}

	private IEnumerator SmokeParticles(float time)
	{
		yield return new WaitForSeconds(time);

		GameObject smokeInst;
        if (!PhotonNetwork.offlineMode)
			smokeInst = (GameObject)PhotonNetwork.Instantiate(smoke.name, transform.localPosition + Vector3.down*2, transform.rotation, 0);
		else
			smokeInst = (GameObject) Instantiate(smoke, transform.localPosition + Vector3.down*2, transform.rotation);
		smokeInst.transform.parent = pelvis;

		yield return new WaitForSeconds(5f);

        if (!PhotonNetwork.offlineMode)
			PhotonNetwork.Destroy(smokeInst);
		else
			Destroy(smokeInst);
	}
}

