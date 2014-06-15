using UnityEngine;
using System.Collections;

public class NetworkController : Photon.MonoBehaviour {

	//private string roomName = "myRoom";

    private Vector2 scrollPos = Vector2.zero;

    private bool connectFailed = false;

    public static readonly string SceneNameMenu = "Menu";

    public static readonly string SceneNameGame = "NewDetroit";

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
        if (roomName.text.Equals(""))
        {
            PhotonNetwork.CreateRoom("Room" + Random.Range(1,9999), true, true, 4);
            info.text = "Room created.";
        }
        else
        {
            PhotonNetwork.CreateRoom(roomName.text, true, true, 4);
            info.text = "Room " + roomName.text + " created.";
        }
        
    }

    public void JoinRoom()
    {
 
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void PlayButton()
    {
        //rooms = PhotonNetwork.GetRoomList();
    }

    public void UpdateRooms()
    {
        RoomInfo[] roomsInfo = PhotonNetwork.GetRoomList();
        Debug.Log(roomsInfo.Length);
        info.text = roomsInfo.Length + " rooms";
        UILabel roomScript = labelRooms.GetComponent<UILabel>();
        UILabel playerScript = labelPlayers.GetComponent<UILabel>();
        roomScript.text = playerScript .text = "";
        originalButton.SetActive(true);
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

            button.transform.localPosition += new Vector3(0,-12*i,0); 
            layerButons.Add(button);

            roomScript.text =roomScript.text +  roomsInfo[i].name + "\n";
            playerScript.text = playerScript.text + roomsInfo[i].playerCount + "/" + roomsInfo[i].maxPlayers + "\n";
        }
        originalButton.SetActive(false);
        
    }

    public void RoomSelected(GameObject s)
    {
        short sel = short.Parse(s.name.Substring(s.name.Length - 1));
        for (int i = 0; i < layerButons.Count; i++)
        {
            UIButton aux = ((GameObject)layerButons[i]).GetComponent<UIButton>();
            aux.defaultColor = new Color(aux.defaultColor.r, aux.defaultColor.g, aux.defaultColor.b, 0.0f);
            
        }
        UIButton button = s.GetComponent<UIButton>();
        if (selected == sel)
        {
            selected = -1;
            button.defaultColor = new Color(button.defaultColor.r, button.defaultColor.g, button.defaultColor.b, 0.0f);
        }
        else
        {
            selected = sel;
            button.defaultColor = new Color(button.defaultColor.r, button.defaultColor.g, button.defaultColor.b, 0.5f);
        }
        info.text = selected + " selected";
    }

    public void OnGUI() 
    {
        /*
        if (!PhotonNetwork.connected)
        {
            if (PhotonNetwork.connectionState == ConnectionState.Connecting)
            {
                GUILayout.Label("Connecting " + PhotonNetwork.ServerAddress);
                GUILayout.Label(Time.time.ToString());
            }
            else
            {
                GUILayout.Label("Not connected. Check console output.");
            }

            if (this.connectFailed)
            {
                GUILayout.Label("Connection failed. Check setup and use Setup Wizard to fix configuration.");
                GUILayout.Label(String.Format("Server: {0}:{1}", new object[] {PhotonNetwork.ServerAddress, PhotonNetwork.PhotonServerSettings.ServerPort}));
                GUILayout.Label("AppId: " + PhotonNetwork.PhotonServerSettings.AppID);
                
                if (GUILayout.Button("Try Again", GUILayout.Width(100)))
                {
                    this.connectFailed = false;
                    PhotonNetwork.ConnectUsingSettings("1.0");
                }
            }

            return;
        }
	        
            GUI.skin.box.fontStyle = FontStyle.Bold;
	        GUI.Box(new Rect((Screen.width - 400) / 2, (Screen.height - 350) / 2, 400, 300), "Join or Create a Room");
	        GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 350) / 2, 400, 300));

	        GUILayout.Space(25);

	        // Player name
	        GUILayout.BeginHorizontal();
	        GUILayout.Label("Player name:", GUILayout.Width(100));
	        PhotonNetwork.playerName = GUILayout.TextField(PhotonNetwork.playerName);
	        GUILayout.Space(105);
	        if (GUI.changed)
	        {
	            // Save name
	            PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);
	        }
	        GUILayout.EndHorizontal();

	        GUILayout.Space(15);

	        // Join room by title
	        GUILayout.BeginHorizontal();
	        GUILayout.Label("Roomname:", GUILayout.Width(100));
	        this.roomName = GUILayout.TextField(this.roomName);
	        
	        if (GUILayout.Button("Create Room", GUILayout.Width(100)))
	        {
				PhotonNetwork.CreateRoom(this.roomName, true, true, 4);
	        }

	        GUILayout.EndHorizontal();

	        // Create a room (fails if exist!)
	        GUILayout.BeginHorizontal();
	        GUILayout.FlexibleSpace();
	        //this.roomName = GUILayout.TextField(this.roomName);
	        if (GUILayout.Button("Join Room", GUILayout.Width(100)))
	        {
				PhotonNetwork.JoinRoom(this.roomName);
	        }

	        GUILayout.EndHorizontal();


	        GUILayout.Space(15);

	        // Join random room
	        GUILayout.BeginHorizontal();

	        GUILayout.Label(PhotonNetwork.countOfPlayers + " users are online in " + PhotonNetwork.countOfRooms + " rooms.");
	        GUILayout.FlexibleSpace();
	        
			
			if (GUILayout.Button("Join Random", GUILayout.Width(100)))
	        {
	            PhotonNetwork.JoinRandomRoom();
	        }
			
	        

	        GUILayout.EndHorizontal();

	        GUILayout.Space(15);
	        if (PhotonNetwork.GetRoomList().Length == 0)
	        {
	            GUILayout.Label("Currently no games are available.");
	            GUILayout.Label("Rooms will be listed here, when they become available.");
	        }
	        else
	        {
	            GUILayout.Label(PhotonNetwork.GetRoomList() + " currently available. Join either:");

	            // Room listing: simply call GetRoomList: no need to fetch/poll whatever!
	            this.scrollPos = GUILayout.BeginScrollView(this.scrollPos);
	            foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList())
	            {
	                GUILayout.BeginHorizontal();
	                GUILayout.Label(roomInfo.name + " " + roomInfo.playerCount + "/" + roomInfo.maxPlayers);
	                if (GUILayout.Button("Join"))
	                {
	                    PhotonNetwork.JoinRoom(roomInfo.name);
	                }

	                GUILayout.EndHorizontal();
	            }

	            GUILayout.EndScrollView();
	        }

	        GUILayout.EndArea();
         */
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



