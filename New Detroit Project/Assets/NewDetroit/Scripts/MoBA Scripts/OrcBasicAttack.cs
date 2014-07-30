using UnityEngine;
using System.Collections;

public class OrcBasicAttack : BasicAttack {

	
	// Update is called once per frame
	void Update () 
	{
		// If the orc is not in basic attack11
        if (this.owner.GetComponent<StateHero>().GetState() != StateHero.StateHeroEnum.AttackBasic)
		{
			this.collider.enabled = false; // The collider is activated
		}
		// Else if the orc is attacking
		else
		{
			this.collider.enabled = true; // The collider is desactivated
		}
	}
}
