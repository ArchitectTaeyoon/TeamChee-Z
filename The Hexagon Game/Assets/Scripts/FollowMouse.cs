using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour {

    public float distance = 2.0f;
    public bool Follow = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Follow)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = distance;
            Vector3 ScreenPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            //See if the position is within the grid system
            if (ScreenPosition.x >=0 && ScreenPosition.x <= GameObject.Find("Grid").GetComponent<TriangularGrid>().GridLength
                && ScreenPosition.y >= 0 && ScreenPosition.y <= GameObject.Find("Grid").GetComponent<TriangularGrid>().GridHeight
                && ScreenPosition.z >= 0 && ScreenPosition.z <= GameObject.Find("Grid").GetComponent<TriangularGrid>().GridWidth)
            {
                transform.position = ClosestGridSphere(ScreenPosition).transform.position;
            }
            else
            {
                transform.position = ScreenPosition;
            }
        }

	}

    public GameObject ClosestGridSphere(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x * GameObject.Find("Grid").GetComponent<TriangularGrid>().GridSize);
        int y = Mathf.RoundToInt(position.y * GameObject.Find("Grid").GetComponent<TriangularGrid>().GridSize);
        int z = Mathf.RoundToInt(position.y * GameObject.Find("Grid").GetComponent<TriangularGrid>().GridSize);
        GameObject ClosestSphere = GameObject.Find("Grid").GetComponent<TriangularGrid>().gridSphereArray[x, y, z];
        return ClosestSphere;
    }
}
