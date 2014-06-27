using UnityEngine;
using System.Collections;

public class CTeamUnit : CTeam
{

    // referent to the component of the Unit itself
    private ControllableCharacter selfUnit;

    public void Awake ()
    {
        base.Awake();
        int wat = teamNumber;
        selfUnit = GetComponent<ControllableCharacter>();

        /*if (!PhotonNetwork.connected)
        if (GetComponent<ControllableCharacter>().getTypeHero() == ControllableCharacter.TypeHeroe.Orc) GetComponent<CTeam>().teamNumber = 0;
        else GetComponent<CTeam>().teamNumber = 1;
        DistanceMeasurerTool.InsertUnit (this);*/
        if (PhotonNetwork.offlineMode)
        {
            // TODO! esto es una ñapa, esta así porque aunque en el inspector la unidad tenga el teamNumber
            // a 1, cuando se llama al Awake de la unidad, este atributo ya es 0, no se sabe dónde se ha
            // cambiado
            if (GetComponent<ControllableCharacter>().getTypeHero() == ControllableCharacter.TypeHeroe.Orc)
                GetComponent<CTeam>().teamNumber = 0;
            else
                GetComponent<CTeam>().teamNumber = 1;
        }
    }

    public void Start ()
    {
        DistanceMeasurerTool.InsertUnit(this);
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
