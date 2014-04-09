using System;
using UnityEngine;

public class GameManagerRTS : Photon.MonoBehaviour {

    public GameObject fogPlane; //The fog plane in the scene

    public GameObject redBase; // Rob Base

    public GameObject blueBase; // Rob Base

    public GameObject redArmyManager;

    public GameObject blueArmyManager;

    public static bool joinned = false;

    public void Awake()
    {
        // in case we started this demo with the wrong scene being active, simply load the menu scene
        if (!PhotonNetwork.connected)
        {
            Application.LoadLevel(MenuRTS.SceneNameMenu);
            return;
        }
        else
        {
            GetComponent<AudioSource>().Play();
            Camera.main.GetComponent<CameraMOBAController>().enabled = false;
            if (!joinned)
            {
                Debug.Log("First joinned");
                Camera.main.transform.position = new Vector3(102, 26, -2);
                redBase.GetComponent<CSelectable>().enabled = true;
                redBase.GetComponent<BaseController>().enabled = true;
                redBase.GetComponent<FogOfWarUnit>().enabled = true;
                blueBase.GetComponent<CSelectable>().enabled = false;
                blueBase.GetComponent<BaseController>().enabled = false;
                blueBase.GetComponent<FogOfWarUnit>().enabled = false;
                DestroyObject(blueArmyManager);
            }
            else
            {
                Debug.Log("Second joinned");
                Camera.main.transform.position = new Vector3(66, 26, -30);
                redBase.GetComponent<CSelectable>().enabled = false;
                redBase.GetComponent<BaseController>().enabled = false;
                redBase.GetComponent<FogOfWarUnit>().enabled = false;
                blueBase.GetComponent<CSelectable>().enabled = true;
                blueBase.GetComponent<BaseController>().enabled = true;
                blueBase.GetComponent<FogOfWarUnit>().enabled = true;
                DestroyObject(redArmyManager);
            }
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
        Application.LoadLevel(MenuRTS.SceneNameMenu);
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("OnDisconnectedFromPhoton");

        // back to main menu        
        Application.LoadLevel(MenuRTS.SceneNameMenu);
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
        Application.LoadLevel(MenuRTS.SceneNameMenu);
    }
}
