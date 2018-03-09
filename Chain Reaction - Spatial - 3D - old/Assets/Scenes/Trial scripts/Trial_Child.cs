using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trial_Child : MonoBehaviour {

    public GameObject Space;
    public Color Color0;
    public Color Color1;
    public Color Color2;

	// Use this for initialization
	void Start () {
        Space.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", Color0);
        Space.transform.GetChild(1).GetComponent<MeshRenderer>().material.SetColor("_Color", Color1);
        Space.transform.GetChild(2).GetComponent<MeshRenderer>().material.SetColor("_Color", Color2);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
