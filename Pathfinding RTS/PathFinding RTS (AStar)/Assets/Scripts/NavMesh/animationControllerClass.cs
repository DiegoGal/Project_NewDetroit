using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class animationControllerClass : MonoBehaviour {
	
	/// <summary>
	/// Referencia al componente Animator
	/// </summary>
	protected Animator animator;
	
	/// <summary>
	/// Velocidad en el plano XZ
	/// Su magnitud controla las transiciones entre Idle, Walk y Run
	/// </summary>
	Vector2 velocity;
	
	/// <summary>
	/// The agent.
	/// </summary>
	protected NavMeshAgent agent;
	
	// Use this for initialization
	void Start () 
	{
		//Obtenemos las referencias
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>(); 
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	
	// Callback for processing animation movements for modifying root motion.
	// This callback will be invoked at each frame,
	// after the state machines and the animations have been evaluated, but before OnAnimatorIK.	
	void OnAnimatorMove()
	{
		if(!animator)
			return;
		
		//Obtenemos la velocidad actual del motor (plano XZ)
		velocity = new Vector2(agent.desiredVelocity.x, agent.desiredVelocity.z);	
		//Establecemos la variable del animator
		animator.SetFloat("velocity", velocity.magnitude);

	}

	
	

}