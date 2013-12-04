using UnityEngine;
using System.Collections;

public class CameraRTSController : MonoBehaviour
{
    private Vector3 initPosition;
    private Vector3 initRotation;

    public float scrollSpeed = 15.0f;
    public float scrollEdge = 0.01f;

    public float panSpeed = 10.0f;

    public Vector2 zoomRange = new Vector2(-24.0f, 5.0f);
    private float currentZoom = 0.0f;
    public float zoomSpeed = 1000.0f;
    public float zoomRotation = 1.0f;

    public float smooth = 1.5f; // The relative speed at which the camera will catch up.

    private Vector3 newPos;     // The position the camera is trying to reach.
    private Vector2 displaceVelocity = new Vector2(50.0f, 100.0f);

	// Use this for initialization
	void Start ()
    {
        initPosition = transform.position;
        initRotation = transform.eulerAngles;

        newPos = initPosition;
	}

    // This function is called every fixed framerate frame
    void LateUpdate ()
    {
        // move te camera at the same altitude
        // pan with the wheel button of the mouse preshed:
        if (Input.GetMouseButton(2))
        {
            transform.Translate
            (
                Vector3.right * Time.deltaTime * panSpeed *
                    (Input.mousePosition.x - Screen.width * 0.5f) / (Screen.width * 0.5f),
                Space.World
            );
            transform.Translate
            (
                Vector3.forward * Time.deltaTime * panSpeed *
                    (Input.mousePosition.y - Screen.height * 0.5f) / (Screen.height * 0.5f),
                Space.World
            );
        }
        else
        {
            if ( Input.GetKey(KeyCode.UpArrow) ||
                (Input.mousePosition.y >= Screen.height * (1 - scrollEdge)) )
            {
                transform.Translate(Vector3.forward * scrollSpeed * Time.deltaTime, Space.World);
            }
            else if ( Input.GetKey(KeyCode.DownArrow) ||
                (Input.mousePosition.y <= Screen.height * scrollEdge) )
            {
                transform.Translate(Vector3.forward * -scrollSpeed * Time.deltaTime, Space.World);
            }

            if ( Input.GetKey(KeyCode.RightArrow) ||
                (Input.mousePosition.x >= Screen.width * (1 - scrollEdge)) )
            {
                transform.Translate(Vector3.right * scrollSpeed * Time.deltaTime, Space.World);
            }
            else if (Input.GetKey(KeyCode.LeftArrow) ||
                (Input.mousePosition.x <= Screen.width * scrollEdge) )
            {
                transform.Translate(Vector3.right * -scrollSpeed * Time.deltaTime, Space.World);
            }
        }

        // Zoom in-out
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, zoomRange.x, zoomRange.y);

        transform.position = new Vector3
        (
            transform.position.x,
            transform.position.y - (transform.position.y - (initPosition.y + currentZoom)) * 0.1f,
            transform.position.z
        );
        transform.eulerAngles = new Vector3
        (
            transform.eulerAngles.x -
                (transform.eulerAngles.x - (initRotation.x + currentZoom * zoomRotation)) * 0.1f,
            transform.eulerAngles.y,
            transform.eulerAngles.z
        );

        //if (Input.GetKeyDown(KeyCode.UpArrow))
        //    newPos.z += displaceVelocity.y;
        //if (Input.GetKeyDown(KeyCode.DownArrow))
        //    newPos.z -= displaceVelocity.y;
        // Lerp the camera's position between it's current position and it's new position.
        //transform.position = Vector3.Lerp(transform.position, newPos, smooth * Time.deltaTime);
	}

} // class CameraRTSController
