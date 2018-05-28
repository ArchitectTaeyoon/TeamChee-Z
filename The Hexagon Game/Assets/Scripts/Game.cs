using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public GameObject Module;
    //public int NumberOfModules;
    public List<GameObject> ModulesList;

    public bool reassigned = true;

	// Use this for initialization
	void Start () {
        ModulesList = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		if(GameObject.Find("Main Camera").GetComponent<CameraController>().mouseCarriesModule)
        {
            MaintainMovingModule(GameObject.Find("Main Camera").GetComponent<CameraController>().MovingModuleId);
        }
        else
        {
            GameObject.Find("Main Camera").GetComponent<CameraController>().MovingModuleId = -1;
            MaintainMovingModule(GameObject.Find("Main Camera").GetComponent<CameraController>().MovingModuleId);
        }

        if (!reassigned)
            ReassignModuleNumbers();
	}

    public void AddToModulesList(GameObject _Module)
    {
        ModulesList.Add(_Module);
    }

    //Move the right module and disable the movement of other
    public void MaintainMovingModule(int _ModuleId)
    {
        for(int i=0;i<ModulesList.Count;i++)
        {
            GameObject ChosenModule = ModulesList[i];
            if(ChosenModule.GetComponent<Module>().ModuleNumber == _ModuleId)
            {
                ChosenModule.GetComponent<FollowMouse>().Follow = true;
            }
            else
            {
                ChosenModule.GetComponent<FollowMouse>().Follow = false;
            }
        }
    }

    //Reassign module numbers after a module deletion
    public void ReassignModuleNumbers()
    {
        for (int i = 0; i < ModulesList.Count; i++)
        {
            GameObject ChosenModule = ModulesList[i];
            ChosenModule.GetComponent<Module>().ModuleNumber = i;
            ChosenModule.name = "Module " + i;
        }
        reassigned = true;
    }
}
