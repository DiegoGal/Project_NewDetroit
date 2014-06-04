using UnityEngine;
using System.Collections;

public class OrcBasicAttack : BasicAttack {

	
	// Update is called once per frame
	void Update () 
	{
		//move();
		if (this.owner != null && this.owner.GetComponent<HeroeController> ().isMine)
		{

			// If the orc is not in basic attack11
			if (this.owner.GetComponent<HeroeController> ().getState() != HeroeController.StateHeroe.AttackBasic)
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
}
