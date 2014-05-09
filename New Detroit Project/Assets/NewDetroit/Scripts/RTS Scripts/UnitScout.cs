using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitScout : UnitController
{

	public int attackPower = 2;

	private int nextPositionIndex = 0;

	public List<Vector3> patrolPositionsList = new List<Vector3>();

    // modelo del asset de la máquina cortacesped
    public GameObject mower;

    // fire particles
    public GameObject fireMower;
    private GameObject fireMowerInst;

    // ardiendo
    public bool afire = false;

    // indica el porcentaje de vida al que empieza a salir fuego de la montura
    public float startAfire = 0.25f;

    // indica el porcentaje de vida al que, si hay fuego, este desaparece
    public float stopAfire = 0.5f;

	// Explosion particles references
	public GameObject particlesExplosionSmoke;
    public GameObject particlesExplosionFire;
    public GameObject particlesExplosionPieces;
	
    // Explosion particles instances
	private GameObject explosionSmokeInst;
    private GameObject explosionFireInst;
	private GameObject explosionPiecesInst;

	public enum ScoutState
	{
		None,
		Patrolling
	}
	public ScoutState currentScoutState = ScoutState.None;

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

            GUI.Label(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 65, 200, 50),
                currentState.ToString() + "\n" +
                currentScoutState.ToString() + "\n" +
                "NextPatrolPoint: " + nextPositionIndex );

            /*GUI.Label(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 45, 100, 50),
                currentState.ToString());
            GUI.Label(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 55, 100, 50),
                currentScoutState.ToString());
            GUI.Label(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 65, 100, 50),
                "NextPatrolPoint: " + nextPositionIndex);*/
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
                new Quaternion(0f,180f,180f, 0f)
            ) as GameObject;

            explosionPiecesInst = Instantiate
            (
                particlesExplosionPieces,
                transform.position,
                new Quaternion(0f, 180f, 180f, 0f)
            ) as GameObject;

            Destroy(explosionFireInst,   2.5f);
            Destroy(explosionSmokeInst,  2.5f);
            Destroy(explosionPiecesInst, 2.5f);
			
            return true;
        }
        else
        {
            if ( !afire && getLife() <= (GetMaximunLife() * startAfire) )
            {
                // la vida es muy baja, instanciar el fuego
                fireMowerInst = Instantiate
                (
                    fireMower,
                    mower.transform.position,
                    mower.transform.rotation
                ) as GameObject;
                fireMowerInst.transform.parent = mower.transform;

                afire = true;
            }
            return false;
        }
    }

    public override bool Heal (float amount)
    {
        bool aux = base.Heal(amount);

        // si la vida es superior al 85% y hay fuego, se destruye este fuego
        if ( afire && getLife() >= (GetMaximunLife() * stopAfire) )
        {
            Destroy(fireMowerInst);

            afire = false;
        }

        return aux;
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
