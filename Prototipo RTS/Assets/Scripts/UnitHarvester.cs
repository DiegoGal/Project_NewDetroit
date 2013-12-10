using UnityEngine;
using System.Collections;

public class UnitHarvester : UnitController
{
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

    private enum HarvestState
    {
        None,
        GoingToMine,
        Waiting, // espera hasta que halla hueco en la mina
        GoingToChopPosition,
        Choping, // picando
        ReturningToBase
    }
    private HarvestState currentHarvestState = HarvestState.None;
    private HarvestState nextHarvestState = HarvestState.None;

    // referencia a la mina que se está cosechando
    private Transform currentMine;

    private Vector3 lastHarvestPos;
    private int lastHarvestIndex;

    public override void RightClickOnSelected(Vector3 destiny, Transform destTransform)
    {
        if (destTransform.name == "WorldFloor")
        {
            Debug.Log("ojo suelo!");
            if (currentHarvestState == HarvestState.Waiting ||
                currentHarvestState == HarvestState.GoingToChopPosition ||
                currentHarvestState == HarvestState.Choping)
                currentMine.GetComponent<CResources>().LeaveHarvestPosition(lastHarvestIndex);
            nextHarvestState = HarvestState.None;
            currentHarvestState = HarvestState.None;
            base.RightClickOnSelected(destiny, destTransform);
        }
        else if (destTransform.name == "ResourcesMine")
        {
			// actualizar la referencia de la última mina seleccionada
			currentMine = destTransform;
			Debug.Log("vamos pa la mina!");
			if (currentHarvestState == HarvestState.None)
			{
				// actualizar el estado de cosecha
				if (resourcesLoaded == harvestCapacity)
				{
					// si la unidad ya esta llena de recursos, vuelve a la base para dejarlos
					currentHarvestState = HarvestState.ReturningToBase;
					GoTo(basePosition);
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
					GoTo(basePosition);
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
			}
			else
			{
				currentHarvestState = HarvestState.None;
			}
			GoTo(basePosition);
			nextHarvestState = HarvestState.None;
        }
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
                if (Vector3.Distance(transform.position, currentMine.position) <
                    currentMine.GetComponent<CResources>().distanceToWait)
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
                    }
                }
                else
                    base.Update();
                break;
            case HarvestState.Waiting:

                break;
            case HarvestState.GoingToChopPosition:
                if (Vector3.Distance(transform.position, lastHarvestPos) < destinyThreshold)
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

                        GoTo(basePosition);
                    }
                    actualHarvestTime = 0;
                }
                break;
            case HarvestState.ReturningToBase:
                base.Update();
                break;
        }
    } // Update

    void OnGUI()
    {
        Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);

		GUI.skin.label.fontSize = 12;

		GUI.Label(new Rect(camPos.x - 10, Screen.height - camPos.y - 25, 100, 50),
		          currentState.ToString());
        GUI.Label(new Rect(camPos.x - 10, Screen.height - camPos.y - 35, 100, 50),
            currentHarvestState.ToString());
        GUI.Label(new Rect(camPos.x - 10, Screen.height - camPos.y - 45, 100, 50),
            "resources: " + resourcesLoaded);
    }

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

    public override void ArrivedToBase()
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