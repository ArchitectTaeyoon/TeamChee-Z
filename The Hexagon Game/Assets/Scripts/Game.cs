using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public GameObject Module;
    public int NumberOfModules;
    public List<GameObject> ModulesList;

	// Use this for initialization
	void Start () {
        ModulesList = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddToModulesList(GameObject _Module)
    {
        ModulesList.Add(_Module);
    }
}
