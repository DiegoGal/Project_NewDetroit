using UnityEngine;
using System.Collections;

public class UnitHarvesterRemote : ControllableCharacter
{

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
    public UnitController.State currentState = UnitController.State.Idle;

    // última posición a donde se va a dejar los recursos
    // es el punto más cercano de la base a la mina de recursos actual
    private Vector3 lastBasePos = new Vector3();

    // referencia a la unidad que se está curando
    private ControllableCharacter currentCharacterHealed;

    // indicates the time remaining until the next waiting animation
    private float timeToNextWaitAnimation;
    // Dice si el harvester de original está cargado.
    public bool loaded = false;

    public void Awake()
    {

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

    public void Start()
    {
        network();

        // instanciamos un casco o un cono encima de la cabeza (o nada)
        float rand = Random.value;
        if (rand <= 0.25f)
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
        else if (rand <= 0.5f)
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
        else if (rand <= 0.75f)
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
        rand = Random.value;
        if (Random.value <= 0.33f)
        {
            backpack = (GameObject)Instantiate
            (
                Resources.Load("Goblin Army/GoblinHarvesterBackpack_A"),
                dummyBackPack.transform.position,
                new Quaternion()
            );
            backpack.transform.parent = dummyBackPack;
        }
        else if (rand <= 0.66f)
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
        rand = Random.value;
        if (Random.value <= 0.33f)
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
        else if (Random.value <= 0.66f)
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

        // capturamos la instancia del pico en la mano
        if (dummyHand)
            peak = dummyHand.FindChild("GoblinHarvesterBeak").gameObject;

        // intanciamos un pack de minerales encima de la unidad
        actualMineralPack = (GameObject)Instantiate
        (
            mineralPack,
            dummyMineralPack.transform.position,
            new Quaternion()
        ) as GameObject;
        actualMineralPack.transform.name = "MineralPack";
        actualMineralPack.transform.parent = dummyMineralPack;
        // se esconde:
        ShowMineralPack(loaded);
    }

    // esconde (o muestra) todos los objetos que componen el pack de minerales
    private void ShowMineralPack(bool enable = true)
    {
        Renderer[] renderers = actualMineralPack.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
            r.enabled = enable;
    }

    public void Update()
    {
        switch (currentState)
        {
            case UnitController.State.Idle: UpdateIdle(); break;
            case UnitController.State.GoingTo: UpdateGoingTo(); break;
            case UnitController.State.GoingToAnEnemy: UpdateGoingToAnEnemy(); break;
            case UnitController.State.Attacking: UpdateAttacking(); break;
            case UnitController.State.Flying: UpdateFlying(); break;
            case UnitController.State.Dying: UpdateDying(); break;
            case UnitController.State.AscendingToHeaven: UpdateAscendingToHeaven(); break;
        }

        switch (currentHarvestState)
        {
            case UnitHarvester.HarvestState.None:
                break;
            case UnitHarvester.HarvestState.GoingToMine:
                PlayAnimationCrossFade("Walk");
                break;
            case UnitHarvester.HarvestState.Waiting:
                break;
            case UnitHarvester.HarvestState.GoingToChopPosition:
                break;
            case UnitHarvester.HarvestState.Choping:
                PlayAnimationCrossFade("Picar");
                break;
            case UnitHarvester.HarvestState.ReturningToBase:
                break;
            case UnitHarvester.HarvestState.GoingToHealUnit:
                break;
            case UnitHarvester.HarvestState.Healing:
                PlayAnimationCrossFade("Heal");
                break;
        }
    } // Update

    private void UpdateIdle()
    {
        timeToNextWaitAnimation -= Time.deltaTime;
        if (timeToNextWaitAnimation <= 0)
        {
            PlayAnimationCrossFade("Idle Wait");
            PlayAnimationCrossFadeQueued("Idle01");
            timeToNextWaitAnimation = Random.Range(5.0f, 15.0f);
        }
        else 
        { if (!animation.IsPlaying("Idle Wait"))
            PlayAnimationCrossFade("Idle01"); 
        }
    }

    private void UpdateGoingTo()
    {
        ShowMineralPack(loaded);
        PlayAnimationCrossFade("Walk");
    }

    private void UpdateGoingToAnEnemy()
    {
        ShowMineralPack(loaded);
        PlayAnimationCrossFade("Walk");
    }

    private void UpdateAttacking()
    {    
        PlayAnimationCrossFade("Attack1");
    }

    private void UpdateFlying()
    {
        PlayAnimationCrossFade("Idle01");
    }

    private void UpdateDying()
    {
        PlayAnimationCrossFade("Die");
    }

    private void UpdateAscendingToHeaven()
    {
        RemoveAssetsFromModel();
    }

    public void OnGUI()
    {
    }

    protected void PlayAnimationCrossFade(string animationName)
    {
        // si la unidad esta cargada de minerales cambian algunas animaciones
        if (loaded)
        {
            if (animationName == "Walk")
                animation.CrossFade("Walk Loaded");
            else if (animationName == "Idle01")
                animation.CrossFade("Idle Loaded");
            else if (animationName == "Idle Wait")
                animation.CrossFade("Idle Loaded");
            else
                animation.CrossFade(animationName);
        }
        else
            animation.CrossFade(animationName);
    }

    protected void PlayAnimationCrossFadeQueued(string animationName)
    {
        // si la unidad esta cargada de minerales cambian algunas animaciones
        if (loaded)
        {
            if (animationName == "Walk")
                animation.CrossFadeQueued("Walk Loaded");
            else if (animationName == "Idle01")
                animation.CrossFadeQueued("Idle Loaded");
            else if (animationName == "Idle Wait")
                animation.CrossFadeQueued("Idle Loaded");
            else
                animation.CrossFadeQueued(animationName);
        }
        else
            animation.CrossFadeQueued(animationName);
    }

    protected void PlayIdleWaitAnimation()
    {
        // solo se reproduce la animación de espera si el estado es None
        if (currentHarvestState == UnitHarvester.HarvestState.None)
        {
            PlayAnimationCrossFade("Idle Wait");
            PlayAnimationCrossFadeQueued("Idle01");
        }
    }

    public int GetUnitType()
    {
        return 0;
    }

    protected void RemoveAssetsFromModel()
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
