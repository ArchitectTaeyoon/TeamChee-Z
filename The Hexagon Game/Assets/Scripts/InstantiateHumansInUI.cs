using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstantiateHumansInUI : MonoBehaviour {

    [Header("Human Prefabs")]
    public GameObject man;
    public GameObject woman;
    public GameObject usersWithSpecialNeeds;
    public GameObject baby;
    public GameObject boy;
    public GameObject girl;

    [Header("Panels")]
    public GameObject panel_Man;
    public GameObject panel_Woman;
    public GameObject panel_UsersWithSpecialNeeds;
    public GameObject panel_Baby;
    public GameObject panel_Boy;
    public GameObject panel_Girl;

	// Use this for initialization
	void Start () {
        InstantiateHumanforUI(baby, panel_Baby);
        InstantiateHumanforUI(boy, panel_Boy);
        InstantiateHumanforUI(girl, panel_Girl);
        InstantiateHumanforUI(man, panel_Man);
        InstantiateHumanforUI(woman, panel_Woman);
        InstantiateHumanforUI(usersWithSpecialNeeds, panel_UsersWithSpecialNeeds);
	}
	
	// Update is called once per frame1
	void Update () {
		if(GameObject.Find("Main Camera").GetComponent<CameraController>().mouseCarriesHuman)
        {
            SetHumansOnUIColliders(false);
        }
        else
        {
            SetHumansOnUIColliders(true);
        }
	}

    //Instantiate Humans in panel
    public void InstantiateHumanforUI(GameObject HumanPrefab, GameObject HumanPanel)
    {
        //Debug.Log("Instantiated Man");
        Quaternion rotation = this.transform.rotation;
        GameObject humanUI = Instantiate(HumanPrefab, HumanPanel.transform.position, this.transform.rotation, HumanPanel.transform);
        humanUI.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        humanUI.transform.localPosition = new Vector3(60f, 25f, -18f);
        humanUI.name = humanUI.GetComponent<Agent>().Designation+ " on UI";
        humanUI.GetComponent<HangOntoMouse>().Follow = false;
        //if(humanUI.GetComponent<Agent>().Designation !=  "User with Special Needs")
        //{
        //    humanUI.GetComponent<Animator>().enabled = false;
        //}
        //else
        //{
        //    humanUI.transform.GetChild(0).GetComponent<Animator>().enabled = false;
        //}
        //float RotationAdjustment = Vector3.Angle(transform.forward, GameObject.Find("Main Camera").transform.position - humanUI.transform.position);
        //Debug.Log("Rotation Adjustment: " + RotationAdjustment);
        //humanUI.transform.localRotation = Quaternion.Euler(0, RotationAdjustment, 0);
        humanUI.transform.localRotation = Quaternion.Euler(0, 180f, 0);
        //humanUI.layer = GameObject.Find("Main Camera").GetComponent<CameraController>().HumanOnUI;
        humanUI.layer = 13;
    }

    public GameObject InstantiateHumanforWorldSpace(string designation)
    {
        GameObject humanPrefab = man;
        switch (designation)
        {
            case "Baby":
                humanPrefab = baby;
                break;
            case "Boy":
                humanPrefab = boy;
                break;
            case "Girl":
                humanPrefab = girl;
                break;
            case "Users with Special Needs":
                humanPrefab = usersWithSpecialNeeds;
                break;
            case "Man":
                humanPrefab = man;
                break;
            case "Woman":
                humanPrefab = woman;
                break;
            default:
                Debug.Log("Wrong keyword.");
                break;
        }
        GameObject instantiatedHuman = Instantiate(humanPrefab, GameObject.Find("Main Camera").GetComponent<CameraController>().RayEnd, Quaternion.Euler(0,180f,0));
        return instantiatedHuman;
    }

    public void SetHumansOnUIColliders(bool _UIHUmanColliderState)
    {
        if (_UIHUmanColliderState)
        {
            panel_Baby.transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = true;
            panel_Boy.transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = true;
            panel_Girl.transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = true;
            panel_Man.transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = true;
            panel_Woman.transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = true;
            panel_UsersWithSpecialNeeds.transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = true;
        }
        else
        {
            panel_Baby.transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = false;
            panel_Boy.transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = false;
            panel_Girl.transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = false;
            panel_Man.transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = false;
            panel_Woman.transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = false;
            panel_UsersWithSpecialNeeds.transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = false;
        }
    }
}
