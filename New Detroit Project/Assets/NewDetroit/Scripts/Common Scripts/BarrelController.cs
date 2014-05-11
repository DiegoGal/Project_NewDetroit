using UnityEngine;
using System.Collections;

public class BarrelController : MonoBehaviour
{

    public float velocity = 3.0f;
    public float amplitude = 0.2f;
    private float valueY, initialY;
    private float acum = 0.0f;
    public float rotationVelocity = 1.5f;

    // Use this for initialization
    void Start ()
    {
        initialY = transform.position.y;
    }
	
    // Update is called once per frame
    void Update ()
    {
        // oscillatory motion in the y axis
        acum += Time.deltaTime;
        valueY = initialY + Mathf.Cos(acum * velocity) * amplitude;
        transform.position = new Vector3
        (
            transform.position.x,
            valueY,
            transform.position.z
        );

        float rotation = Time.deltaTime * rotationVelocity;
        transform.Rotate
        (
            rotation,
            rotation,
            rotation
        );
    }
}
