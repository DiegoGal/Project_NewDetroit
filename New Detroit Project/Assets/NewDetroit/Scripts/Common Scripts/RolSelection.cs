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
    // label selection
    public GameObject labelRed;
    public GameObject labelBlue;
    // label room name
    public GameObject labelRoomName;
    // Play game button
    public GameObject playButton;
    // local player selection
    public int localSelection;
    // exit button reference
    public GameObject onlineExitButton;
    // lock button reference
    public GameObject lockButton;

    public bool heroes;
    // 0 = Rob Render, 1 = Skelterbot, 2 = Rob Army, 3 = Skelter Army
    public int[] rolSelected;
    public string[] players;

    public static bool isOnline;
    public bool locked;

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

    public void UpdateSelection()
    {
        if (heroes)
            UpdateNGUI("Heroes");
        else
            UpdateNGUI("Army");
        StartCoroutine(SetRoomName());
        lockButton.SetActive(true);
        locked = false;
        rightPanel.SetActive(true);
        leftPanel.SetActive(true);
        playButton.GetComponent<UIButton>().isEnabled = false;
		playButton.GetComponentInChildren<UILabel>().text = "Start";
    }

    private IEnumerator SetRoomName()
    {
        while (PhotonNetwork.room == null)
            yield return new WaitForSeconds(0.1f);
        labelRoomName.GetComponent<UILabel>().text = "Room " + PhotonNetwork.room.name + " | Player " + PhotonNetwork.playerName;
        Debug.Log("Players " + PhotonNetwork.room.playerCount);
        if (PhotonNetwork.room.playerCount > 1)
            photonView.RPC("ReciveUpdate", PhotonTargets.MasterClient, PhotonNetwork.player);
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
    public void ReciveUpdate(PhotonPlayer toMe)
    {
        photonView.RPC("SendUpdate", toMe, rolSelected, players);
    }

    [RPC]
    public void SendUpdate(int[] rolExtSelected, string[] playersExt)
    {
        for (int i = 0; i < playersExt.Length; i++)
        {
            rolSelected[i] = rolExtSelected[i];
            players[i] = playersExt[i];
            Debug.Log(players[i]);
            if (rolSelected[i] != -1)
                setSelected(rolSelected[i], 0.5f);
        }
        if (heroes)
            UpdateNGUI("Heroes");
        else
            UpdateNGUI("Army");
    }

    [RPC]
    public void PlayerSelection(int selected, string name)
    {
        int i = 0; bool enc = false;
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
        if (heroes)
            UpdateNGUI("Heroes");
        else
            UpdateNGUI("Army");
    }



    [RPC]
    public void PlayerDeselection(int selected, string name)
    {
        int i = 0; bool enc = false;
        while (i < rolSelected.Length && !enc)
        {
            if (rolSelected[i] == selected && players[i].Equals(name))
            {
                rolSelected[i] = -1;
                players[i] = "";
                enc = true;
            }
            i++;
        }

        setSelected(selected, 1.0f);
        if (heroes)
            UpdateNGUI("Heroes");
        else
            UpdateNGUI("Army");
    }


    public void SelectionUpdate(int i)
    {        
        if (heroes)
        {            
            if (localSelection == i)
            {
                if (isOnline)
                    photonView.RPC("PlayerDeselection", PhotonTargets.All, localSelection,PhotonNetwork.playerName);
                else
                    PlayerDeselection(localSelection, PhotonNetwork.playerName);
                localSelection = -1;
            }
            else
            {
                localSelection = i;
                if (i == 0) // Rob Render
                {
                    robRender.animation.CrossFade("FloorHit");
                    robRender.animation.CrossFadeQueued("Iddle01");
                    StartCoroutine(DoAnimSelection(robRender.animation, "FloorHit"));
                }
                else  // Skelterbot
                {
                    skelterBot.animation.CrossFade("Attack1");
                    skelterBot.animation.CrossFadeQueued("Idle01");
                    StartCoroutine(DoAnimSelection(skelterBot.animation, "Attack1"));
                }
            }
        }
        else
        {
            i = i + 2;
            if (localSelection == i)
            {
                if (isOnline)
                    photonView.RPC("PlayerDeselection", PhotonTargets.All, localSelection, PhotonNetwork.playerName);
                else
                    PlayerDeselection(localSelection, PhotonNetwork.playerName);
                localSelection = -1;
            }
            else
            {
                localSelection = i;
                Animation anim = null;
                for (int j = 0; j < 3; j++)
                {                    
                    if (i == 2) // Rob Army                
                        anim = robArmy.transform.GetChild(j).animation;
                    else
                        anim = skelterArmy.transform.GetChild(j).animation;
                    anim["Attack1"].wrapMode = WrapMode.Default;
                    anim.CrossFade("Attack1");
                    anim.CrossFadeQueued("Idle01");
                }
                StartCoroutine(DoAnimSelection(anim, "Attack1"));                                              
            }
        }
    }

    private IEnumerator DoAnimSelection(Animation anim, string name)
    {
        while (anim.IsPlaying(name))
            yield return new WaitForSeconds(0.1f);
        if (isOnline)
            photonView.RPC("PlayerSelection", PhotonTargets.All, localSelection, PhotonNetwork.playerName);
        else
            PlayerSelection(localSelection, PhotonNetwork.playerName);
    }

    private bool isSelected(int i)
    {
        int j=0;
        while (j < rolSelected.Length)
        {
            if (rolSelected[j] == i)
                return true;
            j++;
        }
        return false;
    }

    private string whoSelected(int i)
    {
        int j = 0;
        while (j < rolSelected.Length)
        {
            if (rolSelected[j] == i)
                return players[j];
            j++;
        }
        return "Error";
    }

    private bool AllPlayersIn()
    {
        int j = 0, numPlay = 0;
        while (j < rolSelected.Length)
        {
            if (rolSelected[j] == -1);
                //return false;
            else numPlay++;
            j++;
        }
        return numPlay >= 1;// true;
    }

    public void UpdateNGUI(string change)
    {
        if (change.Equals("Heroes"))
            heroes = true;
        else
            heroes = false;
        
        if (heroes)
        {
            bool selected0 = isSelected(0);
            bool selected1 = isSelected(1);

            if (selected0)
                labelRed.GetComponent<UILabel>().text = whoSelected(0) + " as Rob Render";
            else
                labelRed.GetComponent<UILabel>().text = "Play as Rob Render";

            if (selected1)
                labelBlue.GetComponent<UILabel>().text = whoSelected(1) + " as Skelterbot";
            else
                labelBlue.GetComponent<UILabel>().text = "Play as Skelterbot";

            if (!locked)
            {
                rightPanel.SetActive(!selected0);
                leftPanel.SetActive(!selected1);
            }
        }
        else
        {
            bool selected2 = isSelected(2);
            bool selected3 = isSelected(3);

            if (selected2)
                labelRed.GetComponent<UILabel>().text = whoSelected(2) + " as Rob Army";
            else
                labelRed.GetComponent<UILabel>().text = "Play as Rob Army";

            if (selected3)
                labelBlue.GetComponent<UILabel>().text = whoSelected(3) + " as Skelter Army";
            else
                labelBlue.GetComponent<UILabel>().text = "Play as Skelter Army";

            if (!locked)
            {
                rightPanel.SetActive(!selected2);
                leftPanel.SetActive(!selected3);
            }
        }
        robRender.SetActive(heroes);
        skelterBot.SetActive(heroes);
        robArmy.SetActive(!heroes);
        skelterArmy.SetActive(!heroes);        
        /*
        if (!AllPlayersIn())
        {
            playButton.GetComponent<UIButton>().enabled = false;
            playButton.GetComponent<UIButton>().SetState(UIButtonColor.State.Disabled, true);
            playButton.GetComponentInChildren<UILabel>().text = "Waiting for players";
        }
        else
        {
            playButton.GetComponent<UIButton>().enabled = true;
            playButton.GetComponent<UIButton>().SetState(UIButtonColor.State.Normal, true);
            playButton.GetComponentInChildren<UILabel>().text = "Start Game";
        }*/
    }

    public void Lock()
    {
        if (localSelection != -1)
        {
            setSelected(localSelection, 0.5f);
            lockButton.SetActive(false);
            locked = true;
            rightPanel.SetActive(false);
            leftPanel.SetActive(false);
            playButton.GetComponent<UIButton>().isEnabled = true;
        }
    }

    public void ExitRoom()
    {
        if (isOnline)
            photonView.RPC("PlayerDeselection", PhotonTargets.All, localSelection, PhotonNetwork.playerName);
        else
            PlayerDeselection(localSelection, PhotonNetwork.playerName);
        PhotonNetwork.LeaveRoom();
    }

    public void LoadScene()
    {
        LocalGameManager.joinedId = localSelection;
        PhotonNetwork.LoadLevel(NetworkController.SceneNameGame);
    }

    private IEnumerator FinalCountdown()
    {
        int count = 5;
            //yield return new WaitForSeconds(0.1f);
            //count -= 0.1f - Time.deltaTime;
        playButton.GetComponentInChildren<UILabel>().text = "" + count;
        while (count > 0)
        {
            yield return new WaitForSeconds(1f);
            count -= 1;
            playButton.GetComponentInChildren<UILabel>().text = "" + count;
        }
        LoadScene();        
    }

    [RPC]
    private void CallToCountdown()
    {
        playButton.GetComponent<UIButton>().isEnabled = false;
        StartCoroutine(FinalCountdown());
    }

    public void StartGame()
    {
        //here is where has to be loaded the level
        if (isOnline)
        {
            if (AllPlayersIn())
                photonView.RPC("CallToCountdown", PhotonTargets.All);
        }
        else
        {
            // Load the offline level
			if (localSelection == -1)
			{
				playButton.GetComponentInChildren<UILabel>().text = "Select role";
			}
			else
			{
				LocalGameManagerOffline.joinedId = localSelection;
				Application.LoadLevel(NetworkController.SceneNameGameOffline);
			}			
        }

        //PhotonNetwork.LoadLevel(NetworkController.SceneNameGame);
    }
}
