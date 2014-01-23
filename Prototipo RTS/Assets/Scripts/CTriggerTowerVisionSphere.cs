﻿using UnityEngine;
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
		UnitController unit = other.transform.GetComponent<UnitController>();
		if (unit != null)
		{
            Debug.Log("unidad detectada");
			NeutralTower selfUnit = transform.parent.GetComponent<NeutralTower>();
			if ( (selfUnit != null) && (selfUnit.teamNumber != unit.teamNumber) )
			{
                Debug.Log("unidad enemiga");
				//selfUnit.EnemyEntersInVisionSphere(unit);
			}
		}
	}
}
