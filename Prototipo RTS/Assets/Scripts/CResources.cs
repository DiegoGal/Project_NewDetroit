using UnityEngine;
using System.Collections;

public class CResources : MonoBehaviour
{
    public int totalResources = 10000;
    private int actualResources;

    void Start()
    {
        actualResources = totalResources;
    }

    public int GetResources(int cant)
    {
        if (cant >= actualResources)
        {
            actualResources -= cant;
        }
        else
        {
            cant = actualResources;
            actualResources = 0;
        }
        return cant;
    }

}