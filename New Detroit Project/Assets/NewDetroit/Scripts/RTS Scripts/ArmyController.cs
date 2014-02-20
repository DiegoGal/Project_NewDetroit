using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArmyController : MonoBehaviour
{
	public int teamNumber;

	public GameObject armyBase;
    public List<GameObject> unitList = new List<GameObject>();
    public List<GameObject> unitSelectedList = new List<GameObject>();

    private RaycastHit myHit; // Structure used to get information back from a raycast.
    private Ray myRay;

    private bool selecting;
    private Vector3 lastClick;
    private bool mouseButtonPreshed;
    private Vector3[] squareSelectionPointsScreen;      // positions of the corners in the screen
    private Vector3[] squareSelectionPointsProyected;   // positions of the corners in the world

    private int resources;
    private float lastCrowdAngle; // último ángulo de desplazamiento de la bandada

	private int layerMask; // para obviar la capa de la niebla

    // Use this for initialization
    void Start ()
    {
        myHit = new RaycastHit();
        selecting = false;
        lastClick = new Vector3();
        mouseButtonPreshed = false;
        squareSelectionPointsScreen = new Vector3[4];
        squareSelectionPointsProyected = new Vector3[4];

        resources = 0;
        lastCrowdAngle = 0;

		// ejemplo Unity: http://docs.unity3d.com/Documentation/Components/Layers.html
		// Bit shift the index of the layer (8) to get a bit mask
		layerMask = 1 << 8 | 1 << 2;

		// This would cast rays only against colliders in layer 8 and 2.
		// But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
		layerMask = ~layerMask;
    }

    // Update is called once per frame
    void Update ()
    {
        // se pulsa el botón izquierdo del ratón
        if (Input.GetMouseButtonDown(0))
        {
            lastClick = Input.mousePosition;
            mouseButtonPreshed = true;

            lastCrowdAngle = 0;
        }
        // se levanta el botón izquierdo del ratón
        if (mouseButtonPreshed && Input.GetMouseButtonUp(0))
            mouseButtonPreshed = false;

        // se esta pulsando el botón izquierdo mientras se arrastra el ratón
        if (mouseButtonPreshed && (Input.mousePosition != lastClick))
        {
            // se inicia el modo de selección múltiple
            if (!selecting)
            {
                // deselect all the units
                DeselectAll();
                //Debug.Log("iniciando seleccion: " + Input.mousePosition);
                squareSelectionPointsScreen[0] = lastClick;// Input.mousePosition;
                selecting = true;
            }
            else // se continúa el modo de selección múltiple
            {
                // select units on the fly
                CreatingSquare2();
            }
        }

        if (selecting && Input.GetMouseButtonUp(0))
        {
            // fin del modo de selección múltiple
            //Debug.Log("fin seleccion: " + Input.mousePosition);
            selecting = false;
        }
        else
        {
            // hacemos click izquierdo
            // seleccion simple: si se levanta el botón y la posición del ratón
            // es la misma que cuando se pulso por última vez
            if ((Input.mousePosition == lastClick) &&
                    Input.GetMouseButtonUp(0))
            {
                //selecting = true;
                // lanzamos rayo y recogemos donde choca
                myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(myRay, out myHit, 1000f, layerMask))
                {
                    //Debug.Log("he tocado: " + myHit.transform.name);
					CSelectable objSel = (CSelectable)myHit.transform.GetComponent("CSelectable");
					if (objSel != null)
					{
						// la marcamos como seleccionada
						objSel.SetSelected();

						//Miramos si el objeto es una unidad
						UnitController unitCont = (UnitController)objSel.GetComponent("UnitController");
						if (unitCont != null)
						{
							// si NO tenemos control pulsada, se deselecciona lo que hubiera
							// y se selecciona la nueva unidad
							if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
							{
								DeselectAll();
							}
							// seleccionamos la nueva unidad
							GameObject unit = (GameObject)myHit.transform.gameObject;
							// la añadimos a la lista de seleccionados
							unitSelectedList.Add(unit);
							// y la marcamos como seleccionada
							unit.GetComponent<CSelectable>().SetSelected();
						}
						else
						{
							DeselectAll();
							
							BaseController baseCont = (BaseController)myHit.transform.GetComponent("BaseController");
							if (baseCont != null)
							{
								// seleccionamos la nueva unidad
								armyBase.GetComponent<CSelectable>().SetSelected();
							}
						}
					}
					else
					{
						//Deseleccionar las unidades
						DeselectAll();
					}
                }
            }
        }

        // hacemos click derecho
        if (Input.GetMouseButtonDown(1))
        {
            // lanzamos rayo y recogemos donde choca
            myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(myRay, out myHit, 1000f, layerMask))
            {
                Vector3 destiny = myHit.point;
				if (unitSelectedList.Count > 1)
				{
					//Calcular dependiendo de el numero de seleccionados distintos puntos de llegada
					List<Vector3> destinyList;
					destinyList = SwarmAlgorithm(destiny);
					int i = 0;
					foreach (GameObject u in unitSelectedList)
					{
						Debug.DrawLine(u.transform.localPosition, destinyList[i], Color.red, 1);
						u.GetComponent<UnitController>().RightClickOnSelected(destinyList[i], myHit.transform);
						i++;
					}
				}
				else if (unitSelectedList.Count == 1)
				{
					GameObject u = unitSelectedList[0];
					Debug.DrawLine(u.transform.localPosition, destiny, Color.red, 1);
					//u.GetComponent<UnitController>().GoTo(destiny);
					u.GetComponent<UnitController>().RightClickOnSelected(destiny, myHit.transform);
				}

            }
        }

		if (Input.GetKeyDown (KeyCode.A))
		{
			if (armyBase.GetComponent<CSelectable>().IsSelected())
			{
				// spawn a new unit
				GameObject newUnit = armyBase.GetComponent<BaseController>().SpawnUnit(0);
				unitList.Add(newUnit);
			}
		}

		if (Input.GetKeyDown (KeyCode.S))
		{
			if (armyBase.GetComponent<CSelectable>().IsSelected())
			{
				// spawn a new unit
				GameObject newUnit = armyBase.GetComponent<BaseController>().SpawnUnit(1);
				unitList.Add(newUnit);
			}
		}

		if (Input.GetKeyDown (KeyCode.I))
		{
			if (armyBase.GetComponent<CSelectable>().IsSelected())
			{
				// spawn a new unit
				GameObject newUnit = armyBase.GetComponent<BaseController>().SpawnUnit(3);
				unitList.Add(newUnit);
			}
		}

    } // Update ()

    void OnGUI ()
    {
		GUI.skin.label.fontSize = 12;

        GUI.Label(new Rect(0, 0, 150, 50), "Total resources: " + resources);
		GUI.Label(new Rect(0, 14, 150, 50), "Total Units: " + unitList.Count);

        // selecting rectangle
        if (selecting)
        {
            DrawQuad(
                new Rect(
                    squareSelectionPointsScreen[0].x,
                    Screen.height - squareSelectionPointsScreen[0].y,
                    squareSelectionPointsScreen[2].x - squareSelectionPointsScreen[0].x,
                    squareSelectionPointsScreen[0].y - squareSelectionPointsScreen[2].y
                ),
                new Color(0.0f, 0.0f, 1.0f, 0.085f)
            );
        }
    } // OnGUI()

	private List<Vector3> SwarmAlgorithm (Vector3 destiny)
	{
		List<Vector3> destinyList = new List<Vector3>();
		double radious = System.Math.Sqrt(unitSelectedList.Count);
		int truncateRadious = (int)System.Math.Truncate(radious);
		Vector3 destinyAux = destiny;
		Vector3 origDestiny = destiny;
		//Esquina superior izquierda
		int squareNum = (truncateRadious * truncateRadious);
		int alpha = 2;
		float beta = 0.5f;
		int cont = 0;
		int row = 0;
		int contSqr = 0;
		bool first = true;
		int numSelected = unitSelectedList.Count;

		//Para saber como centrar las tropas
		if (numSelected % 2 != 0)
		{
			beta = 1;
		}
		origDestiny.x = destinyAux.x = destiny.x - (float)(truncateRadious/2) - beta;
		if (numSelected == 2)//Caso especial NumSelected = 2
		{
			foreach (GameObject u in unitSelectedList)
			{
				destinyList.Add(destinyAux);
				destinyAux.x += alpha;
			}
		}
        else if (numSelected == 3)//Caso especial NumSelected = 2
        {
            foreach (GameObject u in unitSelectedList)
            {
                destinyList.Add(destinyAux);
                destinyAux.x += alpha;
            }
        }
        else
        {
            origDestiny.z = destinyAux.z = destiny.z + (float)(truncateRadious / 2) + beta;
            foreach (GameObject u in unitSelectedList)
            {
                cont++;
                contSqr++;
                if (squareNum + 1 > contSqr) //Si todavia estoy haciendo el cuadrado
                {
                    destinyList.Add(destinyAux);
                    //Siguiente unidad
                    if (cont == truncateRadious)//Si hay que hacer otra fila
                    {
                        cont = 0;
                        destinyAux.z -= alpha;
                        destinyAux.x = origDestiny.x;
                    }
                    else//Si es en la misma fila
                    {
                        destinyAux.x += alpha;
                    }
                }
                else if ((squareNum + (2 * (truncateRadious - 1))) >= contSqr)//Si tengo que añadir al resto de filas
                {
                    //Lo inicializo en la esquina sup, izq
                    if (squareNum + 1 == contSqr)
                    {
                        destinyAux = origDestiny;
                    }
                    //Si es el primero baja de fila, a la izquierda del resto y lo meto en array 
                    if (first)
                    {
                        row++;
                        destinyAux.z -= alpha;//Baja de fila
                        destinyAux.x = origDestiny.x - alpha; //Posiciona en la izquierda
                        destinyList.Add(destinyAux);
                        if (contSqr == numSelected) //Si es el ultimo -> centro la fila
                        {
                            //Se centra la fila
                            Vector3 unity = new Vector3();
                            int min = truncateRadious * row;
                            int max = min + truncateRadious;
                            for (int i = min; i < max; i++)
                            {
                                unity = destinyList[i];
                                unity.x += alpha / 2;
                                destinyList[i] = unity;
                            }
                            unity = destinyList[destinyList.Count - 1];
                            unity.x += alpha / 2;
                            destinyList[destinyList.Count - 1] = unity;
                        }
                        first = false;
                    }
                    else//Si es el segundo lo pongo a la derecha del resto y lo meto en array. Despues centro la fila
                    {
                        destinyAux.x += ((alpha) * truncateRadious) + 2; //Se posiciona al final
                        destinyList.Add(destinyAux);
                        first = true;
                    }
                }
                else //Si son las ultimas y hay que hacer una fila
                {
                    destinyAux = origDestiny;
                    destinyAux.z -= alpha * truncateRadious;
                    for (int i = contSqr; i < numSelected + 1; i++)
                    {
                        destinyList.Add(destinyAux);
                        destinyAux.x += alpha;
                    }
                }
            }
        }

        // rotación de los destinos en función de la media de todos los orígenes
        Vector3 origCenter;
        float minX = float.MaxValue, maxX = float.MinValue, minZ = minX, maxZ = maxX;
        for (int i = 0; i < numSelected; i++)
        {
            if (unitSelectedList[i].transform.position.x > maxX)
                maxX = unitSelectedList[i].transform.position.x;
            if (unitSelectedList[i].transform.position.z > maxZ)
                maxZ = unitSelectedList[i].transform.position.z;
            if (unitSelectedList[i].transform.position.x < minX)
                minX = unitSelectedList[i].transform.position.x;
            if (unitSelectedList[i].transform.position.z < minZ)
                minZ = unitSelectedList[i].transform.position.z;
        }
        origCenter = new Vector3( (maxX + minX) / 2.0f, 0.0F, (maxZ + minZ) / 2.0f);
        float angleDeg = Mathf.Atan2(destiny.z - origCenter.z, destiny.x - origCenter.x) * Mathf.Rad2Deg;
        angleDeg = (angleDeg + lastCrowdAngle) % 90;
        Quaternion rot = Quaternion.Euler(0, -angleDeg + 90, 0);
        for (int i = 0; i < destinyList.Count; i++)
        {
            // http://answers.unity3d.com/questions/532297/rotate-a-vector-around-a-certain-point.html
            //Vector3 myVector = Quaternion.Euler(0, angleDeg, 0) * Vector3.up;
            /*GameObject cubi = AddBlackBox(destinyList[i]);
            Destroy(cubi, 1.0f);*/
            Vector3 dir = destinyList[i] - destiny; // get point direction relative to pivot
            dir = rot * dir; // rotate it
            destinyList[i] = dir + destiny; // calculate rotated point
            /*GameObject cubo = AddBox(destinyList[i], new Color(0.0f, 0.0f - 0.1f * i, 1.0f - 0.1f * i));
            Destroy(cubo, 2.0f);*/
        }

        /*GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = destiny;
        cube.transform.localScale = new Vector3(0.5f, 2.0f, 0.5f);
        cube.renderer.material.color = Color.white;
        Destroy(cube, 2.0f);

        GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube2.transform.position = origCenter;
        cube2.transform.localScale = new Vector3(0.5f, 2.0f, 0.5f);
        cube2.renderer.material.color = Color.white;
        Destroy(cube2, 2.0f);*/

		return destinyList;	
	}

    void DrawQuad (Rect position, Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        GUI.skin.box.normal.background = texture;
        GUI.Box(position, GUIContent.none);
    }

    private void CreatingSquare ()
    {
		unitSelectedList.Clear();

        // se actualizan las posiciones de los vértices del cuadrado en pantalla:
        UpdateSelectionPointScreen();        

        //Lanzamos 4 rayos para coger los 4 vertices del rectángulo
        for (int i = 0; i < squareSelectionPointsProyected.Length; i++)
        {
            // rayo:
            myRay = Camera.main.ScreenPointToRay(squareSelectionPointsScreen[i]);
            // recogemos posición
			if (Physics.Raycast(myRay, out myHit, 1000f, layerMask))
                squareSelectionPointsProyected[i] = myHit.point;
            
            // aparece un cubo negro en la proyección del rayo
            //AddBlackBox(myHit.point);
        }

        // dibujamos el rectángulo proyectado en el mundo:
        Debug.DrawLine(squareSelectionPointsProyected[0], squareSelectionPointsProyected[1], Color.red);
        Debug.DrawLine(squareSelectionPointsProyected[1], squareSelectionPointsProyected[2], Color.green);
        Debug.DrawLine(squareSelectionPointsProyected[2], squareSelectionPointsProyected[3], Color.blue);
        Debug.DrawLine(squareSelectionPointsProyected[3], squareSelectionPointsProyected[0], Color.black);
        Debug.DrawLine(squareSelectionPointsProyected[0], squareSelectionPointsProyected[2], Color.magenta);
        Debug.DrawLine(squareSelectionPointsProyected[1], squareSelectionPointsProyected[3], Color.magenta);

        // seleccionar las unidades que estén dentro del cuadrado:
        // calculamos el centro del cuadrado: (http://en.wikipedia.org/wiki/Line-line_intersection)
        /*float x1 = squareSelectionPointsProyected[0].x;
        float y1 = squareSelectionPointsProyected[0].z;
        float x2 = squareSelectionPointsProyected[2].x;
        float y2 = squareSelectionPointsProyected[2].z;
        float x3 = squareSelectionPointsProyected[3].x;
        float y3 = squareSelectionPointsProyected[3].z;
        float x4 = squareSelectionPointsProyected[1].x;
        float y4 = squareSelectionPointsProyected[1].z;
        Vector2 aux = new Vector2(
            ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4))
            /
            ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)),
            ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4))
            /
            ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4))
        );*/
        float den = ((squareSelectionPointsProyected[0].x - squareSelectionPointsProyected[2].x) *
            (squareSelectionPointsProyected[3].z - squareSelectionPointsProyected[1].z)) -
            ((squareSelectionPointsProyected[0].z - squareSelectionPointsProyected[2].z) *
            (squareSelectionPointsProyected[3].x - squareSelectionPointsProyected[1].x));
        float num1 = ((squareSelectionPointsProyected[0].x * squareSelectionPointsProyected[2].z) -
            (squareSelectionPointsProyected[0].z * squareSelectionPointsProyected[2].x));
        float num2 = ((squareSelectionPointsProyected[3].x * squareSelectionPointsProyected[1].z) -
            (squareSelectionPointsProyected[3].z * squareSelectionPointsProyected[1].x));
        Vector2 squareCenter = new Vector2(
            ((num1 * (squareSelectionPointsProyected[3].x - squareSelectionPointsProyected[1].x)) -
            ((squareSelectionPointsProyected[0].x - squareSelectionPointsProyected[2].x) * num2)) /
            den,
            ((num1 * (squareSelectionPointsProyected[3].z - squareSelectionPointsProyected[1].z)) -
            ((squareSelectionPointsProyected[0].z - squareSelectionPointsProyected[2].z) * num2)) /
            den
        );
        //AddBlackBox(new Vector3(squareCenter.x, 1, squareCenter.y));
        // se comprueba uno por uno si la unidad esta dentro del cuadrado
        int count = unitList.Count;
        for (int i = 0; i < count; i++)
        {
            Vector2 unitPos = new Vector2(unitList[i].transform.position.x,
                unitList[i].transform.position.z);
            // distance between the unit and the center of the selection square
            float dist = Vector2.Distance(unitPos, squareCenter);
            //float radius = Vector2.Distance(squareCenter,
            //    new Vector2(squareSelectionPointsProyected[2].x, squareSelectionPointsProyected[3].z));
            float radius = Vector2.Distance(
                new Vector2(
                    squareSelectionPointsProyected[0].x,
                    squareSelectionPointsProyected[0].z
                ),
                new Vector2(
                    squareSelectionPointsProyected[2].x,
                    squareSelectionPointsProyected[2].z
                ) ) / 2;
            float d = 0;
            int contNeg = 0, contPos = 0;
            // // 1st: Manhattan distance
            if (dist < radius)
            {
                // 2nd: distance unit to segments of the square
                for (int j = 0; j < 4; j++)
                {
                    /*d = DistancePointToSegment(
                        new Vector2(squareSelectionPointsProyected[j].x, squareSelectionPointsProyected[j].z),
                        new Vector2(squareSelectionPointsProyected[(j + 1) % 4].x, squareSelectionPointsProyected[(j + 1) % 4].z),
                        unitPos
                    );*/
                    // this alternative is less expensive:
                    d = SignPointToSegment(
                        squareSelectionPointsProyected[j],
                        squareSelectionPointsProyected[(j + 1) % 4],
                        unitPos
                    );
                    if (d < 0)
                        contNeg++;
                    else
                        contPos++;
                }
                if (contNeg == 4 || contPos == 4)
                {
                    // the unit is inside the selection square
                    // we add it to the selection list
                    unitSelectedList.Add(unitList[i]);
                    // and mark it as selected
                    unitList[i].GetComponent<CSelectable>().SetSelected();
                }
                else
                {
                    // delete the unit from the selection list
                    unitSelectedList.Remove(unitList[i]);
                    // and mark it as deselected
					unitList[i].GetComponent<CSelectable>().SetDeselect();
                }
            }
        }

    } // CreatingSquare ()

    // alternative version which avoids the (presumably expensive) use of rays
    // doc: http://docs.unity3d.com/Documentation/ScriptReference/Camera.WorldToScreenPoint.html
    private void CreatingSquare2 ()
    {
        unitSelectedList.Clear();

        // se actualizan las posiciones de los vértices del cuadrado en pantalla:
        UpdateSelectionPointScreen();

        Vector3 unitScreenPos;
        int count = unitList.Count;
        for (int i = 0; i < count; i++)
        {
            // capture the screen position of the unit
            unitScreenPos = Camera.main.WorldToScreenPoint(unitList[i].transform.position);
            // check if the position if inside the square we are creating
            if (unitScreenPos.x >= squareSelectionPointsScreen[0].x &&
                unitScreenPos.y <= squareSelectionPointsScreen[0].y &&
                unitScreenPos.x <= squareSelectionPointsScreen[2].x &&
                unitScreenPos.y >= squareSelectionPointsScreen[2].y)    // select the unit
            {
                // we add it to the selection list
                unitSelectedList.Add(unitList[i]);
                // and mark it as selected
                unitList[i].GetComponent<CSelectable>().SetSelected();
            }
            else
            {
                // delete the unit from the selection list
                unitSelectedList.Remove(unitList[i]);
                // and mark it as deselected
                unitList[i].GetComponent<CSelectable>().SetDeselect();
            }
        }
    } // CreatingSquare2 ()

    private void UpdateSelectionPointScreen ()
    {
        squareSelectionPointsScreen[2] = Input.mousePosition;

        // miramos que el 0 sea el de más arriba a la izquierda
        if (lastClick.x > Input.mousePosition.x)
        {
            // voltear las x
            squareSelectionPointsScreen[2].x = lastClick.x;
            squareSelectionPointsScreen[0].x = Input.mousePosition.x;
        }

        if (lastClick.y < Input.mousePosition.y)
        {
            // voltear las y
            squareSelectionPointsScreen[2].y = lastClick.y;
            squareSelectionPointsScreen[0].y = Input.mousePosition.y;
        }

        squareSelectionPointsScreen[1].x = squareSelectionPointsScreen[2].x;
        squareSelectionPointsScreen[1].y = squareSelectionPointsScreen[0].y;
        squareSelectionPointsScreen[3].x = squareSelectionPointsScreen[0].x;
        squareSelectionPointsScreen[3].y = squareSelectionPointsScreen[2].y;

        /*squareSelectionPointsScreen[2] = Input.mousePosition;
        squareSelectionPointsScreen[1].x = squareSelectionPointsScreen[2].x;
        squareSelectionPointsScreen[1].y = squareSelectionPointsScreen[0].y;
        squareSelectionPointsScreen[3].x = squareSelectionPointsScreen[0].x;
        squareSelectionPointsScreen[3].y = squareSelectionPointsScreen[2].y;*/
    }

    /// <summary>
    /// The distance of the point to the segment
    /// </summary>
    /// <param name="A">First point of the segment</param>
    /// <param name="B">Second point of the segment</param>
    /// <param name="p">The point</param>
    /// <returns>The distance</returns>
    private float DistancePointToSegment (Vector2 A, Vector2 B, Vector2 p)
    {
        return (((B.x - A.x) * (A.y - p.y) - (A.x - p.x) * (B.y - A.y)) /
            (float)(Mathf.Sqrt((B.x - A.x) * (B.x - A.x) + (B.y - A.y) * (B.y - A.y))));
    }

    private float SignPointToSegment (Vector3 A, Vector3 B, Vector2 p)
    {
        return ((B.x - A.x) * (A.z - p.y) - (A.x - p.x) * (B.z - A.z));
    }

    // deselect all the units in the selected list
    private void DeselectAll ()
    {
        // deseleccionamos los que hubiera seleccionado
        foreach (GameObject u in unitSelectedList)
			u.GetComponent<CSelectable>().SetDeselect();
        // vaciamos la lista de seleccionados previamente
        unitSelectedList.Clear();

		//Deseleccionar la base
		armyBase.GetComponent<CSelectable>().SetDeselect();
	}

    private GameObject AddBox (Vector3 position, Color color)
    {
        // aparece un cubo del color indicado en la posición indicada
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.renderer.material.color = color;
        return cube;
    }

    private GameObject AddBlackBox (Vector3 position)
    {
        // aparece un cubo negro en la proyección del rayo
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.renderer.material.color = Color.black;
        return cube;
    }

    public void IncreaseResources (int resources)
    {
        this.resources += resources;
    }

    public void UnitDied (GameObject unit)
    {
        // delete the unit from the unit list
        unitList.Remove(unit);
        // if the unit is selected, remove it from the selected list
        if (unitSelectedList.Contains(unit))
            unitSelectedList.Remove(unit);

        // destroy the unit from the game
        Destroy(unit, 0.5f);
    }

} // class ArmyController
