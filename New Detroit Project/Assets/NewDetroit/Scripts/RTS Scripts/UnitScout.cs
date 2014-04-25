using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitScout : UnitController
{

	public int attackPower = 2;

	private int nextPositionIndex = 0;

	public List<Vector3> patrolPositionsList = new List<Vector3>();

	// Explosion particle
	public GameObject particlesSmokeMower;
	public GameObject particlesPiecesMower;
	public GameObject fireMower;
	public GameObject piecesMower;
	public GameObject explosion; 
	private bool explosionActivated = false;
	private bool otherParticlesActivated = false;
	private float explosionCD = 1.5f;
	private GameObject explosionInst;
	private GameObject fireMowerInst;
	private GameObject piecesMowerInst;

	public enum ScoutState
	{
		None,
		Patrolling
	}
	public ScoutState currentScoutState = ScoutState.None;

    // modelo del asset de la máquina cortacesped
    public GameObject mower;

    public override void Awake ()
    {
        base.Awake();

        if (!mower)
            mower = transform.FindChild("Box002").gameObject;
    }

	// Use this for initialization
	public override void Start ()
	{
		base.Start();

		basicAttackPower = secondaryAttackPower = attackPower;
	}
	
	// Update is called once per frame
	public override void Update () 
	{
		base.Update();
		//UpdateParticles();
        if ( (currentScoutState == ScoutState.Patrolling) && (currentState == State.Idle) )
		{
            nextPositionIndex = (nextPositionIndex + 1) % patrolPositionsList.Count;
            base.GoTo(patrolPositionsList[nextPositionIndex]);
        }
	}

	public override void OnGUI ()
	{
        if (currentState != State.AscendingToHeaven)
        {
            base.OnGUI();

            GUI.skin.label.fontSize = 10;

            GUI.Label(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 45, 100, 50),
                currentState.ToString());
            GUI.Label(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 55, 100, 50),
                currentScoutState.ToString());
            GUI.Label(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 65, 100, 50),
                "NextPatrolPoint: " + nextPositionIndex);
        }
	}

	public void StartPatrol (List<Vector3> positionList)
	{
        // si solo se ha marcado un punto de patrulla se hace GoTo simple a ese punto
        if (positionList.Count == 1)
        {
            nextPositionIndex = 0;
            currentScoutState = ScoutState.None;
            GoTo(positionList[0]);
        }
        else if (positionList.Count > 1)
        {
            nextPositionIndex = 0;
            currentScoutState = ScoutState.Patrolling;
            patrolPositionsList = new List<Vector3>(positionList);
            Debug.Log("siguiente goto a: " + patrolPositionsList[0]);
            base.GoTo(patrolPositionsList[0]);
        }
	}

	public override void RightClickOnSelected (Vector3 destiny, Transform destTransform)
	{
		if (destTransform.name == "WorldFloor")
			currentScoutState = ScoutState.None;

        base.RightClickOnSelected(destiny, destTransform);
	}

    public override bool Damage (float damage, char type)
    {
        if (base.Damage(damage, type))
        {
			if (mower)
			{ 
				Destroy(mower);
				// TODO: instanciar una explosión
				
				/*fireMowerInst = Instantiate
					(
						fireMower,
						transform.position,
						transform.rotation
						) as GameObject;

				Destroy(fireMowerInst, 1f);
				explosionActivated = true;*/

				explosionInst = Instantiate
					(
						explosion,
						transform.position,
						transform.rotation
						) as GameObject;
				
				Quaternion rotationAux = new Quaternion(0f,180f,180f, 0f);
				
				piecesMowerInst = Instantiate
					(
						piecesMower,
						transform.position,
						rotationAux
						) as GameObject;
				
				Destroy(piecesMowerInst, 1.5f);
				Destroy(explosionInst, 1.5f);
				
			}
            return true;
        }
        else
            return false;
    }

    protected override void PlayAnimationCrossFade (string animationName)
    {
        // esta unidad no tiene animación de Idle Wait
        if (animationName != "Idle Wait")
            base.PlayAnimationCrossFade(animationName);
    }

    protected override void PlayAnimationCrossFadeQueued (string animationName)
    {
        // esta unidad no tiene animación de Idle Wait
        if (animationName != "Idle Wait")
            base.PlayAnimationCrossFadeQueued(animationName);
    }

    public override int GetUnitType ()
    {
        return 4;
    }

    protected override void RemoveAssetsFromModel ()
    {
        if (mower)
            Destroy(mower);
    }

	//Particles
	/*protected void UpdateParticles()
	{		
		if (explosionActivated)
		{
			if (explosionCD <= 0)
			{
				explosionCD = 1.5f;
				explosionActivated = false;
				otherParticlesActivated = true;
			}
			else explosionCD -= Time.deltaTime;
		}

		if (otherParticlesActivated)
		{
			//Destroy(mower);
			explosionInst = Instantiate
				(
					explosion,
					transform.position,
					transform.rotation
					) as GameObject;

			Quaternion rotationAux = new Quaternion(0f,180f,180f, 0f);

			piecesMowerInst = Instantiate
				(
					piecesMower,
					transform.position,
					rotationAux
					) as GameObject;
			
			Destroy(piecesMowerInst, 1.7f);
			Destroy(explosionInst, 1.7f);
			
			otherParticlesActivated = false;
		}
	}*/

}
