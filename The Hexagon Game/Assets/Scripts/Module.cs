using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour {

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

    //Arrays to store the guiding spheres
    [HideInInspector]
    public GameObject[] A_Spheres;
    public GameObject[] B_Spheres;
    public GameObject[] C_Spheres;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Populate The Sphere Arrays
    public void PopulateSphereArrays()
    {
        A_Spheres = new GameObject[] { A0, A1, A12, A2, A23, A3, A34, A4, A45, A5, A56, A6, A61 };
        B_Spheres = new GameObject[] { B1, B12, B2, B23, B3, B34, B4, B45, B5, B56, B6, B61 };
        C_Spheres = new GameObject[] { C0, C1, C12, C2, C23, C3, C34, C4, C45, C5, C56, C6, C61 };
    }
}
