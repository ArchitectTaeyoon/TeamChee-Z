using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangOntoMouse : MonoBehaviour {

    public bool placed = false;
    public bool placeable = true;

    public bool Follow = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (Follow)
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, GameObject.Find("Main Camera").GetComponent<CameraController>().WalkableSurface))
            {
                Debug.Log("Hit object: " + hitInfo.transform.gameObject.name);
                //hitInfo.transform.gameObject.GetComponent<MeshRenderer>().material = GameObject.Find("Main Camera").GetComponent<CameraController>().selectedModuleSphere;
                Vector3 position = hitInfo.point;
                transform.position = position;
                //GameObject.Find("Main Camera").GetComponent<CameraController>().RayEnd = position;
            }

            else
            {
                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, GameObject.Find("Main Camera").GetComponent<CameraController>().Grid))
                {
                    Debug.Log("Enters Grid.");
                    transform.position = hitInfo.point;
                    //GameObject.Find("Main Camera").GetComponent<CameraController>().RayEnd = hitInfo.point;
                }
                else
                {
                    Debug.Log("Exits Gridspace.");
                    //Vector3 mousePosition = Input.mousePosition;
                    //mousePosition.y = distance;
                    //transform.position = Camera.main.ScreenToWorldPoint(mousePosition) + this.GetComponent<Module>().ReferenceDisplacement;
                    transform.position = GameObject.Find("Main Camera").GetComponent<CameraController>().RayEnd;
                }
            }
        }
    }

    public void Place()
    {
        if (placeable)
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, GameObject.Find("Main Camera").GetComponent<CameraController>().WalkableSurface))
            {
                Debug.Log("Hit object: " + hitInfo.transform.gameObject.name);
                hitInfo.transform.gameObject.GetComponent<MeshRenderer>().material = GameObject.Find("Main Camera").GetComponent<CameraController>().selectedModuleSphere;
                Vector3 position = hitInfo.point;
                transform.position = position;
                //GameObject.Find("Main Camera").GetComponent<CameraController>().RayEnd = position;
                Follow = false;
                GameObject.Find("Main Camera").GetComponent<CameraController>().mouseCarriesHuman = false;
                this.gameObject.GetComponent<Agent>().LastPosition = position;
                this.gameObject.GetComponent<Agent>().placedOnce = true;
            }

            else
            {
                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, GameObject.Find("Main Camera").GetComponent<CameraController>().Grid))
                {
                    transform.position = hitInfo.point;
                    Follow = false;
                    GameObject.Find("Main Camera").GetComponent<CameraController>().mouseCarriesHuman = false;
                    //GameObject.Find("Main Camera").GetComponent<CameraController>().RayEnd = transform.position;
                    this.gameObject.GetComponent<Agent>().LastPosition = transform.position;
                    this.gameObject.GetComponent<Agent>().placedOnce = true;
                }
                
            }
        }
    }
}
