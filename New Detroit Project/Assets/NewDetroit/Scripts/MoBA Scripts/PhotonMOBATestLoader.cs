using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class PhotonMOBATestLoader : MonoBehaviour
{

    private string roomName = "myRoom";

    private bool connectFailed = false;

    public static int joinedId = 0;


    public void Awake()
    {
		Destroy (GameObject.Find ("Army Manager Team0"));
		Destroy (GameObject.Find ("Army Manager Team1"));

        if (!PhotonNetwork.connected)
        {
            Application.LoadLevel(MenuDetroitMapa.SceneNameMenu);
            return;
        }
        else
        {
            if (joinedId == 0)
            {
				GameObject heroInst = PhotonNetwork.Instantiate("Skelterbot", Camera.main.transform.position + Vector3.down * 24f, new Quaternion(), 0);
//				GameObject heroInst = PhotonNetwork.Instantiate("Rob Render", Camera.main.transform.position + Vector3.down * 24f, new Quaternion(), 0);
				heroInst.GetComponent<ThirdPersonCamera>().cameraTransform = Camera.main.transform;

				Camera.main.GetComponent<CameraRTSController>().enabled = false;
				Camera.main.GetComponent<CameraMOBAController>().enabled = true;
				Camera.main.GetComponent<CameraMOBAController>().heroe = heroInst.GetComponent<HeroeController>();
            }
            else
            {
				GameObject heroInst = PhotonNetwork.Instantiate("Skelterbot", Camera.main.transform.position + Vector3.down * 24f, new Quaternion(), 0);
//				GameObject heroInst = PhotonNetwork.Instantiate("Rob Render", Camera.main.transform.position + Vector3.down * 24f, new Quaternion(), 0);
				heroInst.GetComponent<ThirdPersonCamera>().cameraTransform = Camera.main.transform;
				
				Camera.main.GetComponent<CameraRTSController>().enabled = false;
				Camera.main.GetComponent<CameraMOBAController>().enabled = true;
				Camera.main.GetComponent<CameraMOBAController>().heroe = heroInst.GetComponent<HeroeController>();
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
