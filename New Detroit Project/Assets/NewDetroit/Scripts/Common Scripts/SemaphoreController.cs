using UnityEngine;
using System.Collections;

public class SemaphoreController : MonoBehaviour
{

    public Light redLight, amberLight, greenLight;
    public enum State
    {
        Red,
        Amber,
        Green
    };
    public State currentState = State.Red;

    public float timeRed = 2.0f;
    public float timeAmber = 1.0f;
    public float timeGreen = 2.0f;
    private float currentTimer;
    
	// Use this for initialization
	void Start ()
    {
        currentTimer = timeRed;

        // get the lights references
        redLight =   transform.FindChild("Spotlight red").GetComponent<Light>();
        amberLight = transform.FindChild("Spotlight amber").GetComponent<Light>();
        greenLight = transform.FindChild("Spotlight green").GetComponent<Light>();

        // turn off all the lights
        redLight.enabled = true;
        amberLight.enabled = false;
        greenLight.enabled = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        currentTimer -= Time.deltaTime;
        if (currentTimer <= 0.0f)
        {
            switch (currentState)
            {
                case State.Red:
                    currentState = State.Green;
                    redLight.enabled = false;
                    greenLight.enabled = true;
                    currentTimer = timeGreen;
                    break;
                case State.Amber:
                    currentState = State.Red;
                    amberLight.enabled = false;
                    redLight.enabled = true;
                    currentTimer = timeRed;
                    break;
                case State.Green:
                    currentState = State.Amber;
                    greenLight.enabled = false;
                    amberLight.enabled = true;
                    currentTimer = timeAmber;
                    break;
            }
        }
	}
}
