using UnityEngine;
using System.Collections;

// This class is maded to take the damage depending on the level of the hero that uses it
// and the reduction it has.
public class ParticleDamage : Photon.MonoBehaviour {
    // Public
    // Private
    protected int totalDamage = 0;

    public int GetDamage()
    {
        return totalDamage;
    }

    public void SetDamage(int newDamage)
    {
        totalDamage = newDamage;
    }
}
