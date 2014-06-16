using UnityEngine;
using System.Collections;

public class AnimationHeroMenu : MonoBehaviour {

    public GameObject army;
    public GameObject heroe;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnHover(bool isOver)
    {
        if (heroe.activeInHierarchy)
        {
            if (heroe.name.Equals("Skelterbot"))
            {
                if (isOver)
                    heroe.animation.CrossFade("Run");
                else
                    heroe.animation.CrossFade("Idle01");
            }
            else
            {
                if (isOver)
                    heroe.animation.CrossFade("Run");
                else
                    heroe.animation.CrossFade("Iddle01");
            }
        }
        if (army.activeInHierarchy)
        {
            for (int i = 0; i < 3; i++)
            {
                if (isOver)
                    army.transform.GetChild(i).gameObject.animation.CrossFade("Walk");
                else 
                    army.transform.GetChild(i).gameObject.animation.CrossFade("Idle01");
            }
        }
    }
}
