using UnityEngine;
using System.Collections;

public class CFireballVisionSphere : MonoBehaviour {

    private GameObject owner;
    private int damage;
    private bool thrown;
    public GameObject splash;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void SetThrown(bool thrown)
    {
        this.thrown = thrown;
    }

    public void SetOwner(GameObject owner)
    {
        this.owner = owner;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    void OnTriggerEnter(Collider other)
    {
        if (thrown)
        {
            if (other.gameObject.name != owner.name && other.gameObject.name != "TowerVisionSphere"
            && other.gameObject.name != "VisionSphere" && other.gameObject.name != "FireballSplash")
            {
                GameObject newSplash = Instantiate
                (
                    splash,
                    transform.position,
                    new Quaternion()
                ) as GameObject;
                newSplash.transform.name = "FireballSplash";
                newSplash.transform.rotation = other.transform.rotation;
                newSplash.GetComponent<FireballAttack>().SetDamage(damage);
                newSplash.GetComponent<FireballAttack>().SetOwner(owner);

                thrown = false;
                Destroy(transform.parent.gameObject, 1.2f);
                transform.parent.rigidbody.isKinematic = true;
                Destroy(newSplash, 1.2f);
                transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
