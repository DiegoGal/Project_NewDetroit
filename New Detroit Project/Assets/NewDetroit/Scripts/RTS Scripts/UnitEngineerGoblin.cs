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

		if (PhotonNetwork.connected)
		{
        	photonView.RPC("InstanciateLaptop", PhotonTargets.All);
        	photonView.RPC("InstanciateHammer", PhotonTargets.All);
        }
        else
        {
        	InstanciateLaptop();
        	InstanciateHammer();
        }

        type = TypeHeroe.Orc;
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
                if (PhotonNetwork.connected)
	                towerArmy = PhotonNetwork.Instantiate
	                (
	                    "Goblin Tower",
	                    transform.position + new Vector3(30,-30,30),
						towerArmyPrefab.transform.rotation,
						0
	                ) as GameObject;
	            else
	            {
					towerArmy = (GameObject) Instantiate(towerArmyPrefab, transform.position + new Vector3(30,-30,30), towerArmyPrefab.transform.rotation);
	            }
                
                //towerArmy.transform.Rotate(270.0f, 0.0f, 0.0f);
				towerArmy.transform.Rotate(0.0f, 0.0f, 180.0f);
				towerArmy.name = towerArmy.name.Replace("(Clone)", "");
                //towerArmy.GetComponent<TowerArmy>().SetTeamNumber(this.teamNumber, team.teamColorIndex);
                towerArmy.GetComponent<CTeamTowerArmy>().teamNumber = this.team.teamNumber;
                towerArmy.GetComponent<CTeamTowerArmy>().teamColorIndex = this.team.teamColorIndex;
                towerArmy.GetComponent<TowerArmy>().SetBaseController(baseController);
				towerArmy.GetComponent<TowerArmy>().SetConstructMaterial();
                newTAConstruct = true;
                break;
            case 1:
                if (towerArmy)
                    lastTowerArmy = towerArmy;
                if (warehouse)
                    lastWarehouse = warehouse;
                if (PhotonNetwork.connected)
					warehouse = PhotonNetwork.Instantiate
	                (
						"Goblin Warehouse",
	                    transform.position + new Vector3(30, -30, 30),
						warehousePrefab.transform.rotation,
						0
	                ) as GameObject;
	            else
					warehouse = (GameObject) Instantiate(warehousePrefab, transform.position + new Vector3(30, -30, 30), warehousePrefab.transform.rotation);
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
