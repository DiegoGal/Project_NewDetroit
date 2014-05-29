using UnityEngine;
using System.Collections;

public class CLife : MonoBehaviour
{
    
    // Life variables
    public float maximunLife = 100.0f;
    public float currentLife;

    // Indicates if the unit can receive damage or not
    public bool invincible = false;

    public void Start ()
    {
        currentLife = maximunLife;
    }

    public bool IsAlive ()
    {
        return (currentLife > 0);
    }

    // if type == 'P' is phisical damage if type == 'M' is magical damage
    public bool Damage(float damage, char type = 'P')
    {
        //Debug.Log("damage");
        if (!invincible)
            currentLife -= damage;
        if (currentLife <= 0)
        {
            Die();
            return true;
        }
        else
            return false;
    }

    public void Die ()
    {
        currentLife = 0;
    }

    public bool Heal (float amount)
    {
        if (currentLife < maximunLife)
        {
            currentLife += amount;
            // si lo hemos curado "de más" le damos el valor máximo
            if (maximunLife < currentLife)
                currentLife = maximunLife;
        }
        if (currentLife == maximunLife)
            return true;
        else
            return false;
    }

    public float getLife()
    {
        return currentLife;
    }

    public void setLife(float eLife)
    {
        currentLife = eLife;
    }
}
