using UnityEngine;
using System.Collections.Generic;

public static class DistanceMeasurerTool : MonoBehaviour
{

    // List of references at all the units of the Team 0
    private static List<ControllableCharacter> Army0 = new List<ControllableCharacter>();
    // List of references al all the units of the Team 1
    private static List<ControllableCharacter> Army1 = new List<ControllableCharacter>();

    // rows = Team 0, cols = Team 1
    // example distancesMatrix[2][3] is the distances between the units Army0[2] and Army1[3]
    private static List< List< float > > distancesMatrix;

    private static int maxCalculationsPerUpdate = 20;
    private static int calculationsPerUpdate = 1;

    private static bool pair = true;
    private enum SearchMode
    {
        pair_pair,
        pair_odd,
        odd_pair,
        odd_odd
    };
    private static SearchMode searchMode = SearchMode.pair_pair;
	
	private static int prevIndexi = 0,
                       prevIndexj =  0;

	// Use this for initialization
	void Start ()
	{
		/*for (int i=0; i<MAX_UNITS; i++)
			for (int j=0; j<MAX_UNITS; j++)
				distancesMatrix[i][j] = -1.0f;*/
	}
	
	// Update is called once per frame 
    void Update ()
    {
        
        switch(searchMode)
        {
            case SearchMode.pair_pair:

                SearchStep();
                prevIndexj++;
                searchMode = SearchMode.pair_odd;
			
                break;
			
            case SearchMode.pair_odd:

                SearchStep();
                prevIndexi++;
                prevIndexj--;
                searchMode = SearchMode.odd_pair;

                break;
			
            case SearchMode.odd_pair:

                SearchStep();
                prevIndexj++;
                searchMode = SearchMode.odd_odd;

                break;
			
            case SearchMode.odd_odd:

                SearchStep();
                if (prevIndexj + calculationsPerUpdate >= calculationsPerUpdate)
                    prevIndexi = (calculationsPerUpdate + prevIndexi) % calculationsPerUpdate;
                else
                    prevIndexi = 0;
                prevIndexj = (calculationsPerUpdate + prevIndexj) % calculationsPerUpdate;
                searchMode = SearchMode.pair_pair;

                break;
			
        } // switch
		
    } // Update

    private void SearchStep ()
    {
        int list0Count = Army0.Count,
            list1Count = Army1.Count;

        for (int i = prevIndexi; i < calculationsPerUpdate + prevIndexi; i = (i + 2) % list0Count)
        {
            for (int j = prevIndexj; j < calculationsPerUpdate + prevIndexj; j = (j + 2) % list1Count)
            {
                ControllableCharacter unit0 = Army0[i], unit1 = Army1[j];
                if (unit0 && unit1)
                {
                    // descartamos los casos por las distancias de x y z
                    float distAux = Mathf.Abs(unit0.transform.position.x - unit1.transform.position.x);
                    if (distAux < unit0.visionSphereRadious && distAux < unit1.visionSphereRadious)
                    {
                        distAux = Mathf.Abs(unit0.transform.position.z - unit1.transform.position.z);
                        if (distAux < unit0.visionSphereRadious && distAux < unit1.visionSphereRadious)
                        {
                            distancesMatrix[i][j] = Vector3.Distance
                            (
                                unit0.transform.position,
                                unit1.transform.position
                            );
                        }
                    }
                    else
                    {
                        distancesMatrix[i][j] = -1.0f;
                    }
                }
            } // for j
        } // for i

    } // SearchStep

    public static void InsertUnit (ControllableCharacter unit)
    {
        if (unit.teamNumber == 0)
        {
            Army0.Add(unit);

            List<float> listAux = new List<float>();
            int auxCont = Army1.Count;
            for (int i = 0; i < auxCont; i++)
                listAux.Add(-1.0f);

            distancesMatrix.Add(listAux);
        }
        else if (unit.teamNumber == 1)
        {
            Army1.Add(unit);

            int auxCont = Army0.Count;
            for (int i = 0; i < auxCont; i++)
                distancesMatrix[i].Add(-1.0f);
        }
        /*else
            throw Exception e;*/

        if (calculationsPerUpdate < maxCalculationsPerUpdate)
            calculationsPerUpdate++;
    }

    public static void DeleteUnit (ControllableCharacter unit)
    {
        if (unit.teamNumber == 0)
        {
            int unitIndex = Army0.IndexOf(unit);
            Army0.RemoveAt(unitIndex);
            distancesMatrix.RemoveAt(unitIndex);
        }
        else if (unit.teamNumber == 1)
        {
            int unitIndex = Army1.IndexOf(unit);
            Army1.RemoveAt(unitIndex);
            int auxCont = Army0.Count;
            for (int i = 0; i < auxCont; i++)
                distancesMatrix[i].RemoveAt(unitIndex);
        }
        /*else
            throw Exception e;*/

        if (calculationsPerUpdate > Army0.Count + Army1.Count)
            calculationsPerUpdate--;
    }

}
