using UnityEngine;
using System.Collections;

public class CTriggerVisionSphere : MonoBehaviour
{

    void OnTriggerEnter (Collider other)
    {
		ControllableCharacter unit = other.transform.GetComponent<ControllableCharacter>();
        if (unit != null)
        {
            UnitArtillery selfUnit = transform.parent.GetComponent<UnitArtillery>();
            if ( (selfUnit != null) && (selfUnit.teamNumber != unit.teamNumber) )
            {
                selfUnit.EnemyEntersInVisionSphere(unit);
            }
        }
    }

    void OnTriggerExit (Collider other)
    {
		ControllableCharacter unit = other.transform.GetComponent<ControllableCharacter>();
        if (unit != null)
        {
            UnitArtillery selfUnit = transform.parent.GetComponent<UnitArtillery>();
            if ((selfUnit != null) && (selfUnit.teamNumber != unit.teamNumber))
            {
                selfUnit.EnemyLeavesVisionSphere(unit);
            }
        }
    }

}
