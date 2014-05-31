using UnityEngine;
using System.Collections;

public class GameManagerJacob : MonoBehaviour {


    public Vector3 cameraPositionRTS;
    public Vector3 cameraRotationRTS;
    public bool onGUI = true;
    public GameObject menu;
    public GameObject RobRender;
    public GameObject armyManager;

	// Use this for initialization
	void Start () {
        Camera.mainCamera.transform.position = new Vector3(75.37802f, 10.55257f, -106.6093f);
        Camera.mainCamera.transform.rotation = new Quaternion(27f, 0f, 0f,180f);
	}

    public void OnGUI()
    {
        if (GUI.Button(new Rect(0, 40, 120, 40), "", GUIStyle.none))
        {
            Destroy(menu);
            GameObject instRob = (GameObject)Instantiate(RobRender, new Vector3(95f,3f,-116f),Quaternion.identity);
            // Activar cosas de rob
            instRob.GetComponent<ThirdPersonCamera>().enabled = true;
            instRob.GetComponent<ThirdPersonNetwork>().enabled = false;
            instRob.GetComponent<OrcController>().isMine = true;
        }

        //GUILayout.EndHorizontal();

        if (GUI.Button(new Rect(0, 90, 120, 40), "", GUIStyle.none))
        {
            Destroy(menu);
            armyManager.active = true;
            Camera.mainCamera.transform.position = cameraPositionRTS;
            Camera.mainCamera.transform.rotation = new Quaternion(45f, 0f, 0f, 180f);
        }

        if (GUI.Button(new Rect(Screen.width - 120, 180, 120, 40), "", GUIStyle.none))
        {
            Application.Quit();
        }
    }

	// Update is called once per frame
	void Update () {
	
	}

}
