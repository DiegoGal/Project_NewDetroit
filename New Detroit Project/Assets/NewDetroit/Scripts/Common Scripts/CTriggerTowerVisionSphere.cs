using UnityEngine;
using System.Collections;

public class CTriggerTowerVisionSphere : MonoBehaviour
{

    private int search = 0;
    private bool up = false;
	
	// Update is called once per frame
	void Update () 
    {
        if (search == 5)
        {
            if (up)
                transform.localScale = new Vector3(transform.localScale.x + 0.1f, transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(transform.localScale.x - 0.1f, transform.localScale.y, transform.localScale.z);
            up = !up;
            search = 0;
        }
        else search++;
	}

	void OnTriggerEnter (Collider other)
	{
        CTeam unit = other.transform.GetComponent<CTeam>();
		if (unit != null)
		{
            //Debug.Log("Entra uno!");
			Tower selfUnit = transform.parent.GetComponent<Tower>();
            if ( (selfUnit != null) && (selfUnit.team.teamNumber != unit.teamNumber) )
			{
				selfUnit.EnemyEntersInVisionSphere(unit);
			}
           
		}
	}

	void OnTriggerExit (Collider other)
	{
        CTeam unit = other.transform.GetComponent<CTeam>();
		if (unit != null)
		{
            Tower selfUnit = transform.parent.GetComponent<Tower>();
            if ( (selfUnit != null) && (selfUnit.team.teamNumber != unit.teamNumber) )
            {
                selfUnit.EnemyExitsInVisionSphere(unit);
            }
           
		}
	}
}
