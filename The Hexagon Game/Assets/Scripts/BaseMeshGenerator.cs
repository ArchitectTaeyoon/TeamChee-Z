using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class BaseMeshGenerator : MonoBehaviour {

    public Mesh baseMesh;

    Vector3[] vertices;
    int[] triangles;

    void Awake()
    {
       baseMesh = GetComponent<MeshFilter>().mesh;
    }
    // Use this for initialization
    void Start () {
        //MakeMeshData();
        //CreateMesh();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MakeMeshData()
    {
        //Get the size of the base grid
        int lengthCount = this.gameObject.GetComponent<TriangularGrid>().LengthCount;
        int widthCount = this.gameObject.GetComponent<TriangularGrid>().WidthCount;

        vertices = new Vector3[]
        {
            this.gameObject.GetComponent<TriangularGrid>().gridSphereArray[0,0,0].transform.position,
            this.gameObject.GetComponent<TriangularGrid>().gridSphereArray[0,0,widthCount].transform.position,
            this.gameObject.GetComponent<TriangularGrid>().gridSphereArray[lengthCount,0,0].transform.position,
            this.gameObject.GetComponent<TriangularGrid>().gridSphereArray[lengthCount,0,widthCount].transform.position,
        };

        triangles = new int[] { 0, 1, 2, 2, 1, 3 };
    }

    public void CreateMesh()
    {
        baseMesh.Clear();
        baseMesh.vertices = vertices;
        baseMesh.triangles = triangles;
        baseMesh.RecalculateNormals();
    }
}
