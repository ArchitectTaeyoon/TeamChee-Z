using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float zoomSpeed = 5.0f;
    public float panSpeed = 5.0f;
    public float turnSpeed = 5.0f;

    Vector3 mousePosition;
    bool mouseInsideScreen = false;
    bool mouseCarriesModule = false;

    GameObject InstantiatedModule;
    GameObject ModulePrefab;

	// Use this for initialization
	void Start () {
        ModulePrefab = GameObject.Find("Game").GetComponent<Game>().Module;
	}
	
	// Update is called once per frame
	void Update () {

        CheckMouseInsideScreen();

        if (mouseInsideScreen)
        {
            if (mouseCarriesModule)
            {
               // mousePosition = Input.mousePosition;
               // InstantiatedModule.transform.position = mousePosition;
            }

            //Zoom in
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                GetComponent<Transform>().position = new Vector3(transform.position.x, transform.position.y - (zoomSpeed * Time.deltaTime), transform.position.z + (zoomSpeed * Time.deltaTime));
                transform.Rotate(zoomSpeed * Time.deltaTime * -1, 0, 0);
            }

            //Zoom out
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                GetComponent<Transform>().position = new Vector3(transform.position.x, transform.position.y + (zoomSpeed * Time.deltaTime), transform.position.z - (zoomSpeed * Time.deltaTime));
                transform.Rotate(zoomSpeed * Time.deltaTime, 0, 0);
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
        if (!mouseCarriesModule)
        {
            InstantiatedModule = Instantiate(ModulePrefab, Input.mousePosition, Quaternion.Euler(0,90,0));
            InstantiatedModule.name = "Module " + GameObject.Find("Game").GetComponent<Game>().NumberOfModules.ToString();
            GameObject.Find("Game").GetComponent<Game>().AddToModulesList(InstantiatedModule);
            mouseCarriesModule = true;
            InstantiatedModule.GetComponent<FollowMouse>().Follow = true;
        }
    }
}
