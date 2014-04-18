using UnityEngine;
using System.Collections;

public class UnitHarvesterRemote : UnitController
{

    public int attackPower = 1;

    // referencia al pack de minerales
    public GameObject mineralPack;
    private GameObject actualMineralPack;

    // dummys donde se instanciará el pack de minerales y otros objetos
    public Transform dummyMineralPack;
    public Transform dummyHand;
    public Transform dummyGlasses;
    public Transform dummyHead;
    public Transform dummyBackPack;

    // references to the assets the unit can have
    public GameObject peak;
    private GameObject helmet;
    private GameObject backpack;
    private GameObject glasses;

    public UnitHarvester.HarvestState currentHarvestState = UnitHarvester.HarvestState.None;
    private UnitHarvester.HarvestState nextHarvestState = UnitHarvester.HarvestState.None;

    // última posición a donde se va a dejar los recursos
    // es el punto más cercano de la base a la mina de recursos actual
    private Vector3 lastBasePos = new Vector3();

    // referencia a la unidad que se está curando
    private ControllableCharacter currentCharacterHealed;

    // Dice si el harvester de original está cargado.
    public bool loaded = false;

    public override void Awake()
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
    }

    public override void Start()
    {
        base.Start();

        basicAttackPower = secondaryAttackPower = attackPower;

        // instanciamos un casco o un cono encima de la cabeza
        if (Random.value <= 0.5f)
            helmet = (GameObject)Instantiate
            (
                Resources.Load("Goblin Army/GoblinHarvesterHelmet"),
                dummyHead.transform.position,
                dummyHead.transform.rotation
            );
        else
            helmet = (GameObject)Instantiate
            (
                Resources.Load("Goblin Army/GoblinHarvesterCone"),
                dummyHead.transform.position,
                dummyHead.transform.rotation
            );
        helmet.transform.Rotate(90.0f, 0.0f, 0.0f);
        helmet.transform.parent = dummyHead;
        // instanciamos aleatóriamente una mochila detrás
        if (Random.value <= 0.5f)
        {
            backpack = (GameObject)Instantiate
            (
                Resources.Load("Goblin Army/GoblinHarvesterBackpack"),
                dummyBackPack.transform.position,
                new Quaternion()
            );
            backpack.transform.parent = dummyBackPack;
        }
        // instanciamos unas gafas
        glasses = (GameObject)Instantiate
        (
            Resources.Load("Goblin Army/GoblinHarvesterGlasses"),
            dummyGlasses.transform.position,
            new Quaternion()
            //dummyGlasses.transform.rotation
        );
        glasses.transform.parent = dummyGlasses;
        glasses.transform.Rotate(new Vector3(0.0f, transform.rotation.y, 0.0f));

        // capturamos la instancia del pico en la mano
        if (dummyHand)
            peak = dummyHand.FindChild("GoblinHarvesterBeak").gameObject;

        /*GameObject newPack = Instantiate
        (
            mineralPack,
            dummyPack.transform.position,
            new Quaternion()
        ) as GameObject;
        newPack.transform.name = "MineralPack";
        newPack.transform.parent = dummyPack;
        newPack.transform.Rotate(new Vector3(180.0f, 180.0f, 180.0f));*/
        actualMineralPack = null;
    }

    public override void Update()
    {
        switch (currentHarvestState)
        {
            case UnitHarvester.HarvestState.None:
                base.Update();
                break;
            case UnitHarvester.HarvestState.GoingToMine:
                // si la distancia a la mina es menor que la distanceToWait preguntamos si hay hueco                
                base.Update();
                PlayAnimationCrossFade("Walk");
                break;
            case UnitHarvester.HarvestState.Waiting:
                base.Update();
                break;
            case UnitHarvester.HarvestState.GoingToChopPosition:
                base.Update();
                break;
            case UnitHarvester.HarvestState.Choping:
                base.Update();
                PlayAnimationCrossFade("Picar");
                break;
            case UnitHarvester.HarvestState.ReturningToBase:
                base.Update();
                break;
            case UnitHarvester.HarvestState.GoingToHealUnit:
                base.Update();
                break;
            case UnitHarvester.HarvestState.Healing:
                /*actualHealTime += Time.deltaTime;
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
                if (actualHealTime >= healTime)
                {
                    healed = currentItem.GetComponent<ControllableCharacter>().Heal(amountOfLifePerHeal);
                    // The item has been repaired
                    if (healed)
                    {
                        Debug.Log("Unidad curada");
                        currentHarvestState = HarvestState.None;
                        nextHarvestState = HarvestState.None;
                    }
                    actualHealTime = 0;
                }*/
                base.Update();
                PlayAnimationCrossFade("Heal");
                break;
        }
        // comprobamos si el harvester base está cargado
        if (loaded && actualMineralPack == null)
        {
            actualMineralPack = Instantiate
            (
                mineralPack,
                dummyMineralPack.transform.position,
                new Quaternion()
            ) as GameObject;
            actualMineralPack.transform.name = "MineralPack";
            actualMineralPack.transform.parent = dummyMineralPack;
            actualMineralPack.transform.Rotate(new Vector3(180.0f, 180.0f, 180.0f));
        }
        if (!loaded && actualMineralPack != null)
        {
            Destroy(actualMineralPack);
            actualMineralPack = null;
        }
    } // Update

    public override void OnGUI()
    {
        if (currentState != State.AscendingToHeaven)
        {
            base.OnGUI();

            GUI.skin.label.fontSize = 10;

            GUI.Label(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 45, 100, 50),
                currentState.ToString());
            GUI.Label(new Rect(screenPosition.x - 10, Screen.height - screenPosition.y - 55, 100, 50),
                currentHarvestState.ToString());
        }
    }

    protected override void PlayAnimationCrossFade(string animationName)
    {
        // si la unidad esta cargada de minerales cambian algunas animaciones
        if (actualMineralPack)
        {
            if (animationName == "Walk")
                animation.CrossFade("Walk Loaded");
            else if (animationName == "Idle01")
                animation.CrossFade("Idle Loaded");
            else if (animationName == "Idle Wait")
                animation.CrossFade("Idle01");
            else
                base.PlayAnimationCrossFade(animationName);
        }
        else
            base.PlayAnimationCrossFade(animationName);
    }

    protected override void PlayAnimationCrossFadeQueued(string animationName)
    {
        // si la unidad esta cargada de minerales cambian algunas animaciones
        if (actualMineralPack)
        {
            if (animationName == "Walk")
                animation.CrossFadeQueued("Walk Loaded");
            else if (animationName == "Idle01")
                animation.CrossFadeQueued("Idle Loaded");
            else if (animationName == "Idle Wait")
                animation.CrossFadeQueued("Idle01");
            else
                base.PlayAnimationCrossFadeQueued(animationName);
        }
        else
            base.PlayAnimationCrossFadeQueued(animationName);
    }

    public override int GetUnitType()
    {
        return 0;
    }

    protected override void RemoveAssetsFromModel()
    {
        if (helmet)
            Destroy(helmet);
        if (backpack)
            Destroy(backpack);
        if (glasses)
            Destroy(glasses);
        if (actualMineralPack)
            Destroy(actualMineralPack);
        if (peak)
            Destroy(peak);
    }

}
