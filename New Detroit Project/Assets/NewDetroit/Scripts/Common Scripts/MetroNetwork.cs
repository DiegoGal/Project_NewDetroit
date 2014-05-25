using UnityEngine;
using System.Collections;

public class MetroNetwork : Photon.MonoBehaviour
{

    CResources resourcesScript;

    void Awake()
    {
        resourcesScript = GetComponent<CResources>();

        if (photonView.isMine)
        {
            resourcesScript.enabled = true;
        }
        else
        {
            resourcesScript.enabled = true;
        }

        gameObject.name = gameObject.name + photonView.viewID;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(resourcesScript.GetActualResources());

        }
        else
        {
            //Network player, receive data
            resources = (int)stream.ReceiveNext();

        }
    }

    private int resources = 0; // Resources

    void Update()
    {
        if (!photonView.isMine)
        {
            resourcesScript.setActualResources(resources);
        }
    }
}
