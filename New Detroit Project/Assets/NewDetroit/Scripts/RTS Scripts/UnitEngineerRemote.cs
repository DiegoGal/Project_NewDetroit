using UnityEngine;
using System.Collections;

public class UnitEngineerRemote : ControllableCharacter
{

    // tiempo en segundos que la unidad tarda en realizar una construccion, conquista y/o reparacion
    public int engineerTime = 1;
    private float actualEngineerTime = 0;

    public UnitEngineer.EngineerState currentEngineerState = UnitEngineer.EngineerState.None;
    public UnitController.State currentState = UnitController.State.Idle;

    // dummy where is going to be instanciated a laptop
    public Transform dummyLaptop;
    public Transform dummyHand;

    // reference to the laptop and hammer
    public GameObject laptop;
    public GameObject hammer;

    //For attacking1
    public GameObject fireball;
    private GameObject newFireball;

    GameObject newHammer; //instantiation of the hammer
    GameObject newLaptop; //instantiation of the laptop

    bool showHammer = false;
    bool showLaptop = false;

    private float attackCadenceAux = 2.5f;

    private float timeToNextWaitAnimation;

    // To knows the direction of the fireball
    public Vector3 fireballDir;

    public void Awake()
    {
        // Por si no se han establecido las referencias a los dummys del modelo
        // en el editor de Unity las buscamos ahora:
        if (dummyLaptop == null)
            dummyLaptop = transform.FindChild("Bip001/Bip001 Footsteps");
        if (dummyHand == null)
            dummyHand = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Mano Der");
        // We instanciate a Hammer
        newHammer = Instantiate
        (
            hammer,
            dummyHand.transform.position,
            new Quaternion()
        ) as GameObject;
        newHammer.transform.name = "Hammer";
        newHammer.transform.parent = dummyHand;
        newHammer.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
        // We instanciate a laptop
        newLaptop = Instantiate
        (
            laptop,
            dummyLaptop.transform.position,
            new Quaternion()
        ) as GameObject;
        newLaptop.transform.name = "Laptop";
        newLaptop.transform.parent = dummyLaptop;
        newLaptop.transform.rotation = transform.rotation;
        // No mostramos ni el martillo ni el ordenador
        ShowHammer(false);
        ShowLaptop(false);
    }

    // esconde (o muestra) todos los objetos que componen el ordenador martillo
    private void ShowHammer(bool enable = true)
    {
        Renderer[] renderers = newHammer.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
            r.enabled = enable;
    }

    // esconde (o muestra) todos los objetos que componen el ordenador portatil
    private void ShowLaptop(bool enable = true)
    {
        Renderer[] renderers = newLaptop.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
            r.enabled = enable;
    }

    public void Start()
    {
        InsertToTheDistanceMeasurerTool();
        timeToNextWaitAnimation = Random.Range(5.0f, 15.0f);
    }

    // Update is called once per frame
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
        switch (currentEngineerState)
        {
            case UnitEngineer.EngineerState.None:
                if (showHammer)
                {
                    showHammer = false;
                    ShowHammer(false);
                }
                if (showLaptop)
                {
                    showLaptop = false;
                    ShowLaptop(false);
                }
                break;
            case UnitEngineer.EngineerState.GoingToRepairItem:
                break;
            case UnitEngineer.EngineerState.GoingToConquerableItem:
                break;
            case UnitEngineer.EngineerState.GoingToConstructItem:
                break;
            case UnitEngineer.EngineerState.Waiting:
                break;
            case UnitEngineer.EngineerState.GoingToRepairPosition:
                break;
            case UnitEngineer.EngineerState.GoingToConquestPosition:
                break;
            case UnitEngineer.EngineerState.GoingToConstructPosition:
                break;
            case UnitEngineer.EngineerState.Repairing:
                if (!showHammer)
                {
                    showHammer = true;
                    ShowHammer(true);

                    showLaptop = false;
                    ShowLaptop(false);
                }
                animation.CrossFade("Build");
                break;
            case UnitEngineer.EngineerState.Conquering:
                if (!showLaptop)
                {
                    showHammer = false;
                    ShowHammer(false);

                    showLaptop = true;
                    ShowLaptop(true);
                }
                animation.CrossFade("Capture");
                break;
            case UnitEngineer.EngineerState.Constructing:
                if (!showHammer)
                {
                    showHammer = true;
                    ShowHammer(true);

                    showLaptop = false;
                    ShowLaptop(false);
                }
                animation.CrossFade("Build");
                break;
        } // Switch

    } // Update
    private void UpdateIdle()
    {
        // plays the waiting idle animation
        timeToNextWaitAnimation -= Time.deltaTime;
        if (timeToNextWaitAnimation <= 0)
        {
            PlayAnimationCrossFade("Idle Wait");
            PlayAnimationCrossFadeQueued("Idle01");
            timeToNextWaitAnimation = Random.Range(5.0f, 15.0f);
        }
        else
        {
            if (!animation.IsPlaying("Idle Wait"))
                PlayAnimationCrossFade("Idle01");
        }
    }

    private void UpdateGoingTo()
    {
        PlayAnimationCrossFade("Walk");
    }

    private void UpdateGoingToAnEnemy()
    {
        PlayAnimationCrossFade("Walk");
    }

    private void UpdateAttacking()
    {
        if (attackCadenceAux <= 0.0f)
        {
            attackCadenceAux = 2.5f;
            animation.CrossFade("Attack1");
            if (!newFireball)
            {
                // Instanciate a new Fireball
                newFireball = Instantiate
                (
                    fireball,
                    dummyHand.transform.position,
                    //new Vector3(transform.position.x + 3.0f, 1.0f, transform.position.z),
                    new Quaternion()
                ) as GameObject;
                newFireball.rigidbody.isKinematic = false;
                newFireball.transform.name = "Fireball";
                newFireball.transform.parent = dummyHand;
                newFireball.transform.rotation = transform.rotation;
                newFireball.transform.FindChild("FireballVisionSphere").GetComponent<CFireballVisionSphere>().SetOwner(this.gameObject);
                newFireball.transform.FindChild("FireballVisionSphere").GetComponent<CFireballVisionSphere>().SetDamage(1);
                newFireball.transform.FindChild("FireballVisionSphere").GetComponent<CFireballVisionSphere>().SetDestroyTime(2.5f);
            }

            newFireball.rigidbody.AddForce(fireballDir, ForceMode.Impulse);

        }
        else
            attackCadenceAux -= Time.deltaTime;
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

    protected void PlayAnimation(string animationName)
    {
        animation.Play(animationName);
    }

    protected void PlayAnimationQueued(string animationName)
    {
        animation.PlayQueued(animationName);
    }

    protected void PlayAnimationCrossFade(string animationName)
    {
        animation.CrossFade(animationName);
    }

    protected void PlayAnimationCrossFadeQueued(string animationName)
    {
        animation.CrossFadeQueued(animationName);
    }

    public int GetUnitType()
    {
        return 3;
    }

    protected void RemoveAssetsFromModel()
    {
        // We destroy the Hammer
        Transform hammer1 = dummyHand.transform.FindChild("Hammer");
        if (hammer1 != null)
            GameObject.Destroy(hammer1.gameObject);
        // We destroy the Laptop
        Transform laptop1 = dummyLaptop.transform.FindChild("Laptop");
        if (laptop1 != null)
            GameObject.Destroy(laptop1.gameObject);
    }
}
