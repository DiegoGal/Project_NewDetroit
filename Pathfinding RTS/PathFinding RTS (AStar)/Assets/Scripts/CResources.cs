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

    void OnCollisionEnter(Collision collision)
    {
        UnitHarvester unit = collision.transform.GetComponent<UnitHarvester>();
        if (unit != null)
        {
            //Debug.Log("COLISION2");
            
            // esto es interesante para instanciar partículas cuando se esté recolectando:
            /*ContactPoint contact = collision.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;
            Instantiate(explosionPrefab, pos, rot) as Transform;
            Destroy(gameObject);*/

            unit.StartChoping();
        }
    }

    void OnGUI()
    {
        Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);

        GUI.Label(new Rect(camPos.x - 35, Screen.height - camPos.y - 20, 100, 50),
            actualResources.ToString());
    }

    public int GetResources(int cant)
    {
        if (cant <= actualResources)
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

} // class CResources