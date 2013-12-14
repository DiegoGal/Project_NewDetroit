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

    private enum HarvestState
    {
        None,
        GoingToMine,
        Choping, // picando
        ReturningToBase
    }
    private HarvestState currentHarvestState = HarvestState.None;
    private HarvestState lastHarvestState = HarvestState.None;

    // referencia a la mina que se está cosechando
    private Transform currentMine;

    public override void RightClickOnSelected(Vector3 destiny, Transform destTransform)
    {
        if (destTransform.name == "WorldFloor")
        {
            Debug.Log("ojo suelo!");
            lastHarvestState = HarvestState.None;
            currentHarvestState = HarvestState.None;
            base.RightClickOnSelected(destiny, destTransform);
        }
        else if (destTransform.name == "ResourcesMine")
        {
            // actualizar la referencia de la última mina seleccionada
            Debug.Log("vamos pa la mina!");
            currentMine = destTransform;

            // actualizar el estado de cosecha
            if (resourcesLoaded == harvestCapacity)
            {
                // si la unidad ya esta llena de recursos, vuelve a la base para dejarlos
                lastHarvestState = currentHarvestState;
                currentHarvestState = HarvestState.ReturningToBase;
                GoTo(basePosition);
            }
            else
            {
                // todavía tiene capacidad para más recursos, se le envía a la mina
                lastHarvestState = currentHarvestState;
                currentHarvestState = HarvestState.GoingToMine;
                GoTo(destiny);
            }

            // transcurrido el tiempo de recolecta recoger los recursos y volver a la base
            //destTransform.GetComponent<CResources>().GetResources(harvestCapacity);

            // reiniciar el proceso una vez se ha llegado a la base
        }
        else if (destTransform.name == "Army Base")
        {
            // vuelve a la base, si tiene recursos los deja
            Debug.Log("vuelta a la base");
            lastHarvestState = currentHarvestState;
            currentHarvestState = HarvestState.ReturningToBase;
            GoTo(basePosition);
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
                base.Update();
                break;
            case HarvestState.Choping:
                actualHarvestTime += Time.deltaTime;
                if (actualHarvestTime >= harvestTime)
                {
                    resourcesLoaded += currentMine.GetComponent<CResources>().GetResources(amountOfResourcesPerHarvest);
                    Debug.Log("Chop! " + resourcesLoaded);
                    if (resourcesLoaded == harvestCapacity)
                    {
                        // la unidad se ha "llenado"
                        Debug.Log("Estamos llenos, vamos pa la base");
                        lastHarvestState = currentHarvestState;
                        currentHarvestState = HarvestState.ReturningToBase;
                        GoTo(basePosition);
                    }
                    actualHarvestTime = 0;
                }
                break;
            case HarvestState.ReturningToBase:
                base.Update();
                break;
        }
    }

    void OnGUI()
    {
        Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);

        GUI.Label(new Rect(camPos.x - 10, Screen.height - camPos.y - 20, 100, 50),
            currentHarvestState.ToString());
    }

    public void StartChoping()
    {
        // cuando llegue a la mina pasar el estado a Choping
        if (currentHarvestState == HarvestState.GoingToMine)
        {
            Debug.Log("comenzando cosecha...");
            lastHarvestState = currentHarvestState;
            currentHarvestState = HarvestState.Choping;
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

            if (lastHarvestState == HarvestState.Choping)
            {
                // si estaba cosechando, volvemos a la mina
                Debug.Log("volvemos a la mina");
                lastHarvestState = currentHarvestState;
                currentHarvestState = HarvestState.GoingToMine;
                GoTo(currentMine.position);
            }
        }
    }

} // class UnitHarvester