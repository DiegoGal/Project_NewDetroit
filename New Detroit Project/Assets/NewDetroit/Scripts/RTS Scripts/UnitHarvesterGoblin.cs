using UnityEngine;
using System.Collections;

public class UnitHarvesterGoblin : UnitHarvester
{

    public override void Awake ()
    {
        base.Awake();

        // Por si no se han establecido las referencias a los dummys del modelo
        // en el editor de Unity las buscamos ahora:
        if (dummyMineralPack == null)
            dummyMineralPack = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Dummy Mineral");
        if (dummyHand == null)
            dummyHand = transform.FindChild("Bip001 L Hand/Dummy Pico");
        if (dummyGlasses == null)
            dummyGlasses = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head/Dummy Gafas");
        if (dummyHead == null)
            dummyHead = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head/Dummy Sombrero");
        if (dummyBackPack == null)
            dummyBackPack = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Dummy Espalda");

        type = TypeHeroe.Orc;
    }

    public override void Start ()
    {
        base.Start();

        // instanciamos los accesorios del modelo
        float[] randoms = { Random.value, Random.value, Random.value };
        if (PhotonNetwork.connected)
        	photonView.RPC("InstanciateAccesories", PhotonTargets.All, randoms[0], randoms[1], randoms[2]);
        else
       		InstanciateAccesories(randoms[0], randoms[1], randoms[2]);

        // capturamos la instancia del pico en la mano
        if (dummyHand)
            peak = dummyHand.FindChild("GoblinHarvesterBeak").gameObject;
         
        // intanciamos un pack de minerales encima de la unidad
        if (PhotonNetwork.connected)
        	photonView.RPC("InstanciateMineralPack", PhotonTargets.All);
        else
        	InstanciateMineralPack();
        // se esconde:
        if (PhotonNetwork.connected)
        	photonView.RPC("ShowMineralPack", PhotonTargets.All, false);
        else 
        	ShowMineralPack(false);
    }

    [RPC]
    public void InstanciateAccesories (float rand1, float rand2, float rand3)
    {
        // instanciamos un casco o un cono encima de la cabeza (o nada)
        if (rand1 <= 0.25f)
        {
            helmet = (GameObject)Instantiate
            (
                Resources.Load("Goblin Army/GoblinHarvesterHelmet_A"),
                dummyHead.transform.position,
                dummyHead.transform.rotation
            );
            helmet.transform.Rotate(90.0f, 0.0f, 0.0f);
            helmet.transform.parent = dummyHead;
        }
        else if (rand1 <= 0.5f)
        {
            helmet = (GameObject)Instantiate
            (
                Resources.Load("Goblin Army/GoblinHarvesterHelmet_B"),
                dummyHead.transform.position,
                dummyHead.transform.rotation
            );
            helmet.transform.Rotate(90.0f, 0.0f, 0.0f);
            helmet.transform.parent = dummyHead;
        }
        else if (rand1 <= 0.75f)
        {
            helmet = (GameObject)Instantiate
            (
                Resources.Load("Goblin Army/GoblinHarvesterCone"),
                dummyHead.transform.position,
                dummyHead.transform.rotation
            );
            helmet.transform.Rotate(90.0f, 0.0f, 0.0f);
            helmet.transform.parent = dummyHead;
        }

        // instanciamos aleatóriamente una mochila detrás (o nada)
        if (rand2 <= 0.33f)
        {
            backpack = (GameObject)Instantiate
            (
                Resources.Load("Goblin Army/GoblinHarvesterBackpack_A"),
                dummyBackPack.transform.position,
                new Quaternion()
            );
            backpack.transform.parent = dummyBackPack;
        }
        else if (rand2 <= 0.66f)
        {
            backpack = (GameObject)Instantiate
            (
                Resources.Load("Goblin Army/GoblinHarvesterBackpack_B"),
                dummyBackPack.transform.position,
                new Quaternion()
            );
            backpack.transform.parent = dummyBackPack;
        }

        // instanciamos unas gafas (o nada)
        if (rand3 <= 0.33f)
        {
            glasses = (GameObject)Instantiate
            (
                Resources.Load("Goblin Army/GoblinHarvesterGlasses_A"),
                dummyGlasses.transform.position,
                new Quaternion()
                //dummyGlasses.transform.rotation
            );
            glasses.transform.parent = dummyGlasses;
            glasses.transform.Rotate(new Vector3(0.0f, transform.rotation.y, 0.0f));
        }
        else if (rand3 <= 0.66f)
        {
            glasses = (GameObject)Instantiate
            (
                Resources.Load("Goblin Army/GoblinHarvesterGlasses_B"),
                dummyGlasses.transform.position,
                new Quaternion()
            );
            glasses.transform.parent = dummyGlasses;
            glasses.transform.Rotate(new Vector3(0.0f, transform.rotation.y, 0.0f));
        }
    }

} // class UnitHarvesterGoblin