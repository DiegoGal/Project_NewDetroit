﻿using UnityEngine;
using System.Collections;

public class CTeamTowerArmy : CTeam
{

    // referent to the component of the Unit itself
    private TowerArmy selfUnit;

    public void Awake()
    {
        base.Awake();
        selfUnit = GetComponent<TowerArmy>();
    }

    public override void EnemyEntersInVisionSphere (CTeam unit)
    {
        selfUnit.EnemyEntersInVisionSphere(unit);
    }

    public override void EnemyLeavesVisionSphere (CTeam unit)
    {
        selfUnit.EnemyExitsInVisionSphere(unit);
    }

}