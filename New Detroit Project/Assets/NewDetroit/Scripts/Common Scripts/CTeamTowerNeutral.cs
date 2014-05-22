using UnityEngine;
using System.Collections;

public class CTeamTowerNeutral : CTeam
{

    // referent to the component of the Unit itself
    private TowerNeutral selfUnit;

    public void Awake ()
    {
        selfUnit = GetComponent<TowerNeutral>();
    }

    public override void EnemyEntersInVisionSphere (CTeam unit)
    {
        selfUnit.EnemyEntersInVisionSphere(unit);
    }

    public override void EnemyLeavesVisionSphere(CTeam unit)
    {
        selfUnit.EnemyExitsInVisionSphere(unit);
    }

}
