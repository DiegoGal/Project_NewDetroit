using UnityEngine;
using System.Collections;

public class CFireballVisionSphere : MonoBehaviour {

    private GameObject owner;
    private int damage;
    public GameObject splash;
    private float destroyTime;
    private float timer = 0.0f;

	// Use this for initialization
	void Start () 
    {
        timer = Time.time;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if ((Time.time - timer) <= destroyTime)
        {
            GameObject newSplash = Instantiate
                (
                    splash,
                    transform.position,
                    new Quaternion()
                ) as GameObject;
            newSplash.transform.name = "FireballSplash";
            newSplash.GetComponent<FireballAttack>().SetDamage(damage);
            newSplash.GetComponent<FireballAttack>().SetOwner(owner);

            Destroy(transform.parent.gameObject, 0.2f);
            transform.parent.rigidbody.isKinematic = true;
            Destroy(newSplash, 1.2f);
            transform.parent.gameObject.SetActive(false);
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
