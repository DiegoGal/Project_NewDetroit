using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxConstruct : MonoBehaviour
{

    private List<Collider> colliderList = new List<Collider>();

    private TowerGoblin selfUnitTower;
    private Warehouse selfUnitWarehouse;

    public void Awake ()
    {
        selfUnitTower = transform.parent.GetComponent<TowerGoblin>();
        selfUnitWarehouse = transform.parent.GetComponent<Warehouse>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name != "VisionSphere" && other.name != "TowerVisionSphere")
        {
            Debug.Log("No puedes construir");
            colliderList.Add(other);
            if (colliderList.Count == 1)
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
        if (other.name != "VisionSphere" && other.name != "TowerVisionSphere")
        {
            Debug.Log("No puedes construir");
            colliderList.Remove(other);
            if (colliderList.Count == 0)
            {
                if (selfUnitTower)
                {
                    selfUnitTower.SetCanConstruct(true);
                    Debug.Log("Puedes construir");
                }
                else if (selfUnitWarehouse)
                {
                    selfUnitWarehouse.SetCanConstruct(true);
                    Debug.Log("Puedes construir");
                }
            }
        }
	}

}
