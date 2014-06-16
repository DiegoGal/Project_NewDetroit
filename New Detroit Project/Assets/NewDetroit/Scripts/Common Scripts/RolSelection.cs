using UnityEngine;
using System.Collections;

public class RolSelection : MonoBehaviour {
    
    public GameObject robArmy;
    public GameObject skelterArmy;
    public GameObject robRender;
    public GameObject skelterBot;
    bool heroes = true;

	// Use this for initialization
	void Start () {
        for (int j = 0; j < 3; j++)
        {
            Transform model = skelterArmy.transform.GetChild(j).FindChild("Model");         
            Transform model1 = robArmy.transform.GetChild(j).FindChild("Model");
            for (int i = 0; i < model.renderer.materials.Length; i++)            
                model.renderer.materials[i].SetColor("_OutlineColor", Color.black);                
            for (int i = 0; i < model1.renderer.materials.Length; i++)
                model1.renderer.materials[i].SetColor("_OutlineColor", Color.black);
        }
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ChangeSelection()
    {
        robRender.SetActive(heroes);
        skelterBot.SetActive(heroes);
        robArmy.SetActive(!heroes);
        skelterArmy.SetActive(!heroes);
        heroes = !heroes;
    }
}
