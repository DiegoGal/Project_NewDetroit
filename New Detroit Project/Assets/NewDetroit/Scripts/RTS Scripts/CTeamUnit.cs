using UnityEngine;
using System.Collections;

public class CTeamUnit : CTeam
{

    // referent to the component of the Unit itself
    private ControllableCharacter selfUnit;

    void Start () { }

    public void Awake ()
    {
        selfUnit = GetComponent<ControllableCharacter>();
    }

    public override void EnemyEntersInVisionSphere (CTeam unit)
    {
        selfUnit.EnemyEntersInVisionSphere(unit);
    }

    public override void EnemyLeavesVisionSphere (CTeam unit)
    {
        selfUnit.EnemyLeavesVisionSphere(unit);
    }

}
