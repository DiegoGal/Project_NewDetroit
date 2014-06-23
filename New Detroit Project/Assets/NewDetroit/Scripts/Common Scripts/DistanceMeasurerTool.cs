using UnityEngine;
using System.Collections.Generic;

public class DistanceMeasurerTool : MonoBehaviour
{

    // List of references at all the units of the Team 0
    public static List< CTeam > Army0 = new List< CTeam >();
    // List of references al all the units of the Team 1
    public static List< CTeam > Army1 = new List< CTeam >();

    // rows = Team 0, cols = Team 1
    // example distancesMatrix[2][3] is the distances between the units Army0[2] and Army1[3]
    public static List< List< float > > distancesMatrix = new List< List< float > >();

    private static int maxCalculationsPerUpdate = 4; // ojo esto se eleva al cuadrado
    private static int calculationsPerUpdate = 1;
    
    private enum SearchMode
    {
        no_search,
        pair_pair,
        pair_odd,
        odd_pair,
        odd_odd
    };
    private static SearchMode searchMode = SearchMode.no_search;
	
	private static int prevIndexi = 0,
                       prevIndexj = 0;

	
    void LateUpdate ()
    {
        /*if (searchMode != SearchMode.no_search)
            Debug.Log("Búsqueda (" + searchMode.ToString() + "): " + prevIndexi + ", " + prevIndexj + ".");*/
        switch(searchMode)
        {
            case SearchMode.no_search:

                break;

            case SearchMode.pair_pair:

                SearchStep();
                prevIndexj = (prevIndexj + 1) % Army1.Count;
                searchMode = SearchMode.pair_odd;
			
                break;
			
            case SearchMode.pair_odd:

                SearchStep();
                prevIndexi = (prevIndexi + 1) % Army0.Count;
                prevIndexj = (prevIndexj + calculationsPerUpdate + 2) % Army1.Count;
                searchMode = SearchMode.odd_pair;

                break;
			
            case SearchMode.odd_pair:

                SearchStep();
                prevIndexj = (prevIndexj + 1) % Army1.Count;
                searchMode = SearchMode.odd_odd;

                break;
			
            case SearchMode.odd_odd:

                SearchStep();

                if (prevIndexj + calculationsPerUpdate >= Army1.Count)
                    prevIndexi = (prevIndexi - 1 + calculationsPerUpdate) % Army0.Count;
                else
                {
                    prevIndexi--;
                    if (prevIndexi == -1)
                        prevIndexi = Army0.Count - 1;
                }
                prevIndexj = (prevIndexj + calculationsPerUpdate + 1) % Army1.Count;

                searchMode = SearchMode.pair_pair;

                break;
			
        } // switch
		
    } // Update

    /*public void OnGUI ()
    {
        GUI.skin.label.fontSize = 10;

        int icount = distancesMatrix.Count;
        int jcount;
        for (int i = 0; i < icount; i++)
        {
            jcount = distancesMatrix[i].Count;
            for (int j = 0; j < jcount; j++)
            {
                float aux = distancesMatrix[i][j];
                if (aux != float.MaxValue)
                    GUI.Label(new Rect(100 + 50 * j, 100 + 20 * i, 50, 20), aux.ToString());
                else
                    GUI.Label(new Rect(100 + 50 * j, 100 + 20 * i, 50, 20), "---");
            }
        }
    }
    */
    private void SearchStep ()
    {
        int list0Count = Army0.Count,
            list1Count = Army1.Count;

        for (int i = prevIndexi; i <= calculationsPerUpdate + prevIndexi; i += 2)
        {
            for (int j = prevIndexj; j <= calculationsPerUpdate + prevIndexj; j += 2)
            {
                //Debug.Log("Búsqueda: i:" + i + ", j:" + j + ".");
                int iAux = i % list0Count,
                    jAux = j % list1Count;

                CTeam unit0 = Army0[iAux],
                      unit1 = Army1[jAux];
                if (unit0 && unit1)
                {
                    // descartamos los casos por las distancias de x y z
                    float auxDist = Mathf.Abs(unit0.transform.position.x - unit1.transform.position.x);
                    float prevDist = distancesMatrix[iAux][jAux];
                    if (auxDist < unit0.visionSphereRadius || auxDist < unit1.visionSphereRadius)
                    {
                        auxDist = Mathf.Abs(unit0.transform.position.z - unit1.transform.position.z);
                        if (auxDist < unit0.visionSphereRadius || auxDist < unit1.visionSphereRadius)
                        {
                            // se calcula la nueva distancia
                            float newDist = Vector3.Distance
                            (
                                unit0.transform.position,
                                unit1.transform.position
                            );
                            distancesMatrix[iAux][jAux] = newDist;

                            // si la nueva distancia está dentro del área de visión de alguna de
                            // las dos unidades y la distancia prévia fuera mayor al área de visión
                            // significa que una nueva unidad ha entrado en el área de visión
                            // si, en cambio, la distancia previa era menor al área de visión y la nueva
                            // es mayor, significa que antes estaba dentro y acaba de salir
                            if (prevDist >= unit0.visionSphereRadius && newDist <= unit0.visionSphereRadius)
                                unit0.EnemyEntersInVisionSphere(unit1);
                            else if (prevDist < unit0.visionSphereRadius && newDist > unit0.visionSphereRadius)
                                unit0.EnemyLeavesVisionSphere(unit1);

                            if (prevDist >= unit1.visionSphereRadius && newDist <= unit1.visionSphereRadius)
                                unit1.EnemyEntersInVisionSphere(unit0);
                            else if (prevDist < unit1.visionSphereRadius && newDist > unit1.visionSphereRadius)
                                unit1.EnemyLeavesVisionSphere(unit0);
                        }
                        else
                        {
                            distancesMatrix[iAux][jAux] = float.MaxValue;
                        }
                    }
                    else
                    {
                        if (prevDist <= unit0.visionSphereRadius)
                            unit0.EnemyLeavesVisionSphere(unit1);
                        if (prevDist <= unit1.visionSphereRadius)
                            unit1.EnemyLeavesVisionSphere(unit0);

                        distancesMatrix[iAux][jAux] = float.MaxValue;
                    }
                }
            } // for j
        } // for i

    } // SearchStep

    public static void InsertUnit (CTeam unit)
    {
        if (unit.teamNumber == 0)
        {
            Army0.Add(unit);

            List<float> listAux = new List<float>();
            int auxCont = Army1.Count;
            for (int i = 0; i < auxCont; i++)
                listAux.Add(float.MaxValue);

            distancesMatrix.Add(listAux);
        }
        else if (unit.teamNumber == 1)
        {
            Army1.Add(unit);

            int auxCont = Army0.Count;
            for (int i = 0; i < auxCont; i++)
                distancesMatrix[i].Add(float.MaxValue);
        }
        /*else
            throw Exception e;*/

        if (calculationsPerUpdate < maxCalculationsPerUpdate)
            calculationsPerUpdate++;

        if (searchMode == SearchMode.no_search && Army0.Count > 0 && Army1.Count > 0)
            searchMode = SearchMode.pair_pair;
    }

    public static void DeleteUnit(CTeam unit)
    {
        if (unit.teamNumber == 0)
        {
            int unitIndex = Army0.IndexOf(unit);
            try
            {
                Army0.RemoveAt(unitIndex);
            }
            catch (System.Exception e)
            {
                bool WAT = true;
            }
            distancesMatrix.RemoveAt(unitIndex);

            if (prevIndexi > 0)
                prevIndexi--;
        }
        else if (unit.teamNumber == 1)
        {
            int unitIndex = Army1.IndexOf(unit);
            try
            {
                Army1.RemoveAt(unitIndex);
            }
            catch (System.Exception e)
            {
                bool WAT = true;
            }
            int auxCont = Army0.Count;
            for (int i = 0; i < auxCont; i++)
                distancesMatrix[i].RemoveAt(unitIndex);

            if (prevIndexj > 0)
                prevIndexj--;
        }
        /*else
            throw Exception e;*/

        if (Army0.Count == 0 || Army1.Count == 0)
        {
            prevIndexi = prevIndexj = 0;
            searchMode = SearchMode.no_search;
        }

        if (calculationsPerUpdate > Army0.Count + Army1.Count)
            calculationsPerUpdate--;
    }

}
