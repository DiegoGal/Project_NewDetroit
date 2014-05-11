using UnityEngine;
using System.Collections;

public class CTriggerVisionSphere : MonoBehaviour
{

    void OnTriggerEnter (Collider other)
    {
        CTeam unit = other.transform.GetComponent<CTeam>();
        if (unit != null)
        {
            UnitArtillery selfUnit = transform.parent.GetComponent<UnitArtillery>();
            if ( (selfUnit != null) && (selfUnit.GetTeamNumber() != unit.teamNumber) )
            {
                selfUnit.EnemyEntersInVisionSphere(unit);
            }
        }
    }

    void OnTriggerExit (Collider other)
    {
        CTeam unit = other.transform.GetComponent<CTeam>();
        if (unit != null)
        {
            UnitArtillery selfUnit = transform.parent.GetComponent<UnitArtillery>();
            if ((selfUnit != null) && (selfUnit.GetTeamNumber() != unit.teamNumber))
            {
                selfUnit.EnemyLeavesVisionSphere(unit);
            }
        }
    }

}
