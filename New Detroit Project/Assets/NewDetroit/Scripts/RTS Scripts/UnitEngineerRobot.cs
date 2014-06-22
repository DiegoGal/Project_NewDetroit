using UnityEngine;
using System.Collections;

public class UnitEngineerRobot : UnitEngineer
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

        type = TypeHeroe.Robot;
    }
	
    public override void SetCanConstruct (int item)
	{
        switch (item)
        {
            case 0:
				towerArmy = PhotonNetwork.Instantiate
                (
                    "Goblin Tower",
                    transform.position + new Vector3(30,-30,30),
                    new Quaternion(0, 0, 0, 0),
                    0
                ) as GameObject;
                //towerArmy.transform.Rotate(270.0f, 0.0f, 0.0f);
                towerArmy.name = towerArmy.name.Replace("(Clone)", "");
                towerArmy.GetComponent<TowerArmy>().SetTeamNumber(this.teamNumber, team.teamColorIndex);
                towerArmy.GetComponent<TowerArmy>().SetBaseController(baseController);
                newTAConstruct = true;
                break;
            case 1:
				warehouse = PhotonNetwork.Instantiate
                (
                    "Goblin Warehouse",
                    transform.position + new Vector3(30, -30, 30),
                    new Quaternion(0, 0, 0, 0),
                    0
                ) as GameObject;
                warehouse.transform.Rotate(0.0f, 180.0f, 0.0f);
                warehouse.name = warehouse.name.Replace("(Clone)", "");
                warehouse.GetComponent<Warehouse>().SetTeamNumber(this.teamNumber, team.teamColorIndex);
                warehouse.GetComponent<Warehouse>().SetBaseController(baseController);
                newWConstruct = true;
                break;
        }
	}

}
