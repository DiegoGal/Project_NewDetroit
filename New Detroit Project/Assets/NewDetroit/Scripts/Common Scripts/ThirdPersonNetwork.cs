using UnityEngine;
using System.Collections;

public class ThirdPersonNetwork : Photon.MonoBehaviour
{

ThirdPersonCamera cameraScript;
ThirdPersonController controllerScript;
Animator animator;
//Collider collideObjectSend;
//int damageCollideSend;
//OrcBasicAttack attackScript;

void Awake()
{
	cameraScript = GetComponent<ThirdPersonCamera>();
	controllerScript = GetComponent<ThirdPersonController>();
	animator = GetComponent<Animator>();
	//attackScript = GetComponent<OrcBasicAttack> ();
	
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
		//We own this player: send the others our data
		//stream.SendNext((int)controllerScript._characterState);
		//stream.SendNext(attackScript.getDamage());
		stream.SendNext(animator.GetBool("isRunning"));
		stream.SendNext(animator.GetFloat("Speed"));
		stream.SendNext(transform.position);
		stream.SendNext(transform.rotation);
		//stream.SendNext(attackScript.getDamage());
	}
	else
	{
		//Network player, receive data
		//controllerScript._characterState = (CharacterState)(int)stream.ReceiveNext();
		isRunning = (bool)stream.ReceiveNext();
		Speed = (float)stream.ReceiveNext();
		correctPlayerPos = (Vector3)stream.ReceiveNext();
		correctPlayerRot = (Quaternion)stream.ReceiveNext();
		//collideObject = (Collider)stream.ReceiveNext();
		//damageCollide = (int)stream.ReceiveNext();
	}
}

private bool isRunning;
private float Speed;
private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this
private Collider collideObject; //The collide object that we receive
private int damageCollide; // The damage of the collider

void Update()
{
	if (!photonView.isMine)
	{
		//Update remote player (smooth this, this looks good, at the cost of some accuracy)
		transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
		transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
		animator.SetBool("isRunning",isRunning);
		animator.SetFloat("Speed",Speed);
		//if (collideObject != null)
		//{
			//GameObject go = collideObject.gameObject;
			//go.GetComponent<HeroeController>().damage(damageCollide);
		//}
	}
}

}