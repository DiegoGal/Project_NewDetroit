using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArmyController : MonoBehaviour
{

    public List<GameObject> unitList = new List<GameObject>();
    public List<GameObject> unitSelectedList = new List<GameObject>();

    private RaycastHit myHit; // Structure used to get information back from a raycast.
    private Ray myRay;

    private bool selecting;
    private Vector3 lastClick;
    private bool mouseButtonPreshed;
    private Vector3[] squareSelectionPointsScreen;      // positions of the corners in the screen
    private Vector3[] squareSelectionPointsProyected;   // positions of the corners in the world

    // Use this for initialization
    void Start ()
    {
        myHit = new RaycastHit();
        selecting = false;
        lastClick = new Vector3();
        mouseButtonPreshed = false;
        squareSelectionPointsScreen = new Vector3[4];
        squareSelectionPointsProyected = new Vector3[4];
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
                squareSelectionPointsScreen[0] = Input.mousePosition;
                selecting = true;
            }
            else // se continúa el modo de selección múltiple
            {
                // select units on the fly
                CreatingSquare();
            }
        }

        if (selecting && Input.GetMouseButtonUp(0))
        {
            // fin del modo de selección múltiple
            //Debug.Log("fin seleccion: " + Input.mousePosition);
            selecting = false;
            CreatingSquare();
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
                    UnitController unitCont = (UnitController)myHit.transform.GetComponent("UnitController");
                    // el rayo ha chocado en una Unidad
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
                        unit.GetComponent<UnitController>().SetSelected();
                    }
                    else
                    {
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
                    u.GetComponent<UnitController>().GoTo(destiny);
                }
            }
        }

    } // Update ()

    void OnGUI()
    {
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
        // se actualizan las posiciones de los vértices del cuadrado en pantalla:
        squareSelectionPointsScreen[2] = Input.mousePosition;
        squareSelectionPointsScreen[1].x = squareSelectionPointsScreen[2].x;
        squareSelectionPointsScreen[1].y = squareSelectionPointsScreen[0].y;
        squareSelectionPointsScreen[3].x = squareSelectionPointsScreen[0].x;
        squareSelectionPointsScreen[3].y = squareSelectionPointsScreen[2].y;

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
                ) );
            float d = 0;
            int contNeg = 0, contPos = 0;
            // // 1st: Manhattan distance
            if (dist < radius)
            {
                // 2nd: distance unit to segments of the square
                for (int j = 0; j < 4; j++)
                {
                    d = DistancePointToSegment(
                        new Vector2(squareSelectionPointsProyected[j].x, squareSelectionPointsProyected[j].z),
                        new Vector2(squareSelectionPointsProyected[(j + 1) % 4].x, squareSelectionPointsProyected[(j + 1) % 4].z),
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
                    unitList[i].GetComponent<UnitController>().SetSelected();
                }
                else
                {
                    // delete the unit from the selection list
                    unitSelectedList.Remove(unitList[i]);
                    // and mark it as deselected
                    unitList[i].GetComponent<UnitController>().SetDeselect();
                }
            }
        }

    } // CreatingSquare ()

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

    // deselect all the units in the selected list
    private void DeselectAll ()
    {
        // deseleccionamos los que hubiera seleccionado
        foreach (GameObject u in unitSelectedList)
            u.GetComponent<UnitController>().SetDeselect();
        // vaciamos la lista de seleccionados previamente
        unitSelectedList.Clear();
    }


    private void AddBlackBox (Vector3 position)
    {
        // aparece un cubo negro en la proyección del rayo
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.renderer.material.color = Color.black;
    }

} // class ArmyController
