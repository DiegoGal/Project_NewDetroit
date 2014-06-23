using UnityEngine;
using System.Collections;

public class  TowerNeutralNetwork : BasicNetwork
{

    public virtual void Awake()
    {

        if (photonView.isMine)
        {
            
        }
        else
        {

        }

    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            
           // stream.SendNext(GetComponent<TowerNeutral>().engineerPosTaken);
                        
        }
        else
        {
            //Network player, receive data
           //GetComponent<TowerNeutral>().engineerPosTaken = (bool[])stream.ReceiveNext();
        }
    }

}
