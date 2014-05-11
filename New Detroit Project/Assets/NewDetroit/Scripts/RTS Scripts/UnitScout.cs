using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitScout : UnitController
{

	public int attackPower = 2;

	private int nextPositionIndex = 0;

	public List<Vector3> patrolPositionsList = new List<Vector3>();

    // modelo del asset de la máquina cortacesped
    public GameObject mount;

    // fire particles
    public GameObject fireMount;
    protected GameObject fireMountInst;

    // ardiendo
    public bool afire = false;

    // indica el porcentaje de vida al que empieza a salir fuego de la montura
    public float startAfire = 0.25f;

    // indica el porcentaje de vida al que, si hay fuego, este desaparece
    public float stopAfire = 0.5f;

	public enum ScoutState
	{
		None,
		Patrolling
	}
	public ScoutState currentScoutState = ScoutState.None;


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

	/*public override void OnGUI ()
	{
        if (currentState != State.AscendingToHeaven)
        {
            base.OnGUI();

            GUI.skin.label.fontSize = 10;

            GUI.Label(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 65, 200, 50),
                currentState.ToString() + "\n" +
                currentScoutState.ToString() + "\n" +
                "NextPatrolPoint: " + nextPositionIndex );
        }
	}*/

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
        if (destTransform.name == "WorldFloor" || destTransform.name == "Terrain")
			currentScoutState = ScoutState.None;

        base.RightClickOnSelected(destiny, destTransform);
	}

    public override bool Heal (float amount)
    {
        bool aux = base.Heal(amount);

        // si la vida es superior al 85% y hay fuego, se destruye este fuego
        if ( afire && getLife() >= (GetMaximunLife() * stopAfire) )
        {
            Destroy(fireMountInst);

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
        if (mount)
            Destroy(mount);
    }

}
