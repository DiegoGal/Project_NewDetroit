using UnityEngine;
using System.Collections;

public class BaseNetwork : Photon.MonoBehaviour
{

    private CSelectable selectableScript;
    private BaseController baseScript;
    private FogOfWarUnit fogOfWarScript;
    
    void Awake()
    {
        /*selectableScript = GetComponent<CSelectable>();
        baseScript = GetComponent<BaseController>();
        fogOfWarScript = GetComponent<FogOfWarUnit>();

        if (photonView.isMine)
        {
            //MINE: local player, simply enable the local scripts
            selectableScript.enabled = true;
            baseScript.enabled       = true;
            fogOfWarScript.enabled   = true;
        }
        else
        {
            selectableScript.enabled = false;
            baseScript.enabled       = false;
            fogOfWarScript.enabled   = false;
        }*/

        //gameObject.name = gameObject.name + "_" + photonView.viewID;
    }

	// Use this for initialization
	void Start ()
    {
	
	}

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // habría que mandar la vida
        }
        else
        {
            // recibir la vida
        }
    }

	// Update is called once per frame
	void Update ()
    {
	
	}
}
