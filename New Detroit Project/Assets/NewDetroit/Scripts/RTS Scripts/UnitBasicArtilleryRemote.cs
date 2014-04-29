using UnityEngine;
using System.Collections;

public class UnitBasicArtilleryRemote : MonoBehaviour
{

    public UnitArtillery.ArtilleryState currentArtilleryState = UnitArtillery.ArtilleryState.None;
    public UnitController.State currentState = UnitController.State.Idle;
    public UnitController.State lastState = UnitController.State.Idle;

    public CLife life;

    //TODO Animar según el estado actual

}
