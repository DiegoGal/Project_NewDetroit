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
    public Texture textureMyHeroe;
    public Texture textureEnemyHeroe;
    public Texture textureSemiTransparent;

    // Minimap properties
    public static float size        = 200.0f;
    public static float margin      = 10.0f;
    public static float posHeight;

    //Terrain properties
    public static float sizeWorldFloor = 250;
    public static float sizeProportionY;
    public static float sizeProportionX;

    public Terrain terrain;

    private int contUpdate0 = 0;
    private int contUpdate1 = 0;
    private int contX = 0;
    private int contY = 0;

    public GameObject orcBase;
    public GameObject robotBase;

    public Texture map;

    public class StructMatrix
    {
        private int fogType;
        private int cont;
        public int alwaysVisible;
        private const int MAXCONT = 10;

        public StructMatrix()
        {
            fogType = 0;
            cont = 0;
            alwaysVisible = 0;
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
    private static StructMatrix[,] fogTypeMatrix; // 0 -> opaque, 1 -> semitransparent, 2 -> transparent
    private const int MATRIXSIZE = 20;
    private static float tileSize;
    
    //Unit lists
    public static List<Transform> myArmy    = new List<Transform>();
    public static List<Transform> enemyArmy = new List<Transform>();

    // Heroes
    public static Transform myHeroe;
    public static Transform enemyHeroe;

    // Positions of the own GameObjects lists
    public static List<Vector2> myUnitList          = new List<Vector2>();    
    public static List<Vector2> myTowerNeutralList  = new List<Vector2>();
    public static List<Vector2> myTowerList         = new List<Vector2>();
    public static List<Vector2> myWarehouseList     = new List<Vector2>();
    public static Vector2 myBase;
    public static Vector2 myHeroePos;

    // Positions and fogtype of the enemy GameObjects lists
    public static List<StructUnitFogPos> enemyUnitList              = new List<StructUnitFogPos>();
    public static List<StructBuildingFogPos> enemyTowerNeutralList  = new List<StructBuildingFogPos>();
    public static List<StructBuildingFogPos> enemyTowerList         = new List<StructBuildingFogPos>();
    public static List<StructBuildingFogPos> enemyWarehouseList     = new List<StructBuildingFogPos>();
    public static StructBuildingFogPos enemyBase;
    public static StructUnitFogPos enemyHeroePos;

    private int cont = 0;

	// Use this for initialization
	void Start () 
    {
        sizeWorldFloor = terrain.terrainData.size.x;
        sizeProportionY = size / sizeWorldFloor;
        sizeProportionX = size / (sizeWorldFloor);

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

        // Bases
        Vector3 bcOrc = orcBase.transform.position;
        Vector3 bcRobot = robotBase.transform.position;

        float posxOrc = margin + (bcOrc.x + (sizeWorldFloor / 2)) * sizeProportionX;
        float posyOrc = posHeight + ((sizeWorldFloor / 2) - bcOrc.z) * sizeProportionY;

        float posxRobot = margin + (bcRobot.x + (sizeWorldFloor / 2)) * sizeProportionX;
        float posyRobot = posHeight + ((sizeWorldFloor / 2) - bcRobot.z) * sizeProportionY;
        
        if (teamNumber == 1)
        {
            myBase = new Vector2(posxRobot, posyRobot);
            SetTileTransparent(posxRobot, posyRobot, true);
            // Update the tile
            int tileX = (int)((posxOrc - margin) / tileSize);
            int tileY = (int)((posyOrc - posHeight) / tileSize);
            enemyBase = new StructBuildingFogPos(new Vector2(posxOrc, posyOrc), 0, tileX, tileY);
        }
        else
        {
            myBase = new Vector2(posxOrc, posyOrc);
            SetTileTransparent(posxOrc, posyOrc, true);
            // Update the tile
            int tileX = (int)((posxRobot - margin) / tileSize);
            int tileY = (int)((posyRobot - posHeight) / tileSize);
            enemyBase = new StructBuildingFogPos(new Vector2(posxRobot, posyRobot), 0, tileX, tileY);
        } 
	}

	void LateUpdate () 
    {
        int max0 = myArmy.Count;
        int max1 = enemyArmy.Count;
        
        // The transparent positions of the matrix to semitransparent
        for (int i = contY; i < MATRIXSIZE; i += 3)
        {
            for (int j = contX; j < MATRIXSIZE; j += 3)
            {
                i = i % MATRIXSIZE;
                j = j % MATRIXSIZE;
                // If the tile is transparent and has to change to semitransparent
                if (fogTypeMatrix[i, j].alwaysVisible == 0 && fogTypeMatrix[i, j].IsMaxCont() && fogTypeMatrix[i, j].GetFogType() == 2)
                {
                    fogTypeMatrix[i, j].SetFogType(1);
                    fogTypeMatrix[i, j].ResetCont();
                        
                }
                // else if the tile is transparent we increase its cont
                else if (fogTypeMatrix[i, j].alwaysVisible == 0 && fogTypeMatrix[i, j].GetFogType() == 2)
                    fogTypeMatrix[i, j].IncreaseCont();
            }
        }
        contX = (contX + 1) % 4;
        if (contX == 0)
            contY = (contY + 1) % 4;
        
        // myArmy
        if (myArmy.Count > 0)
        {
            for (int i = contUpdate0; i < max0; i += 3)
            {
                // Update the unit position
                float posx = margin + (myArmy[i].position.x + (sizeWorldFloor / 2)) * sizeProportionX;
                float posy = posHeight + ((sizeWorldFloor / 2) - myArmy[i].position.z) * sizeProportionY;
                myUnitList[i] = new Vector2(posx, posy);

                // Update the matrix
                SetTileTransparent(posx, posy, false);
            }
            contUpdate0 = (contUpdate0 + 1) % 4;
        }
        else
            contUpdate0 = (contUpdate0 + 1) % 4;

        // myHeroe
        if (myHeroe && contUpdate0 == 0)
        {
            // Update the heroe position
            float posx = margin + (myHeroe.position.x + (sizeWorldFloor / 2)) * sizeProportionX;
            float posy = posHeight + ((sizeWorldFloor / 2) - myHeroe.position.z) * sizeProportionY;
            myHeroePos = new Vector2(posx, posy);

            // Update the matrix
            SetTileTransparent(posx, posy, false);
        }
        
        // enemyArmy
        if (enemyArmy.Count > 0)
        {
            for (int i = contUpdate1; i < max1; i += 3)
            {
                // update the unit position
                float posx = margin + (enemyArmy[i].position.x + (sizeWorldFloor / 2)) * sizeProportionX;
                float posy = posHeight + ((sizeWorldFloor / 2) - enemyArmy[i].position.z) * sizeProportionY;
                enemyUnitList[i].SetPosition(new Vector2(posx, posy));

                // update the unit fog type            
                int tileX = (int)((posx - margin) / tileSize);
                int tileY = (int)((posy - posHeight) / tileSize);
                enemyUnitList[i].SetFogType(fogTypeMatrix[tileX, tileY].GetFogType());
            }
            contUpdate1 = (contUpdate1 + 1) % 4;
        }
        //enemyHeroe
        if (enemyHeroe && contUpdate0 == 0)
        {
            // Update the heroe position
            float posx = margin + (enemyHeroe.position.x + (sizeWorldFloor / 2)) * sizeProportionX;
            float posy = posHeight + ((sizeWorldFloor / 2) - enemyHeroe.position.z) * sizeProportionY;
            enemyHeroePos.SetPosition(new Vector2(posx, posy));

            // update the heroe fog type            
            int tileX = (int)((posx - margin) / tileSize);
            int tileY = (int)((posy - posHeight) / tileSize);
            enemyHeroePos.SetFogType(fogTypeMatrix[tileX, tileY].GetFogType());
        }
	}

    public static void SetTileTransparent(float posx, float posy, bool alwaysTransparent)
    {
        // Get the tiles
        int tileX = (int)((posx - margin) / tileSize);
        int tileY = (int)((posy - posHeight) / tileSize);
        
        // Update the current tile
        fogTypeMatrix[tileX, tileY].SetFogType(2);
        fogTypeMatrix[tileX, tileY].ResetCont();

        // Update the around tiles
        if (tileX > 0)
        {
            if (tileY > 0)
            {
                fogTypeMatrix[tileX - 1, tileY - 1].SetFogType(2);
                fogTypeMatrix[tileX - 1, tileY - 1].ResetCont();
            }
            fogTypeMatrix[tileX - 1, tileY].SetFogType(2);
            fogTypeMatrix[tileX - 1, tileY].ResetCont();
            if (tileY < MATRIXSIZE - 1)
            {
                fogTypeMatrix[tileX - 1, tileY + 1].SetFogType(2);
                fogTypeMatrix[tileX - 1, tileY + 1].ResetCont();
            }
        }
        if (tileX < MATRIXSIZE - 1)
        {
            if (tileY > 0)
            {
                fogTypeMatrix[tileX + 1, tileY - 1].SetFogType(2);
                fogTypeMatrix[tileX + 1, tileY - 1].ResetCont();
            }
            fogTypeMatrix[tileX + 1, tileY].SetFogType(2);
            fogTypeMatrix[tileX + 1, tileY].ResetCont();
            if (tileY < MATRIXSIZE - 1)
            {
                fogTypeMatrix[tileX + 1, tileY + 1].SetFogType(2);
                fogTypeMatrix[tileX + 1, tileY + 1].ResetCont();
            }
        }
        if (tileY > 0)
        {
            fogTypeMatrix[tileX, tileY - 1].SetFogType(2);
            fogTypeMatrix[tileX, tileY - 1].ResetCont();
        }
        if (tileY < MATRIXSIZE - 1)
        {
            fogTypeMatrix[tileX, tileY + 1].SetFogType(2);
            fogTypeMatrix[tileX, tileY + 1].ResetCont();
        }

        // Reset the cont
        //if (fogTypeMatrix[tileX, tileY].GetCont() != 0)
        //{
        //    fogTypeMatrix[tileX, tileY].ResetCont();
        //}

        // Always transparent
        if (alwaysTransparent)
            //fogTypeMatrix[tileX, tileY].alwaysVisible++;
            SetTileAlwaysTransparent(tileX, tileY);
    }

    public static void SetTileAlwaysTransparent(int tileX, int tileY)
    {
        // Current tile
        fogTypeMatrix[tileX , tileY].alwaysVisible++;

        // Around tiles
        if (tileX > 0)
        {
            if (tileY > 0) 
                fogTypeMatrix[tileX - 1, tileY - 1].alwaysVisible++;
            fogTypeMatrix[tileX - 1, tileY].alwaysVisible++;
            if (tileY < 19) 
                fogTypeMatrix[tileX - 1, tileY + 1].alwaysVisible++;
        }
        if (tileX < 19)
        {
            if (tileY > 0) 
                fogTypeMatrix[tileX + 1, tileY - 1].alwaysVisible++;
            fogTypeMatrix[tileX + 1, tileY].alwaysVisible++;
            if (tileY < 19) 
                fogTypeMatrix[tileX + 1, tileY + 1].alwaysVisible++;
        }
        if (tileY > 0) 
            fogTypeMatrix[tileX, tileY - 1].alwaysVisible++;
        if (tileY < 19) 
            fogTypeMatrix[tileX, tileY + 1].alwaysVisible++;
    }

    public static void InsertUnit(Transform unit, int teamN)
    {
        if (teamN == teamNumber)
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

    public static void DeleteUnit(Transform unit, int teamN)
    {
        if (teamN == teamNumber)
        {
            int pos = myArmy.IndexOf(unit);
            if (pos != -1)
            {
                // Update the unit position
                float posx = margin + (unit.position.x + (sizeWorldFloor / 2)) * sizeProportionX;
                float posy = posHeight + ((sizeWorldFloor / 2) - unit.position.z) * sizeProportionY;
                //myUnitList[i] = new Vector2(posx, posy);

                // Update the matrix
                int tileX = (int)((posx - margin) / tileSize);
                int tileY = (int)((posy - posHeight) / tileSize);

                if (fogTypeMatrix[tileX, tileY].alwaysVisible == 0) 
                    fogTypeMatrix[tileX, tileY].SetFogType(1);
                
                myArmy.Remove(unit);
                myUnitList.RemoveAt(pos);
            }
        }
        else
        {
            int pos = enemyArmy.IndexOf(unit);
            if (pos != -1)
            {
                enemyArmy.Remove(unit);
                enemyUnitList.RemoveAt(pos);
            }
        }

    }

    public static void InsertTower(Transform tower, int teamN)
    {
        float posx = margin + (tower.position.x + (sizeWorldFloor / 2)) * sizeProportionX;
        float posy = posHeight + ((sizeWorldFloor / 2) - tower.position.z) * sizeProportionY;

        if (teamN == teamNumber)
        {
            myTowerList.Add(new Vector2(posx, posy));
            SetTileTransparent(posx, posy, true);
        }
        else
        {
            // Update the tile
            int tileX = (int)((posx - margin) / tileSize);
            int tileY = (int)((posy - posHeight) / tileSize);
            enemyTowerList.Add(new StructBuildingFogPos(new Vector2(posx, posy), 0, tileX, tileY));
        }
    }

    public static void InsertWarehouse (Transform warehouse, int teamN)
    {
        float posx = margin + (warehouse.position.x + (sizeWorldFloor / 2)) * sizeProportionX;
        float posy = posHeight + ((sizeWorldFloor / 2) - warehouse.position.z) * sizeProportionY;

        if (teamN == teamNumber)
        {
            myWarehouseList.Add(new Vector2(posx, posy));
            SetTileTransparent(posx, posy, true);
        }
        else
        {
            // Update the tile
            int tileX = (int)((posx - margin) / tileSize);
            int tileY = (int)((posy - posHeight) / tileSize);
            enemyWarehouseList.Add(new StructBuildingFogPos(new Vector2(posx, posy), 0, tileX, tileY));
        }
    }

    public static void SetTowerNeutral(Transform tower, int teamN)
    {

        float posx = margin + (tower.position.x + (sizeWorldFloor / 2)) * sizeProportionX;
        float posy = posHeight + ((sizeWorldFloor / 2) - tower.position.z) * sizeProportionY;

        if (teamN == teamNumber)
        {
            myTowerNeutralList.Add(new Vector2(posx, posy));
            SetTileTransparent(posx, posy, true);
        }
        else
        {
            // Update the tile
            int tileX = (int)((posx - margin) / tileSize);
            int tileY = (int)((posy - posHeight) / tileSize);
            enemyTowerNeutralList.Add(new StructBuildingFogPos(new Vector2(posx, posy), 0, tileX, tileY));
        }   
    }

    public static void InsertHeroe(Transform heroe, int teamN)
    {
        if (teamN == teamNumber)
        {
            myHeroe = heroe;
            myHeroePos = new Vector2();
        }
        else
        {
            enemyHeroe = heroe;
            enemyHeroePos = new StructUnitFogPos();
        }

    }

    public virtual void OnGUI ()
    {
        // Esquina superior izquierda: x = -250, z = 250;
        Rect rect1;
        rect1 = new Rect(margin, posHeight, size, size);
        //GUI.DrawTexture(rect1, textureMap);
        GUI.DrawTexture(rect1, map);

        // Draw tiles
        float littleSize = size / 20;
        //for (int i = 0; i <= 20; i++)
        //{
        //    GUI.DrawTexture(new Rect(i * littleSize + margin, posHeight, 1f, size), textureBase1);
        //    GUI.DrawTexture(new Rect(margin, i * littleSize + posHeight, size, 1f), textureBase1);
        //}

        //Draw transparent tiles
        for (int i = 0; i < MATRIXSIZE; i++)
            for (int j = 0; j < MATRIXSIZE; j++)
            {
                if (fogTypeMatrix[i, j].GetFogType() == 0) 
                    GUI.DrawTexture(new Rect(i * littleSize + margin, j * littleSize + posHeight, littleSize, littleSize), textureMap);
                else if (fogTypeMatrix[i, j].GetFogType() == 1)
                    GUI.DrawTexture(new Rect(i * littleSize + margin, j * littleSize + posHeight, littleSize, littleSize), textureSemiTransparent);
            }
        
        // my own team
        foreach (Vector2 unit in myUnitList)
        {
            rect1 = new Rect(unit.x, unit.y, 2.0f, 2.0f);
            GUI.DrawTexture(rect1, textureUnit0);
        }
        foreach (Vector2 tower in myTowerList)
        {
            rect1 = new Rect(tower.x, tower.y, 4.0f, 4.0f);
            GUI.DrawTexture(rect1, textureTower0);
        }
        foreach (Vector2 warehouse in myWarehouseList)
        {
            rect1 = new Rect(warehouse.x, warehouse.y, 4.0f, 4.0f);
            GUI.DrawTexture(rect1, textureWarehouse0);
        }
        foreach (Vector2 tower in myTowerNeutralList)
        {
            rect1 = new Rect(tower.x, tower.y, 4.0f, 4.0f);
            GUI.DrawTexture(rect1, textureTowerNeutral);
        }
        if (myHeroe)
        {
            rect1 = new Rect(myHeroePos.x, myHeroePos.y, 4.0f, 4.0f);
            GUI.DrawTexture(rect1, textureMyHeroe);
        }

        // enemy team
        foreach (StructUnitFogPos unit in enemyUnitList)
        {
            if (unit.GetFogType() == 2)
            {
                Vector2 posUnit = unit.GetPosition();
                rect1 = new Rect(posUnit.x, posUnit.y, 2.0f, 2.0f);
                GUI.DrawTexture(rect1, textureUnit1);
            }
        } 
        foreach (StructBuildingFogPos tower in enemyTowerList)
        {
            int fogT = fogTypeMatrix[tower.GetTileX(), tower.GetTileY()].GetFogType();
            if (fogT != 0)
            {
                Vector2 posBuilding = tower.GetPosition();
                rect1 = new Rect(posBuilding.x, posBuilding.y, 4.0f, 4.0f);
                GUI.DrawTexture(rect1, textureTower1);
            }
        }
        foreach (StructBuildingFogPos warehouse in enemyWarehouseList)
        {
            int fogT = fogTypeMatrix[warehouse.GetTileX(), warehouse.GetTileY()].GetFogType();
            if (fogT != 0)
            {
                Vector2 posBuilding = warehouse.GetPosition();
                rect1 = new Rect(posBuilding.x, posBuilding.y, 4.0f, 4.0f);
                GUI.DrawTexture(rect1, textureWarehouse1);
            }
        }
        foreach (StructBuildingFogPos tower in enemyTowerNeutralList)
        {
            int fogT = fogTypeMatrix[tower.GetTileX(), tower.GetTileY()].GetFogType();
            if (fogT != 0)
            {
                Vector2 posBuilding = tower.GetPosition();
                rect1 = new Rect(posBuilding.x, posBuilding.y, 4.0f, 4.0f);
                GUI.DrawTexture(rect1, textureTowerNeutral);
            }
        }
        if (enemyHeroe && enemyHeroePos.GetFogType() == 2)
        {
            Vector2 posHeroe = enemyHeroePos.GetPosition();
            rect1 = new Rect(posHeroe.x, posHeroe.y, 4.0f, 4.0f);
            GUI.DrawTexture(rect1, textureEnemyHeroe);
        }
        
        // myBase
        rect1 = new Rect(myBase.x, myBase.y, 8.0f, 8.0f);
        GUI.DrawTexture(rect1, textureBase0);
        // enemyBase
        int fogTBase = fogTypeMatrix[enemyBase.GetTileX(), enemyBase.GetTileY()].GetFogType();
        if (fogTBase != 0)
        {
            Vector2 posBuilding = enemyBase.GetPosition();
            rect1 = new Rect(posBuilding.x, posBuilding.y, 8.0f, 8.0f);
            GUI.DrawTexture(rect1, textureBase1);
        }
    }

}
