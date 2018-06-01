using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour {

    [Header("Actual Module")]
    public GameObject ActualModule;

    #region SPHERE GAMEOBJECTS
    [Header("Spheres for reference")]
    [Header("A Spheres")]
    public GameObject A0;
    public GameObject A1;
    public GameObject A12;
    public GameObject A2;
    public GameObject A23;
    public GameObject A3;
    public GameObject A34;
    public GameObject A4;
    public GameObject A45;
    public GameObject A5;
    public GameObject A56;
    public GameObject A6;
    public GameObject A61;

    [Header("B Spheres")]
    public GameObject B1;
    public GameObject B12;
    public GameObject B2;
    public GameObject B23;
    public GameObject B3;
    public GameObject B34;
    public GameObject B4;
    public GameObject B45;
    public GameObject B5;
    public GameObject B56;
    public GameObject B6;
    public GameObject B61;

    [Header("C Spheres")]
    public GameObject C0;
    public GameObject C1;
    public GameObject C12;
    public GameObject C2;
    public GameObject C23;
    public GameObject C3;
    public GameObject C34;
    public GameObject C4;
    public GameObject C45;
    public GameObject C5;
    public GameObject C56;
    public GameObject C6;
    public GameObject C61;
    #endregion

    #region SURFACE GAMEOBJECTS
    [Header("Surfaces")]
    [Header("A Surfaces")]
    public GameObject S_A0_A1_A2;
    public GameObject S_A0_A2_A3;
    public GameObject S_A0_A3_A4;
    public GameObject S_A0_A4_A5;
    public GameObject S_A0_A5_A6;
    public GameObject S_A0_A6_A1;

    [Header("C Surfaces")]
    public GameObject S_C0_C1_C2;
    public GameObject S_C0_C2_C3;
    public GameObject S_C0_C3_C4;
    public GameObject S_C0_C4_C5;
    public GameObject S_C0_C5_C6;
    public GameObject S_C0_C6_C1;

    [Header("Inner Walls")]
    public GameObject IW_C0_C1_A1_A0;
    public GameObject IW_C0_C2_A2_A0;
    public GameObject IW_C0_C3_A3_A0;
    public GameObject IW_C0_C4_A4_A0;
    public GameObject IW_C0_C5_A5_A0;
    public GameObject IW_C0_C6_A6_A0;

    [Header("Outer Walls")]
    public GameObject OW_C1_C12_B12_B1;
    public GameObject OW_A1_A12_B12_B1;
    public GameObject OW_A2_A12_B12_B2;
    public GameObject OW_C2_C12_B12_B2;
    public GameObject OW_C2_C23_B23_B2;
    public GameObject OW_A2_A23_B23_B2;
    public GameObject OW_A3_A23_B23_B3;
    public GameObject OW_C3_C23_B23_B3;
    public GameObject OW_C3_C34_B34_B3;
    public GameObject OW_A3_A34_B34_B3;
    public GameObject OW_A4_A34_B34_B4;
    public GameObject OW_C4_C34_B34_B4;
    public GameObject OW_C4_C45_B45_B4;
    public GameObject OW_A4_A45_B45_B4;
    public GameObject OW_A5_A45_B45_B5;
    public GameObject OW_C5_C45_B45_B5;
    public GameObject OW_C5_C56_B56_B5;
    public GameObject OW_A5_A56_B56_B5;
    public GameObject OW_A6_A56_B56_B6;
    public GameObject OW_C6_C56_B56_B6;
    public GameObject OW_C6_C61_B61_B6;
    public GameObject OW_A6_A61_B61_B6;
    public GameObject OW_A1_A61_B61_B1;
    public GameObject OW_C1_C61_B61_B1;
    #endregion

    //Arrays to store the guiding spheres
    //[HideInInspector]
    public GameObject[] Guide_Spheres;
    public GameObject[] Floor_Surfaces;
    public GameObject[] Inner_Walls;
    public GameObject[] Outer_Walls;

    //Booleans
    public bool highlighted;
    public bool placedOnce = false;

    //Positions
    public Vector3 LastPosition;

    //Identity
    public int ModuleNumber;

    //ModuleSphere as reference point
    public int ReferenceSphereID;
    public Vector3 ReferenceDisplacement;

    //Colliding Boolean
    public bool Colliding = false;

    // Use this for initialization
    void Start () {
        PopulateArrays();
        ReferenceSphereID = 0;
        
	}
	
	// Update is called once per frame
	void Update () {
        UpdateGuideSphereMaterial();
        ReferenceDisplacement = transform.position - Guide_Spheres[ReferenceSphereID].transform.position;
        if (GetComponent<FollowMouse>().Follow)
        {
            SetGuideSphereColliders(false);
            SetFloorSurfaceColliders(false);
        }
        else
        {
            SetGuideSphereColliders(true);
            SetFloorSurfaceColliders(true);
        }
    }

    //Populate The Sphere Arrays
    public void PopulateArrays()
    {
        Guide_Spheres = new GameObject[] { A0, A1, A12, A2, A23, A3, A34, A4, A45, A5, A56, A6, A61,
                                           B1, B12, B2, B23, B3, B34, B4, B45, B5, B56, B6, B61,
                                           C0, C1, C12, C2, C23, C3, C34, C4, C45, C5, C56, C6, C61 };
        Floor_Surfaces = new GameObject[] { S_A0_A1_A2, S_A0_A2_A3, S_A0_A3_A4, S_A0_A4_A5, S_A0_A5_A6, S_A0_A6_A1 };
        Inner_Walls = new GameObject[] { IW_C0_C1_A1_A0, IW_C0_C2_A2_A0, IW_C0_C3_A3_A0, IW_C0_C4_A4_A0, IW_C0_C5_A5_A0, IW_C0_C6_A6_A0 };
        Outer_Walls = new GameObject[] { OW_C1_C12_B12_B1, OW_A1_A12_B12_B1, OW_A2_A12_B12_B2, OW_C2_C12_B12_B2, OW_C2_C23_B23_B2, OW_A2_A23_B23_B2,
                                         OW_A3_A23_B23_B3, OW_C3_C23_B23_B3, OW_C3_C34_B34_B3, OW_A3_A34_B34_B3, OW_A4_A34_B34_B4, OW_C4_C34_B34_B4,
                                         OW_C4_C45_B45_B4, OW_A4_A45_B45_B4, OW_A5_A45_B45_B5, OW_C5_C45_B45_B5, OW_C5_C56_B56_B5, OW_A5_A56_B56_B5,
                                         OW_A6_A56_B56_B6, OW_C6_C56_B56_B6, OW_C6_C61_B61_B6, OW_A6_A61_B61_B6, OW_A1_A61_B61_B1, OW_C1_C61_B61_B1 };
    }

    //Highlight
    public void Highlight()
    {
        this.gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = GameObject.Find("Main Camera").GetComponent<CameraController>().highlightedModuleFrame;
    }

    //On Move
    public void OnMove()
    {
        this.gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = GameObject.Find("Main Camera").GetComponent<CameraController>().selectedModuleFrame;
    }

    public void OnMouseEnter()
    {
        Highlight();
        if(GameObject.Find("Main Camera").GetComponent<CameraController>().mouseCarriesModule)
        {
            if(GameObject.Find("Main Camera").GetComponent<CameraController>().MovingModuleId != ModuleNumber)
            {
                GameObject.Find("Game").GetComponent<Game>().ModulesList[GameObject.Find("Main Camera").GetComponent<CameraController>().MovingModuleId].GetComponent<FollowMouse>().SnappableModuleAvailable = true;
                GameObject.Find("Game").GetComponent<Game>().ModulesList[GameObject.Find("Main Camera").GetComponent<CameraController>().MovingModuleId].GetComponent<FollowMouse>().SnappableModuleID = ModuleNumber;
            }
        }
    }

    public void OnMouseExit()
    {
        this.gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = GameObject.Find("Main Camera").GetComponent<CameraController>().unselectedModuleFrame;
        if (GameObject.Find("Main Camera").GetComponent<CameraController>().mouseCarriesModule)
            GameObject.Find("Game").GetComponent<Game>().ModulesList[GameObject.Find("Main Camera").GetComponent<CameraController>().MovingModuleId].GetComponent<FollowMouse>().SnappableModuleAvailable = false;
    }

    public void UpdateGuideSphereMaterial()
    {
        for (int i = 0; i < Guide_Spheres.Length; i++)
        {
            if (i != ReferenceSphereID)
                Guide_Spheres[i].GetComponent<MeshRenderer>().material = GameObject.Find("Main Camera").GetComponent<CameraController>().unselectedModuleSphere;
            else
                Guide_Spheres[i].GetComponent<MeshRenderer>().material = GameObject.Find("Main Camera").GetComponent<CameraController>().selectedModuleSphere;
        }

    }

    //public void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("Colliding object: " + other.gameObject.name);
    //    other.gameObject.GetComponent<Module>().Colliding = true;
    //    this.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = GameObject.Find("Main Camera").GetComponent<CameraController>().collidingModuleFrame;
    //    other.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = GameObject.Find("Main Camera").GetComponent<CameraController>().collidingModuleFrame;
    //}

    //public void OnTriggerExit(Collider other)
    //{
    //    Colliding = false;
    //    //if (other.gameObject.GetComponent<FollowMouse>().Follow)
    //        other.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = GameObject.Find("Main Camera").GetComponent<CameraController>().selectedModuleFrame;
    //    //else
    //        this.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = GameObject.Find("Main Camera").GetComponent<CameraController>().unselectedModuleFrame;
    //}

    //Toggle Guide Sphere Colliders
    public void SetGuideSphereColliders(bool _SphereState)
    {
        if (_SphereState == true)
        {
            for (int i = 0; i < Guide_Spheres.Length; i++)
            {
                Guide_Spheres[i].GetComponent<SphereCollider>().enabled = true;
                this.GetComponent<MeshCollider>().enabled = true;
            }
        }
        else
        {
            for (int i = 0; i < Guide_Spheres.Length; i++)
            {
                Guide_Spheres[i].GetComponent<SphereCollider>().enabled = false;
                this.GetComponent<MeshCollider>().enabled = false;
            }
        }
    }

    //Toggle floor surface colliders
    public void SetFloorSurfaceColliders(bool _FloorState)
    {
        if (_FloorState == true)
        {
            for(int i = 0; i < Floor_Surfaces.Length; i++)
            {
                Floor_Surfaces[i].GetComponent<MeshCollider>().enabled = true;
            }
        }
        else
        {
            for(int i = 0; i < Floor_Surfaces.Length; i++)
            {
                Floor_Surfaces[i].GetComponent<MeshCollider>().enabled = false;
            }
        }
    }

    //Switch reference sphere 
    public void SwitchReferenceSphereForward()
    {
        if (ReferenceSphereID < Guide_Spheres.Length-1)
            ReferenceSphereID++;
        else
            ReferenceSphereID = 0;
    }

    public void SwitchReferenceSphereBackward()
    {
        if (ReferenceSphereID > 0)
            ReferenceSphereID--;
        else
            ReferenceSphereID = Guide_Spheres.Length-1;
    }

    //Rotate Reference soheres on rotation
    public void RotateReferenceSpheres()
    {
        if (ReferenceSphereID == 0)
        {
            ReferenceSphereID = 0;
            return;
        }
        if (ReferenceSphereID == 1)
        {
            ReferenceSphereID = 11;
            return;
        }
        if (ReferenceSphereID == 2)
        {
            ReferenceSphereID = 12;
            return;
        }
        if (ReferenceSphereID >= 3 && ReferenceSphereID <= 12)
        {
            ReferenceSphereID -= 2;
            return;
        }
        if (ReferenceSphereID == 13)
        {
            ReferenceSphereID = 23;
            return;
        }
        if (ReferenceSphereID == 14)
        {
            ReferenceSphereID = 24;
            return;
        }
        if (ReferenceSphereID == 25)
        {
            ReferenceSphereID = 25;
            return;
        }
        if (ReferenceSphereID >= 15 && ReferenceSphereID <= 24)
        {
            ReferenceSphereID -= 2;
            return;
        }
        if (ReferenceSphereID == 26)
        {
            ReferenceSphereID = 36;
            return;
        }
        if (ReferenceSphereID == 27)
        {
            ReferenceSphereID = 37;
            return;
        }
        if (ReferenceSphereID >= 28 && ReferenceSphereID <= 37)
        {
            ReferenceSphereID -= 2;
            return;
        }

    }

    public void RotateModule()
    {
        StartCoroutine("ModuleRotation");
    }

    IEnumerator ModuleRotation()
    {
        int NumOfDivisions = (int)60 / 6;
        for (int i = 0; i < NumOfDivisions; i++)
        {
            gameObject.transform.RotateAround(transform.position,Vector3.up,6.0f);
            yield return new WaitForSeconds(0.01f);
        }
    }

    //Collision Detection
    public void CollisionDetection()
    {
        if (GetComponent<FollowMouse>().SnappableModuleAvailable)
        {
            Colliding = true;
            this.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = GameObject.Find("Main Camera").GetComponent<CameraController>().collidingModuleFrame;
            GameObject.Find("Game").GetComponent<Game>().ModulesList[GetComponent<FollowMouse>().SnappableModuleID].transform.GetChild(0).GetComponent<MeshRenderer>().material = GameObject.Find("Main Camera").GetComponent<CameraController>().collidingModuleFrame;
        }
        else
        {
            Colliding = false;
            this.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = GameObject.Find("Main Camera").GetComponent<CameraController>().selectedModuleFrame;
            GameObject.Find("Game").GetComponent<Game>().ModulesList[GetComponent<FollowMouse>().SnappableModuleID].transform.GetChild(0).GetComponent<MeshRenderer>().material = GameObject.Find("Main Camera").GetComponent<CameraController>().unselectedModuleFrame;
        }
    }

    //Set 
}
