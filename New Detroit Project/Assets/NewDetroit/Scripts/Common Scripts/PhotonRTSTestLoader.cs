﻿using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class PhotonRTSTestLoader : MonoBehaviour
{

    private string roomName = "myRoom";

    private bool connectFailed = false;

    public static int joinedId = 0;

    public ArmyController army0, army1;

    public GameObject RedBase;
    public GameObject BlueBase;

    public void Awake()
    {

        if (!PhotonNetwork.connected)
        {
            Application.LoadLevel(MenuDetroitMapa.SceneNameMenu);
            return;
        }
        else
        {
            if (joinedId == 0)
            {
                BlueBase.GetComponent<CSelectable>().enabled = false;
                BlueBase.GetComponent<BaseController>().enabled = false;
                Destroy(army1);
                Debug.Log(PhotonNetwork.GetRoomList().Length);
            }
            else
            {

                RedBase.GetComponent<CSelectable>().enabled = false;
                RedBase.GetComponent<BaseController>().enabled = false;
                Destroy(army0);
            }
        }

    }

	// Use this for initialization
	void Start ()
    {
        Debug.Log(PhotonNetwork.GetRoomList().Length);
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
    }

    public void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
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