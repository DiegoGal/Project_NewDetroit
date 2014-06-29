using UnityEngine;
using System.Collections;

public class RobAnimatorVideo : MonoBehaviour {

    public float waitTimer = 2.5f;
    public float waitTimerAux;

	// Use this for initialization
	void Start ()
    {
        animation.Play("Iddle01");

        waitTimerAux = waitTimer;
	}
	
	// Update is called once per frame
	void Update ()
    {
        waitTimerAux -= Time.deltaTime;

        if (waitTimerAux <= 0.0f)
        {
            animation.CrossFade("Iddle02");
            animation.CrossFadeQueued("Iddle01");
            waitTimerAux = waitTimer;
        }
	}
}
