using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    public string Designation;
    public int IdNumber;

    public Vector3 LastPosition;
    public bool placedOnce;

    [Header("Spatial Picks")]
    public GameObject SpatialPickBedroom;
    public GameObject SpatialPickRestroom;
    public GameObject SpatialPickKitchen;
    public GameObject SpatialPickLiving;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (this.gameObject.GetComponent<HangOntoMouse>().Follow)
        {

            this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }
        else
        {
            this.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        }
	}

  
}

