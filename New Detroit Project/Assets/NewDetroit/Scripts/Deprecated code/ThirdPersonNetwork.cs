using UnityEngine;
using System.Collections;

public class ThirdPersonNetwork : Photon.MonoBehaviour
{
	//==========================
	//====    ATTRIBUTES    ====
	//==========================

//    ThirdPersonCamera cameraScript;
////	ThirdPersonController controllerScript;
//    Animator animator;

//    private HeroeController heroeControlScript;

//    //============================
//    //====    MAIN METHODS    ====
//    //============================

//    void Awake()
//    {
//        cameraScript = GetComponent<ThirdPersonCamera>();
//        //animator = GetComponent<Animator>();
//        heroeControlScript = GetComponent<HeroeController> ();
		
//        if (photonView.isMine)
//        {
//            //MINE: local player, simply enable the local scripts
//            cameraScript.enabled = true;

//            this.heroeControlScript.isMine = true;
//            heroeControlScript.enabled = true;
//        }
//        else
//        {           
//            cameraScript.enabled = false;			
//            this.heroeControlScript.isMine = false;
//            heroeControlScript.enabled = true;
//        }

//        gameObject.name = gameObject.name + photonView.viewID;
//    }

//    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//    {
//        if (stream.isWriting)
//        {
//            //We own this player: send the others our data
//            stream.SendNext(transform.position);
//            stream.SendNext(transform.rotation);
//            stream.SendNext(heroeControlScript.getState());
//            stream.SendNext(heroeControlScript.getStateSecondAttack());
//            // Life's orc
//            if (heroeControlScript.getTypeHero() == HeroeController.TypeHeroe.Orc)
//            {
//                /*OrcController orcControlScript = this.GetComponent<OrcController>();
//                OrcBasicAttack	leftAttack = orcControlScript.leftArm.GetComponent<OrcBasicAttack>(),
//                                rightAttack = orcControlScript.rightArm.GetComponent<OrcBasicAttack>();

//                // if the left arm attack has collided with something
//                if (leftAttack.getHasCollided())
//                {
//                    stream.SendNext(leftAttack.getNameCollideOnce()); // Send the name of the object that has been collided with the left arm of an orc.
//                    stream.SendNext(leftAttack.getLifeCollide()); // Send the life of the object that has been collided with the left arm of an orc.
//                }
//                else
//                {
//                    stream.SendNext(null);
//                    stream.SendNext(0);
//                }
//                // if the right arm attack has collided with something
//                if (rightAttack.getHasCollided())
//                {
//                    stream.SendNext(rightAttack.getNameCollideOnce()); // Send the name of the object that has been collided with the right arm of an orc.
//                    stream.SendNext(rightAttack.getLifeCollide()); // Send the life of the object that has been collided with the right arm of an orc.
//                }
//                else
//                {
//                    stream.SendNext(null);
//                    stream.SendNext(0);
//                }*/
//            }

//            // Experience
//            stream.SendNext(heroeControlScript.getExperience());
//        }
//        // Is reading
//        else
//        {
//            //Network player, receive data
//            correctPlayerPos = (Vector3)stream.ReceiveNext();
//            correctPlayerRot = (Quaternion)stream.ReceiveNext();
//            newState = (HeroeController.StateHeroe)stream.ReceiveNext();
//            newStateAttackSecond = (HeroeController.AttackSecond)stream.ReceiveNext();

//            // Life's orc
//            nameOrcLA = (string) stream.ReceiveNext(); //Receive the name of the object that has been collided by an orc with his left arm
//            lifeOrcLA = (int) stream.ReceiveNext(); // Receive the life of the object that has been collided by an orc with his left arm
//            nameOrcRA = (string) stream.ReceiveNext(); // Receive the name of the object that has been collided by an orc with his right arm
//            lifeOrcRA = (int) stream.ReceiveNext(); // Receive the life of the object that has been collided by an orc with his right arm

//            // Experience
//            experience = (int) stream.ReceiveNext();
//        }
//    }

//    //private bool isRunning;
//    //private float Speed;
//    //private bool isAttacking, isSecondAttack1, isSecondAttack2, isSecondAttack3;
//    private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
//    private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this
//    private HeroeController.StateHeroe newState; // New state of the heroe
//    private HeroeController.AttackSecond newStateAttackSecond; // New state Attack
//    private string nameOrcLA; // The name of the collide object that an orc has collided with his left arm
//    private string nameOrcRA; // The name of the collide object that an orc has collided with his rigth arm
//    private int lifeOrcLA; // The life of the collided object that has been hit with the left arm of an orc
//    private int lifeOrcRA; // The life of the collided object that has been hit with the right arm of an orc
//    private int experience;

//    void Update()
//    {
//        if (!photonView.isMine)
//        {
//            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
//            transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
//            transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
//            heroeControlScript.setState(newState);
//            heroeControlScript.setStateSecondAttack(newStateAttackSecond);

//            this.updateCollidedOrc();

//            heroeControlScript.setExperience(experience);
//        }
//    }

//    //===============================
//    //====    PRIVATE METHODS    ====
//    //===============================

//    // Update the life of the objects that the orc has collided
//    private void updateCollidedOrc()
//    {
//        // If the object that has been collided with the right arm is the same than the object that has been collided
//        // with the left arm
//        if (this.nameOrcLA != null && this.nameOrcRA != null && this.nameOrcLA == this.nameOrcRA)
//        {
//            GameObject go = GameObject.Find(nameOrcLA);
//            if (this.lifeOrcLA <= this.lifeOrcRA) go.GetComponent<HeroeController>().GetComponent<CLife>().currentLife = lifeOrcLA;
//            else go.GetComponent<HeroeController>().GetComponent<CLife>().currentLife = lifeOrcRA;

//            this.nameOrcLA = null;
//            this.nameOrcRA = null;
//        }
//        else 
//        {
//            // Update the life of the object that has been collided from an orc with his left arm
//            if (nameOrcLA != null)
//            {
//                GameObject go = GameObject.Find(nameOrcLA);
//                go.GetComponent<HeroeController>().GetComponent<CLife>().currentLife = lifeOrcLA;
//                this.nameOrcLA = null;
//                // Here we have to check if the collide object is a heroe or a unit from RTS game!!!!! <---------------
//            }
//            // Update the life of the object that has been collided from an orc with his right arm
//            if (nameOrcRA != null)
//            {
//                GameObject go = GameObject.Find(nameOrcRA);
//                go.GetComponent<HeroeController>().GetComponent<CLife>().currentLife = lifeOrcRA;
//                this.nameOrcRA = null;
//                // Here we have to check if the collide object is a heroe or a unit from RTS game!!!!! <---------------
//            }
//        }
//    }

}