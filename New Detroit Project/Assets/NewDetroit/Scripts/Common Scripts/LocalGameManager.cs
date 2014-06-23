using UnityEngine;
using System.Collections;

public class LocalGameManager : MonoBehaviour {

    public static int joinedId = -1;

    public ArmyController army0, army1;

    public GameObject redBase;
    public GameObject blueBase;
    public GameObject redArmyManager;
    public GameObject blueArmyManager;
    public Transform redArmyCameraPosition;
    public Transform blueArmyCameraPosition;
    public GameObject navmeshColliders;

    public void Awake()
    {
        if (!PhotonNetwork.connected)
        {
            Application.LoadLevel(NetworkController.SceneNameMenu);
            return;
        }
        else
        {
            GameObject heroInst;
            switch (joinedId)
            {
                case 0: // Rob Render
                    heroInst = PhotonNetwork.Instantiate("Rob Render", redArmyCameraPosition.position + Vector3.down * 24f + Vector3.forward * 16f, new Quaternion(), 0);
                    heroInst.GetComponent<ThirdPersonCamera>().cameraTransform = Camera.main.transform;
                    heroInst.GetComponent<HeroNetwork>().enabled = true;
                    Camera.main.GetComponent<CameraRTSController>().enabled = false;
                    Camera.main.GetComponent<CameraMOBAController>().enabled = true;
                    Camera.main.GetComponent<CameraMOBAController>().heroe = heroInst.GetComponent<HeroeController>();
                    Destroy(army1);
                    Destroy(army0);
                break;
                case 1: // Skelterbot
                    heroInst = PhotonNetwork.Instantiate("Skelterbot", blueArmyCameraPosition.position + Vector3.down * 24f, new Quaternion(), 0);
                    heroInst.GetComponent<ThirdPersonCamera>().cameraTransform = Camera.main.transform;
                    heroInst.GetComponent<HeroNetwork>().enabled = true;
                    Camera.main.GetComponent<CameraRTSController>().enabled = false;
                    Camera.main.GetComponent<CameraMOBAController>().enabled = true;
                    Camera.main.GetComponent<CameraMOBAController>().heroe = heroInst.GetComponent<HeroeController>();
                    Destroy(army1);
                    Destroy(army0);
                break;
                case 2: // Rob Army

                    Camera.main.transform.position = redArmyCameraPosition.position;
                    redBase.GetComponent<CSelectable>().enabled = true;
                    redBase.GetComponent<BaseController>().enabled = true;
                    redBase.GetComponent<FogOfWarUnit>().enabled = true;
                    blueBase.GetComponent<CSelectable>().enabled = false;
                    blueBase.GetComponent<BaseController>().enabled = false;
                    blueBase.GetComponent<FogOfWarUnit>().enabled = false;
                    navmeshColliders.SetActive(false);
                    Destroy(army1);
                break;
                case 3: // Skelter Army
                Camera.main.transform.position = blueArmyCameraPosition.position;
                    redBase.GetComponent<CSelectable>().enabled = false;
                    redBase.GetComponent<BaseController>().enabled = false;
                    redBase.GetComponent<FogOfWarUnit>().enabled = false;
                    blueBase.GetComponent<CSelectable>().enabled = true;
                    blueBase.GetComponent<BaseController>().enabled = true;
                    blueBase.GetComponent<FogOfWarUnit>().enabled = true;
                    navmeshColliders.SetActive(false);
                	Destroy(army0);
                break;
                default:
                    Debug.Log("Error at selection");
                    Destroy(army0);
                    Destroy(army1);
                    break;
            }           
        }

    }

    // Use this for initialization
    void Start()
    {
        Debug.Log(PhotonNetwork.GetRoomList().Length);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width/2 - 40,0,80,20),"Exit room"))
        {
            PhotonNetwork.LeaveRoom();
            Application.LoadLevel(NetworkController.SceneNameMenu);
        }
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
        Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters);
    }
}
