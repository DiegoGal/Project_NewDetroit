using UnityEngine;
using System.Collections;

// For the registration of the selection 
public class RolSelection : Photon.MonoBehaviour {
    
    public GameObject robArmy;
    public GameObject skelterArmy;
    public GameObject robRender;
    public GameObject skelterBot;
    // for input selection
    public GameObject rightPanel;
    public GameObject leftPanel;
    // local player selection
    public int localSelection;

    public bool heroes;
    // 0 = Rob Render, 1 = Skelterbot, 2 = Rob Army, 3 = Skelter Army
    public int[] rolSelected;
    public string[] players;

	// Use this for initialization
	void Start () 
    {
        players = new string[4];
        rolSelected = new int[4];
        for (int i = 0; i < 4; i++)
        {
            rolSelected[i] = -1;
            players[i] = "";
        }
        heroes = true;
        rightPanel.SetActive(true);
        leftPanel.SetActive(true);
        localSelection = -1;
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
	
	// Udate is called once per frame
	void Updatpe () 
    {
        
	}

    private void setSelected(int selected, float color)
    {   
        switch (selected)
        {
            case 0: robRender.transform.FindChild("Orc_M").renderer.material.color = new Color(color, color, color); break;
            case 1: skelterBot.transform.FindChild("Skelterbot").renderer.material.color = new Color(color, color, color); break;
            case 2:
                for (int j = 0; j < 3; j++)
                {
                    robArmy.transform.GetChild(j).FindChild("Model").renderer.material.color = new Color(color, color, color);
                }
                break;
            case 3:
                for (int j = 0; j < 3; j++)
                {
                    skelterArmy.transform.GetChild(j).FindChild("Model").renderer.material.color = new Color(color, color, color);
                }
                break;
            default:
                break;
        }
    }

    [RPC]
    public void PlayerSelection(int selected, string name)
    {
        short i = 0; bool enc = false;
        while (i < rolSelected.Length && !enc)
        {
            if (players[i].Equals(name))
            {
                setSelected(rolSelected[i], 1.0f);
                rolSelected[i] = -1;
                players[i] = "";
            }
            if (rolSelected[i] == -1)
            {
                rolSelected[i] = selected;
                players[i] = name;
                enc = true;
            }
            i++;
        }
        setSelected(selected, 0.5f);
    }



    [RPC]
    public void PlayerDeselection(int selected, string name)
    {
        short i = 0; bool enc = false;
        while (i < rolSelected.Length && !enc)
        {
            if (rolSelected[i] == selected)
            {
                rolSelected[i] = -1;
                players[i] = "";
                enc = true;
            }
            i++;
        }

        setSelected(selected, 1.0f);
    }


    public void SelectionUpdate(int i)
    {        
        if (heroes)
        {            
            if (localSelection == i)
            {
                
                photonView.RPC("PlayerDeselection", PhotonTargets.All, localSelection,PhotonNetwork.playerName);
                localSelection = -1;
            }
            else
            {
                localSelection = i;             
                photonView.RPC("PlayerSelection", PhotonTargets.All, localSelection, PhotonNetwork.playerName);                
            }
        }
        else
        {
            i = i + 2;
            if (localSelection == i)
            {                
                photonView.RPC("PlayerDeselection", PhotonTargets.All, localSelection, PhotonNetwork.playerName);
                localSelection = -1;
            }
            else
            {
                localSelection = i;
                photonView.RPC("PlayerSelection", PhotonTargets.All, localSelection, PhotonNetwork.playerName);                
            }
        }
    }

    private bool isSelected(short i)
    {
        short j=0;
        while (j < rolSelected.Length)
        {
            if (rolSelected[j] == i)
                return true;
            j++;
        }
        return false;
    }

    public void ChangeSelection(string change)
    {
        if (change.Equals("Heroes"))
            heroes = true;
        else
            heroes = false;
        if (heroes)
        {
            rightPanel.SetActive(!isSelected(0));
            leftPanel.SetActive(!isSelected(1));
        }
        else
        {
            rightPanel.SetActive(!isSelected(2));
            leftPanel.SetActive(!isSelected(3));
        }
        robRender.SetActive(heroes);
        skelterBot.SetActive(heroes);
        robArmy.SetActive(!heroes);
        skelterArmy.SetActive(!heroes);        
    }
}
