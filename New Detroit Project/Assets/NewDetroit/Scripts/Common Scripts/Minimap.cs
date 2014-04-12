using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Minimap : MonoBehaviour
{
    public static int teamNumber = 0;

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
    public static float size        = 180.0f;
    public static float margin      = 10.0f;
    public static float posHeight;

    //Terrain properties
    public const float sizeWorldFloor = 500;
    public static float sizeProportion;

    private int contUpdate0 = 0;
    private int contUpdate1 = 0;
    private int contX = 0;
    private int contY = 0;

    public class StructMatrix
    {
        private int fogType;
        private int cont;
        private const int MAXCONT = 10;

        public StructMatrix()
        {
            fogType = 0;
            cont = 0;
        }

        public StructMatrix(int fogType, int cont)
        {
            this.fogType = fogType;
            this.cont = cont;
        }

        public void SetFogType(int fogType)
        {
            this.fogType = fogType;
        }

        public int GetFogType()
        {
            return fogType;
        }

        public int GetCont()
        {
            return cont;
        }

        public void IncreaseCont()
        {
            cont++;
        }

        public bool IsMaxCont()
        {
            return cont == MAXCONT;
        }

        public void ResetCont()
        {
            cont = 0;
        }
    }
    public class StructUnitFogPos
    {
        private Vector2 position;
        private int fogType;

        public StructUnitFogPos()
        {
            position = new Vector2();
            fogType = 0;
        }

        public void SetPosition(Vector2 position)
        {
            this.position = position;
        }

        public void SetFogType(int fogType)
        {
            this.fogType = fogType;
        }

        public Vector2 GetPosition()
        {
            return position;
        }

        public int GetFogType()
        {
            return fogType;
        }
    }
    public class StructBuildingFogPos
    {
        private Vector2 position;
        private int fogType;
        private int tileX;
        private int tileY;

        public StructBuildingFogPos(Vector2 position, int fogType, int tileX, int tileY)
        {
            this.position = position;
            this.fogType = fogType;
            this.tileX = tileX;
            this.tileY = tileY;
        }
        
        public Vector2 GetPosition()
        {
            return position;
        }

        public int GetFogType()
        {
            return fogType;
        }

        public int GetTileX()
        {
            return tileX;
        }
        
        public int GetTileY()
        {
            return tileY;
        }
    }

    // Array for fogType
    private StructMatrix[,] fogTypeMatrix; // 0 -> opaque, 1 -> semitransparent, 2 -> transparent
    private const int MATRIXSIZE = 20;
    private static float tileSize;
    
    //Unit lists
    public static List<UnitController> myArmy       = new List<UnitController>();
    public static List<UnitController> enemyArmy    = new List<UnitController>();

    // Positions of the own GameObjects lists
    public static List<Vector2> myUnitList          = new List<Vector2>();    
    public static List<Vector2> myTowerNeutralList  = new List<Vector2>();
    public static List<Vector2> myTowerList         = new List<Vector2>();
    public static List<Vector2> myWarehouseList     = new List<Vector2>();
    public static Vector2 myBase;

    // Positions and fogtype of the enemy GameObjects lists
    public static List<StructUnitFogPos> enemyUnitList              = new List<StructUnitFogPos>();
    public static List<StructBuildingFogPos> enemyTowerNeutralList  = new List<StructBuildingFogPos>();
    public static List<StructBuildingFogPos> enemyTowerList         = new List<StructBuildingFogPos>();
    public static List<StructBuildingFogPos> enemyWarehouseList     = new List<StructBuildingFogPos>();
    public static StructBuildingFogPos enemyBase;
    
    private int cont = 0;

	// Use this for initialization
	void Start () 
    {
        sizeProportion = size / sizeWorldFloor;
        posHeight = Screen.height - (size + margin);
        
        // Inicialize the matrix
        fogTypeMatrix = new StructMatrix[MATRIXSIZE, MATRIXSIZE];
        for (int i = 0; i < MATRIXSIZE; i++)
        {
            for (int j = 0; j < MATRIXSIZE; j++)
            {
                fogTypeMatrix[i, j] = new StructMatrix(0, 0);
            }
        }
        tileSize = size / MATRIXSIZE;
	}

	void LateUpdate () 
    {
        int max0 = myArmy.Count;
        int max1 = enemyArmy.Count;
        
        // The transparent positions of the matrix to semitransparent
        for (int i = contX; i < MATRIXSIZE; i += 3)
        {
            for (int j = contY; j < MATRIXSIZE; j += 3)
            {
                i = i % MATRIXSIZE;
                j = j % MATRIXSIZE;
                // If the tile is transparent and has to change to semitransparent
                if (fogTypeMatrix[i, j].IsMaxCont() && fogTypeMatrix[i, j].GetFogType() == 2)
                {
                    fogTypeMatrix[i, j].SetFogType(1);
                    fogTypeMatrix[i, j].ResetCont();
                        
                }
                // else if the tile is transparent we increase its cont
                else if (fogTypeMatrix[i, j].GetFogType() == 2)
                    fogTypeMatrix[i, j].IncreaseCont();
            }
        }
        contY = (contY + 1) % 4;
        if (contY == 0)
            contX = (contX + 1) % 4;
        
        // myArmy
        for (int i = contUpdate0; i < max0; i += 3 )
        {            
            // Update the unit position
            float posx = margin + (myArmy[i].transform.position.x + (sizeWorldFloor / 2)) * sizeProportion;
            float posy = posHeight + ((sizeWorldFloor / 2) - myArmy[i].transform.position.z) * sizeProportion;
            myUnitList[i] = new Vector2(posx, posy);          
            
            // Update the matrix
            int tileX = (int)((posx - margin) / tileSize);
            int tileY = (int)((posy - posHeight) / tileSize);
            fogTypeMatrix[tileX, tileY].SetFogType(2);
            if (fogTypeMatrix[tileX, tileY].GetCont() != 0)
                fogTypeMatrix[tileX, tileY].ResetCont();
        }
        contUpdate0 = (contUpdate0 + 1) % 4;
        
        // enemyArmy
        for (int i = contUpdate1; i < max1; i += 3)
        {
            // update the unit position
            float posx = margin + (enemyArmy[i].transform.position.x + (sizeWorldFloor / 2)) * sizeProportion;
            float posy = posHeight + ((sizeWorldFloor / 2) - enemyArmy[i].transform.position.z) * sizeProportion;
            enemyUnitList[i].SetPosition(new Vector2(posx, posy));

            // update the unit fog type            
            int tileX = (int)((posx - margin) / tileSize);
            int tileY = (int)((posy - posHeight) / tileSize);
            enemyUnitList[i].SetFogType(fogTypeMatrix[tileX, tileY].GetFogType());
        }
        contUpdate1 = (contUpdate1 + 1) % 4;
	}

    public static void InsertUnit (UnitController unit)
    {
        if (unit.teamNumber == teamNumber)
        {
            myArmy.Add(unit);
            myUnitList.Add(new Vector2());
        }
        else
        {
            enemyArmy.Add(unit);
            enemyUnitList.Add(new StructUnitFogPos());
        }
               
    }

    public static void DeleteUnit (UnitController unit)
    {
        if (unit.teamNumber == teamNumber)
        {
            int pos = myArmy.IndexOf(unit);
            myArmy.Remove(unit);
            myUnitList.RemoveAt(pos);
        }
        else
        {
            int pos = enemyArmy.IndexOf(unit);
            enemyArmy.Remove(unit);
            enemyUnitList.RemoveAt(pos);
        }

    }

    public static void InsertTower (Tower tower)
    {
        float posx = margin + (tower.transform.position.x + (sizeWorldFloor / 2)) * sizeProportion;
        float posy = posHeight + ((sizeWorldFloor / 2) - tower.transform.position.z) * sizeProportion;

        if (tower.teamNumber == teamNumber)
            myTowerList.Add(new Vector2(posx, posy));
        else
        {
            // Update the tile
            int tileX = (int)((posx - margin) / tileSize);
            int tileY = (int)((posy - posHeight) / tileSize);
            enemyTowerList.Add(new StructBuildingFogPos(new Vector2(posx, posy), 0, tileX, tileY));
        }
    }

    public static void InsertWarehouse (Warehouse warehouse)
    {
        float posx = margin + (warehouse.transform.position.x + (sizeWorldFloor / 2)) * sizeProportion;
        float posy = posHeight + ((sizeWorldFloor / 2) - warehouse.transform.position.z) * sizeProportion;

        if (warehouse.teamNumber == teamNumber)
            myWarehouseList.Add(new Vector2(posx, posy));
        else
        {
            // Update the tile
            int tileX = (int)((posx - margin) / tileSize);
            int tileY = (int)((posy - posHeight) / tileSize);
            enemyWarehouseList.Add(new StructBuildingFogPos(new Vector2(posx, posy), 0, tileX, tileY));
        }
    }

    public static void SetTowerNeutral(TowerNeutral tower)
    {

        float posx = margin + (tower.transform.position.x + (sizeWorldFloor / 2)) * sizeProportion;
        float posy = posHeight + ((sizeWorldFloor / 2) - tower.transform.position.z) * sizeProportion;
        
        if (tower.teamNumber == teamNumber)
            myTowerNeutralList.Add(new Vector2(posx, posy));
        else
        {
            // Update the tile
            int tileX = (int)((posx - margin) / tileSize);
            int tileY = (int)((posy - posHeight) / tileSize);
            enemyTowerNeutralList.Add(new StructBuildingFogPos(new Vector2(posx, posy), 0, tileX, tileY));
        }   
    }

    public static void SetBase(CResourceBuilding baseBuilding)
    {
        float posx = margin + (baseBuilding.transform.position.x + (sizeWorldFloor / 2)) * sizeProportion;
        float posy = posHeight + ((sizeWorldFloor / 2) - baseBuilding.transform.position.z) * sizeProportion;

        if (baseBuilding.teamNumber == 0)
            myBase = new Vector2(posx, posy);
        else
        {
            // Update the tile
            int tileX = (int)((posx - margin) / tileSize);
            int tileY = (int)((posy - posHeight) / tileSize);
            enemyBase = new StructBuildingFogPos(new Vector2(posx, posy), 0, tileX, tileY);
        }  
    }

    public virtual void OnGUI()
    {
        // Esquina superior izquierda: x = -250, z = 250;
        Rect rect1;
        rect1 = new Rect(margin, posHeight, size, size);
        GUI.DrawTexture(rect1, textureMap);
        
        // my own team
        foreach (Vector2 unit in myUnitList)
        {
            rect1 = new Rect(unit.x, unit.y, 1.0f, 1.0f);
            GUI.DrawTexture(rect1, textureUnit0);
        }
        foreach (Vector2 tower in myTowerList)
        {
            rect1 = new Rect(tower.x, tower.y, 3.0f, 3.0f);
            GUI.DrawTexture(rect1, textureTower0);
        }
        foreach (Vector2 warehouse in myWarehouseList)
        {
            rect1 = new Rect(warehouse.x, warehouse.y, 3.0f, 3.0f);
            GUI.DrawTexture(rect1, textureWarehouse0);
        }
        foreach (Vector2 tower in myTowerNeutralList)
        {
            rect1 = new Rect(tower.x, tower.y, 3.0f, 3.0f);
            GUI.DrawTexture(rect1, textureTowerNeutral);
        }

        // enemy team
        foreach (StructUnitFogPos unit in enemyUnitList)
        {
            if (unit.GetFogType() == 2)
            {
                Vector2 posUnit = unit.GetPosition();
                rect1 = new Rect(posUnit.x, posUnit.y, 1.0f, 1.0f);
                GUI.DrawTexture(rect1, textureUnit1);
            }
        } 
        foreach (StructBuildingFogPos tower in enemyTowerList)
        {
            if (fogTypeMatrix[tower.GetTileX(), tower.GetTileY()].GetFogType() == 2)
            {
                Vector2 posBuilding = tower.GetPosition();
                rect1 = new Rect(posBuilding.x, posBuilding.y, 3.0f, 3.0f);
                GUI.DrawTexture(rect1, textureTower1);
            }
        }
        foreach (StructBuildingFogPos warehouse in enemyWarehouseList)
        {
            if (fogTypeMatrix[warehouse.GetTileX(), warehouse.GetTileY()].GetFogType() == 2)
            {
                Vector2 posBuilding = warehouse.GetPosition();
                rect1 = new Rect(posBuilding.x, posBuilding.y, 3.0f, 3.0f);
                GUI.DrawTexture(rect1, textureWarehouse1);
            }
        }
        foreach (StructBuildingFogPos tower in enemyTowerNeutralList)
        {
            if (fogTypeMatrix[tower.GetTileX(), tower.GetTileY()].GetFogType() == 2)
            {
                Vector2 posBuilding = tower.GetPosition();
                rect1 = new Rect(posBuilding.x, posBuilding.y, 3.0f, 3.0f);
                GUI.DrawTexture(rect1, textureTowerNeutral);
            }
        }
        
        // myBase
        rect1 = new Rect(myBase.x, myBase.y, 4.0f, 4.0f);
        GUI.DrawTexture(rect1, textureBase0);
        // enemyBase
        if (fogTypeMatrix[enemyBase.GetTileX(), enemyBase.GetTileY()].GetFogType() == 2)
        {
            Vector2 posBuilding = enemyBase.GetPosition();
            rect1 = new Rect(posBuilding.x, posBuilding.y, 4.0f, 4.0f);
            GUI.DrawTexture(rect1, textureBase1);
        }
    }

}
