using UnityEngine;
using System.Collections;

// This class is maded to take the damage depending on the level of the hero that uses it
// and the reduction it has.
public class ParticleDamage : MonoBehaviour {
    // Public
    // Private
    private int totalDamage = 0;

    public int getDamage()
    {
        return totalDamage;
    }

    public void setDamage(int newDamage)
    {
        totalDamage = newDamage;
    }
}
