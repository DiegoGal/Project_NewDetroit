using UnityEngine;
using System.Collections;

public class CMineralLightBeat : MonoBehaviour {

    public float minIntensity = 1.5f;
    public float maxIntensity = 3.0f;
    public float growInt = 2.0f;

    //private bool growing = true;

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
        /*if (growing)
        {
            transform.light.intensity += growInt;
            if (transform.light.intensity >= maxIntensity)
                growing = false;
        }
        else
        {
            transform.light.intensity -= growInt;
            if (transform.light.intensity <= minIntensity)
                growing = true;
        }*/
        transform.light.intensity = ((Mathf.Cos(Time.timeSinceLevelLoad * growInt) * 0.5f) + 0.5f) *
            maxIntensity + minIntensity;
	}
}
