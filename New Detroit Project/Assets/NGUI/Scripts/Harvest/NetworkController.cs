using UnityEngine;
using System.Collections;

public class NetworkController : Photon.MonoBehaviour
{

    //private string roomName = "myRoom";

    private Vector2 scrollPos = Vector2.zero;

    private bool connectFailed = false;

    public static readonly string SceneNameMenu = "MainMenu";

    public static readonly string SceneNameGame = "NewDetroit01";

    // Input of the names
    public GameObject labelRoomName;
    public GameObject labelPlayerName;
    // Output of the rooms
    public GameObject labelRooms;
    public GameObject labelPlayers;
    // Auxiliar variables
    public GameObject originalButton;
    private ArrayList layerButons;
    private short selected;
    private string roomName;
    public UILabel info;

    public void Start()
    {
        UILabel scriptRoom = labelRooms.GetComponent<UILabel>();
        UILabel scriptPlayer = labelPlayers.GetComponent<UILabel>();
        scriptRoom.text = "";
        scriptRoom.UpdateNGUIText();
        scriptPlayer.text = "";
        scriptPlayer.UpdateNGUIText();
        layerButons = new ArrayList();
        selected = -1;
        info.text = "";
        originalButton.SetActive(false);
        roomName = "";
    }

    public void Awake()
    {
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;

        // the following line checks if this client was just created (and not yet online). if so, we connect
        if (PhotonNetwork.connectionStateDetailed == PeerState.PeerCreated)
        {
            // Connect to the photon master-server. We use the settings saved in PhotonServerSettings (a .asset file in this project)
            PhotonNetwork.ConnectUsingSettings("1.0");
        }

        // generate a name for this player, if none is assigned yet
        if (string.IsNullOrEmpty(PhotonNetwork.playerName))
        {
            PhotonNetwork.playerName = "Guest" + Random.Range(1, 9999);
        }

        // if you wanted more debug out, turn this on:
        // PhotonNetwork.logLevel = NetworkLogLevel.Full;
    }

    public void CreateRoom()
    {
        UILabel roomName = labelRoomName.GetComponent<UILabel>();
        RoomInfo[] roomsInfo = PhotonNetwork.GetRoomList();
        int i = 0; bool enc = false;
        while (i < roomsInfo.Length && !enc)
        {
            if (roomsInfo[i].name.Equals(roomName))
                enc = true;
        }
        // If there's no other roome with the same name
        if (!enc)
        {
            if (roomName.text.Equals(""))
            {
                PhotonNetwork.CreateRoom("Room" + Random.Range(1, 9999), true, true, 4);
                info.text = "Room created.";
            }
            else
            {
                PhotonNetwork.CreateRoom(roomName.text, true, true, 4);
                info.text = "Room " + roomName.text + " created.";
            }
        }
        else
        {
            info.text = "already exist a room with that name";
        }
    }

    public void JoinRoom()
    {
        if (selected == -1)
        {
            info.text = "Select one room first";
        }
        else
        {
            RoomInfo[] roomsInfo = PhotonNetwork.GetRoomList();

            if (roomsInfo[selected].playerCount == roomsInfo[selected].maxPlayers)
            {
                info.text = "This room is full";
            }
            else
                PhotonNetwork.JoinRoom(roomName);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void UpdateRooms()
    {
        RoomInfo[] roomsInfo = PhotonNetwork.GetRoomList();
        Debug.Log(roomsInfo.Length);
        info.text = roomsInfo.Length + " rooms";
        UILabel roomScript = labelRooms.GetComponent<UILabel>();
        UILabel playerScript = labelPlayers.GetComponent<UILabel>();
        roomScript.text = playerScript.text = "";
        originalButton.SetActive(true);
        originalButton.transform.GetChild(0).gameObject.SetActive(true);
        for (int i = 0; i < layerButons.Count; i++)
        {
            Destroy((Object)layerButons[i]);
            layerButons.RemoveAt(i);
        }
        for (int i = 0; i < roomsInfo.Length; i++)
        {
            GameObject button = (GameObject)Instantiate(originalButton);
            button.name = "labelButton" + i;
            button.transform.parent = labelRooms.transform;
            button.transform.position = originalButton.transform.position;
            button.transform.localScale = originalButton.transform.localScale;

            button.transform.localPosition += new Vector3(0, -12 * i, 0);
            layerButons.Add(button);

            roomScript.text = roomScript.text + roomsInfo[i].name + "\n";
            playerScript.text = playerScript.text + roomsInfo[i].playerCount + "/" + roomsInfo[i].maxPlayers + "\n";
        }
        originalButton.SetActive(false);
        originalButton.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void RoomSelected(GameObject s)
    {
        short sel = short.Parse(s.name.Substring(s.name.Length - 1));
        for (int i = 0; i < layerButons.Count; i++)
        {
            GameObject buttonAux = ((GameObject)layerButons[i]);
            if (buttonAux != s)
            {
                UIButton aux = buttonAux.GetComponent<UIButton>();
                aux.defaultColor = new Color(aux.defaultColor.r, aux.defaultColor.g, aux.defaultColor.b, 0.0f);
                buttonAux.SendMessage("OnDisable", true);
            }
        }
        UIButton button = s.GetComponent<UIButton>();
        if (selected == sel)
        {
            selected = -1;
            button.defaultColor = new Color(button.defaultColor.r, button.defaultColor.g, button.defaultColor.b, 0.0f);
            roomName = "";
        }
        else
        {
            selected = sel;
            RoomInfo[] roomsInfo = PhotonNetwork.GetRoomList();
            roomName = roomsInfo[sel].name;
            button.defaultColor = new Color(button.defaultColor.r, button.defaultColor.g, button.defaultColor.b, 0.5f);
        }
        if (roomName.Equals(""))
            info.text = "none selected";
        else
            info.text = roomName + " selected";
    }

    // We have two options here: we either joined(by title, list or random) or created a room.
    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
    }

    public void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
        //PhotonNetwork.LoadLevel(SceneNameGame);
    }


    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("Disconnected from Photon.");
    }

    public void OnFailedToConnectToPhoton(object parameters)
    {
        this.connectFailed = true;
        Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters);
    }
}



