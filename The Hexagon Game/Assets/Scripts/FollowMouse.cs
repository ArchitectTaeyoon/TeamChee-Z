using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour {

    public float distance = 2.0f;
    public bool Follow = false;

    public bool SnappableModuleAvailable = false;
    public int SnappableModuleID;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (Follow)
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


            if (SnappableModuleAvailable)
            {
                if (Physics.Raycast(ray, out hitInfo, 10))
                {
                    transform.position = ClosestModuleSphere(hitInfo.point, GameObject.Find("Game").GetComponent<Game>().ModulesList[SnappableModuleID]);
                }
            }

            else
            {

                if (Physics.Raycast(ray, out hitInfo, 9))
                {
                    //Debug.Log(hitInfo.point);
                    Vector3 position = ClosestGridSphere(hitInfo.point).transform.position + GetComponent<Module>().directionVector;
                    transform.position = position;

                }
                else
                {
                    Vector3 mousePosition = Input.mousePosition;
                    mousePosition.z = distance;
                    transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
                }
            }

        }
	}

    public GameObject ClosestGridSphere(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / GameObject.Find("Grid").GetComponent<TriangularGrid>().GridXDimension);
        int y = Mathf.RoundToInt(position.y / GameObject.Find("Grid").GetComponent<TriangularGrid>().GridXDimension);
        int z = Mathf.RoundToInt(position.z / GameObject.Find("Grid").GetComponent<TriangularGrid>().GridZDimension);
        //Debug.Log(x + ", " + y + ", " + z);
        GameObject ClosestSphere = GameObject.Find("Grid").GetComponent<TriangularGrid>().gridSphereArray[x, 0, z];
        return ClosestSphere;
    }

    public void Place()
    {
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (SnappableModuleAvailable)
        {
            if (Physics.Raycast(ray, out hitInfo, 10))
            {
                transform.position = ClosestModuleSphere(hitInfo.point, GameObject.Find("Game").GetComponent<Game>().ModulesList[SnappableModuleID]);
                Follow = false;
                GameObject.Find("Main Camera").GetComponent<CameraController>().mouseCarriesModule = false;
                this.gameObject.GetComponent<Module>().LastPosition = transform.position;
                this.gameObject.GetComponent<Module>().placedOnce = true;
                Debug.Log("Placed at a module");
            }
        }

        else
        {
            if (Physics.Raycast(ray, out hitInfo, 9))
            {
                //Debug.Log(hitInfo.point);
                Vector3 position = ClosestGridSphere(hitInfo.point).transform.position + GetComponent<Module>().directionVector;
                transform.position = position;
                Follow = false;
                GameObject.Find("Main Camera").GetComponent<CameraController>().mouseCarriesModule = false;
                this.gameObject.GetComponent<Module>().LastPosition = transform.position;
                this.gameObject.GetComponent<Module>().placedOnce = true;
                Debug.Log("Placed at a grid");
            }
        }

        Debug.Log(this.gameObject.name + " is placed at " + this.gameObject.transform.position);
    }

    public Vector3 ClosestModuleSphere(Vector3 hitLocation,GameObject SpanningModule)
    {
        Debug.Log("Snapping Module: " + SpanningModule.name);
        GameObject closestSphere = SpanningModule.GetComponent<Module>().A_Spheres[0];
        float minDistance = 1000000.00f;
        for(int i = 0; i < SpanningModule.GetComponent<Module>().C_Spheres.Length; i++)
        {
            float distance = Vector3.Distance(SpanningModule.GetComponent<Module>().C_Spheres[i].transform.position, hitLocation);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestSphere = SpanningModule.GetComponent<Module>().C_Spheres[i];
            }
        }
        for (int i = 0; i < SpanningModule.GetComponent<Module>().B_Spheres.Length; i++)
        {
            float distance = Vector3.Distance(SpanningModule.GetComponent<Module>().B_Spheres[i].transform.position, hitLocation);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestSphere = SpanningModule.GetComponent<Module>().B_Spheres[i];
            }
        }
        Debug.Log("Closest distance: " + minDistance);
        Debug.Log("Closest Sphere: " + closestSphere.name);
        Debug.Log("Closest Sphere Direct location: " + closestSphere.transform.position);
        Debug.Log("Closest Sphere Inverse Transform location: " + SpanningModule.transform.TransformPoint(closestSphere.transform.position));
        return closestSphere.transform.position;
    }
}
