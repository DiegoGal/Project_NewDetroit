using UnityEngine;
using System.Collections;

public class UnitEngineerGoblin : UnitEngineer
{

    public override void Awake ()
    {
        base.Awake();

        // Por si no se han establecido las referencias a los dummys del modelo
        // en el editor de Unity las buscamos ahora:
        if (dummyLaptop == null)
            dummyLaptop = transform.FindChild("Bip001/Bip001 Footsteps");
        if (dummyHand == null)
            dummyHand = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Mano Der");

        // instanciate a laptop
        laptopInst = Instantiate
        (
            laptop,
            dummyLaptop.transform.position,
            new Quaternion()
        ) as GameObject;
        laptopInst.transform.name = "Laptop";
        laptopInst.transform.parent = dummyLaptop;
        laptopInst.transform.rotation = transform.rotation;
        // hide it
        laptopInst.SetActive(false);

        // instanciate a Hammer
        hammerInst = Instantiate
        (
            hammer,
            dummyHand.transform.position,
            new Quaternion()
        ) as GameObject;
        hammerInst.transform.name = "Hammer";
        hammerInst.transform.parent = dummyHand;
        hammerInst.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
        // hide it
        hammerInst.SetActive(false);
    }
	
    public override void SetCanConstruct (int item)
	{
        switch (item)
        {
            case 0:
                if (towerArmy)
                    lastTowerArmy = towerArmy;
                if (warehouse)
                    lastWarehouse = warehouse;
                towerArmy = PhotonNetwork.Instantiate
                (
                    "Goblin Tower",
                    transform.position + new Vector3(30,-30,30),
					towerArmyPrefab.transform.rotation,
					0
                ) as GameObject;
                
                //towerArmy.transform.Rotate(270.0f, 0.0f, 0.0f);
				towerArmy.transform.Rotate(0.0f, 0.0f, 180.0f);
				towerArmy.name = towerArmy.name.Replace("(Clone)", "");
                towerArmy.GetComponent<TowerArmy>().SetTeamNumber(this.teamNumber, team.teamColorIndex);
                towerArmy.GetComponent<TowerArmy>().SetBaseController(baseController);
				towerArmy.GetComponent<TowerArmy>().SetConstructMaterial();
                newTAConstruct = true;
                break;
            case 1:
                if (towerArmy)
                    lastTowerArmy = towerArmy;
                if (warehouse)
                    lastWarehouse = warehouse;
				warehouse = PhotonNetwork.Instantiate
                (
					"Goblin Warehouse",
                    transform.position + new Vector3(30, -30, 30),
					warehousePrefab.transform.rotation,
					0
                ) as GameObject;
				warehouse.transform.Rotate(0.0f, 180.0f, 0.0f);
				warehouse.name = warehouse.name.Replace("(Clone)", "");
                warehouse.GetComponent<Warehouse>().SetTeamNumber(this.teamNumber, team.teamColorIndex);
                warehouse.GetComponent<Warehouse>().SetBaseController(baseController);
				warehouse.GetComponent<Warehouse>().SetConstructMaterial();
                newWConstruct = true;
                break;
        }
	}

}
