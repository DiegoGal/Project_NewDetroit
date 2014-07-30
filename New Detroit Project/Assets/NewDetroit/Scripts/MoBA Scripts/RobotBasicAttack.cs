﻿using UnityEngine;
using System.Collections;

public class RobotBasicAttack : BasicAttack {
	

	// Update is called once per frame
	void Update () {
		if (owner.GetComponent<StateRobot>() != null && owner.animation.IsPlaying("Attack3"))
		{
			collider.enabled = false;
		}
		// If the orc is not in basic attack11
        else if (owner.GetComponent<StateHero>().GetState() != StateHero.StateHeroEnum.AttackBasic)
		{
			collider.enabled = false; // The collider is activated
		}
		// Else if the orc is attacking
		else
		{
			collider.enabled = true; // The collider is desactivated
		}
	}
}
