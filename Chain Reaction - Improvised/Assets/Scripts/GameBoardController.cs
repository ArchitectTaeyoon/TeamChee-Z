using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardController : MonoBehaviour {

    //Variables for gameboard grid size
    public int length=10;
    public int breadth=10;

    //Grid Prefab
    public GameObject gridPrefab;
    public GameObject spherePrefab;

    //Array to hold grid units
    public GameObject[,] GameBoardArray;

    //Boolean whether the chain reaction is over
    [HideInInspector]
    public bool ChainReactionOver = true;

    //Store current player data
    PlayerData CurrentPlayer;

    //base plane variables
    GameObject baseplane;
    MeshRenderer meshren;
    Color baseColor;


	// Use this for initialization
	void Start () {
        CreateBoard();
        ChainReactionOver = true;
        gameObject.GetComponent<PlayerController>().NextTurn();
        baseplane = GameObject.Find("BasePlane");
        meshren = baseplane.GetComponent<MeshRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        //Get current player data every frame
        CurrentPlayer = gameObject.GetComponent<PlayerController>().GetCurrentPlayer();
        if (ChainReactionOver == false) {
            ChainReaction();
        }
        CheckReactionState();
        if (ChainReactionOver&&gameObject.GetComponent<PlayerController>().GetPlayState()) {
            gameObject.GetComponent<PlayerController>().NextTurn();
        }
        //meshren.material.SetColor("_Color", baseColor);
    }

    //Create Board
    void CreateBoard(){
        GameBoardArray = new GameObject[length, breadth];
        for (int i=0;i<length;i++) {
            for (int j = 0; j < breadth; j++) {
                //Instantiate gridunit
                GameBoardArray[i, j] = Instantiate(gridPrefab, new Vector3(i, 0, j), Quaternion.identity);
                //GameBoardArray[i, j].transform.parent = this.transform;
                GameBoardArray[i, j].name = "Grid(" + i.ToString() + "," + j.ToString() + ")";
                GameBoardArray[i, j].GetComponent<GridUnit>().SetAddress(i, j);
                //Instantiate sphere inside the grid
                GameObject sphere = Instantiate(spherePrefab, new Vector3(i, 0.5f, j), Quaternion.identity);
                sphere.name = "Sphere(" + i.ToString() + "," + j.ToString() + ")";
                sphere.transform.parent = GameBoardArray[i, j].transform;
                if (i == 0 && j != 0 || i != 0 && j == breadth - 1 || i != length - 1 && j == 0 || i == length - 1 && j != breadth - 1)
                {
                    GameBoardArray[i, j].GetComponent<GridUnit>().SetEdge(true);
                    GameBoardArray[i, j].GetComponent<GridUnit>().SetCorner(false);
                    GameBoardArray[i, j].GetComponent<GridUnit>().SetInner(false);
                }
                if (i == 0 && j == 0 || i == 0 && j == breadth - 1 || i == length - 1 && j == 0 || i == length - 1 && j == breadth - 1)
                {
                    GameBoardArray[i, j].GetComponent<GridUnit>().SetCorner(true);
                    GameBoardArray[i, j].GetComponent<GridUnit>().SetEdge(false);
                    GameBoardArray[i, j].GetComponent<GridUnit>().SetInner(false);
                }
                if(i!=0&&j!=0&&i!=length-1&&j!=breadth-1)
                {
                    GameBoardArray[i, j].GetComponent<GridUnit>().SetInner(true);
                    GameBoardArray[i, j].GetComponent<GridUnit>().SetEdge(false);
                    GameBoardArray[i, j].GetComponent<GridUnit>().SetCorner(false);
                }
            }
        }
    }

    //Change grid color based on the turn
    public void GridColorChange(Color _color) {
        /*for (int i = 0; i < length; i++) {
            for (int j = 0; j < breadth; j++) {
                GameBoardArray[i, j].GetComponent<GridUnit>().SetGridColor(_color);
            }
        }*/
        baseColor = _color;
        
    }

    //Initial Chain Reaction
    public void TriggerChainReaction(int _i, int _j) {
        //Debug.Log("Chain reaction triggered");
        ChainReactionOver = false;
        GameObject currentGrid = GameBoardArray[_i, _j];
        int numberOfSpheres = currentGrid.GetComponent<GridUnit>().GetNumberOfSpheres() + 1;
        GameBoardArray[_i, _j].GetComponent<GridUnit>().SetGridState(true,CurrentPlayer.PlayerID,numberOfSpheres);
        GameBoardArray[_i, _j].GetComponent<GridUnit>().UpdateGrid() ;
    }

    //Carry out chain reaction
    public void ChainReaction() {
        for (int i = 0; i < length; i++) {
            for (int j = 0; j < breadth; j++) {
                GameObject currentGrid = GameBoardArray[i, j];
                //Inner grids
                if (currentGrid.GetComponent<GridUnit>().GetInner() && currentGrid.GetComponent<GridUnit>().GetNumberOfSpheres() > 3) {
                    ChainReactionOver = false;
                    //Explode
                    //Make the current grid vacant
                    currentGrid.GetComponent<GridUnit>().EmptyGrid();
                    //InstantiateCollisionFour(currentGrid.transform.position,0);
                    //Make face-to-face adjacent grids occupied
                    GameBoardArray[i + 1, j].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i + 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                    GameBoardArray[i - 1, j].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i - 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                    GameBoardArray[i, j + 1].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i, j+1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                    GameBoardArray[i, j - 1].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i, j-1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                }
                //Edge grids
                if (currentGrid.GetComponent<GridUnit>().GetEdge() && currentGrid.GetComponent<GridUnit>().GetNumberOfSpheres() > 2)
                {
                    ChainReactionOver = false;
                    //Explode
                    //Make the current grid vacant
                    currentGrid.GetComponent<GridUnit>().EmptyGrid();
                    //Make face-to-face adjacent grids occupied
                    if (i==0) {
                        GameBoardArray[i + 1, j].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i + 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                        GameBoardArray[i, j + 1].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i, j + 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                        GameBoardArray[i, j - 1].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i, j - 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                    }
                    if (i==length-1) {
                        GameBoardArray[i - 1, j].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i - 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                        GameBoardArray[i, j + 1].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i, j + 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                        GameBoardArray[i, j - 1].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i, j - 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                    }
                    if (j==0) {
                        GameBoardArray[i + 1, j].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i + 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                        GameBoardArray[i - 1, j].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i - 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                        GameBoardArray[i, j + 1].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i, j + 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                    }
                    if (j==breadth-1) {
                        GameBoardArray[i + 1, j].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i + 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                        GameBoardArray[i - 1, j].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i - 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                        GameBoardArray[i, j - 1].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i, j - 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                    }
                }
                //Corner grids
                if (currentGrid.GetComponent<GridUnit>().GetCorner() && currentGrid.GetComponent<GridUnit>().GetNumberOfSpheres() > 1)
                {
                    ChainReactionOver = false;
                    //Explode
                    //Make the current grid vacant
                    currentGrid.GetComponent<GridUnit>().EmptyGrid();
                    //Make face-to-face adjacent grids occupied
                    if (i == 0&&j==0)
                    {
                        GameBoardArray[i + 1, j].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i + 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                        GameBoardArray[i, j + 1].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i, j + 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                    }
                    if (i == length - 1&&j==breadth-1)
                    {
                        GameBoardArray[i - 1, j].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i - 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                        GameBoardArray[i, j - 1].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i, j - 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                    }
                    if (j == 0&&i==breadth-1)
                    {
                        GameBoardArray[i - 1, j].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i - 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                        GameBoardArray[i, j + 1].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i, j + 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                    }
                    if (j == breadth - 1&&i==0)
                    {
                        GameBoardArray[i + 1, j].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i + 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                        GameBoardArray[i, j - 1].GetComponent<GridUnit>().SetGridState(true, CurrentPlayer.PlayerID, GameBoardArray[i, j - 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1);
                    }
                }
            }
        }
    }

    //Check if chain reaction is over
    public void CheckReactionState() {
        int numberExceeding=0;
        for (int i = 0; i < length; i++) {
            for (int j = 0; j < breadth; j++) {
                if (GameBoardArray[i, j].GetComponent<GridUnit>().GetInner() && GameBoardArray[i, j].GetComponent<GridUnit>().GetNumberOfSpheres() > 3) {
                    numberExceeding++;
                }
                if (GameBoardArray[i, j].GetComponent<GridUnit>().GetEdge() && GameBoardArray[i, j].GetComponent<GridUnit>().GetNumberOfSpheres() > 2) {
                    numberExceeding++;
                }
                if (GameBoardArray[i, j].GetComponent<GridUnit>().GetCorner() && GameBoardArray[i, j].GetComponent<GridUnit>().GetNumberOfSpheres() > 1) {
                    numberExceeding++;
                }
            }
        }
        if (numberExceeding > 0)
        {
            ChainReactionOver = false;
        }
        else {
            ChainReactionOver = true;
        }
    }

    public void InstantiateCollisionFour(Vector3 location, int angle)
    {
        //Instantiate Collision4
        GameObject CollidingSphere = Instantiate(spherePrefab.GetComponent<MeshDataBase>().Collision4,location,Quaternion.Euler(0,angle*-1,0));
        GameObject[] spheres = CollidingSphere.GetComponent<CollidingSphereUnits>().spheres;
        spheres[0].transform.position = Vector3.Lerp(spheres[0].transform.position, spheres[0].transform.position + Vector3.forward, 0.2f);
        spheres[1].transform.position = Vector3.Lerp(spheres[1].transform.position, spheres[1].transform.position - Vector3.right, 0.2f);
        spheres[2].transform.position = Vector3.Lerp(spheres[2].transform.position, spheres[2].transform.position - Vector3.forward, 0.2f);
        spheres[3].transform.position = Vector3.Lerp(spheres[3].transform.position, spheres[3].transform.position + Vector3.right, 0.2f);
    }
}

