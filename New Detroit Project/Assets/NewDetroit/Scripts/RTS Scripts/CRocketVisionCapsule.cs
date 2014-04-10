using UnityEngine;
using System.Collections;

public class CRocketVisionCapsule : MonoBehaviour {

    private GameObject owner;
    private int damage;
    public GameObject splash;
    private float destroyTime;
    private float timer = 0.0f;
    private bool thrown = false;
	// Use this for initialization
	void Start () 
    {
        timer = Time.time;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!thrown && (Time.time - timer) >= destroyTime)
        {
            GameObject newSplash = Instantiate
                (
                    splash,
                    transform.position,
                    new Quaternion()
                ) as GameObject;
            newSplash.transform.name = "RocketSplash";
            newSplash.GetComponent<RocketAttack>().SetDamage(damage);
            newSplash.GetComponent<RocketAttack>().SetOwner(owner);
            newSplash.AddComponent<Rigidbody>();
            newSplash.GetComponent<Rigidbody>().useGravity = false;
            
            Destroy(transform.parent.gameObject, 0.5f);
            transform.parent.rigidbody.isKinematic = true;
            Destroy(newSplash, 1.2f);
            //transform.parent.gameObject.SetActive(false);
            thrown = true;
        }
	}

    public void SetOwner(GameObject owner)
    {
        this.owner = owner;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public void SetDestroyTime(float destroyTime)
    {
        this.destroyTime = destroyTime;
    }
}
