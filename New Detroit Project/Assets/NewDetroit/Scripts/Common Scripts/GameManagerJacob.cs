using UnityEngine;
using System.Collections;

public class GameManagerJacob : MonoBehaviour {


    public Vector3 cameraPositionRTS;
    public Vector3 cameraPositionInitial;

    public bool onGUI = true;
    public GameObject menu;
    public GameObject RobRender;
	public GameObject SkelterBot;
    public GameObject armyManager;
    public Camera cameraRTS;
    private float propWidth, propHeight;

	// Use this for initialization
	void Start () {
        Camera.mainCamera.transform.position = cameraPositionInitial;
        //Camera.mainCamera.transform.rotation = transform.Rotate(cameraRotationInitial);

        //1024 x 512
        propWidth = (float)Screen.width / 1024;
        propHeight = (float)Screen.height / 512;
	}

    public void OnGUI()
    {
        if (onGUI)
        {
            if (GUI.Button(new Rect(propWidth * 35, propHeight * 70, propWidth * 250, propHeight * 60), "", GUIStyle.none))
            {
                Destroy(menu);
                /*GameObject instRob = (GameObject)Instantiate(RobRender, new Vector3(95f, 3f, -116f), Quaternion.identity);
                // Activar cosas de rob
                instRob.GetComponent<ThirdPersonCamera>().enabled = true;
                instRob.GetComponent<ThirdPersonNetwork>().enabled = false;
                instRob.GetComponent<OrcController>().isMine = true;*/

				GameObject skelterBot = (GameObject)Instantiate(SkelterBot, new Vector3(95f, 3f, -116f), Quaternion.identity);
				// Activar cosas de rob
				skelterBot.GetComponent<ThirdPersonCamera>().enabled = true;
				skelterBot.GetComponent<ThirdPersonNetwork>().enabled = false;
				skelterBot.GetComponent<RobotController>().isMine = true;
				
				Camera.mainCamera.GetComponent<CameraRTSController>().enabled = false;
                CameraMOBAController camera = Camera.mainCamera.GetComponent<CameraMOBAController>();
                camera.enabled = true;
				camera.heroe = skelterBot.GetComponent<HeroeController>();
                onGUI = false;
            }

            //GUILayout.EndHorizontal();

            if (GUI.Button(new Rect(propWidth * 35, propHeight * 160, propWidth * 250, propHeight * 60), "", GUIStyle.none))
            {
                Destroy(menu);
                armyManager.active = true;
                cameraRTS.active = true;
                Camera.mainCamera.active = false;
                Camera.SetupCurrent(cameraRTS);
                onGUI = false;
            }

            if (GUI.Button(new Rect(propWidth * 735, propHeight * 325, propWidth * 250, propHeight * 60), "", GUIStyle.none))
            {
                Application.Quit();
            }
        }
    }

	// Update is called once per frame
	void Update () {
	
	}

}
