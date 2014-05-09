using UnityEngine;
using System.Collections;

public class CTeamTowerArmy : CTeam
{

    // referent to the component of the Unit itself
    private TowerGoblin selfUnit;

    public void Awake ()
    {
        selfUnit = GetComponent<TowerGoblin>();
    }

    public override void EnemyEntersInVisionSphere (CTeam unit)
    {

    }

    public override void EnemyLeavesVisionSphere(CTeam unit)
    {

    }

}
