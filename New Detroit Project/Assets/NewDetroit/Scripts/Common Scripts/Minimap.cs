using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Minimap : MonoBehaviour {

    // Minimap properties
    float size = 180.0f;
    float margin = 10.0f;
    public Texture textureMap;
    public Texture textureUnit0;
    public Texture textureUnit1;
    float posHeight;

    //Terrain properties
    public float sizeWorldFloor;
    public float sizeProportion;

    private int contUpdate0 = 0;
    private int contUpdate1 = 0;

    //Unit lists
    public static List<UnitController> army0 = new List<UnitController>();
    public static List<UnitController> army1 = new List<UnitController>();
    public static List<UnitController> building0 = new List<UnitController>();
    public static List<UnitController> building1 = new List<UnitController>();

    // Positions of the GameObjects lists
    public static List<Vector2> unitList0 = new List<Vector2>();
    public static List<Vector2> unitList1 = new List<Vector2>();
    public static List<Vector2> buildingList0 = new List<Vector2>();
    public static List<Vector2> buildingList1 = new List<Vector2>();

    private int cont = 0;

	// Use this for initialization
	void Start () 
    {
        sizeProportion = size / sizeWorldFloor;
        posHeight = Screen.height - (size + margin);
	}
	
	// Update is called once per frame
	void Update () 
    {
        int max0 = army0.Count;
        int max1 = army1.Count;
        //army0
        for (int i = contUpdate0; i < max0; i = i + 1 )
        {
            float posx = margin + (army0[i].transform.position.x + (sizeWorldFloor / 2)) * sizeProportion;
            float posy = posHeight + ((sizeWorldFloor / 2) - army0[i].transform.position.y) * sizeProportion;
            unitList0[i] = new Vector2(posx, posy);          
        }
        //contUpdate0 = (contUpdate0 + 1) % 4;
        
        // army1
        for (int i = contUpdate1; i < max1; i = i + 3)
        {
            float posx = margin + (army1[i].transform.position.x + (sizeWorldFloor / 2)) * sizeProportion;
            float posy = posHeight + ((sizeWorldFloor / 2) - army1[i].transform.position.y) * sizeProportion;
            unitList1[i] = new Vector2(posx, posy);
        }
        contUpdate1 = (contUpdate1 + 1) % 4;
       
        /*if (contUpdate == 20)
        {
            List<GameObject> unitList0AC = armyController.GetComponent<ArmyController>().unitList;
            unitList0.Clear();
            foreach (GameObject unit in unitList0AC)
            {
                unitList0.Add(new Vector2(unit.transform.position.x, unit.transform.position.z));
            }
            contUpdate = 0;
        }
        else
            contUpdate++;
        */
	}

    public static void InsertUnit(UnitController unit)
    {
        if (unit.teamNumber == 0)
        {
            army0.Add(unit);
            unitList0.Add(new Vector2());
        }
        else
        {
            army1.Add(unit);
            unitList1.Add(new Vector2());
        }
               
    }

    public virtual void OnGUI()
    {
        Rect rect1;
        rect1 = new Rect(margin, posHeight, size, size);
        GUI.DrawTexture(rect1, textureMap);
        
        foreach (Vector2 unit in unitList0)
        {
            // Esquina superior izquierda: x = -250, z = 250;
            rect1 = new Rect(unit.x, unit.y, 0.8f, 0.8f);
            GUI.DrawTexture(rect1, textureUnit0);
        }
        foreach (Vector2 unit in unitList1)
        {
            // Esquina superior izquierda: x = -250, z = 250;
            rect1 = new Rect(unit.x, unit.y, 0.8f, 0.8f);
            GUI.DrawTexture(rect1, textureUnit1);
        }

    }

}
