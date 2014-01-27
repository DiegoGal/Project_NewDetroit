using UnityEngine;
using System.Collections;

public class ThirdPersonNetwork : Photon.MonoBehaviour
{

ThirdPersonCamera cameraScript;
ThirdPersonController controllerScript;
Animator animator;
OrcController orcControllerScript;

void Awake()
{
	cameraScript = GetComponent<ThirdPersonCamera>();
	controllerScript = GetComponent<ThirdPersonController>();
	animator = GetComponent<Animator>();
	orcControllerScript = GetComponent<OrcController> ();
	
	if (photonView.isMine)
	{
		//MINE: local player, simply enable the local scripts
		cameraScript.enabled = true;
		controllerScript.enabled = true;
	}
	else
	{           
		cameraScript.enabled = false;
		
		controllerScript.enabled = true;
		controllerScript.isControllable = false;
	}

	gameObject.name = gameObject.name + photonView.viewID;
}

void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
	if (stream.isWriting)
	{
		OrcBasicAttack leftAttack = orcControllerScript.leftArm.GetComponent<OrcBasicAttack>(),
			rightAttack = orcControllerScript.rightArm.GetComponent<OrcBasicAttack>();
		//We own this player: send the others our data
		//stream.SendNext((int)controllerScript._characterState);
		stream.SendNext(animator.GetBool("isRunning"));
		stream.SendNext(animator.GetFloat("Speed"));
		stream.SendNext(animator.GetBool("isAttacking"));
		stream.SendNext(animator.GetBool("isSecondAttack1"));
		stream.SendNext(animator.GetBool("isSecondAttack2"));
		stream.SendNext(animator.GetBool("isSecondAttack3"));
		stream.SendNext(transform.position);
		stream.SendNext(transform.rotation);

		// if the left arm attack has collided with something
		if (leftAttack.getHasCollided())
		{
			stream.SendNext(leftAttack.getNameCollideOnce()); // Send the name of the object that has been collided with the left arm of an orc.
			stream.SendNext(leftAttack.getLifeCollide()); // Send the life of the object that has been collided with the left arm of an orc.
		}
		else
		{
			stream.SendNext(null);
			stream.SendNext(0);
		}
		// if the right arm attack has collided with something
		if (rightAttack.getHasCollided())
		{
			stream.SendNext(rightAttack.getNameCollideOnce()); // Send the name of the object that has been collided with the right arm of an orc.
			stream.SendNext(rightAttack.getLifeCollide()); // Send the life of the object that has been collided with the right arm of an orc.
		}
		else
		{
			stream.SendNext(null);
			stream.SendNext(0);
		}
	}
	else
	{
		//Network player, receive data
		//controllerScript._characterState = (CharacterState)(int)stream.ReceiveNext();
		isRunning = (bool)stream.ReceiveNext();
		Speed = (float)stream.ReceiveNext();
		isAttacking = (bool)stream.ReceiveNext();
		isSecondAttack1 = (bool)stream.ReceiveNext();
		isSecondAttack2 = (bool)stream.ReceiveNext();
		isSecondAttack3 = (bool)stream.ReceiveNext();
		correctPlayerPos = (Vector3)stream.ReceiveNext();
		correctPlayerRot = (Quaternion)stream.ReceiveNext();

		nameOrcLA = (string) stream.ReceiveNext(); //Receive the name of the object that has been collided by an orc with his left arm
		lifeOrcLA = (int) stream.ReceiveNext(); // Receive the life of the object that has been collided by an orc with his left arm
		nameOrcRA = (string) stream.ReceiveNext(); // Receive the name of the object that has been collided by an orc with his right arm
		lifeOrcRA = (int) stream.ReceiveNext(); // Receive the life of the object that has been collided by an orc with his right arm
	}
}

private bool isRunning;
private float Speed;
private bool isAttacking, isSecondAttack1, isSecondAttack2, isSecondAttack3;
private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this
private string nameOrcLA; // The name of the collide object that an orc have collided from his left arm
private string nameOrcRA; // The name of the collide object that an orc have collided from his rigth arm
private int lifeOrcLA; // The life the the collided object from the left arm of an orc
private int lifeOrcRA; // The life the the collided object from the right arm of an orc

void Update()
{
	if (!photonView.isMine)
	{
		//Update remote player (smooth this, this looks good, at the cost of some accuracy)
		transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
		transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
		animator.SetBool("isRunning",isRunning);
		animator.SetFloat("Speed",Speed);
		animator.SetBool("isAttacking", isAttacking);
		animator.SetBool("isSecondAttack1", isSecondAttack1);
		animator.SetBool("isSecondAttack2", isSecondAttack2);
		animator.SetBool("isSecondAttack3", isSecondAttack3);
		
		// Update the life of the object that has been collided from an orc with his left arm
		if (nameOrcLA != null)
		{
			GameObject go = GameObject.Find(nameOrcLA);
			go.GetComponent<HeroeController>().setLife(lifeOrcLA);
			nameOrcLA = null;
			// Here we have to check if the collide object is a heroe or a unit from RTS game!!!!! <---------------
		}
		// Update the life of the object that has been collided from an orc with his right arm
		if (nameOrcRA != null)
		{
			GameObject go = GameObject.Find(nameOrcRA);
			go.GetComponent<HeroeController>().setLife(lifeOrcRA);
			nameOrcRA = null;
			// Here we have to check if the collide object is a heroe or a unit from RTS game!!!!! <---------------
		}
	}
}

}