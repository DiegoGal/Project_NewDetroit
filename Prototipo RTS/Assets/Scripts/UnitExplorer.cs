using UnityEngine;
using System.Collections;

public class UnitExplorer : UnitController {

	public int attackPower = 1;

	protected const float totalLife = 100.0f;

	// Use this for initialization
	public override void Start ()
	{
		base.Start();
		basicAttackPower = secondaryAttackPower = attackPower;
	}
	
	// Update is called once per frame
	public override void Update () 
	{
		base.Update();
	}

	// Repair is called by the engineers
	public bool Heal(float sum)
	{
		// increasement of the towers life
		if (currentLife < totalLife)
		{
			currentLife += sum;
			if (totalLife < currentLife)
				currentLife = totalLife;
		}
		if (currentLife == totalLife)
		{
			return true;
		}
		else
			return false;
	}
}
