using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridUnit : MonoBehaviour {

    //Corner/ Edge/ Inner booleans

    public bool corner;
    public bool edge;
    public bool inner;

    //GridColor
    Color gridColor;

    //Grid State variables
    //Holds three data --> bool whether it is occupied or not; int to which player occupies it; int to how many spheres are in it
    bool occupied = false;
    public int occupyingPlayer = -1;
    public int NumberOfSpheres = 0;

    //Stores the grid address
    int i;
    int j;

    //Store current Player data
    PlayerData currentPlayer;

    //Store the sphere gameobject
    [HideInInspector]
    public GameObject walls;
    [HideInInspector]
    public GameObject floor;

    //rotation speed
    float rotationSpeed = 20f;

    // Use this for initialization
    void Start () {
        walls = GameObject.Find("Walls(" + i.ToString() + "," + j.ToString() + ")");
        floor = GameObject.Find("Floor(" + i.ToString() + "," + j.ToString() + ")");
    }
	
	// Update is called once per frame
	void Update () {
        currentPlayer = GameObject.Find("GameBoard").GetComponent<PlayerController>().GetCurrentPlayer();
        //currentPlayer = GetComponentInParent<PlayerController>().GetCurrentPlayer();
        UpdateGrid();
	}

    //Getter & Setters for the three booleans ---> Corner/ Edge/ Inner
    public bool GetCorner() {
        return corner;
    }
    public void SetCorner(bool _corner) {
        corner = _corner;
    }
    public bool GetEdge() {
        return edge;
    }
    public void SetEdge(bool _edge) {
        edge = _edge;
    }
    public bool GetInner() {
        return inner;
    }
    public void SetInner(bool _inner) {
        inner = _inner;
    }

    //Setter for the grid address
    public void SetAddress(int _i, int _j) {
        i = _i;
        j = _j;
    }

    //Change color of grid unit based on turn
    public void SetGridColor(Color _gridColor){
        gridColor = _gridColor;
    }

    //Set Grid State
    public void SetGridState(bool _occupied, int _occupyingPlayer, int _NumberOfSpheres) {
        occupied = _occupied;
        occupyingPlayer = _occupyingPlayer;
        NumberOfSpheres = _NumberOfSpheres;
    }

    //Empty a grid
    public void EmptyGrid() {
        occupied = false;
        occupyingPlayer = -1;
        NumberOfSpheres = 0;
    }

    //Update the grid
    public void UpdateGrid() {
        //Setting the grid color
        //gameObject.GetComponent<MeshRenderer>().material.SetColor("_SpecColor", gridColor);
        MeshFilter embeddedWalls = walls.GetComponent<MeshFilter>();
        MeshRenderer FloorColor = floor.GetComponent<MeshRenderer>();
        if (occupied)
        {
            if (occupyingPlayer == -1)
            {
                floor.transform.position = new Vector3(i, 0.05f, j);
                FloorColor.enabled = true;
                FloorColor.material.SetColor("_Color", currentPlayer.PlayerColor);
            }
            else {
                floor.transform.position = new Vector3(i, 0.05f, j);
                FloorColor.enabled = true;
                FloorColor.material.SetColor("_Color", GameObject.Find("GameBoard").GetComponent<PlayerController>().PlayerArray[occupyingPlayer].PlayerColor);
            }
            
            switch (NumberOfSpheres)
            {
                case 1:
                    embeddedWalls.mesh = walls.GetComponent<MeshDataBase>().SingleSphere;
                    walls.transform.position = new Vector3(i, 0, j);
                    break;
                case 2:
                    embeddedWalls.mesh = walls.GetComponent<MeshDataBase>().DoubleSphere;
                    walls.transform.position = new Vector3(i, 0, j);
                    break;
                case 3:
                    embeddedWalls.mesh = walls.GetComponent<MeshDataBase>().TripleSphere;
                    walls.transform.position = new Vector3(i, 0, j);
                    break;
                default:
                    embeddedWalls.mesh = null;
                    walls.transform.position = new Vector3(i, 0, j);
                    break;
            }
        }
        else {
            FloorColor.enabled = false;
            embeddedWalls.mesh = null;
            walls.transform.position = new Vector3(i, 0, j);
        }
    }

    //Inform board on mouse click
    void OnMouseDown()
    {
        if (GameObject.Find("GameBoard").GetComponent<GameBoardController>().ChainReactionOver==true) {
            if (occupied)
            {
                if (occupyingPlayer == currentPlayer.PlayerID)
                {
                    Debug.Log("Grid(" + i.ToString() + "," + j.ToString() + ") clicked.");
                    GameObject.Find("GameBoard").GetComponent<GameBoardController>().TriggerChainReaction(i, j);
                    GameObject.Find("GameBoard").GetComponent<PlayerController>().Played();
                }
            }
            else {
                Debug.Log("Grid(" + i.ToString() + "," + j.ToString() + ") clicked.");
                GameObject.Find("GameBoard").GetComponent<GameBoardController>().TriggerChainReaction(i, j);
                GameObject.Find("GameBoard").GetComponent<PlayerController>().Played();
            }
        }
    }

    //Return number of occupying spheres
    public int GetNumberOfSpheres() {
        return NumberOfSpheres;
    }

    //check if grid is occupied
    public bool IsOccupied()
    {
        return occupied;
    }

    //return occupying player id
    public int FindOccupyingPlayer()
    {
        return occupyingPlayer;
    }
}
