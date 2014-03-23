// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkerInGame.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using System;
using UnityEngine;

public class GameManager : Photon.MonoBehaviour
{
    public Transform playerPrefab;
		
	private String[] selStrings = {"Rob Render","Skelterbot","Rob Army", "Skelter Army"};
	
	private int selInt = 0;

	enum state {selecting, playing}
	private state GUIState=state.selecting;

	public GameObject fogPlane; //The fog plane in the scene

	public GameObject redBase; // Rob Base

	public GameObject redArmyManager;

    public void Awake()
    {
        // in case we started this demo with the wrong scene being active, simply load the menu scene
        if (!PhotonNetwork.connected)
        {
            Application.LoadLevel(MenuDetroit.SceneNameMenu);
            return;
        }
	
    }

    public void OnGUI()
    {
		GUILayout.BeginHorizontal();
		GUILayout.Space(25);
        if (GUILayout.Button("Return to Lobby"))
        {
            PhotonNetwork.LeaveRoom();  // we will load the menu level when we successfully left the room
        }
		GUILayout.EndHorizontal();

		switch (GUIState)
		{
		case state.selecting:
			GUI.Box(new Rect((Screen.width - 400) / 2, (Screen.height - 350) / 2, 400, 300), "Select your rol:");
			GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 350) / 2, 400, 300));
			
			GUILayout.Space(25);
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Play as:", GUILayout.Width(100));
			GUILayout.EndHorizontal();
			
			GUILayout.Space(15);
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Team RED:", GUILayout.Width(100));
			GUILayout.Space(25);
			GUILayout.Label("Team BLUE:", GUILayout.Width(100));
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			selInt = GUILayout.SelectionGrid (selInt, selStrings, 2);
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Play", GUILayout.Width(100)))
			{
				if (selInt==0) // Instantiate ROB RENDER
				{
					GameObject heroe = PhotonNetwork.Instantiate("Rob Render", transform.position, Quaternion.identity, 0);
					Camera.main.GetComponent<CameraMOBAController>().heroe = heroe.GetComponent<HeroeController>();
					DestroyObject (fogPlane);
					DestroyObject (redArmyManager);
					Camera.main.GetComponent<CameraRTSController>().enabled=false;
					redBase.GetComponent<CSelectable>().enabled=false;
					redBase.GetComponent<BaseController>().enabled=false;
					redBase.GetComponent<FogOfWarUnit>().enabled=true;
					heroe.GetComponent<HeroeController>().enabled = true;
				}
				if (selInt==1) // Instantiate SKELTERBOT
				{
					GameObject heroe = PhotonNetwork.Instantiate("Rob Render", transform.position, Quaternion.identity, 0);
					Camera.main.GetComponent<CameraMOBAController>().heroe = heroe.GetComponent<HeroeController>();
					DestroyObject(fogPlane);
					DestroyObject (redArmyManager);
					Camera.main.GetComponent<CameraRTSController>().enabled=false;
					redBase.GetComponent<CSelectable>().enabled=false;
					redBase.GetComponent<BaseController>().enabled=false;
					redBase.GetComponent<FogOfWarUnit>().enabled=true;
                    heroe.GetComponent<HeroeController>().enabled = true;
				}
				if (selInt==2) // Rob Army
				{
					Camera.main.GetComponent<CameraMOBAController>().enabled = false;
					redBase.GetComponent<CSelectable>().enabled=true;
					redBase.GetComponent<BaseController>().enabled=true;
					redBase.GetComponent<FogOfWarUnit>().enabled=true;
				}
				if (selInt==3) // Skelter Army
				{
					Camera.main.GetComponent<CameraMOBAController>().enabled = false;
					redBase.GetComponent<CSelectable>().enabled=true;
					redBase.GetComponent<BaseController>().enabled=true;
					redBase.GetComponent<FogOfWarUnit>().enabled=true;
				}
				GUIState=state.playing;
			}
			GUILayout.EndArea();
			break;
		}
    }

    public void OnMasterClientSwitched(PhotonPlayer player)
    {
        Debug.Log("OnMasterClientSwitched: " + player);

        string message;
        InRoomChat chatComponent = GetComponent<InRoomChat>();  // if we find a InRoomChat component, we print out a short message

        if (chatComponent != null)
        {
            // to check if this client is the new master...
            if (player.isLocal)
            {
                message = "You are Master Client now.";
            }
            else
            {
                message = player.name + " is Master Client now.";
            }


            chatComponent.AddLine(message); // the Chat method is a RPC. as we don't want to send an RPC and neither create a PhotonMessageInfo, lets call AddLine()
        }
    }

    public void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom (local)");
        
        // back to main menu        
        Application.LoadLevel(MenuDetroit.SceneNameMenu);
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("OnDisconnectedFromPhoton");

        // back to main menu        
        Application.LoadLevel(MenuDetroit.SceneNameMenu);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("OnPhotonInstantiate " + info.sender);    // you could use this info to store this or react
    }

    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        Debug.Log("OnPhotonPlayerConnected: " + player);
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log("OnPlayerDisconneced: " + player);
    }

    public void OnFailedToConnectToPhoton()
    {
        Debug.Log("OnFailedToConnectToPhoton");

        // back to main menu        
        Application.LoadLevel(MenuDetroit.SceneNameMenu);
    }
}
