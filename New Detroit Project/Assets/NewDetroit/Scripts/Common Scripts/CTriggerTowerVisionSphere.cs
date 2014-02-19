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
        Debug.Log("trigger!");
		ControllableCharacter unit = other.transform.GetComponent<ControllableCharacter>();
		if (unit != null)
		{
			TowerNeutral selfUnit = transform.parent.GetComponent<TowerNeutral>();
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
			TowerNeutral selfUnit = transform.parent.GetComponent<TowerNeutral>();
			if ( (selfUnit != null) && (selfUnit.teamNumber != unit.teamNumber) )
			{
				selfUnit.EnemyExitsInVisionSphere(unit);
			}
		}
	}
}
