using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangularGrid : MonoBehaviour {

    [Header("Grid dimensions")]
    public float ModuleSize = 1.0f;
    public float GridXDimension;
    public float GridZDimension;
    public float GridLength = 10;
    public float GridWidth = 10;
    public float GridHeight = 10; 

    [Header("Grid prefabs")]
    public GameObject gridSpherePrefab;

    [HideInInspector]
    public GameObject[,,] gridSphereArray;

    [HideInInspector]
    public int LengthCount;
    public int WidthCount;
    public int HeightCount;

	// Use this for initialization
	void Start () {
        GridXDimension = ModuleSize / 2;
        GridZDimension = GridXDimension * Mathf.Tan(60 * Mathf.Deg2Rad) / 2 ;
        GenerateGrid();
        this.gameObject.GetComponent<BaseMeshGenerator>().MakeMeshData();
        this.gameObject.GetComponent<BaseMeshGenerator>().CreateMesh();
        this.gameObject.GetComponent<MeshCollider>().sharedMesh = this.gameObject.GetComponent<BaseMeshGenerator>().baseMesh;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // Generate the grid
    public void GenerateGrid()
    {
        LengthCount = Mathf.RoundToInt(GridLength / GridXDimension);
        WidthCount = Mathf.RoundToInt(GridWidth / GridZDimension);
        HeightCount = Mathf.RoundToInt(GridHeight / GridXDimension);

        Debug.Log("Length Count: " + LengthCount + ", Width Count: " + WidthCount + ", Height Count: " + HeightCount);
        //Instantiate the gridsphereArray to hold the grid sphere objects
        gridSphereArray = new GameObject[LengthCount + 1, HeightCount + 1, WidthCount + 1];

        for(int y=0; y < HeightCount; y++)
        {
            //Use a for loop to instantiate and assemble the grid spheres in a triangular fashion
            //They are staggered along the z-axis (say)
            for (int z = 0; z <= WidthCount; z++)
            {
                //The sphere placement differs for even nd odd grids (Staggering)
                //Starting X position
                float X_StartPoint;
                if (z % 2 == 0) //Even
                {
                    X_StartPoint = 0;
                    for (int x = 0; x <= LengthCount; x++)
                    {
                        Vector3 SpherePosition = new Vector3(X_StartPoint + x * GridXDimension, y * GridXDimension, z * GridZDimension);
                        GameObject currentSphere = Instantiate(gridSpherePrefab, SpherePosition, Quaternion.identity);
                        currentSphere.transform.parent = transform.parent;
                        currentSphere.name = "Grid Sphere (" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")";
                        gridSphereArray[x, y, z] = currentSphere;
                        //Turn on the mesh renderer for the lower level gris spheres alone
                        if (y == 0)
                        {
                            currentSphere.GetComponent<MeshRenderer>().enabled = true;
                        }
                    }
            }
                else //Odd
                {
                    X_StartPoint = GridXDimension / 2;
                    for (int x = 0; x < LengthCount; x++)
                    {
                        Vector3 SpherePosition = new Vector3(X_StartPoint + x * GridXDimension, y * GridXDimension, z * GridZDimension);
                        GameObject currentSphere = Instantiate(gridSpherePrefab, SpherePosition, Quaternion.identity);
                        currentSphere.transform.parent = transform.parent;
                        currentSphere.name = "Grid Sphere (" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")";
                        gridSphereArray[x, y, z] = currentSphere;
                        //Turn on the mesh renderer for the lower level gris spheres alone
                        if (y == 0)
                        {
                            currentSphere.GetComponent<MeshRenderer>().enabled = true;
                        }
                    }
                    //Assign the last Array element to be null because of the staggering
                    gridSphereArray[LengthCount, y, z] = null;
                }
            }

            
        }
    }

}
