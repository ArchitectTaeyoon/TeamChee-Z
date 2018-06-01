using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

    [Header("Camera handling speed")]
    public float zoomSpeed = 5.0f;
    public float panSpeed = 5.0f;
    public float turnSpeed = 5.0f;

    [Header("Module selection material")]
    public Material unselectedModuleFrame;
    public Material selectedModuleFrame;
    public Material highlightedModuleFrame;
    public Material unselectedGridSphere;
    public Material highlightedGridSphere;
    public Material unselectedModuleSphere;
    public Material selectedModuleSphere;
    public Material collidingModuleFrame;

    Vector3 mousePosition;
    
    public bool mouseInsideScreen = false;
    public bool mouseCarriesModule = false;
    public bool mouseCarriesHuman = false;
    public bool DisplayFramework = true;

    GameObject InstantiatedModule;
    GameObject SelectedModule;
    GameObject ModulePrefab;
    public int MovingModuleId;
    public int MovingHumanId;
    public string MovingHumanDesignation;

    int MouseClickCounter = 0;
    float DoubleClickInterval = 0.2f;

    //LayerMasks
    public LayerMask Grid;
    public LayerMask Spheres;
    public LayerMask Module;
    public LayerMask Humans;
    public LayerMask HumanOnUI;
    public LayerMask Framework;
    public LayerMask ActualModule;
    public LayerMask WalkableSurface;

    public Vector3 RayEnd;

    public Button RealWorldView;
    public Button FrameworkView;

	// Use this for initialization
	void Start () {
        ModulePrefab = GameObject.Find("Game").GetComponent<Game>().Module; 
	}
	
	// Update is called once per frame
	void Update () {

        CastRay();
        
        CheckMouseInsideScreen();
        if (MouseClickCounter > 2)
            MouseClickCounter = 0;

        UpdateWorldView();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleWorldView();
        }

        if (mouseInsideScreen && GameObject.Find("Game").GetComponent<Game>().reassignedModules && GameObject.Find("Game").GetComponent<Game>().reassignedHumans)
        {
            
            if (mouseCarriesModule)
            {
                if (Input.GetMouseButton(0))
                {
                    GameObject.Find("Game").GetComponent<Game>().ModulesList[MovingModuleId].GetComponent<FollowMouse>().Place();
                }

                if (Input.GetKeyDown(KeyCode.W))
                {
                    GameObject.Find("Game").GetComponent<Game>().ModulesList[MovingModuleId].GetComponent<Module>().SwitchReferenceSphereForward();
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    GameObject.Find("Game").GetComponent<Game>().ModulesList[MovingModuleId].GetComponent<Module>().SwitchReferenceSphereBackward();
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    GameObject.Find("Game").GetComponent<Game>().ModulesList[MovingModuleId].GetComponent<Module>().RotateReferenceSpheres();
                    GameObject.Find("Game").GetComponent<Game>().ModulesList[MovingModuleId].GetComponent<Module>().RotateModule();
                }

                //if (GameObject.Find("Game").GetComponent<Game>().ModulesList[MovingModuleId].GetComponent<Module>().placedOnce == true)
                //{
                    
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        GameObject.Find("Game").GetComponent<Game>().ModulesList[MovingModuleId].GetComponent<Module>().OnMove();
                        if (GameObject.Find("Game").GetComponent<Game>().ModulesList[MovingModuleId].GetComponent<Module>().placedOnce)
                        {
                            GameObject.Find("Game").GetComponent<Game>().ModulesList[MovingModuleId].transform.position = GameObject.Find("Game").GetComponent<Game>().ModulesList[MovingModuleId].GetComponent<Module>().LastPosition;
                            GameObject.Find("Game").GetComponent<Game>().ModulesList[MovingModuleId].GetComponent<FollowMouse>().Follow = false;
                            mouseCarriesModule = false;
                            GameObject.Find("Game").GetComponent<Game>().ModulesList[MovingModuleId].GetComponent<Module>().LastPosition = transform.position;
                        }
                        else
                        {
                            GameObject.Find("Game").GetComponent<Game>().reassignedModules = false;
                            Destroy(GameObject.Find("Game").GetComponent<Game>().ModulesList[MovingModuleId].gameObject);
                            GameObject.Find("Game").GetComponent<Game>().ModulesList.RemoveAt(GameObject.Find("Game").GetComponent<Game>().ModulesList[MovingModuleId].GetComponent<Module>().ModuleNumber);
                            mouseCarriesModule = false;
                        }
                    }
                //}

                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    GameObject.Find("Game").GetComponent<Game>().reassignedModules = false;
                    Destroy(GameObject.Find("Game").GetComponent<Game>().ModulesList[MovingModuleId].gameObject);
                    GameObject.Find("Game").GetComponent<Game>().ModulesList.RemoveAt(GameObject.Find("Game").GetComponent<Game>().ModulesList[MovingModuleId].GetComponent<Module>().ModuleNumber);
                    mouseCarriesModule = false;
                }
            }
            else if (mouseCarriesHuman)
            {
                if (Input.GetMouseButton(0))
                {
                    GameObject.Find("Game").GetComponent<Game>().GetMovingHuman().GetComponent<HangOntoMouse>().Place();
                }
            }
            else
            {
                //Select a module
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hitInfo;
                    bool humanUIhit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, HumanOnUI);
                    if (humanUIhit)
                    {
                        SelectHumanFromUI();
                        return;
                    }
                        // Debug.Log("Mouse Click: " + MouseClickCounter);
                        MouseClickCounter++;
                    if(MouseClickCounter==1)
                        StartCoroutine("DoubleClickDetector");
                }
            }

            //Zoom in
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                //GetComponent<Transform>().position = new Vector3(transform.position.x, transform.position.y - (zoomSpeed * Time.deltaTime), transform.position.z + (zoomSpeed * Time.deltaTime));
                //transform.Rotate(zoomSpeed * Time.deltaTime * -1, 0, 0);
                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = 100.0f;
                transform.position = Vector3.MoveTowards(transform.position,Camera.main.ScreenToWorldPoint(mousePosition), zoomSpeed);
            }

            //Zoom out
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                //GetComponent<Transform>().position = new Vector3(transform.position.x, transform.position.y + (zoomSpeed * Time.deltaTime), transform.position.z - (zoomSpeed * Time.deltaTime));
                //transform.Rotate(zoomSpeed * Time.deltaTime, 0, 0);
                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = -100.0f;
                transform.position = Vector3.MoveTowards(transform.position, Camera.main.ScreenToWorldPoint(mousePosition), zoomSpeed);
            }

            //Rotate
            if (Input.GetMouseButtonDown(1))
            {
                mousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(1))
            {
                Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mousePosition);
                transform.RotateAround(transform.position, transform.right, -pos.y * turnSpeed);
                transform.RotateAround(transform.position, Vector3.up, pos.x * turnSpeed);
            }

            //Pan
            if (Input.GetMouseButtonDown(2))
            {
                mousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(2))
            {
                Vector3 pos = Camera.main.ScreenToViewportPoint(mousePosition - Input.mousePosition);

                Vector3 move = new Vector3(pos.x * panSpeed, pos.y * panSpeed, 0);
                transform.Translate(move, Space.Self);
            }
        }
    }

    public void CheckMouseInsideScreen()
    {
        if(Input.mousePosition.x>=0 && Input.mousePosition.x <= Screen.width && Input.mousePosition.y>=0 && Input.mousePosition.y<=Screen.height)
        {
            mouseInsideScreen = true;
        }
        else
        {
            mouseInsideScreen = false;
        }
    }

    public void InstantiateModule()
    {
        if (!mouseCarriesModule && !mouseCarriesHuman)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 100.0f;
            InstantiatedModule = Instantiate(ModulePrefab, new Vector3(0,0,0), Quaternion.Euler(0,90,0));
            GameObject.Find("Game").GetComponent<Game>().AddToModulesList(InstantiatedModule);
            InstantiatedModule.GetComponent<Module>().ModuleNumber = GameObject.Find("Game").GetComponent<Game>().ModulesList.Count-1;
            InstantiatedModule.name = "Module " + InstantiatedModule.GetComponent<Module>().ModuleNumber.ToString();
           
            MovingModuleId = InstantiatedModule.GetComponent<Module>().ModuleNumber;
           
            mouseCarriesModule = true;

            //InstantiatedModule.GetComponent<FollowMouse>().Follow = true;
        }
    }

    public void SelectModule()
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity ,Module);
        if (hit)
        {
            SelectedModule = hitInfo.transform.gameObject;
            MovingModuleId = SelectedModule.GetComponent<Module>().ModuleNumber;
            Debug.Log("Selected Module: " + SelectedModule.name);
            mouseCarriesModule = true;
            StartCoroutine("SuspendModulePlacing");
            //SelectedModule.GetComponent<FollowMouse>().Follow = true;
        }
        
    }

    IEnumerator DoubleClickDetector()
    {
        yield return new WaitForSeconds(DoubleClickInterval);
        if (MouseClickCounter > 1)
        {
            Debug.Log("Double Click.");

            SelectModule();
        }
        MouseClickCounter = 0;
        yield return new WaitForSeconds(0.05f);
    }

    //Cast Ray onto screen
    public void CastRay()
    {
        GetComponent<LineRenderer>().enabled = true;
        RaycastHit hit;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);


        if (Physics.Raycast(ray, out hit))
        {
            RayEnd = hit.point;

        }
        else
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 2000.0f;
            RayEnd = Camera.main.ScreenToWorldPoint(mousePosition);
    }

    GetComponent<LineRenderer>().SetPosition(0, this.transform.position + Vector3.up * -0.05f);
        GetComponent<LineRenderer>().SetPosition(1, RayEnd);
    }

    public void ToggleWorldView()
    {
        GameObject[] GuideSpheres = GameObject.FindGameObjectsWithTag("GuidingSphere");
        if (DisplayFramework == true) {
            DisplayFramework = false;
        }
        else
        {
            DisplayFramework = true;
        }
    }

    public void UpdateWorldView()
    {
        GameObject[] GuideSpheres = GameObject.FindGameObjectsWithTag("GuidingSphere");
        GameObject[] Framework = GameObject.FindGameObjectsWithTag("Framework");
        GameObject[] ActualModules = GameObject.FindGameObjectsWithTag("ActualModule");
        if (DisplayFramework)
        {
            RealWorldView.enabled = true;
            RealWorldView.gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            FrameworkView.gameObject.GetComponent<Image>().color = new Color(180, 180, 180, 255);
            FrameworkView.enabled = false;
            
            for (int i = 0; i < GuideSpheres.Length; i++)
            {
                GuideSpheres[i].GetComponent<MeshRenderer>().enabled = true;
            }
            for (int i = 0; i < Framework.Length; i++)
            {
                Framework[i].GetComponent<MeshRenderer>().enabled = true;
            }
            for (int i = 0; i < ActualModules.Length; i++)
            {
                ActualModules[i].GetComponent<MeshRenderer>().enabled = false;
            }
        }
        else
        {
            RealWorldView.gameObject.GetComponent<Image>().color = new Color(180, 180, 180, 255);
            RealWorldView.enabled = false;
            
            //RealWorldView.GetComponent<Image>().color = new Color(180, 180, 180, 255);
            FrameworkView.enabled = true;
            FrameworkView.gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            for (int i = 0; i < GuideSpheres.Length; i++)
            {
                GuideSpheres[i].GetComponent<MeshRenderer>().enabled = false;
            }
            for (int i = 0; i < Framework.Length; i++)
            {
                Framework[i].GetComponent<MeshRenderer>().enabled = false;
            }
            for (int i = 0; i < ActualModules.Length; i++)
            {
                ActualModules[i].GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }


    IEnumerator SuspendModulePlacing()
    {
        GameObject.Find("Game").GetComponent<Game>().ModulesList[MovingModuleId].GetComponent<FollowMouse>().Placeable = false;
        //mouseCarriesModule = false;
        yield return new WaitForSeconds(1.0f);
        GameObject.Find("Game").GetComponent<Game>().ModulesList[MovingModuleId].GetComponent<FollowMouse>().Placeable = true;
        //mouseCarriesModule = true;
    }

    public void SelectHuman()
    {

    }

    public void SelectHumanFromUI()
    {
        if(!mouseCarriesHuman && !mouseCarriesModule)
        {
            RaycastHit hitInfo;
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, HumanOnUI);
            if (hit)
            {
                mouseCarriesHuman = true;
                string humanDesignation = hitInfo.transform.gameObject.GetComponent<Agent>().Designation;
                GameObject CreatedHuman = GameObject.Find("Canvas").GetComponent<InstantiateHumansInUI>().InstantiateHumanforWorldSpace(humanDesignation);
                GameObject.Find("Game").GetComponent<Game>().AddToHumansList(CreatedHuman);
                CreatedHuman.name = humanDesignation + " " + CreatedHuman.GetComponent<Agent>().IdNumber;
                
                MovingHumanId = CreatedHuman.GetComponent<Agent>().IdNumber;
                MovingHumanDesignation = humanDesignation;
                StartCoroutine("SuspendHumanPlacing");
            }
        }

    }

    //Suspend human placing
    IEnumerator SuspendHumanPlacing()
    {
        GameObject.Find("Game").GetComponent<Game>().GetMovingHuman().GetComponent<HangOntoMouse>().placeable = false;
        yield return new WaitForSeconds(0.5f);
        GameObject.Find("Game").GetComponent<Game>().GetMovingHuman().GetComponent<HangOntoMouse>().placeable = true;
    }
}
