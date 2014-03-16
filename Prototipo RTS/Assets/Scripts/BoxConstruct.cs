using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxConstruct : MonoBehaviour {

    private List<Collider> colliderList = new List<Collider>();
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.name != "VisionSphere" && other.name != "TowerVisionSphere")
        {
            Debug.Log("No puedes construir");
            colliderList.Add(other);
            TowerGoblin selfUnit = transform.parent.GetComponent<TowerGoblin>();
            if (selfUnit != null && colliderList.Count == 1)
                selfUnit.SetCanConstruct(false);
            else
            {
                Warehouse selfUnitWH = transform.parent.GetComponent<Warehouse>();
                if (selfUnitWH != null && colliderList.Count == 1)
                    selfUnitWH.SetCanConstruct(false);
            }
        }
    }

    void OnTriggerExit (Collider other)
	{
        if (other.name != "VisionSphere" && other.name != "TowerVisionSphere")
        {
            Debug.Log("No puedes construir");
            colliderList.Remove(other);
            TowerGoblin selfUnit = transform.parent.GetComponent<TowerGoblin>();
            if (selfUnit != null && colliderList.Count == 0)
            {
                selfUnit.SetCanConstruct(true);
                Debug.Log("Puedes construir");
            }
            else
            {
                Warehouse selfUnitWH = transform.parent.GetComponent<Warehouse>();
                if (selfUnitWH != null && colliderList.Count == 0)
                {
                    selfUnitWH.SetCanConstruct(true);
                    Debug.Log("Puedes construir");
                }
            }
        }
	}
}
