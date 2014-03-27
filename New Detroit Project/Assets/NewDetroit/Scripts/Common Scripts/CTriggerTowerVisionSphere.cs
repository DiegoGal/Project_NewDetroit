using UnityEngine;
using System.Collections;

public class CTriggerTowerVisionSphere : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter (Collider other)
	{
		ControllableCharacter unit = other.transform.GetComponent<ControllableCharacter>();
		if (unit != null)
		{
            //Debug.Log("Entra uno!");
			Tower selfUnit = transform.parent.GetComponent<Tower>();
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
            Tower selfUnit = transform.parent.GetComponent<Tower>();
            if ((selfUnit != null) && (selfUnit.teamNumber != unit.teamNumber))
            {
                selfUnit.EnemyExitsInVisionSphere(unit);
            }
           
		}
	}
}
