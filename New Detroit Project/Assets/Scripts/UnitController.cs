﻿using UnityEngine;
using System.Collections;

public class UnitController : MonoBehaviour
{
	public int teamNumber;

    protected enum State
    {
        Iddle,	// reposo
        GoingTo,
        Harvesting,
        Attacking
    }
	protected State currentState = State.Iddle;
    private State lastState = State.Iddle;
	
    public float velocity = 5.0f;
    public float rotationVelocity = 10.0f;
    public Vector3 dirMovement = new Vector3();
    private Vector3 destiny = new Vector3();
    protected float destinyThreshold = 1.0f;

    // referencia a la posición de la base de la unidad
    protected Vector3 basePosition = new Vector3();

    // referencia a la base
    public BaseController baseController;

    // Use this for initialization
    void Start ()
    {
        if (destiny == Vector3.zero)
            destiny = transform.position;
    }

    // Update is called once per frame
    public virtual void Update ()
    {
        switch (currentState)
        {
            case State.Iddle:

                break;
            case State.GoingTo:
                //Vector3 direction = destiny - transform.position;
                Vector3 direction = new Vector3(destiny.x - transform.position.x, 0,
                    destiny.z - transform.position.z);
                if (direction.magnitude >= destinyThreshold)
                {
                    /*Quaternion qu = new Quaternion();
                    qu.SetLookRotation(direction, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, qu, Time.deltaTime * rotationVelocity);
                    transform.position += direction.normalized *
                        velocity * Time.deltaTime;*/

                    //transform.position = new Vector3(transform.position.x, 1, transform.position.z);
                    //transform.Translate(direction.normalized * velocity * Time.deltaTime);

                    //GetComponent<NavMeshAgent>().destination = destiny;
                }
                else
                    currentState = State.Iddle;
                break;
        }
    }

    public void SetBasePosition (Vector3 basePosition)
    {
        this.basePosition = basePosition;
    }

    public void SetArmyBase (BaseController baseController)
    {
        this.baseController = baseController;
    }

    public void GoTo (Vector3 destiny)
    {
        this.destiny = destiny;
        GetComponent<NavMeshAgent>().destination = destiny;
        currentState = State.GoingTo;
    }

    public virtual void RightClickOnSelected (Vector3 destiny, Transform destTransform)
    {
        GoTo(destiny);
    }

    // this method is called when a unit collides with the army base
    public virtual void ArrivedToBase ()
    {

    }

}