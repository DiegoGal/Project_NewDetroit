using UnityEngine;
using System.Collections;

public class CTeam : MonoBehaviour
{

    public int teamNumber = -1;

    // indicates the color of the units, 0=green, 1=purple
    public int teamColorIndex = 0;

    public float visionSphereRadius;

    public virtual void EnemyEntersInVisionSphere (CTeam unit)
    {

    }

    public virtual void EnemyLeavesVisionSphere (CTeam unit)
    {

    }

}
