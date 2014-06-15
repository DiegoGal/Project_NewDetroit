using UnityEngine;
using System.Collections;

public class CGrenadeVisionSphere : Photon.MonoBehaviour {

    private GameObject owner;
    private int damage;
    public GameObject splash;
    private float destroyTime, destroyTimeAcum = 0;
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
        GameObject newSplash = null;
        if (!thrown && (Time.time - timer) >= destroyTime)
        {
            newSplash = PhotonNetwork.Instantiate
            (
                "ShockwaveEngineer",
                transform.position,
                new Quaternion(),
                0
            ) as GameObject;
            newSplash.transform.name = "ShockwaveEngineer";
            newSplash.GetComponent<GrenadeAttack>().SetDamage(damage);
            newSplash.GetComponent<GrenadeAttack>().SetOwner(owner);
            newSplash.AddComponent<Rigidbody>();
            newSplash.GetComponent<Rigidbody>().useGravity = false;

            //Destroy(transform.parent.gameObject, 0.5f);
            //transform.parent.rigidbody.isKinematic = true;

            //Destroy(gameObject, 0.5f);
            rigidbody.isKinematic = true;

            //Destroy(newSplash, 1.2f);
            //transform.parent.gameObject.SetActive(false);
            thrown = true;
        }
        else if (!thrown)
        {
            transform.Rotate(new Vector3 (8.0f, 15.0f, 3.0f));
        }
        
        destroyTimeAcum += Time.deltaTime;
        if (destroyTimeAcum >= destroyTime) 
            PhotonNetwork.Destroy(gameObject);
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
