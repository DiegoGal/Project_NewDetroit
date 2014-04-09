using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Minimap : MonoBehaviour {

    // Textures
    public Texture textureMap;
    public Texture textureUnit0;
    public Texture textureUnit1;
    public Texture textureTower0;
    public Texture textureTower1;
    public Texture textureWarehouse0;
    public Texture textureWarehouse1;
    public Texture textureBase0;
    public Texture textureBase1;
    public Texture textureTowerNeutral;

    // Minimap properties
    public static float size = 180.0f;
    public static float margin = 10.0f;
    public static float posHeight;

    //Terrain properties
    public static float sizeWorldFloor = 500;
    public static float sizeProportion;

    private int contUpdate0 = 0;
    private int contUpdate1 = 0;

    //Unit lists
    public static List<UnitController> army0 = new List<UnitController>();
    public static List<UnitController> army1 = new List<UnitController>();
    /*public static List<TowerNeutral> towersNeutral = new List<TowerNeutral>();
    public static List<Tower> towers0 = new List<Tower>();
    public static List<Tower> towers1 = new List<Tower>();
    */
    // Positions of the GameObjects lists
    public static List<Vector2> unitList0 = new List<Vector2>();
    public static List<Vector2> unitList1 = new List<Vector2>();
    public static List<Vector2> towerNeutralList = new List<Vector2>();
    public static List<Vector2> towerList0 = new List<Vector2>();
    public static List<Vector2> towerList1 = new List<Vector2>();
    public static List<Vector2> warehouseList0 = new List<Vector2>();
    public static List<Vector2> warehouseList1 = new List<Vector2>();
    public static Vector2 base0;
    public static Vector2 base1;

    private int cont = 0;
    public static bool towersNeutralSetted = false;

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
        for (int i = contUpdate0; i < max0; i = i + 3 )
        {
            float posx = margin + (army0[i].transform.position.x + (sizeWorldFloor / 2)) * sizeProportion;
            float posy = posHeight + ((sizeWorldFloor / 2) - army0[i].transform.position.z) * sizeProportion;
            unitList0[i] = new Vector2(posx, posy);          
        }
        contUpdate0 = (contUpdate0 + 1) % 4;
        
        // army1
        for (int i = contUpdate1; i < max1; i = i + 3)
        {
            float posx = margin + (army1[i].transform.position.x + (sizeWorldFloor / 2)) * sizeProportion;
            float posy = posHeight + ((sizeWorldFloor / 2) - army1[i].transform.position.z) * sizeProportion;
            unitList1[i] = new Vector2(posx, posy);
        }
        contUpdate1 = (contUpdate1 + 1) % 4;
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

    public static void DeleteUnit(UnitController unit)
    {
        if (unit.teamNumber == 0)
        {
            int pos = army0.IndexOf(unit);
            army0.Remove(unit);
            unitList0.RemoveAt(pos);
        }
        else
        {
            int pos = army1.IndexOf(unit);
            army1.Remove(unit);
            unitList1.RemoveAt(pos);
        }

    }

    public static void InsertTower(Tower tower)
    {
        float posx = margin + (tower.transform.position.x + (sizeWorldFloor / 2)) * sizeProportion;
        float posy = posHeight + ((sizeWorldFloor / 2) - tower.transform.position.z) * sizeProportion;
        
        if (tower.teamNumber == 0)
            towerList0.Add(new Vector2(posx, posy));
        else
            towerList1.Add(new Vector2(posx, posy));

    }

    public static void InsertWarehouse(Warehouse warehouse)
    {
        float posx = margin + (warehouse.transform.position.x + (sizeWorldFloor / 2)) * sizeProportion;
        float posy = posHeight + ((sizeWorldFloor / 2) - warehouse.transform.position.z) * sizeProportion;

        if (warehouse.teamNumber == 0)
            warehouseList0.Add(new Vector2(posx, posy));
        else
            warehouseList1.Add(new Vector2(posx, posy));

    }

    public static void SetTowerNeutral(TowerNeutral tower)
    {

        float posx = margin + (tower.transform.position.x + (sizeWorldFloor / 2)) * sizeProportion;
        float posy = posHeight + ((sizeWorldFloor / 2) - tower.transform.position.z) * sizeProportion;

        towerNeutralList.Add(new Vector2(posx, posy));
   
    }

    public static void SetBase(CResourceBuilding baseBuilding)
    {
        float posx = margin + (baseBuilding.transform.position.x + (sizeWorldFloor / 2)) * sizeProportion;
        float posy = posHeight + ((sizeWorldFloor / 2) - baseBuilding.transform.position.z) * sizeProportion;

        if (baseBuilding.teamNumber == 0)
            base0 = new Vector2(posx, posy);
        else
            base1 = new Vector2(posx, posy);
    }

    public virtual void OnGUI()
    {
        // Esquina superior izquierda: x = -250, z = 250;
        Rect rect1;
        rect1 = new Rect(margin, posHeight, size, size);
        GUI.DrawTexture(rect1, textureMap);
        
        foreach (Vector2 unit in unitList0)
        {
            rect1 = new Rect(unit.x, unit.y, 1.0f, 1.0f);
            GUI.DrawTexture(rect1, textureUnit0);
        }
        foreach (Vector2 unit in unitList1)
        {
            rect1 = new Rect(unit.x, unit.y, 1.0f, 1.0f);
            GUI.DrawTexture(rect1, textureUnit1);
        }
        foreach (Vector2 tower in towerList0)
        {
            rect1 = new Rect(tower.x, tower.y, 3.0f, 3.0f);
            GUI.DrawTexture(rect1, textureTower0);
        }
        foreach (Vector2 tower in towerList1)
        {
            rect1 = new Rect(tower.x, tower.y, 3.0f, 3.0f);
            GUI.DrawTexture(rect1, textureTower1);
        }
        foreach (Vector2 warehouse in warehouseList0)
        {
            rect1 = new Rect(warehouse.x, warehouse.y, 3.0f, 3.0f);
            GUI.DrawTexture(rect1, textureWarehouse0);
        }
        foreach (Vector2 warehouse in warehouseList1)
        {
            rect1 = new Rect(warehouse.x, warehouse.y, 3.0f, 3.0f);
            GUI.DrawTexture(rect1, textureWarehouse1);
        }
        foreach (Vector2 tower in towerNeutralList)
        {
            rect1 = new Rect(tower.x, tower.y, 3.0f, 3.0f);
            GUI.DrawTexture(rect1, textureTowerNeutral);
        }
        // Base0
        rect1 = new Rect(base0.x, base0.y, 4.0f, 4.0f);
        GUI.DrawTexture(rect1, textureBase0);
        // Base1
        rect1 = new Rect(base1.x, base1.y, 4.0f, 4.0f);
        GUI.DrawTexture(rect1, textureBase1);
    }

}
