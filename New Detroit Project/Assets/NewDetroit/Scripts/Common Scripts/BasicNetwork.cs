using UnityEngine;
using System.Collections;

public class BasicNetwork : Photon.MonoBehaviour
{

    public virtual void Awake()
    {

        if (photonView.isMine)
        {
            
        }
        else
        {

        }

        gameObject.name = gameObject.name + photonView.viewID;
        gameObject.name = gameObject.name.Replace("(Clone)", ""); 
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            //Network player, receive data
            correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();
        }
    }

    private Vector3 correctPlayerPos = new Vector3(0, -10, 0); //We lerp towards this
    private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

    public virtual void Update()
    {
        if (!photonView.isMine && correctPlayerPos.y != -10)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
        }
    }

}
