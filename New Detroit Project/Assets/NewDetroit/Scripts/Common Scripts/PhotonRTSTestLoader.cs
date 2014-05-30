using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class PhotonRTSTestLoader : MonoBehaviour
{

    private bool connectFailed = false;

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
        if (String.IsNullOrEmpty(PhotonNetwork.playerName))
        {
            PhotonNetwork.playerName = "Guest" + Random.Range(1, 9999);
        }

        // if you wanted more debug out, turn this on:
        // PhotonNetwork.logLevel = NetworkLogLevel.Full;


    }

	// Use this for initialization
	void Start ()
    {
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
                GUILayout.Label(String.Format("Server: {0}:{1}", new object[] { PhotonNetwork.ServerAddress, PhotonNetwork.PhotonServerSettings.ServerPort }));
                GUILayout.Label("AppId: " + PhotonNetwork.PhotonServerSettings.AppID);

                if (GUILayout.Button("Try Again", GUILayout.Width(100)))
                {
                    this.connectFailed = false;
                    PhotonNetwork.ConnectUsingSettings("1.0");
                }
            }

            return;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
