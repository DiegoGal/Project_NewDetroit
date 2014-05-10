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
            if ( (selfUnit != null) && (selfUnit.GetTeamNumber() != unit.GetTeamNumber()) )
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
            if ( (selfUnit != null) && (selfUnit.GetTeamNumber() != unit.GetTeamNumber()) )
            {
                selfUnit.EnemyLeavesVisionSphere(unit);
            }
        }
    }

}
