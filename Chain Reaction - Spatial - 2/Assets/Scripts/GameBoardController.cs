using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardController : MonoBehaviour {

    //Variables for gameboard grid size
    public int length=10;
    public int breadth=10;

    //Grid Prefab
    public GameObject gridPrefab;

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

    //floor plane material
    public Material floorPlaneMaterial;

    //prefabs for the spaces
    [Header("Prefabs for the Spaces")]
    public GameObject SingleSpace;
    public GameObject DoubleSpace;
    public GameObject TripleSpace;

    [Header("Shared Space Preference - atleastOne / halfOrAbove")]
    public string sharedSpacePreference = "atleastOne";

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
        baseColor = _color;
    }

    //Initial Chain Reaction
    public void TriggerChainReaction(int _i, int _j) {
        //Debug.Log("Chain reaction triggered");
        ChainReactionOver = false;
        GameObject currentGrid = GameBoardArray[_i, _j];
        int numberOfSpheres = currentGrid.GetComponent<GridUnit>().GetNumberOfSpheres() + 1;
        GameBoardArray[_i, _j].GetComponent<GridUnit>().SetGridState(false,"random",true,CurrentPlayer.PlayerID,numberOfSpheres);
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
                    currentGrid.GetComponent<GridUnit>().setGarden(true);
                    //Make face-to-face adjacent grids occupied
                    GameBoardArray[i + 1, j].GetComponent<GridUnit>().SetGridState(true, "left", true, CurrentPlayer.PlayerID, GameBoardArray[i + 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                    GameBoardArray[i - 1, j].GetComponent<GridUnit>().SetGridState(true, "right", true, CurrentPlayer.PlayerID, GameBoardArray[i - 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                    GameBoardArray[i, j + 1].GetComponent<GridUnit>().SetGridState(true, "down", true, CurrentPlayer.PlayerID, GameBoardArray[i, j + 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                    GameBoardArray[i, j - 1].GetComponent<GridUnit>().SetGridState(true, "up", true, CurrentPlayer.PlayerID, GameBoardArray[i, j - 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                }
                //Edge grids
                if (currentGrid.GetComponent<GridUnit>().GetEdge() && currentGrid.GetComponent<GridUnit>().GetNumberOfSpheres() > 2)
                {
                    ChainReactionOver = false;
                    //Explode
                    //Make the current grid vacant
                    currentGrid.GetComponent<GridUnit>().EmptyGrid();
                    currentGrid.GetComponent<GridUnit>().setGarden(true);
                    //Make face-to-face adjacent grids occupied
                    if (i==0) {
                        GameBoardArray[i + 1, j].GetComponent<GridUnit>().SetGridState(true, "left", true, CurrentPlayer.PlayerID, GameBoardArray[i + 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                        GameBoardArray[i, j + 1].GetComponent<GridUnit>().SetGridState(true, "down", true, CurrentPlayer.PlayerID, GameBoardArray[i, j + 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                        GameBoardArray[i, j - 1].GetComponent<GridUnit>().SetGridState(true, "up", true, CurrentPlayer.PlayerID, GameBoardArray[i, j - 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                    }
                    if (i==length-1) {
                        GameBoardArray[i - 1, j].GetComponent<GridUnit>().SetGridState(true, "right", true, CurrentPlayer.PlayerID, GameBoardArray[i - 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                        GameBoardArray[i, j + 1].GetComponent<GridUnit>().SetGridState(true, "down", true, CurrentPlayer.PlayerID, GameBoardArray[i, j + 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                        GameBoardArray[i, j - 1].GetComponent<GridUnit>().SetGridState(true, "up", true, CurrentPlayer.PlayerID, GameBoardArray[i, j - 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                    }
                    if (j==0) {
                        GameBoardArray[i + 1, j].GetComponent<GridUnit>().SetGridState(true, "left", true, CurrentPlayer.PlayerID, GameBoardArray[i + 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                        GameBoardArray[i - 1, j].GetComponent<GridUnit>().SetGridState(true, "right", true, CurrentPlayer.PlayerID, GameBoardArray[i - 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                        GameBoardArray[i, j + 1].GetComponent<GridUnit>().SetGridState(true, "down", true, CurrentPlayer.PlayerID, GameBoardArray[i, j + 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                    }
                    if (j==breadth-1) {
                        GameBoardArray[i + 1, j].GetComponent<GridUnit>().SetGridState(true, "left", true, CurrentPlayer.PlayerID, GameBoardArray[i + 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                        GameBoardArray[i - 1, j].GetComponent<GridUnit>().SetGridState(true, "right", true, CurrentPlayer.PlayerID, GameBoardArray[i - 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                        GameBoardArray[i, j - 1].GetComponent<GridUnit>().SetGridState(true, "up", true, CurrentPlayer.PlayerID, GameBoardArray[i, j - 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                    }
                }
                //Corner grids
                if (currentGrid.GetComponent<GridUnit>().GetCorner() && currentGrid.GetComponent<GridUnit>().GetNumberOfSpheres() > 1)
                {
                    ChainReactionOver = false;
                    //Explode
                    //Make the current grid vacant
                    currentGrid.GetComponent<GridUnit>().EmptyGrid();
                    currentGrid.GetComponent<GridUnit>().setGarden(true);
                    //Make face-to-face adjacent grids occupied
                    if (i == 0 && j == 0)
                    {
                        GameBoardArray[i + 1, j].GetComponent<GridUnit>().SetGridState(true, "left", true, CurrentPlayer.PlayerID, GameBoardArray[i + 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                        GameBoardArray[i, j + 1].GetComponent<GridUnit>().SetGridState(true, "down", true, CurrentPlayer.PlayerID, GameBoardArray[i, j + 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                    }
                    if (i == length - 1 && j == breadth-1)
                    {
                        GameBoardArray[i - 1, j].GetComponent<GridUnit>().SetGridState(true, "right", true, CurrentPlayer.PlayerID, GameBoardArray[i - 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                        GameBoardArray[i, j - 1].GetComponent<GridUnit>().SetGridState(true, "up", true, CurrentPlayer.PlayerID, GameBoardArray[i, j - 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                    }
                    if (j == 0 && i == breadth-1)
                    {
                        GameBoardArray[i - 1, j].GetComponent<GridUnit>().SetGridState(true, "right", true, CurrentPlayer.PlayerID, GameBoardArray[i - 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                        GameBoardArray[i, j + 1].GetComponent<GridUnit>().SetGridState(true, "down", true, CurrentPlayer.PlayerID, GameBoardArray[i, j + 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                    }
                    if (j == breadth - 1 && i == 0)
                    {
                        GameBoardArray[i + 1, j].GetComponent<GridUnit>().SetGridState(true, "left", true, CurrentPlayer.PlayerID, GameBoardArray[i + 1, j].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
                        GameBoardArray[i, j - 1].GetComponent<GridUnit>().SetGridState(true, "up", true, CurrentPlayer.PlayerID, GameBoardArray[i, j - 1].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
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


}

