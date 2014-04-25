using UnityEngine;
using System.Collections;

public class UnitEngineerRemote : UnitController
{

    public int attackPower = 1;

    // tiempo en segundos que la unidad tarda en realizar una construccion, conquista y/o reparacion
    public int engineerTime = 1;
    private float actualEngineerTime = 0;

    // cantidad de construccion, conquista y/o reparacion por unidad de recolección
    public int amountPerAction = 2;

    public UnitEngineer.EngineerState currentEngineerState = UnitEngineer.EngineerState.None;

    // referencia al item que se está construyendo, conquistando o reparando
    //private Transform currentItem;

    //private Vector3 lastEngineerPos;
    //private int lastEngineerIndex;

    // To instanciate a towerGoblin and warehouse
    public GameObject towerGoblinPrefab;
    public GameObject warehousePrefab;
    // The items towerGoblin and warehouse
    private GameObject towerGoblin;
    private GameObject warehouse;

    private bool newTGConstruct = false;
    private bool newWConstruct = false;
    private Vector3 constructDestiny = new Vector3();

    // dummy where is going to be instanciated a laptop
    public Transform dummyLaptop;
    public Transform dummyHand;

    // reference to the laptop and hammer
    public GameObject laptop;
    public GameObject hammer;

    //For attacking1
    public GameObject fireball;
    private GameObject newFireball;

    // To knows if we done the job
    public bool construct;
    public bool conquest;

    public override void Awake()
    {
        base.Awake();

        // Por si no se han establecido las referencias a los dummys del modelo
        // en el editor de Unity las buscamos ahora:
        if (dummyLaptop == null)
            dummyLaptop = transform.FindChild("Bip001/Bip001 Footsteps");
        if (dummyHand == null)
            dummyHand = transform.FindChild("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/Mano Der");
    }

    public override void Start()
    {
        base.Start();
        basicAttackPower = secondaryAttackPower = attackPower;
        attackCadenceAux = 2.5f;
        attackCadence = 3.2f;
        construct = conquest = false;
    }

    // Update is called once per frame
    public override void Update()
    {
        if (life.currentLife <= 0.0f)
        {
            //LeaveQueues();
            currentEngineerState = UnitEngineer.EngineerState.None;
        }
        switch (currentEngineerState)
        {
            case UnitEngineer.EngineerState.None:
                base.Update();
                break;
            case UnitEngineer.EngineerState.GoingToRepairItem:
                base.Update();
                break;
            case UnitEngineer.EngineerState.GoingToConquerableItem:
                base.Update();
                break;
            case UnitEngineer.EngineerState.GoingToConstructItem:
                base.Update();
                break;
            case UnitEngineer.EngineerState.Waiting:
                animation.CrossFade("Idle Wait");
                break;
            case UnitEngineer.EngineerState.GoingToRepairPosition:
                if (currentState == State.Idle)
                {
                    // when it have arrived to the repair position
                    currentEngineerState = UnitEngineer.EngineerState.Repairing;

                    // We instanciate a Hammer
                    GameObject newHammer = Instantiate
                    (
                        hammer,
                        dummyHand.transform.position,
                        new Quaternion()
                    ) as GameObject;
                    newHammer.transform.name = "Hammer";
                    newHammer.transform.parent = dummyHand;
                    newHammer.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
                }
                else
                    base.Update();
                break;
            case UnitEngineer.EngineerState.GoingToConquestPosition:
                if (currentState == State.Idle)
                {
                    // when it have arrived to the conquest position
                    currentEngineerState = UnitEngineer.EngineerState.Conquering;

                    // We instanciate a laptop
                    GameObject newLaptop = Instantiate
                    (
                        laptop,
                        dummyLaptop.transform.position,
                        new Quaternion()
                    ) as GameObject;
                    newLaptop.transform.name = "Laptop";
                    newLaptop.transform.parent = dummyLaptop;
                    newLaptop.transform.rotation = transform.rotation;
                }
                else
                    base.Update();
                break;
            case UnitEngineer.EngineerState.GoingToConstructPosition:
                if (currentState == State.Idle)
                {
                    // when it have arrived to the construct position
                    currentEngineerState = UnitEngineer.EngineerState.Constructing;
                    if (newTGConstruct)
                        Minimap.InsertTower(towerGoblin.GetComponent<Tower>());
                    else if (newWConstruct)
                        Minimap.InsertWarehouse(warehouse.GetComponent<Warehouse>());
                    newTGConstruct = newWConstruct = false;

                    // intanciamos un Hammer
                    GameObject newHammer = Instantiate
                    (
                        hammer,
                        dummyHand.transform.position,
                        new Quaternion()
                    ) as GameObject;
                    newHammer.transform.name = "Hammer";
                    newHammer.transform.parent = dummyHand;
                    newHammer.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
                }
                else
                    base.Update();
                break;
            case UnitEngineer.EngineerState.Repairing:
                animation.CrossFade("Build");
                break;
            case UnitEngineer.EngineerState.Conquering:
                animation.CrossFade("Capture");
                if (conquest)
                {
                    animation.CrossFade("Idle01");
                    // We destroy the Laptop
                    Transform laptop1 = dummyLaptop.transform.FindChild("Laptop");
                    if (laptop1 != null)
                        GameObject.Destroy(laptop1.gameObject);
                }
                break;
            case UnitEngineer.EngineerState.Constructing:
                animation.CrossFade("Build");
                if (construct)
                {
                    animation.CrossFade("Idle01");
                    // We destroy the Hammer
                    Transform hammer1 = dummyHand.transform.FindChild("Hammer");
                    if (hammer1 != null)
                        GameObject.Destroy(hammer1.gameObject);                    
                }
                break;
        } // Switch

    } // Update

 /* private void LeaveQueues()
    {
        // He has to leave the engineerPosition if he has to
        if (currentEngineerState == UnitEngineer.EngineerState.GoingToConquestPosition || currentEngineerState == UnitEngineer.EngineerState.Conquering)
            currentItem.GetComponent<TowerNeutral>().LeaveEngineerPositionConquest(lastEngineerIndex);
        else if (currentEngineerState == UnitEngineer.EngineerState.GoingToRepairPosition || currentEngineerState == UnitEngineer.EngineerState.Repairing)
            currentItem.GetComponent<BuildingController>().LeaveEngineerPositionRepair(lastEngineerIndex);
        else if (currentEngineerState == UnitEngineer.EngineerState.GoingToConstructPosition ||
                    currentEngineerState == UnitEngineer.EngineerState.Constructing)
        {
            if (currentItem.GetComponent<TowerGoblin>() != null)
                currentItem.GetComponent<TowerGoblin>().LeaveEngineerPositionConstruct(lastEngineerIndex);
            else if (currentItem.GetComponent<Warehouse>() != null)
                currentItem.GetComponent<Warehouse>().LeaveEngineerPositionConstruct(lastEngineerIndex);
        }
        else if (currentEngineerState == UnitEngineer.EngineerState.Waiting)
            currentItem.GetComponent<BuildingController>().LeaveQueue(this);
    }*/



    public bool IsNewConstructing()
    {
        return newTGConstruct || newWConstruct;
    }

    public void SetCanConstruct(int item)
    {
        switch (item)
        {
            case 0:
                towerGoblin = Instantiate(towerGoblinPrefab, new Vector3(Input.mousePosition.x, 0, Input.mousePosition.z), new Quaternion(0, 0, 0, 0))
                    as GameObject; //rotation (-90, -180, 0)
                towerGoblin.name = towerGoblin.name.Replace("(Clone)", "");
                towerGoblin.GetComponent<TowerGoblin>().SetTeamNumber(this.teamNumber);
                newTGConstruct = true;
                break;
            case 1:
                warehouse = Instantiate(warehousePrefab, new Vector3(Input.mousePosition.x, 0, Input.mousePosition.z), new Quaternion(0, 0, 0, 0))
                    as GameObject; //rotation (-90, -180, 0)
                warehouse.name = warehouse.name.Replace("(Clone)", "");
                warehouse.GetComponent<Warehouse>().SetTeamNumber(this.teamNumber);
                newWConstruct = true;
                break;
        }
    }

    public override int GetUnitType()
    {
        return 3;
    }

 /*   protected override void UpdateGoingToAnEnemy()
    {
        // 1- comprobamos si el enemigo está "a mano" y se le puede atacar
        float distToEnemy = Vector3.Distance(transform.position, enemySelected.transform.position);
        if (distToEnemy <= maxAttackDistance)
        {
            // change to Attack state
            currentState = State.Attacking;
            PlayAnimationCrossFade("Attack1");
            GetComponent<NavMeshAgent>().destination = transform.position;

            transform.LookAt(enemySelected.transform);


            /* if (newFireball)
                 Destroy(newFireball.gameObject);
        }
        // 2- comprobamos si el enemigo esta "a vista"
        else if (distToEnemy <= visionSphereRadius)
        {
            this.destiny = enemySelected.transform.position;
            GetComponent<NavMeshAgent>().destination = destiny;
        }
        // 3- se ha llegado al destino y se ha perdido de vista al enemigo
        else if (Vector3.Distance(transform.position, destiny) <= destinyThreshold)
        {
            StopMoving();
        }
    }*/

    protected override void UpdateAttacking()
    {
        attackCadenceAux -= Time.deltaTime;

        float enemyDist = Vector3.Distance(transform.position, enemySelected.transform.position);
        if (enemySelected)
        {
            if (enemyDist <= maxAttackDistance)
            {
                if (attackCadenceAux <= 0)
                {
                    transform.LookAt(enemySelected.transform);

                    if (!newFireball)
                    {
                        // Instanciate a new Fireball
                        Debug.Log("Dummy position: " + dummyHand.transform.position);
                        Debug.Log("Engineer position: " + transform.position);
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
                        newFireball.transform.FindChild("FireballVisionSphere").GetComponent<CFireballVisionSphere>().SetDamage((int)attackPower);
                        newFireball.transform.FindChild("FireballVisionSphere").GetComponent<CFireballVisionSphere>().SetDestroyTime(2.5f);
                    }

                    attackCadenceAux = attackCadence;
                    Vector3 dir = enemySelected.transform.position - newFireball.transform.position;
                    dir = dir.normalized;
                    Vector3 dir1 = transform.forward.normalized;
                    newFireball.transform.parent = null;
                    newFireball.rigidbody.AddForce(new Vector3(dir.x * 8.0f * (enemyDist / maxAttackDistance),
                                                                        7,
                                                                dir.z * 8.0f * (enemyDist / maxAttackDistance)), ForceMode.Impulse);

                    //newFireball.GetComponent<SphereCollider>().isTrigger = true;

                    //newFireball.rigidbody.AddForce(fireball.transform.forward * 500);
                    if (enemySelected.life.currentLife <= 0.0f)
                    {
                        // the enemy has die
                        enemySelected = null;
                        currentState = State.Idle;

                        PlayAnimationCrossFade("Idle01");
                        attackCadenceAux = 2.5f;
                    }
                }
            }
            else if (enemyDist <= visionSphereRadius)
            {
                currentState = State.GoingToAnEnemy;

                this.destiny = enemySelected.transform.position;
                GetComponent<NavMeshAgent>().destination = destiny;

                PlayAnimationCrossFade("Walk");
                attackCadenceAux = 2.5f;
            }
            else
            {
                enemySelected = null;
                currentState = State.Idle;
                attackCadenceAux = 2.5f;
                PlayAnimationCrossFade("Idle01");
            }
        }
        else // the enemy is no longer alive
        {
            enemySelected = null;
            currentState = State.Idle;

            PlayAnimationCrossFade("Idle01");
            attackCadenceAux = 2.5f;
        }
    }

    protected override void RemoveAssetsFromModel()
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
