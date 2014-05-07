using UnityEngine;
using System.Collections;

public class Warehouse : CResourceBuilding 
{
    private bool isActive = false;
    private RaycastHit myHit; // Structure used to get information back from a raycast.
    private Ray myRay;
    private int layerMask; // para obviar la capa de la niebla
    private Vector3 destiny;

    public Material activeMaterial;
    public Material constructMaterial;

    // Conts for Tower conquest
    private float contConstr;

    // Constant when the tower is constructed
    private const float finalCont = 100.0f;
    // The distance over the floor
    private const int delta = 7;

    private bool constructed = false;
    private bool canConstruct = true;

    private int costResources = 8;
    // Reference to the base
    public BaseController baseController;

    public float warehouseRadius = 12.0f;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        radious = warehouseRadius;
        myHit = new RaycastHit();
        // ejemplo Unity: http://docs.unity3d.com/Documentation/Components/Layers.html
        // Bit shift the index of the layer (9) to get a bit mask
        layerMask = 1 << 9;
    }

    public bool StartConstruct(Vector3 destiny, BaseController baseController)
    {
        if (canConstruct && baseController.GetResources() >= costResources)
        {
            this.baseController = baseController;
            baseController.DecreaseResources(costResources);
            this.GetComponent<NavMeshObstacle>().enabled = true;
            Vector3 posN = transform.position;
            posN.y = 0;
            transform.position = posN;
            isActive = true;

            float twoPi = Mathf.PI * 2;
            Vector3 center = transform.position;
            for (int i = 0; i < numEngineerPositions; i++)
            {
                Vector3 pos = new Vector3
                (
                    center.x +
                    (transform.GetComponent<BoxCollider>().size.x + despPosition) * Mathf.Sin(i * (twoPi / numEngineerPositions)),
                    0,
                    center.z +
                    (transform.GetComponent<BoxCollider>().size.x + despPosition) * Mathf.Cos(i * (twoPi / numEngineerPositions))
                );
                engineerPositions[i] = pos;
                engineerPosTaken[i] = false;

                cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cubes[i].transform.position = pos;
                Destroy(cubes[i].GetComponent<BoxCollider>());
                cubes[i].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);
                cubes[i].transform.parent = this.transform;
            }
            DestroyUnnecessaryGameobjects();
            return true;
        }
        return false;
    }

    public void SetActiveMaterial()
    {
        renderer.material = activeMaterial;
    }

    public void SetBaseController(BaseController baseController)
    {
        this.baseController = baseController;
    }

	// Update is called once per frame
    public override void Update ()
    {
        base.Update();

        if (GetComponent<CSelectable>().IsSelected() && Input.GetKey(KeyCode.Delete))
        {
            //TODO que ingenieros cambien de estado
            Destroy(gameObject);
            baseController.IncreaseResources(costResources);
        }
        else if (!isActive)
        {
            Light light = transform.FindChild("Light").light;
            if (canConstruct)
            {
                light.color = Color.green;
                renderer.material.SetColor("_AlphaColor", Color.green);
            }
            else
            {
                light.color = Color.red;
                renderer.material.SetColor("_AlphaColor", Color.red);
            }
            myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(myRay, out myHit, 1000f, layerMask))
            {
                destiny = new Vector3(myHit.point.x, myHit.point.y + delta, myHit.point.z);
                transform.position = destiny;
            }
        }
    }

    public void SetCanConstruct(bool b)
    {
        canConstruct = b;
    }

    public void DestroyUnnecessaryGameobjects()
    {
        // Remove unnecessary GameObjects
        Destroy(transform.FindChild("WarehouseBoxConstruct").gameObject);
        Destroy(transform.FindChild("Light").light);

    }

    // Construct is called by the engineers
    public bool Construct(float sum)
    {
        // increasement of the towers life
        if (!constructed)
        {
            if (contConstr < finalCont)
            {
                contConstr += sum;
                if (finalCont < contConstr)
                    contConstr = finalCont;
            }
            if (contConstr == finalCont)
            {
                life.currentLife = 50.0f;
                RemoveEngineersInQueue();
                for (int i = 0; i < numEngineerPositions; i++)
                    cubes[i].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);
                constructed = true;
                CResourceBuilding cR = this.GetComponent<CResourceBuilding>();        
                return true;
            }
            else
                return false;
        }
        return true;
    }

    public bool HasATeam()
    {
        return teamNumber != -1;
    }

    public bool IsConstructed()
    {
        return constructed;
    }

    public void LeaveEngineerPositionConstruct (int index)
    {
        engineerPosTaken[index] = false;
        cubes[index].renderer.material.color = new Color(0.196f, 0.804f, 0.196f);
        if (engineerQueue.Count > 0)
        {
            UnitEngineer unit = engineerQueue[0];
            unit.FinishWaitingToConstruct(engineerPositions[index], index);
            engineerQueue.RemoveAt(0);
            engineerPosTaken[index] = true;
            cubes[index].renderer.material.color = new Color(0.863f, 0.078f, 0.235f);
        }
    }

    public virtual void OnGUI()
    {
        Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
        Rect rect1;
        Rect rect2;
        if (constructed)
        {
            rect1 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 100.0f, 120.0f, 4.0f);
            rect2 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 100.0f, 120.0f * (life.currentLife / life.maximunLife), 4.0f);
        }
        else
        {
            rect1 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 100.0f, 120.0f, 4.0f);
            rect2 = new Rect(camPos.x - 60.0f, Screen.height - camPos.y - 100.0f, 120.0f * (contConstr / finalCont), 4.0f);
        }
        GUI.DrawTexture(rect1, progressBarEmpty);
        GUI.DrawTexture(rect2, progressBarFull);
    }
}
