using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameBoardController : MonoBehaviour {

    #region VARIABLES
    //Variables for gameboard grid size
    public int length=10;
    public int breadth=10;
    public int height = 5;

    //Grid Prefab
    public GameObject boardGridPrefab;
    public GameObject GridUnitPrefab;
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

    //material
    [Header("Metrials")]
    public Material buildableGridMaterial;
    public Material unbuildableGridMaterial;
    public Material floorPlaneMaterial;

    //prefabs for the spaces
    [Header("Prefabs for the Spaces")]
    public GameObject SingleSpace;
    public GameObject DoubleSpace;
    public GameObject TripleSpace;
    public GameObject ExternallyBuiltSpace;

    [Header("Shared Space Preference - atleastOne / halfOrAbove / anyone")]
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
        //meshren.material.SetColor("_Color", baseColor);
        
    }

    //Create Board
    void CreateBoard(){
        GameBoardArray = new GameObject[length, breadth];
        
        for (int i=0;i<length;i++) {
            for (int j = 0; j < breadth; j++) {

                    //Instantiate gridunit                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
                    GameBoardArray[i, j] = Instantiate(boardGridPrefab, new Vector3(i, 0, j), Quaternion.identity);
                    GameBoardArray[i, j].GetComponent<MeshRenderer>().material = buildableGridMaterial;
                    //GameBoardArray[i, j].transform.parent = this.transform;
                    GameBoardArray[i, j].name = "Board Grid(" + i.ToString() + "," + j.ToString() + ")";
                    GameBoardArray[i, j].GetComponent<BoardGrid>().SetAddress(i, j);
                
            }
        }
    }

    //Change grid color based on the turn 
    public void GridColorChange(Color _color) {
        baseColor = _color;
    }

    //Initial Chain Reaction
    public void TriggerChainReaction(int _i, int _j, int _k) {
        //Debug.Log("Chain reaction triggered");
        ChainReactionOver = false;
        GameObject currentGrid = GameBoardArray[_i, _j].GetComponent<BoardGrid>().VerticalNeighbourhood[_k];
        currentGrid.GetComponent<GridUnit>().SetGridState(false,"random",true,CurrentPlayer.PlayerID,currentGrid.GetComponent<GridUnit>().GetNumberOfSpheres()+1);
        if (currentGrid.GetComponent<GridUnit>().GetNumberOfSpheres() > currentGrid.GetComponent<GridUnit>().MaximumCapacity - 1)
            GetComponent<ChainReactionScheduler>().ScheduleExplosion(_i, _j, _k);
    }

    //Check if chain reaction is over
    public void CheckReactionState() {
        /*int numberExceeding=0;
        for (int i = 0; i < length; i++) {
            for (int j = 0; j < breadth; j++) {
                try
                {
                    GameObject BoardGrid = GameBoardArray[i, j];
                    for(int k = 0; k <= BoardGrid.GetComponent<BoardGrid>().height; k++)
                    {
                        if(BoardGrid.GetComponent<BoardGrid>().VerticalNeighbourhood[k].GetComponent<GridUnit>().GetNumberOfSpheres() > GameBoardArray[i, j].GetComponent<GridUnit>().MaximumCapacity - 1)
                        {
                            numberExceeding++;
                        }
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
        }*/

        if (GetComponent<ChainReactionScheduler>().CurrentTask == GetComponent<ChainReactionScheduler>().ExplosionSchedule.Count)
        {
            ChainReactionOver = true;
            GetComponent<ChainReactionScheduler>().ClearSchedule();
        }
        else {
            ChainReactionOver = false;
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

    //Neglect Costs
    public void NeglectCosts()
    {
        for(int i = 0; i < length; i++)
        {
            for(int j = 0; j < breadth; j++)
            {
                if(!GameBoardArray[i,j].GetComponent<BoardGrid>().bought)
                    GameBoardArray[i, j].GetComponent<BoardGrid>().Bought();
            }
        }
    }
}

