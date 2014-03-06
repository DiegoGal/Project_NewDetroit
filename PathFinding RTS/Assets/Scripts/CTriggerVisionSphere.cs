using UnityEngine;
using System.Collections;

public class CTriggerVisionSphere : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

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
