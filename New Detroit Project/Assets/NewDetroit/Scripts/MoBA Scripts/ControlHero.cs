using UnityEngine;
using System.Collections;

public class ControlHero : MonoBehaviour 
{
    private Vector3 mousePosition;


    //----------------------------------------------------------------------------------------------


    public void Start()
    {
        mousePosition = Input.mousePosition;
    }


    //----------------------------------------------------------------------------------------------

    
    public bool IsWalking() 
    {
        return (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) && !Input.GetKey(KeyCode.LeftShift);
    }

    public bool IsRunning()
    {
        return (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) && Input.GetKey(KeyCode.LeftShift);
    }

    public bool IsAttacking()
    {
        return Input.GetButton("Fire1");
    }

    public bool IsSkilling()
    {
        return Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Alpha3);
    }

    public Vector3 MoveDirection()
    {
        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) direction += transform.forward;
        if (Input.GetKey(KeyCode.A)) direction += transform.right * -1;
        if (Input.GetKey(KeyCode.S)) direction += transform.forward * -1;
        if (Input.GetKey(KeyCode.D)) direction += transform.right;
        return direction.normalized;
    }

    public float ValueRotation()
    {
        if (Screen.lockCursor)
        {
            Screen.lockCursor = false;
            mousePosition = Input.mousePosition;
        }
        if (Screen.width - Input.mousePosition.x < 10 || Input.mousePosition.x < 10)
        {
            Screen.lockCursor = true;
            return 0;
        }

        float value = Input.mousePosition.x - mousePosition.x;
        mousePosition = Input.mousePosition;
        
        return value / 4;
    }

    public int GetSkill()
    {
        if (Input.GetKey(KeyCode.LeftAlt)) return -1;

        if (Input.GetKey(KeyCode.Alpha1)) return 1;
        if (Input.GetKey(KeyCode.Alpha2)) return 2;
        if (Input.GetKey(KeyCode.Alpha3)) return 3;

        return -1;
    }

    public int UnlockSkill()
    {
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.Alpha1)) return 1;
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.Alpha2)) return 2;
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.Alpha3)) return 3;

        return -1;
    }

}
