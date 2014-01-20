using UnityEngine;
using System.Collections;

public class CoinController : MonoBehaviour {

	public float rotationVelocityX = 100.0f;
	public float rotationVelocityY = 60.0f;
	public float rotationVelocityZ = 20.0f;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.Rotate
		(
			Time.deltaTime * rotationVelocityX,
			Time.deltaTime * rotationVelocityY,
			Time.deltaTime * rotationVelocityZ
		);
		//transform.Rotate(Vector3.right, rotationVelocityZ * Time.deltaTime);
		//transform.Rotate(Vector3.forward, rotationVelocityX * Time.deltaTime);
		//transform.Rotate(Vector3.up, rotationVelocityY * Time.deltaTime);
	}
}
