using UnityEngine;
using System.Collections;

public class LocalGameManagerOffline : MonoBehaviour
{

    public static int joinedId = 2;

    public GameObject army0, army1;

    public GameObject redBase;
    public GameObject blueBase;
    public Transform redArmyCameraPosition;
    public Transform blueArmyCameraPosition;
    public GameObject navmeshColliders;
    public GameObject prefabRobRender;
    public GameObject prefabSkelterbot;
    
	public GameObject robRenderEnemies;
    public GameObject robotEnemies;

    public void Awake()
    {

        if (joinedId == -1)
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
                	heroInst = (GameObject)Instantiate(prefabRobRender, redArmyCameraPosition.position + Vector3.down * 24f + Vector3.forward * 16, new Quaternion());
                    heroInst.GetComponent<ThirdPersonCamera>().cameraTransform = Camera.main.transform;
                    
					heroInst.GetComponent<CharacterController>().enabled = true;
					heroInst.GetComponent<ThirdPersonCamera>().enabled = true;
					heroInst.GetComponent<FogOfWarUnit>().enabled = false;
                    //heroInst.GetComponent<OrcController>().enabled = true;
					heroInst.GetComponent<Animation>().enabled = true;
					heroInst.GetComponent<NavMeshObstacle>().enabled = true;
					heroInst.GetComponent<CStateUnit>().enabled = true;
					heroInst.GetComponent<HeroNetwork>().enabled = false;
					heroInst.GetComponent<CTeamUnit>().enabled = true;

                    Camera.main.GetComponent<CameraRTSController>().enabled = false;
                    Camera.main.GetComponent<CameraMOBAController>().enabled = true;
                    Camera.main.GetComponent<CameraMOBAController>().hero = heroInst;
					navmeshColliders.SetActive(true);
                    Destroy(army1);
                    Destroy(army0);
					//Destroy(robRenderEnemies);
                    break;
                case 1: // Skelterbot
                    heroInst = (GameObject)Instantiate(prefabSkelterbot, blueArmyCameraPosition.position + Vector3.down * 24f, new Quaternion());
                    heroInst.GetComponent<ThirdPersonCamera>().cameraTransform = Camera.main.transform;
                    
					heroInst.GetComponent<CharacterController>().enabled = true;
					heroInst.GetComponent<ThirdPersonCamera>().enabled = true;
					heroInst.GetComponent<FogOfWarUnit>().enabled = false;
                    //heroInst.GetComponent<RobotController>().enabled = true;
					heroInst.GetComponent<Animation>().enabled = true;
					heroInst.GetComponent<NavMeshObstacle>().enabled = true;
					heroInst.GetComponent<CStateUnit>().enabled = true;
					heroInst.GetComponent<HeroNetwork>().enabled = false;
					heroInst.GetComponent<CTeamUnit>().enabled = true;

                    Camera.main.GetComponent<CameraRTSController>().enabled = false;
                    Camera.main.GetComponent<CameraMOBAController>().enabled = true;
                    Camera.main.GetComponent<CameraMOBAController>().hero = heroInst;
					navmeshColliders.SetActive(false);
                    Destroy(army1);
                    Destroy(army0);
					//Destroy(robotEnemies);
                    break;
                case 2: // Rob Army
                    Camera.main.GetComponent<CameraRTSController>().enabled = true;
                    Camera.main.GetComponent<CameraMOBAController>().enabled = false;
                    Camera.main.transform.position = redArmyCameraPosition.position;

                    redBase.GetComponent<CSelectable>().enabled = true;
                    redBase.GetComponent<BaseController>().enabled = true;
                    redBase.GetComponent<FogOfWarUnit>().enabled = true;
                    blueBase.GetComponent<CSelectable>().enabled = false;
                    blueBase.GetComponent<BaseController>().enabled = false;
                    blueBase.GetComponent<FogOfWarUnit>().enabled = false;
                    navmeshColliders.SetActive(true);
                    Destroy(army1);
					//Destroy(robRenderEnemies);
                    break;
                case 3: // Skelter Army
                    Camera.main.GetComponent<CameraRTSController>().enabled = true;
                    Camera.main.GetComponent<CameraMOBAController>().enabled = false;
                    Camera.main.transform.position = blueArmyCameraPosition.position;

                    redBase.GetComponent<CSelectable>().enabled = false;
                    redBase.GetComponent<BaseController>().enabled = false;
                    redBase.GetComponent<FogOfWarUnit>().enabled = false;
                    blueBase.GetComponent<CSelectable>().enabled = true;
                    blueBase.GetComponent<BaseController>().enabled = true;
                    blueBase.GetComponent<FogOfWarUnit>().enabled = true;
					navmeshColliders.SetActive(true);
                    Destroy(army0);
					//Destroy(robotEnemies);
                    break;
                default:
                    Debug.Log("Error at selection");
                    Destroy(army0);
                    Destroy(army1);
                    break;
            }
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 2 - 40, 0, 80, 20), "Exit room"))
        {
            Application.LoadLevel(NetworkController.SceneNameMenu);
        }
    }
}
