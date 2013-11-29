using UnityEngine;
using System.Collections;

public class UnitHarvester : UnitController
{
    // capacidad de transporte por viaje que la unidad es capaz de cargar
    public int harvestCapacity = 100;

    // cantidad de recursos almacenados en la unidad
    private int resourcesLoaded = 0;

    // tiempo en segundos que la unidad tarda en realizar una recolección
    public int harvestTime = 3;

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

    // referencia a la mina que se está cosechando
    private Transform currentMine;

    public override void RightClickOnSelected(Vector3 destiny, Transform destTransform)
    {
        if (destTransform.name == "WorldFloor")
            base.RightClickOnSelected(destiny, destTransform);
        else if (destTransform.name == "ResourcesMine")
        {
            // actualizar la referencia de la última mina seleccionada
            currentMine = destTransform;
            // actualizar el estado de cosecha
            if (resourcesLoaded == harvestCapacity)
                currentHarvestState = HarvestState.ReturningToBase;
            else
                currentHarvestState = HarvestState.GoingToMine;

            // enviar a la unidad a la mina con goto

            // cuando llegue a la mina pasar el estado a Harvesting

            // transcurrido el tiempo de recolecta recoger los recursos y volver a la base
            destTransform.GetComponent<CResources>().GetResources(harvestCapacity);

            // reiniciar el proceso una vez se ha llegado a la base
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

                break;
            case HarvestState.Choping:

                break;
            case HarvestState.ReturningToBase:

                break;
        }
    }

}