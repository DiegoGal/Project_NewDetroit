﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArmyController : MonoBehaviour
{
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
    }

    // Update is called once per frame
    void Update ()
    {
        // se pulsa el botón izquierdo del ratón
        if (Input.GetMouseButtonDown(0))
        {
            lastClick = Input.mousePosition;
            mouseButtonPreshed = true;
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
                if (Physics.Raycast(myRay, out myHit, 1000f))
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
            if (Physics.Raycast(myRay, out myHit, 1000f))
            {
                Vector3 destiny = myHit.point;
                foreach (GameObject u in unitSelectedList)
                {
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
				GameObject newUnit = armyBase.GetComponent<BaseController>().SpawnUnit();
				unitList.Add(newUnit);
			}
		}

    } // Update ()

    void OnGUI ()
    {
        GUI.Label(new Rect(0, 0, 150, 50), "Total resources: " + resources);

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
                new Color(0.0f, 0.0f, 1.0f, 0.05f)
            );
        }
    } // OnGUI()

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
            if (Physics.Raycast(myRay, out myHit, 1000f))
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
    }

    private void UpdateSelectionPointScreen()
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

    private void AddBlackBox (Vector3 position)
    {
        // aparece un cubo negro en la proyección del rayo
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.renderer.material.color = Color.black;
    }

    public void IncreaseResources(int resources)
    {
        this.resources += resources;
    }

} // class ArmyController
