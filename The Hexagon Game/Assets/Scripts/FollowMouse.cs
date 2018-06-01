using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour {

    public float distance =200.0f;
    public bool Follow = false;

    public bool SnappableModuleAvailable = false;
    public int SnappableModuleID;

    public bool Placeable = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (Follow)
        {
            //GetComponent<Rigidbody>().detectCollisions = false;
            //GetComponent<Module>().CollisionDetection();
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, GameObject.Find("Main Camera").GetComponent<CameraController>().Spheres))
            {
                Debug.Log("Hit object: " + hitInfo.transform.gameObject.name);
                hitInfo.transform.gameObject.GetComponent<MeshRenderer>().material = GameObject.Find("Main Camera").GetComponent<CameraController>().selectedModuleSphere;
                Vector3 position = hitInfo.transform.gameObject.transform.position + this.GetComponent<Module>().ReferenceDisplacement;
                //GameObject.Find("Main Camera").GetComponent<CameraController>().RayEnd = position;
                transform.position = position;
            }

            else
            {
                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, GameObject.Find("Main Camera").GetComponent<CameraController>().Grid))
                {
                    //Debug.Log(hitInfo.point);
                    Vector3 position = ClosestGridSphere(hitInfo.point).transform.position + this.GetComponent<Module>().ReferenceDisplacement;
                    //Vector3 position = hitInfo.transform.gameObject.transform.position+ this.GetComponent<Module>().ReferenceDisplacement;
                    ClosestGridSphere(hitInfo.point).GetComponent<MeshRenderer>().material = GameObject.Find("Main Camera").GetComponent<CameraController>().highlightedGridSphere;
                    ///hitInfo.transform.gameObject.GetComponent<MeshRenderer>().material = GameObject.Find("Main Camera").GetComponent<CameraController>().highlightedGridSphere;
                   // GameObject.Find("Main Camera").GetComponent<CameraController>().RayEnd = position;
                    transform.position = position;
                }
                else
                {
                    //Vector3 mousePosition = Input.mousePosition;
                    //mousePosition.y = distance;
                    //transform.position = Camera.main.ScreenToWorldPoint(mousePosition) + this.GetComponent<Module>().ReferenceDisplacement;
                    transform.position = GameObject.Find("Main Camera").GetComponent<CameraController>().RayEnd + this.GetComponent<Module>().ReferenceDisplacement;
                }
            }
        }
	}

    public GameObject ClosestGridSphere(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / GameObject.Find("Grid").GetComponent<TriangularGrid>().GridXDimension);
        int z = Mathf.RoundToInt(position.z / GameObject.Find("Grid").GetComponent<TriangularGrid>().GridZDimension);
        //Debug.Log(x + ", " + y + ", " + z);
        GameObject ClosestSphere = GameObject.Find("Grid").GetComponent<TriangularGrid>().gridSphereArray[x, z];
        return ClosestSphere;
    }

    public void Place()
    {
        if (Placeable)
        {
            if (GameObject.Find("Game").GetComponent<Game>().ModulesList[GameObject.Find("Main Camera").GetComponent<CameraController>().MovingModuleId].GetComponent<Module>().Colliding != true)
            {
                RaycastHit hitInfo;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, GameObject.Find("Main Camera").GetComponent<CameraController>().Spheres))
                {
                    hitInfo.transform.gameObject.GetComponent<MeshRenderer>().material = GameObject.Find("Main Camera").GetComponent<CameraController>().selectedModuleSphere;
                    Vector3 position = hitInfo.transform.gameObject.transform.position + this.GetComponent<Module>().ReferenceDisplacement;
                    //GameObject.Find("Main Camera").GetComponent<CameraController>().RayEnd = position;
                    transform.position = position;
                    Follow = false;
                    GameObject.Find("Main Camera").GetComponent<CameraController>().mouseCarriesModule = false;
                    this.gameObject.GetComponent<Module>().LastPosition = transform.position;
                    this.gameObject.GetComponent<Module>().placedOnce = true;
                }
                else
                {
                    if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, GameObject.Find("Main Camera").GetComponent<CameraController>().Grid))
                    {
                        //Debug.Log(hitInfo.point);
                        //Vector3 position = hitInfo.transform.gameObject.transform.position + this.GetComponent<Module>().ReferenceDisplacement;
                        Vector3 position = ClosestGridSphere(hitInfo.point).transform.position + this.GetComponent<Module>().ReferenceDisplacement;
                        ClosestGridSphere(hitInfo.point).GetComponent<MeshRenderer>().material = GameObject.Find("Main Camera").GetComponent<CameraController>().highlightedGridSphere;
                        //hitInfo.transform.gameObject.GetComponent<MeshRenderer>().material = GameObject.Find("Main Camera").GetComponent<CameraController>().highlightedGridSphere;
                        //GameObject.Find("Main Camera").GetComponent<CameraController>().RayEnd = position;
                        transform.position = position;

                        Follow = false;
                        GameObject.Find("Main Camera").GetComponent<CameraController>().mouseCarriesModule = false;
                        this.gameObject.GetComponent<Module>().LastPosition = transform.position;
                        this.gameObject.GetComponent<Module>().placedOnce = true;
                        //Debug.Log("Placed at a grid");
                    }
                }
                Debug.Log(this.gameObject.name + " is placed at " + this.gameObject.transform.position);
            }
        }
    }
}
