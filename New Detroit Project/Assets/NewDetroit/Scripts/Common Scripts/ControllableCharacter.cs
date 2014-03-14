using UnityEngine;
using System.Collections;

public class ControllableCharacter : MonoBehaviour
{
	// --------------------------------------------------------------

    
	// number that identifies the team to which the character belongs
    public int teamNumber;

    // Life variables
    public float maximunLife = 100.0f;
    public float currentLife;

    // Reference to the position of the unit team base
    protected Vector3 basePosition = new Vector3();
    // Reference to the base
    public BaseController baseController;

	//Experience that gives the unit when it dies
	public int experienceGived = 0;

    // Indicates if the unit can receive damage or not
    public bool invincible = false;

    // the screen position of the character
    protected Vector3 screenPosition;

	// --------------------------------------------------------------

    public virtual void Start ()
    {
        screenPosition = Camera.main.WorldToScreenPoint(transform.position);
    }

    public virtual void OnGUI ()
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

    // if type == 'P' is phisical damage if type == 'M' is magical damage
    public virtual bool Damage (float damage, char type)
    {
        //Debug.Log("damage");
        if (!invincible)
            currentLife -= damage;
        return (currentLife <= 0);
    }

    public virtual bool Heal (float life)
    {
        if (currentLife < maximunLife)
        {
            currentLife += life;
            // si lo hemos curado "de más" le damos el valor máximo
            if (maximunLife < currentLife)
                currentLife = maximunLife;
        }
        if (currentLife == maximunLife)
            return true;
        else
            return false;
    }

	// Return the current life of the heroe
	public float getLife ()
	{
		return this.currentLife;
	}

	// Returns the gived experience when it dies
	public int getExperienceGived ()
	{
		return this.experienceGived;
	}

} // class ControllableCharacter
