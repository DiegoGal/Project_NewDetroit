using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitHarvester : UnitController
{
    public int attackPower = 1;

    // capacidad de transporte por viaje que la unidad es capaz de cargar
    public int harvestCapacity = 10;

    // cantidad de recursos almacenados en la unidad
    private int resourcesLoaded = 0;

    // tiempo en segundos que la unidad tarda en realizar una recolección
    public int harvestTime = 1;
    private float actualHarvestTime = 0;

    // cantidad de recurso por unidad de recolección
    public int amountOfResourcesPerHarvest = 1;

    // capacidad total de recursos recolectados por la unidad
    private int totalHarvest = 0;

	// referencia a la moneda
	public GameObject coin;

	// referencia al item que se está curando
	private Transform currentItem;

	// tiempo en segundos que la unidad tarda en realizar una curacion
	public int harvestHealTime = 1;
	private float actualHarvestHealTime = 0;

	// cantidad de curación
	public int amountPerActionHeal = 5;

    private enum HarvestState
    {
        None,
        GoingToMine,
        Waiting, // espera hasta que halla hueco en la mina
        GoingToChopPosition,
        Choping, // picando
        ReturningToBase,
		GoingToHealUnit,
		Healing
    }
    private HarvestState currentHarvestState = HarvestState.None;
    private HarvestState nextHarvestState = HarvestState.None;

    // referencia a la mina que se está cosechando
    private Transform currentMine;

    private Vector3 lastHarvestPos;
    private int lastHarvestIndex;

    // última posición a donde se va a dejar los recursos
    // es el punto más cercano de la base a la mina de recursos actual
    private Vector3 lastBasePos = new Vector3();

    public override void Start ()
    {
        base.Start();

        basicAttackPower = secondaryAttackPower = attackPower;
    }
    
    public override void Update()
    {
        switch (currentHarvestState)
        {
            case HarvestState.None:
                base.Update();
                break;
            case HarvestState.GoingToMine:
                // si la distancia a la mina es menor que la distanceToWait preguntamos si hay hueco
				float distMine = Vector3.Distance(transform.position, currentMine.position);
				float distToWait = currentMine.GetComponent<CResources>().distanceToWait;
				if (distMine < distToWait)
                {
                    if ( currentMine.GetComponent<CResources>().GetHarvestPosition(
                            ref lastHarvestPos,
                            ref lastHarvestIndex,
                            this) )
                    {
                        // hay hueco y tenemos la posicion
                        currentHarvestState = HarvestState.GoingToChopPosition;
                        base.GoTo(lastHarvestPos);
                    }
                    else
                    {
                        currentHarvestState = HarvestState.Waiting;
                        GetComponent<NavMeshAgent>().destination = transform.position;
                    }
                }
                else
                    base.Update();
                break;
            case HarvestState.Waiting:

                break;
            case HarvestState.GoingToChopPosition:
                //if (Vector3.Distance(transform.position, lastHarvestPos) < destinyThreshold)
                if (currentState == State.Idle)
                {
                    // ha llegado a la posición de extracción
                    Debug.Log("comenzando cosecha...");
                    currentHarvestState = HarvestState.Choping;
                    nextHarvestState = HarvestState.ReturningToBase;
                }
                else
                    base.Update();
                break;
            case HarvestState.Choping:
                actualHarvestTime += Time.deltaTime;
                if (actualHarvestTime >= harvestTime)
                {
                    resourcesLoaded +=
                        currentMine.GetComponent<CResources>().GetResources(amountOfResourcesPerHarvest);
                    //Debug.Log("Chop! " + resourcesLoaded);
                    if (resourcesLoaded == harvestCapacity)
                    {
                        // la unidad se ha "llenado"
                        Debug.Log("Estoy lleno, vamos pa la base");
                        currentMine.GetComponent<CResources>().LeaveHarvestPosition(lastHarvestIndex);
                        currentHarvestState = HarvestState.ReturningToBase;
					    nextHarvestState = HarvestState.GoingToMine;

						// intanciamos una monedita encima de la unidad
						Vector3 coinPosition = new Vector3
                        (
                            transform.position.x,
						    transform.position.y + 2.2f,
						    transform.position.z
                        );
						GameObject newCoin = Instantiate(coin, coinPosition, new Quaternion()) as GameObject;
						newCoin.transform.name = "coin";
						newCoin.transform.parent = transform;
                        // actualizar la posición de la base donde se dejarán los recursos por si hay uno más cercano
                        float alpha = Mathf.Atan((currentMine.transform.position.x - basePosition.x) /
                            (currentMine.transform.position.z - basePosition.z));
                        float radius = 6.0f;
                        Vector3 resourceBuilding = baseController.GetArmyController().GetResourceBuilding(currentMine.GetComponent<CResources>());
                        lastBasePos.x = resourceBuilding.x - (Mathf.Sin(alpha) * radius);
                        lastBasePos.z = resourceBuilding.z - (Mathf.Cos(alpha) * radius);
                        GoTo(lastBasePos);
                    }
                    actualHarvestTime = 0;
                }
                break;
            case HarvestState.ReturningToBase:
                if (currentState == State.Idle)
                {
                    ArrivedToBase();
                }
                else
                    base.Update();
                break;
			case HarvestState.GoingToHealUnit:
				float distItem = Vector3.Distance(transform.position, currentItem.position);
				base.GoTo(currentItem.position);
				if (distItem < 6.0f)
				{
					currentHarvestState = HarvestState.Healing;
				}
				else
					base.Update();
				break;
			case HarvestState.Healing:
				actualHarvestHealTime += Time.deltaTime;
				distItem = Vector3.Distance(transform.position, currentItem.position);
				bool healed = false;
				if (distItem < 4.0f)
				{
					base.GoTo(transform.position);
				}
				else if (distItem > 8.0f)
				{
					currentHarvestState = HarvestState.None;
					nextHarvestState = HarvestState.None;
				}
				if (actualHarvestHealTime >= harvestHealTime)
				{
					healed = currentItem.GetComponent<UnitScout>().Heal(amountPerActionHeal);
					// The item has been repaired
					if (healed)
					{
						Debug.Log("Unidad curada");
						currentHarvestState = HarvestState.None;
						nextHarvestState = HarvestState.None;
					}
					actualHarvestHealTime = 0;
				}
				break;
        }
    } // Update

    public override void OnGUI()
    {
        base.OnGUI();

        Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);

		GUI.skin.label.fontSize = 10;
        
		GUI.Label(new Rect(camPos.x - 10, Screen.height - camPos.y - 25, 100, 50),
		    currentState.ToString());
        GUI.Label(new Rect(camPos.x - 10, Screen.height - camPos.y - 35, 100, 50),
            currentHarvestState.ToString());
        GUI.Label(new Rect(camPos.x - 10, Screen.height - camPos.y - 45, 100, 50),
            "resources: " + resourcesLoaded);
    }

    public override void RightClickOnSelected(Vector3 destiny, Transform destTransform)
    {
        if (destTransform.name == "WorldFloor")
        {
            //Debug.Log("ojo suelo!");
            if (currentHarvestState == HarvestState.GoingToChopPosition ||
                currentHarvestState == HarvestState.Choping)
                currentMine.GetComponent<CResources>().LeaveHarvestPosition(lastHarvestIndex);
            else if (currentHarvestState == HarvestState.Waiting)
                currentMine.GetComponent<CResources>().LeaveQueue(this);
            nextHarvestState = HarvestState.None;
            currentHarvestState = HarvestState.None;
            base.RightClickOnSelected(destiny, destTransform);
        }
        else if (destTransform.name == "ResourcesMine")
        {
            baseController.GetArmyController().UpdateMines(destTransform);
            // actualizar la referencia de la última mina seleccionada
            currentMine = destTransform;
            // actualizar la posición de la base donde se dejarán los recursos
            float alpha = Mathf.Atan((currentMine.transform.position.x - basePosition.x) /
                (currentMine.transform.position.z - basePosition.z));
            float radius = 6.0f;
            Vector3 resourceBuilding = baseController.GetArmyController().GetResourceBuilding(destTransform.GetComponent<CResources>());
            lastBasePos.x = resourceBuilding.x - (Mathf.Sin(alpha) * radius);
            lastBasePos.z = resourceBuilding.z - (Mathf.Cos(alpha) * radius);
            //lastBasePos.x = basePosition.x - (Mathf.Sin(alpha) * radius);
            //lastBasePos.z = basePosition.z - (Mathf.Cos(alpha) * radius);
            /*GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = lastBasePos;
            cube.renderer.material.color = Color.red;
            cube.transform.parent = this;*/

            Debug.Log("vamos pa la mina!");
            if (currentHarvestState == HarvestState.None)
            {
                // actualizar el estado de cosecha
                if (resourcesLoaded == harvestCapacity)
                {
                    // si la unidad ya esta llena de recursos, vuelve a la base para dejarlos
                    currentHarvestState = HarvestState.ReturningToBase;
                    GoTo(lastBasePos);
                    nextHarvestState = HarvestState.None;
                }
                else
                {
                    // todavía tiene capacidad para más recursos, se le envía a la mina
                    currentHarvestState = HarvestState.GoingToMine;
                    GoTo(destiny);
                    nextHarvestState = HarvestState.Choping;
                }
            }
            else if (currentHarvestState == HarvestState.ReturningToBase)
            {
                // actualizar el estado de cosecha
                if (resourcesLoaded == harvestCapacity)
                {
                    // si la unidad ya esta llena de recursos, vuelve a la base para dejarlos
                    currentHarvestState = HarvestState.ReturningToBase;
                    GoTo(lastBasePos);
                    nextHarvestState = HarvestState.GoingToMine;
                }
                else
                {
                    // todavía tiene capacidad para más recursos, se le envía a la mina
                    currentHarvestState = HarvestState.GoingToMine;
                    GoTo(destiny);
                    nextHarvestState = HarvestState.Choping;
                }
            }
        }
        else if (destTransform.name == "Army Base")
        {
            // vuelve a la base, si tiene recursos los deja
            if (resourcesLoaded > 0)
            {
                Debug.Log("vuelta a la base");
                currentHarvestState = HarvestState.ReturningToBase;
                GoTo(lastBasePos);
            }
            else
            {
                currentHarvestState = HarvestState.None;
                GoTo(basePosition);
            }
            nextHarvestState = HarvestState.None;
        }
		else if (destTransform.name == "UnitExplorer")
		{
			Debug.Log("¡A curar!");
			currentItem = destTransform;
			currentHarvestState = HarvestState.GoingToHealUnit;
		}
    } // RightClickOnSelected

    public void FinishWaiting (Vector3 chopPosition, int chopIndex)
    {
        lastHarvestPos = chopPosition;
        lastHarvestIndex = chopIndex;
        currentHarvestState = HarvestState.GoingToChopPosition;
        base.GoTo(lastHarvestPos);
    }

    public void StartChoping ()
    {
        // cuando llegue a la mina pasar el estado a Choping
        if (currentHarvestState == HarvestState.GoingToMine)
        {
            Debug.Log("comenzando cosecha...");
            currentHarvestState = HarvestState.Choping;
			nextHarvestState = HarvestState.ReturningToBase;
        }
    }

    public override void ArrivedToBase ()
    {
        if (currentHarvestState == HarvestState.ReturningToBase)
        {
            Debug.Log("dejando la cosecha en la base...");
            baseController.DownloadResources(resourcesLoaded);
            totalHarvest += resourcesLoaded;
            resourcesLoaded = 0;

			// eliminamos la moneda de la cabeza
			Transform coin = transform.FindChild("coin");
			if (coin != null)
				GameObject.Destroy(coin.gameObject);

			if (nextHarvestState == HarvestState.GoingToMine)
            {
                // si estaba cosechando, volvemos a la mina
                Debug.Log("volvemos a la mina");;
                currentHarvestState = HarvestState.GoingToMine;
                GoTo(currentMine.position);
				nextHarvestState = HarvestState.Choping;
            }
        }
    }


} // class UnitHarvester