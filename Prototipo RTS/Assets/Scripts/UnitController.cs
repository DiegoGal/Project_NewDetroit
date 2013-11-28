using UnityEngine;
using System.Collections;

public class UnitController : MonoBehaviour
{
    private Color origColor;
    private Color selectColor = Color.yellow;

    private enum State
    {
        Iddle,	// reposo
        GoingTo
    }
    private State currentState;

    private bool selected;

    public float velocity = 5.0f;
    public Vector3 dirMovement;
    private Vector3 destiny;
    private int destinyThreshold;

    // Use this for initialization
    void Start ()
    {
        origColor = this.renderer.material.color;
        currentState = State.Iddle;

        selected = false;

        dirMovement = new Vector3();
        destiny = transform.position;
        destinyThreshold = 1;
    }

    // Update is called once per frame
    void Update ()
    {
        switch (currentState)
        {
            case State.Iddle:

                break;
            case State.GoingTo:
                Vector3 direction = destiny - transform.position;
                if (direction.magnitude >= destinyThreshold)
                {
                    transform.position += direction.normalized *
                        velocity * Time.deltaTime;
                    transform.position = new Vector3(transform.position.x, 1, transform.position.z);
                }
                else
                    currentState = State.Iddle;
                break;
        }
    }

    public void SetSelected ()
    {
        selected = true;
        this.renderer.material.color = selectColor;
    }

    public void SetDeselect()
    {
        selected = false;
        this.renderer.material.color = origColor;
    }

    public void SetSelected(bool selected)
    {
        if (selected)
            SetSelected();
        else
            SetDeselect();
    }

    public bool IsSelected ()
    {
        return selected;
    }

    public void ResetColor ()
    {
        this.renderer.material.color = origColor;
    }

    public void GoTo (Vector3 destiny)
    {
        this.destiny = destiny;
        currentState = State.GoingTo;
    }
}
