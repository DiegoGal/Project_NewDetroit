using UnityEngine;
using System.Collections;

public class CTeamUnit : CTeam
{

    // referent to the component of the Unit itself
    private ControllableCharacter selfUnit;

    public void Awake ()
    {
        base.Awake();
        selfUnit = GetComponent<ControllableCharacter>();
    }

    public void Start ()
    {
        if (!PhotonNetwork.connected)
        if (GetComponent<ControllableCharacter>().getTypeHero() == ControllableCharacter.TypeHeroe.Orc) GetComponent<CTeam>().teamNumber = 0;
        else GetComponent<CTeam>().teamNumber = 1;
        DistanceMeasurerTool.InsertUnit (this);
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
