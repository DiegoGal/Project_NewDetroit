using UnityEngine;
using System.Collections;

public class ControllableCharacter : MonoBehaviour
{
	// --------------------------------------------------------------

    // referente to the Team component of the character
    public CTeam team;

	// number that identifies the team to which the character belongs
    private int teamNumber;

    // reference to the Life component of the character
    public CLife life;

    // Reference to the position of the unit team base
    protected Vector3 basePosition = new Vector3();
    // Reference to the base
    public BaseController baseController;

	//Experience that gives the unit when it dies
	public int experienceGived = 0;

    // Indicates if the unit can receive damage or not
    //public bool invincible = false;

    // the screen position of the character
    protected Vector3 screenPosition;

    // the vision radious of the unit
    public float visionSphereRadius = 8.0f;
    public float maxAttackDistance = 2.0f;

    // To determinate who's player belongs
    public bool isMine; // Tell us if that instance if ours or not

    public float radius;

	// --------------------------------------------------------------

    public virtual void Start ()
    {
        screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        team = GetComponent<CTeam>();
        life = GetComponent<CLife>();
        teamNumber = team.teamNumber;
        team.visionSphereRadius = visionSphereRadius;
    }

    public virtual void Update ()
    {
        screenPosition = Camera.main.WorldToScreenPoint(transform.position);
    }

    public void SetBasePosition (Vector3 basePosition)
    {
        this.basePosition = basePosition;
    }

    public void SetArmyBase (BaseController baseController)
    {
        this.baseController = baseController;
    }

    protected virtual void SetVisionRadiuSphere (float newRadius)
    {
        visionSphereRadius = newRadius;
        team.visionSphereRadius = visionSphereRadius;
    }

    // if type == 'P' is phisical damage if type == 'M' is magical damage
    public virtual bool Damage (float damage, char type = 'P')
    {
        return life.Damage(damage, type);
    }

    public virtual void Die ()
    {
        life.Die();
    }

    public virtual bool Heal (float amount)
    {
        return life.Heal(amount);
    }

	// Return the current life of the heroe
	public float getLife ()
	{
		return life.currentLife;
	}

    public float GetMaximunLife ()
    {
        return life.maximunLife;
    }

	// Returns the gived experience when it dies
	public int getExperienceGived ()
	{
		return this.experienceGived;
	}

    public bool IsAlive ()
    {
        return (life.currentLife > 0);
    }

    public virtual void EnemyEntersInVisionSphere (ControllableCharacter enemy)
    {
        
    }

    public virtual void EnemyLeavesVisionSphere (ControllableCharacter enemy)
    {
        
    }

    public float GetRadius ()
    {
        return radius;
    }

    public int GetTeamNumber()
    {
        return teamNumber;
    }

    //DistanceMeasureTool - Network
    protected void network()
    {
        DistanceMeasurerTool.InsertUnit(GetComponent<CTeam>());
    }

} // class ControllableCharacter
