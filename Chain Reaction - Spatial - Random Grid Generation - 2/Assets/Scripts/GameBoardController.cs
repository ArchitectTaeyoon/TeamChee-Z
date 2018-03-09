using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameBoardController : MonoBehaviour {

    #region VARIABLES
    //Variables for gameboard grid size
    public int length=10;
    public int breadth=10;

    //Grid Prefab
    public GameObject gridPrefab;
    public GameObject unbuiltGridPrefab;
    public GameObject costBubblePrefab;

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
    #endregion

    // Use this for initialization
    void Start () {
        CreateBoard();
        ChainReactionOver = true;
        gameObject.GetComponent<PlayerController>().NextTurn();
        baseplane = GameObject.Find("BasePlane");
        meshren = baseplane.GetComponent<MeshRenderer>();
        //GetComponent<CostManager>().ComputeCost();
    }
	
	// Update is called once per frame
	void Update () {
        //Get current player data every frame
        CurrentPlayer = gameObject.GetComponent<PlayerController>().GetCurrentPlayer();
        CheckReactionState();
        if (ChainReactionOver&&gameObject.GetComponent<PlayerController>().GetPlayState()&&ChainReactionOver) {
            gameObject.GetComponent<PlayerController>().NextTurn();
        }
        meshren.material.SetColor("_Color", baseColor);
        
    }

    //Create Board
    void CreateBoard(){
        GameBoardArray = new GameObject[length, breadth];
        for (int i=0;i<length;i++) {
            for (int j = 0; j < breadth; j++) {
                int RandomNumber = UnityEngine.Random.Range(0, 7);
                if (RandomNumber==5) //unbuilt
                {
                    //Instantiate gridunit                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
                    GameBoardArray[i, j] = Instantiate(unbuiltGridPrefab, new Vector3(i, 0, j), Quaternion.identity);
                    //GameBoardArray[i, j].transform.parent = this.transform;
                    GameBoardArray[i, j].name = "Grid(" + i.ToString() + "," + j.ToString() + ")" +" Unbuilt";
                    continue;
                }
                else
                {
                    //Instantiate gridunit                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
                    GameBoardArray[i, j] = Instantiate(gridPrefab, new Vector3(i, 0, j), Quaternion.identity);
                    //GameBoardArray[i, j].transform.parent = this.transform;
                    GameBoardArray[i, j].name = "Grid(" + i.ToString() + "," + j.ToString() + ")";
                    GameBoardArray[i, j].GetComponent<GridUnit>().SetAddress(i, j);
                    //Instantiate cost bubble with it
                    GameObject costBubble = Instantiate(costBubblePrefab);
                    costBubble.GetComponent<RectTransform>().SetPositionAndRotation(new Vector3(i, 0.06f, j), Quaternion.Euler(90, 0, 0));
                    costBubble.GetComponent<RectTransform>().SetParent(GameBoardArray[i, j].transform);
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
        if (currentGrid.GetComponent<GridUnit>().GetNumberOfSpheres() > currentGrid.GetComponent<GridUnit>().MaximumCapacity - 1)
            GetComponent<ChainReactionScheduler>().ScheduleExplosion(_i, _j);
    }

    //Check if chain reaction is over
    public void CheckReactionState() {
        int numberExceeding=0;
        for (int i = 0; i < length; i++) {
            for (int j = 0; j < breadth; j++) {
                try
                {
                    if (GameBoardArray[i, j].GetComponent<GridUnit>().GetNumberOfSpheres() > GameBoardArray[i, j].GetComponent<GridUnit>().MaximumCapacity - 1)
                    {
                        numberExceeding++;
                    }
                }
                catch (Exception) { }

            }
        }
        if (numberExceeding > 0)
        {
            ChainReactionOver = false;
        }
        else {
            ChainReactionOver = true;
            GetComponent<ChainReactionScheduler>().ClearSchedule();
        }
    }

    //Check for divide by zero exception
    public bool CheckForDivideZeroException(int i, int j)
    {
        bool allow = true;
        GameObject neighbour;
        try
        {
            neighbour = GameBoardArray[i - 1, j];
            if (neighbour.GetComponent<GridUnit>().MaximumCapacity <= 1)
                allow = false;
        }
        catch (Exception) { }
        try
        {
            neighbour = GameBoardArray[i, j-1];
            if (neighbour.GetComponent<GridUnit>().MaximumCapacity <= 1)
                allow = false;
        }
        catch (Exception) { }
        return allow;
    }
}

