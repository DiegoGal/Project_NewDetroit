using UnityEngine;
using System.Collections;

public class CMissileVisionCapsule : Photon.MonoBehaviour
{

    private GameObject owner;
    private int damage;
    public GameObject splash;
    private float destroyTime;
    private float timer = 0.0f;
    private bool thrown = false;
    private float rotation = 0.9f;

	// Use this for initialization
	void Start () 
    {
        timer = Time.time;
	}
	
	// Update is called once per frame
	void Update () 
    {
        //transform.parent.transform.Rotate(rotation, 0, 0);  
        transform.Rotate(rotation, 0, 0);  
	}

    void OnTriggerEnter(Collider other)
    {
        if (!thrown && other.gameObject.name != "TowerVisionSphere" && other.gameObject.name != "MissileSplash"
            && other.gameObject.name != "ShockwaveArtilleryHeavy")
        {
            GameObject newSplash = PhotonNetwork.Instantiate
                (
                    "ShockwaveArtilleryHeavy",
                    transform.position,
                    new Quaternion(),
                    0
                ) as GameObject;
            newSplash.transform.name = "ShockwaveArtilleryHeavy";
            newSplash.GetComponent<ObjectAttack>().SetDamage(damage);
            newSplash.GetComponent<ObjectAttack>().SetOwner(owner);
            newSplash.GetComponent<ObjectAttack>().typeOfObject = 1;
            
            newSplash.AddComponent<Rigidbody>();
            newSplash.GetComponent<Rigidbody>().useGravity = false;
            
            //gameObject.SetActive(false);
            
            rigidbody.isKinematic = true;
            //Destroy(newSplash, 1.2f);
            thrown = true;
            PhotonNetwork.Destroy(gameObject);
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
