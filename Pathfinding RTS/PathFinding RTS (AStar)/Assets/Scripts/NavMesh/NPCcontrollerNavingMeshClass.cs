using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Clase simple para gestioner una ruta origen-destino
/// que puede tener varios puntos intermedios
/// </summary>
public class NPCcontrollerNavingMeshClass : MonoBehaviour 
{
	/// <summary>
	/// Lista de puntos a desplazarse
	/// </summary>
	public List<Transform> path;
	
	/// <summary>
	/// The punto al que actualmente me dirijo.
	/// </summary>
	protected int currentPathPoint;
	
	/// <summary>
	/// Indica el sentido del recorrido de la ruta
	/// </summary>
	protected enum PathDirection { fwd, bck };
	protected PathDirection pathDirection;
	
	/// <summary>
	/// The agent.
	/// </summary>
	protected NavMeshAgent agent;
	
	/// <summary>
	/// Velocidad máxima de desplazamiento
	/// </summary>
	public float speed = 6.0F;
	
	void Start()
	{
		//Asigno el agent Navigation
		agent = GetComponent<NavMeshAgent>();
		
		//Si no hay puntos a los que desplazarse, lanzar un error
		if(path.Count < 1)
			print("ERROR. No hay puntos a los que desplazarse");
		
		currentPathPoint = 0;
		pathDirection = PathDirection.fwd;
		agent.speed = speed;
		
	}	//	end	Start()
	
	
	void Update() 
	{
		//Si no hay puntos a los que desplazarse, no hacer nada
		if(path.Count < 1)
			return;
		
		//Compruebo si he llegado al punto de destino actual
		if(!agent.hasPath)
		{
			getNextPathPoint();
			agent.SetDestination(path[currentPathPoint].position);
		}
		
	}	//	end Update()
	

	/// <summary>
	/// Gets the next path point.
	/// </summary>
	void getNextPathPoint()
	{

		//Voy del punto de origen al punto de destino?
		if(pathDirection == PathDirection.fwd)
		{
			//Compruebo si he llegado al último pathPoint de la ruta
			if(currentPathPoint == path.Count - 1)
			{
				//Cambio de dirección
				pathDirection = PathDirection.bck;
				currentPathPoint--;
			}
			else
			{
				currentPathPoint++;
			}
			
		}
		//Voy del punto de destino al punto de origen?
		else if(pathDirection == PathDirection.bck)
		{
			//Compruebo si he llegado al primer pathPoint de la ruta
			if(currentPathPoint == 0)
			{
				//Cambio de dirección
				pathDirection = PathDirection.fwd;
				currentPathPoint++;
			}
			else
			{
				currentPathPoint--;
			}
		}
		
	}
	
	/// <summary>
	/// Dibuja los botones de andar y correr
	/// </summary>
	void OnGUI()
	{
		if(GUI.Button(new Rect(10,10,100,40), "Andar"))
		{
			agent.speed = 2.5f;
		}
		if(GUI.Button(new Rect(10,60,100,40), "Correr"))
		{
			agent.speed = 6;
		}
		
	}
	
}