using UnityEngine;
using System.Collections;

public class ControllableCharacter : MonoBehaviour
{
    // number that identifies the team to which the character belongs
    public int teamNumber;

    // Life variables
    public float maximunLife = 100.0f;
    protected float currentLife;

    // Reference to the position of the unit team base
    protected Vector3 basePosition = new Vector3();
    // Reference to the base
    public BaseController baseController;

    public void SetBasePosition (Vector3 basePosition)
    {
        this.basePosition = basePosition;
    }

    public void SetArmyBase (BaseController baseController)
    {
        this.baseController = baseController;
    }

    public virtual bool Damage (float damage)
    {
        //Debug.Log("damage");
        currentLife -= damage;
        return (currentLife <= 0);
    }

} // class ControllableCharacter
