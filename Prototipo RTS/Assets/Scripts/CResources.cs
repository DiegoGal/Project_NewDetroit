using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CResources : MonoBehaviour
{
    public int totalResources = 10000;
    private int actualResources;

    public float distanceToWait = 2.0f;

    public int numHarvestPositions = 8;
    private Vector3[] harvestPositions;
    private bool[] harvestPosTaken;

    // desplazamiento de los harvest positions
    public float despPosition = 0.4f;

    // Queue of units harversters which are waiting in the mine
    private List<UnitHarvester> harvesterQueue;

    // for debugging
    private GameObject[] cubes;

    void Start ()
    {
        actualResources = totalResources;

        distanceToWait += transform.GetComponent<SphereCollider>().radius + despPosition;

        harvestPositions = new Vector3[numHarvestPositions];
        harvestPosTaken = new bool[numHarvestPositions];

        cubes = new GameObject[numHarvestPositions];
        
        float twoPi = Mathf.PI * 2;
        Vector3 center = transform.Find("center").position;
        for (int i = 0; i < numHarvestPositions; i++)
        {
            Vector3 pos = new Vector3
            (
                center.x +
                    (transform.GetComponent<SphereCollider>().radius + despPosition)*Mathf.Sin(i*(twoPi/numHarvestPositions)),
                0,
                center.z +
                    (transform.GetComponent<SphereCollider>().radius + despPosition)*Mathf.Cos(i*(twoPi/numHarvestPositions))
            );
            harvestPositions[i] = pos;
            harvestPosTaken[i] = false;

            cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubes[i].transform.position = pos;
            Destroy(cubes[i].GetComponent<BoxCollider>());
            cubes[i].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);
            cubes[i].transform.parent = this.transform;
        }

        harvesterQueue = new List<UnitHarvester>();
    }

    void OnCollisionEnter (Collision collision)
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

            //unit.StartChoping();
        }
    }

    void OnGUI ()
    {
        Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);

        GUI.Label(new Rect(camPos.x - 35, Screen.height - camPos.y - 20, 100, 50),
            actualResources.ToString());
    }

    public int GetResources (int cant)
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

    public bool GetHarvestPosition (ref Vector3 pos, ref int index, UnitHarvester unit)
    {
        int i = 0; bool found = false;
        while (!found && (i < numHarvestPositions))
        {
            if (!harvestPosTaken[i])
            {
                pos = harvestPositions[i];
                index = i;
                harvestPosTaken[i] = true;
                cubes[i].renderer.material.color = new Color(0.863f, 0.078f, 0.235f);
                found = true;
            }
            else
                i++;
        }
        if (!found)
            harvesterQueue.Add(unit);
        return found;
    }

    public void LeaveHarvestPosition (int index)
    {
        harvestPosTaken[index] = false;
        cubes[index].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);
        if (harvesterQueue.Count > 0)
        {
            UnitHarvester unit = harvesterQueue[0];
            unit.FinishWaiting(harvestPositions[index], index);
            harvesterQueue.RemoveAt(0);
            harvestPosTaken[index] = true;
            cubes[index].renderer.material.color = new Color(0.863f, 0.078f, 0.235f);
        }
    }

    public void LeaveQueue (UnitHarvester unit)
    {
        harvesterQueue.Remove(unit);
    }

} // class CResources