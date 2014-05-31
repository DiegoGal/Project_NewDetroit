using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColliderConstruct : MonoBehaviour
{

    public int collidersInside = 0;

    private TowerArmy selfUnitTower;
    private Warehouse selfUnitWarehouse;

    public void Awake ()
    {
        selfUnitTower = transform.parent.GetComponent<TowerArmy>();
        selfUnitWarehouse = transform.parent.GetComponent<Warehouse>();
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.name != "VisionSphere" && other.name != "TowerVisionSphere" && other.name != "Plane" && other.name != "Terrain")
        {
            //Debug.Log("No puedes construir");
            collidersInside++;
            if (collidersInside >= 1)
            {
                if (selfUnitTower)
                    selfUnitTower.SetCanConstruct(false);
                else if (selfUnitWarehouse)
                    selfUnitWarehouse.SetCanConstruct(false);
            }
        }
    }

    void OnTriggerExit (Collider other)
	{
        if (other.name != "VisionSphere" && other.name != "TowerVisionSphere" && other.name != "Plane" && other.name != "Terrain")
        {
            collidersInside--;
            if (collidersInside <= 0)
            {
                // the first is the floor
                if (selfUnitTower)
                {
                    selfUnitTower.SetCanConstruct(true);
                    //Debug.Log("Puedes construir");
                }
                else if (selfUnitWarehouse)
                {
                    selfUnitWarehouse.SetCanConstruct(true);
                    //Debug.Log("Puedes construir");
                }
            }
        }
	}

}
