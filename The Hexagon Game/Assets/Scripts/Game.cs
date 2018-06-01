using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public GameObject Module;
    //public int NumberOfModules;
    public List<GameObject> ModulesList;

    public bool reassignedModules = true;
    public bool reassignedHumans = true;

    public List<GameObject> ManList;
    public List<GameObject> WomanList;
    public List<GameObject> UsersWithSpecialNeedsList;
    public List<GameObject> BabyList;
    public List<GameObject> BoyList;
    public List<GameObject> GirlList;

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

        if(GameObject.Find("Main Camera").GetComponent<CameraController>().mouseCarriesHuman)
        {
            MaintainMovingHuman(GameObject.Find("Main Camera").GetComponent<CameraController>().MovingHumanId, GameObject.Find("Main Camera").GetComponent<CameraController>().MovingHumanDesignation);
        }
        else
        {
            GameObject.Find("Main Camera").GetComponent<CameraController>().MovingHumanId = -1;
            GameObject.Find("Main Camera").GetComponent<CameraController>().MovingHumanDesignation = "";
            ImmobilizeAllHumans();
        }
        if (!reassignedModules)
        {
            ReassignModuleNumbers();
        }
        if (!reassignedHumans)
        {
            ReassignHumans();
        }
            
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
        reassignedModules = true;
    }

    public void AddToHumansList(GameObject _human)
    {
        string designation = _human.GetComponent<Agent>().Designation;
        switch (designation)
        {
            case "Baby":
                BabyList.Add(_human);
                _human.GetComponent<Agent>().IdNumber = BabyList.Count - 1;
                break;
            case "Boy":
                BoyList.Add(_human);
                _human.GetComponent<Agent>().IdNumber = BoyList.Count - 1;
                break;
            case "Girl":
                GirlList.Add(_human);
                _human.GetComponent<Agent>().IdNumber = GirlList.Count - 1;
                break;
            case "Users with Special Needs":
                UsersWithSpecialNeedsList.Add(_human);
                _human.GetComponent<Agent>().IdNumber = UsersWithSpecialNeedsList.Count - 1;
                break;
            case "Man":
                ManList.Add(_human);
                _human.GetComponent<Agent>().IdNumber = ManList.Count - 1;
                break;
            case "Woman":
                WomanList.Add(_human);
                _human.GetComponent<Agent>().IdNumber = WomanList.Count - 1;
                break;
            default:
                Debug.Log("Wrong keyword.");
                break;
        }
    }

    public void MaintainMovingHuman(int MovingHumanId, string MovingHumanDesignation)
    {
        switch (MovingHumanDesignation)
        {
            case "Baby":
                BabyList[MovingHumanId].GetComponent<HangOntoMouse>().Follow = true;
                break;
            case "Boy":
                BoyList[MovingHumanId].GetComponent<HangOntoMouse>().Follow = true;
                break;
            case "Girl":
                GirlList[MovingHumanId].GetComponent<HangOntoMouse>().Follow = true;
                break;
            case "Users with Special Needs":
                UsersWithSpecialNeedsList[MovingHumanId].GetComponent<HangOntoMouse>().Follow = true;
                break;
            case "Man":
                ManList[MovingHumanId].GetComponent<HangOntoMouse>().Follow = true;
                break;
            case "Woman":
                WomanList[MovingHumanId].GetComponent<HangOntoMouse>().Follow = true;
                break;
            default:
                Debug.Log("Wrong keyword.");
                break;
        }
    }

    public void ImmobilizeAllHumans()
    {
        for(int i = 0; i < BabyList.Count; i++)
        {
            BabyList[i].GetComponent<HangOntoMouse>().Follow = false;
        }
        for (int i = 0; i < BoyList.Count; i++)
        {
            BoyList[i].GetComponent<HangOntoMouse>().Follow = false;
        }
        for (int i = 0; i < GirlList.Count; i++)
        {
            GirlList[i].GetComponent<HangOntoMouse>().Follow = false;
        }
        for (int i = 0; i < ManList.Count; i++)
        {
            ManList[i].GetComponent<HangOntoMouse>().Follow = false;
        }
        for (int i = 0; i < WomanList.Count; i++)
        {
            WomanList[i].GetComponent<HangOntoMouse>().Follow = false;
        }
        for (int i = 0; i < UsersWithSpecialNeedsList.Count; i++)
        {
            UsersWithSpecialNeedsList[i].GetComponent<HangOntoMouse>().Follow = false;
        }
    }

    public void ReassignHumans()
    {
        string designation = GameObject.Find("Main Camera").GetComponent<CameraController>().MovingHumanDesignation;
        switch (designation)
        {
            case "Baby":
                for (int i = 0; i < BabyList.Count; i++)
                {
                    BabyList[i].GetComponent<Agent>().IdNumber = i;
                    BabyList[i].name = BabyList[i].GetComponent<Agent>().Designation+ " " + i;
                }
                reassignedHumans = true;
                break;
            case "Boy":
                for (int i = 0; i < BoyList.Count; i++)
                {
                    BoyList[i].GetComponent<Agent>().IdNumber = i;
                    BoyList[i].name = BoyList[i].GetComponent<Agent>().Designation + " " + i;
                }
                reassignedHumans = true;
                break;
            case "Girl":
                for (int i = 0; i < GirlList.Count; i++)
                {
                    GirlList[i].GetComponent<Agent>().IdNumber = i;
                    GirlList[i].name = GirlList[i].GetComponent<Agent>().Designation + " " + i;
                }
                reassignedHumans = true;
                break;
            case "Users with Special Needs":
                for (int i = 0; i < UsersWithSpecialNeedsList.Count; i++)
                {
                    UsersWithSpecialNeedsList[i].GetComponent<Agent>().IdNumber = i;
                    UsersWithSpecialNeedsList[i].name = UsersWithSpecialNeedsList[i].GetComponent<Agent>().Designation + " " + i;
                }
                reassignedHumans = true;
                break;
            case "Man":
                for (int i = 0; i < ManList.Count; i++)
                {
                    ManList[i].GetComponent<Agent>().IdNumber = i;
                    ManList[i].name = ManList[i].GetComponent<Agent>().Designation + " " + i;
                }
                reassignedHumans = true;
                break;
            case "Woman":
                for (int i = 0; i < WomanList.Count; i++)
                {
                    WomanList[i].GetComponent<Agent>().IdNumber = i;
                    WomanList[i].name = WomanList[i].GetComponent<Agent>().Designation + " " + i;
                }
                reassignedHumans = true;
                break;
            default:
                Debug.Log("Wrong keyword.");
                break;
        }
    }

    public GameObject GetMovingHuman()
    {
        GameObject MovingHuman = null;
        string designation = GameObject.Find("Main Camera").GetComponent<CameraController>().MovingHumanDesignation;
        switch (designation)
        {
            case "Baby":
                MovingHuman = BabyList[GameObject.Find("Main Camera").GetComponent<CameraController>().MovingHumanId];
                break;
            case "Boy":
                MovingHuman = BoyList[GameObject.Find("Main Camera").GetComponent<CameraController>().MovingHumanId];
                break;
            case "Girl":
                MovingHuman = GirlList[GameObject.Find("Main Camera").GetComponent<CameraController>().MovingHumanId];
                break;
            case "Users with Special Needs":
                MovingHuman = UsersWithSpecialNeedsList[GameObject.Find("Main Camera").GetComponent<CameraController>().MovingHumanId];
                break;
            case "Man":
                MovingHuman = ManList[GameObject.Find("Main Camera").GetComponent<CameraController>().MovingHumanId];
                break;
            case "Woman":
                MovingHuman = WomanList[GameObject.Find("Main Camera").GetComponent<CameraController>().MovingHumanId];
                break;
            default:
                Debug.Log("Wrong keyword.");
                break;
        }
        return MovingHuman;
    }
}
