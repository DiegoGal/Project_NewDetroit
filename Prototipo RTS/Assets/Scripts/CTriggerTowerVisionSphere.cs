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
		UnitController unit = other.transform.GetComponent<UnitController>();
		if (unit != null)
		{
			NeutralTower selfUnit = transform.parent.GetComponent<NeutralTower>();
			if ( (selfUnit != null) && (selfUnit.teamNumber != unit.teamNumber) )
			{
				selfUnit.EnemyEntersInVisionSphere(unit);
			}
		}
	}
}
