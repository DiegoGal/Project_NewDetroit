using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Minimap : MonoBehaviour {

    // Minimap properties
    float size = 180.0f;
    float margin = 10.0f;
    public Texture textureMap;
    public Texture textureUnit0;

    //Terrain properties
    public float sizeWorldFloor;
    public float sizeProportion;

    public GameObject armyController;
    private int contUpdate = 0;

    // Positions of the GameObjects lists
    public List<Vector2> unitList0;
    public List<Vector2> unitList1;
    public List<Vector2> buildingList0;
    public List<Vector2> buildingList1;    

	// Use this for initialization
	void Start () 
    {
        sizeProportion = size / sizeWorldFloor;    
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (contUpdate == 20)
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

	}

    public virtual void OnGUI()
    {
        Rect rect1;
        float posHeight =  Screen.height - (size + margin);
        rect1 = new Rect(margin, posHeight, size, size);
        GUI.DrawTexture(rect1, textureMap);
        
        foreach (Vector2 unit in unitList0)
        {
            // Esquina superior izquierda: x = -250, z = 250;
            float posx = margin + (unit.x + (sizeWorldFloor / 2)) * sizeProportion;
            float posy = posHeight + ((sizeWorldFloor / 2) - unit.y) * sizeProportion;
            rect1 = new Rect(posx, posy, 0.8f, 0.8f);
            GUI.DrawTexture(rect1, textureUnit0);
        }

    }

}
