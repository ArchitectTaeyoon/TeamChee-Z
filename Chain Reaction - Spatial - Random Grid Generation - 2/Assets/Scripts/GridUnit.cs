using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GridUnit : MonoBehaviour {

    #region VARIABLES
    //Boolean to check if the grid is Garden
    public bool isGarden=false;

    //GridColor
    [Header("Materials for the Grid Base")]
    public Material grass;
    public Material defaultMaterial;

    //Grid State variables
    [Header("Grid state variables")]
    public int MaximumCapacity = 0;
    public bool occupied = false;
    public int[] occupyingPlayers;
    public int NumberOfSpheres = 0;
    public bool built = false;
    public bool isExplosion = false; //true --> if it is instantiated by an explosion; false --> is it is instantiated by a click
    public string SideOfInstantiation = ""; //left,right,up,down
    public int GridCost = 0;

    //Stores the grid address
    [Header("Grid address")]
    public int i;
    public int j;

    //Store current Player data
    PlayerData currentPlayer;

    //prefabs for the spaces
    GameObject SingleSpace;
    GameObject DoubleSpace;
    GameObject TripleSpace;

    //UI Text that displays cost
    Text cost;

    //Neighbourhood states
    public Dictionary<string, int> Neighbourhood; // Dictionary to store the occupants in side order
    public string[] Sides; // Array to hold the sides in which the cell can explode

    //Array containing the choices of the Rotation indices of the space instantiation when instantiated by CLick 'RANDOM'
    int[] RotationArray;

    //List to hold the face-to-face neighbours
    public List<GameObject> Neighbours;

    //List to hold the spilt players, those beyond the Maximum capacity
    public List<int> SpillPlayers;
    public int numberOfSpill;

#endregion

    // Use this for initialization
    void Start () {
        //instantiate the occupant array with the maximum number the grid can hold
        Neighbourhood = new Dictionary<string, int>();
        Neighbours = new List<GameObject>();
        ComputeNeighbourhood();
        cost = transform.GetChild(0).GetComponent<RectTransform>().GetChild(0).gameObject.GetComponent<Text>();
        SetCost();
        occupyingPlayers = new int[MaximumCapacity];
        ClearOccupants(); 
        SingleSpace = GameObject.Find("GameBoard").GetComponent<GameBoardController>().SingleSpace;
        DoubleSpace = GameObject.Find("GameBoard").GetComponent<GameBoardController>().DoubleSpace;
        TripleSpace = GameObject.Find("GameBoard").GetComponent<GameBoardController>().TripleSpace;
        SpillPlayers = new List<int>();
    }
	
	// Update is called once per frame
	void Update () {
        currentPlayer = GameObject.Find("GameBoard").GetComponent<PlayerController>().GetCurrentPlayer();
        //currentPlayer = GetComponentInParent<PlayerController>().GetCurrentPlayer();
        RefreshGrid();
        CheckBuilt();
        numberOfSpill = SpillPlayers.Count;
        if (GridCost == 0)
        {
            DuplicationCheck();
        }
	}

    //Inform board on mouse click
    void OnMouseDown()
    {
        if (GameObject.Find("GameBoard").GetComponent<GameBoardController>().ChainReactionOver == true && !isGarden)
        {
            if (occupied)
            {
                //Two possibilities:
                string preference = GameObject.Find("GameBoard").GetComponent<GameBoardController>().sharedSpacePreference;
                bool allow = false; //if this turns true, allow the player to expand
                switch (preference)
                {
                    //A. If at least one floor in the grid unit is held by the current player, he can expand in it
                    case "atleastOne":
                        for (int i = 0; i < MaximumCapacity; i++)
                        {
                            int occupant = occupyingPlayers[i];
                            if (currentPlayer.PlayerID == occupant)
                                allow = true;
                        }
                        break;
                    //B. If the player has 50% or more spaces in the grid, he can expand in it
                    case "halfOrAbove":
                        int count = 0; // count of the current player's spaces in the grid
                        for (int i = 0; i < MaximumCapacity; i++)
                        {
                            int occupant = occupyingPlayers[i];
                            if (currentPlayer.PlayerID == occupant)
                                count++;
                        }
                        if (count / NumberOfSpheres >= 0.5)
                        {
                            allow = true;
                        }
                        break;
                    default:
                        break;

                }
                if (allow)
                {
                    DeleteCostPanel();
                    Debug.Log("Grid(" + i.ToString() + "," + j.ToString() + ") clicked.");
                    GameObject.Find("GameBoard").GetComponent<GameBoardController>().TriggerChainReaction(i, j);
                    GameObject.Find("GameBoard").GetComponent<PlayerController>().Played();
                }
                else
                {
                    Debug.Log("Permission failed.");
                }
            }
            else
            {
                if (GameObject.Find("GameBoard").GetComponent<CostManager>().CheckFund(currentPlayer.PlayerID, GridCost))
                {
                    currentPlayer.Money -= GridCost;
                    GameObject.Find("GameBoard").GetComponent<CostManager>().DistributeMoney(currentPlayer.PlayerID, GridCost);
                    GridCost = 0;
                    Destroy(transform.GetChild(0).gameObject);
                    Debug.Log("Grid(" + i.ToString() + "," + j.ToString() + ") clicked.");
                    GameObject.Find("GameBoard").GetComponent<GameBoardController>().TriggerChainReaction(i, j);
                    GameObject.Find("GameBoard").GetComponent<PlayerController>().Played();
                }
                else
                {
                    Debug.Log("Insufficient funds.");
                }
            }
        }
    }

    //Set Grid State - 6 arguments 
    public void SetGridState(bool _isExplosion, string _sideOfInstantiation, bool _occupied, int _occupyingPlayer, int _NumberOfSpheres, bool _isGarden) {
        isExplosion = _isExplosion;
        SideOfInstantiation = InverseKey(_sideOfInstantiation);
        occupied = _occupied;
        NumberOfSpheres = _NumberOfSpheres;
        InsertOccupant(_occupyingPlayer);
        isGarden = _isGarden;
        UpdateGrid();
        UpdateNeighbourhood();
    }

    //Set Grid State - 5 arguments
    public void SetGridState(bool _isExplosion, string _sideOfInstantiation, bool _occupied, int _occupyingPlayer, int _NumberOfSpheres)
    {
        isExplosion = _isExplosion;
        SideOfInstantiation = InverseKey(_sideOfInstantiation);
        occupied = _occupied;
        InsertOccupant(_occupyingPlayer);
        NumberOfSpheres = _NumberOfSpheres;
        UpdateGrid();
        UpdateNeighbourhood();

    }

    //Empty a grid
    public void EmptyGrid() {
        occupied = false;
        ClearOccupants();
        NumberOfSpheres = 0;
    }

    //Update the grid based on the increasing numbers
    public void UpdateGrid() {
        if (NumberOfSpheres<MaximumCapacity)
        {
            if (built)
            {
                DestroyBuilt();
            }

            //Gameobject to be instatiated based on the number of spaces
            GameObject currentSpacePrefab;
            //Determine the rotation in which the embedded space needs to be instantiated
            int RotationAngle = 0;

            switch (NumberOfSpheres)
            {
                case 1:
                    currentSpacePrefab = SingleSpace;
                    break;
                case 2:
                    currentSpacePrefab = DoubleSpace;
                    break;
                case 3:
                    currentSpacePrefab = TripleSpace;
                    break;
                default:
                    currentSpacePrefab = null;
                    break;
            }

            switch (SideOfInstantiation)
            {
                case "left":
                    RotationAngle = 0;
                    break;
                case "up":
                    RotationAngle = 90;
                    break;
                case "right":
                    RotationAngle = 180;
                    break;
                case "down":
                    RotationAngle = 270;
                    break;
                case "random":
                    //RotationAngle = RotationArray[UnityEngine.Random.Range(0, RotationArray.Length)];
                    RotationAngle = 90;
                    break;
                default:
                    break;
            }

            //Instantiate the embeddedSpace
            GameObject currentSpace = Instantiate(currentSpacePrefab, new Vector3(i, 0, j), Quaternion.Euler(0, RotationAngle, 0));
            //Set the space as a child
            currentSpace.transform.parent = transform;
            built = true;
        }

        DuplicationCheck();
    }

    #region MISC FUNCTIONS
    //Return number of occupying spheres
    public int GetNumberOfSpheres() {
        return NumberOfSpheres;
    }

    //check if grid is occupied
    public bool IsOccupied(){
        return occupied;
    }

    //set the boolean isGarden
    public void setGarden(bool _isGarden){
        isGarden = _isGarden;
    }

    //Update cost
    public void SetCost(){
        GridCost = GameObject.Find("GameBoard").GetComponent<CostManager>().basicCost / MaximumCapacity;
        cost.text = "$ " + GridCost.ToString();
    }

    //Setter for the grid address
    public void SetAddress(int _i, int _j){
        i = _i;
        j = _j;
    }

    //Destroy the built space in the grid
    public void DestroyBuilt()
    {
        try
        {
            Destroy(transform.GetChild(0).gameObject);
            built = false;
        }
        catch (Exception) { }

    }
    
    //Inverse keys for the side of instantiation bug
    public string InverseKey(string key)
    {
        string oppSide = "";
        if (key.Equals("left"))
            oppSide = "right";
        if (key.Equals("right"))
            oppSide = "left";
        if (key.Equals("up"))
            oppSide = "down";
        if (key.Equals("down"))
            oppSide = "up";
        if (key.Equals("random"))
            oppSide = "random";
        Debug.Log("Side reversed!");
        return oppSide;

    }

    //Check for duplicate objects
    public void DuplicationCheck()
    {
        try
        {
            GameObject duplicate = transform.GetChild(1).gameObject;
            DestroyBuilt();
        }
        catch (Exception) { }
    }
    #endregion

    #region FUNCTIONS CALLED IN UPDATE

    //Refresh grid materials
    public void RefreshGrid()
    {
        if (isGarden)
        {
            GetComponent<MeshRenderer>().material = grass;
        }
        else
        {
            GetComponent<MeshRenderer>().material = defaultMaterial;
        }
    }

    //Delete Cost panel 
    public void DeleteCostPanel()
    {
        GridCost = 0;
        Destroy(transform.GetChild(0).gameObject);
    }

    //Ckeck built status
    public void CheckBuilt()
    {
        try
        {
            int spacesize = transform.GetChild(0).GetComponent<SpaceOccupantManager>().GetSpaceSize();
            built = true;

        }
        catch (Exception)
        {
            built = false;
            if (occupied && GridCost > 0)
            {
                DeleteCostPanel();
            }
        }
    }
    #endregion

    #region OCCUPANTS FUNCTIONS
    //Insert Occupant into the Occupant array
    public void InsertOccupant(int occupant)
    {
        if (NumberOfSpheres <= MaximumCapacity)
        {
            for (int i = MaximumCapacity - 1; i > 0; i--)
            {
                occupyingPlayers[i] = occupyingPlayers[i - 1];
            }
            occupyingPlayers[0] = occupant;
        }
        else
        {
            SpillPlayers.Add(occupant);
            Debug.Log("Grid(" + i + "," + j + "): Spill player added. Spill count is " + SpillPlayers.Count);
        }
    }

    //Clear occupant array
    public void ClearOccupants()
    {
        for(int i = 0; i < MaximumCapacity; i++)
        {
            occupyingPlayers[i] = -1;
        }
    }
#endregion

    #region NEIGHBOURHOOD FUNCTIONS
    //Compute neighbourhood
    public void ComputeNeighbourhood()
    {
        GameObject neighbour;
        try
        {
            neighbour = GameObject.Find("GameBoard").GetComponent<GameBoardController>().GameBoardArray[i - 1, j];
            if (neighbour.GetComponent<GridUnit>())
            {
                Neighbours.Add(neighbour);
                Neighbourhood.Add("left", -1);
            }
        }
        catch (Exception) { }
        try
        {
            neighbour = GameObject.Find("GameBoard").GetComponent<GameBoardController>().GameBoardArray[i, j + 1];
            if (neighbour.GetComponent<GridUnit>())
            {
                Neighbours.Add(neighbour);
                Neighbourhood.Add("up", -1);
            }
        }
        catch (Exception) { }
        try
        {
            neighbour = GameObject.Find("GameBoard").GetComponent<GameBoardController>().GameBoardArray[i + 1, j];
            if (neighbour.GetComponent<GridUnit>())
            {
                Neighbours.Add(neighbour);
                Neighbourhood.Add("right", -1);
            }
        }
        catch (Exception) { }
        try
        {
            neighbour = GameObject.Find("GameBoard").GetComponent<GameBoardController>().GameBoardArray[i, j - 1];
            if (neighbour.GetComponent<GridUnit>())
            {
                Neighbours.Add(neighbour);
                Neighbourhood.Add("down", -1);
            }
        }
        catch (Exception) { }

        //Initiate the Sides array from the dictionary length
        Sides = new string[Neighbourhood.Keys.Count];
        Neighbourhood.Keys.CopyTo(Sides, 0);
        //Calculate the Maximum capacity from the number of sides
        MaximumCapacity = Sides.Length;
        //Initialize rotation array based on the maximum capacity and the sides that are available
        switch (MaximumCapacity)
        {
            case 2:
                RotationArray = new int[1];
                RotationArray[0] = 0;
                break;
            case 3:
                RotationArray = new int[2];
                if (Neighbourhood.ContainsKey("left") && Neighbourhood.ContainsKey("right"))
                {
                    RotationArray[0] = 0;
                    RotationArray[1] = 180;
                }
                else
                {
                    RotationArray[0] = 90;
                    RotationArray[1] = 270;
                }
                break;
            case 4:
                RotationArray = new int[4];
                RotationArray[0] = 0;
                RotationArray[1] = 90;
                RotationArray[2] = 180;
                RotationArray[3] = 270;
                break;
            default:
                break;
        }
    }

    //Update neighbourhood
    public void UpdateNeighbourhood()
    {
        if (NumberOfSpheres <= MaximumCapacity)
        {
            //If the side of instantiation is not random
            if (!SideOfInstantiation.Equals("random"))
            {
                //string oldSide = SideOfInstantiation;
                //SideOfInstantiation = InverseKey(oldSide);
                //Check if the dictionary has the key empty (-1)
                if (Neighbourhood[SideOfInstantiation] == -1)
                {
                    //Then add the entry to that position in the neighbourhood
                    Neighbourhood[SideOfInstantiation] = occupyingPlayers[0];
                    Debug.Log("Grid(" + i.ToString() + "," + j.ToString() + "): Player " + occupyingPlayers[0] + " is placed in the Dictionary at the index " + SideOfInstantiation);
                }
                else
                {
                    int index = Array.IndexOf(Sides, SideOfInstantiation);
                    Debug.Log("Grid(" + i.ToString() + "," + j.ToString() + "): Index to be inserted in: " + index);
                    //CLOCKWISE CYCLING
                    //Cycle until an empty slot is in the slot that you want to insert the occupant
                    Cycle:
                    //Store the last as a temporary
                    int temp = Neighbourhood[Sides[Sides.Length - 1]];
                    for (int i = Sides.Length - 1; i > 0; i--)
                    {
                        Neighbourhood[Sides[i]] = Neighbourhood[Sides[i - 1]];
                    }
                    Neighbourhood[Sides[0]] = temp;
                    if (Neighbourhood[Sides[index]] != -1)
                    {
                        goto Cycle;
                    }
                    Neighbourhood[Sides[index]] = occupyingPlayers[0];

                    Debug.Log("Grid(" + i.ToString() + ", " + j.ToString() + "): " + "Clockwise cycled.");
                }
            }
            //Else if it is random
            else
            {
                //Place the occupant in any empty slot
                //beginning from index 0 check if any slot is empty
                int index = 0;
                while (Neighbourhood[Sides[index]] != -1)
                {
                    index++;
                }
                Neighbourhood[Sides[index]] = occupyingPlayers[0];
                Debug.Log("Grid(" + i.ToString() + "," + j.ToString() + "): Player " + occupyingPlayers[0] + " is placed in the Dictionary at the index " + Sides[index]);
            }
        }
    }

    //Clear Neighbourhood
    public void ClearNeighbourhood()
    {
        for(int index = 0; index < Sides.Length; index++)
        {
            Neighbourhood[Sides[index]] = -1;
        }
    }

    //Print Neighbourhood
    public void PrintNeighbourhood()
    {
        string printString = "Grid(" + i + "," + j + ") Neighbourhood: ";
        for(int i = 0; i < Sides.Length; i++)
        {
            printString += Sides[i] + ": ";
            printString += Neighbourhood[Sides[i]] + "; ";
        }
        Debug.Log(printString);
    }
#endregion

    //Explosion function
    public void Explode()
    {
        PrintNeighbourhood();
        //First explode the occupants with floor spaces i.e. those within the occupants array
        for(int x = 0; x < Neighbours.Count; x++)
        {
            Neighbours[x].GetComponent<GridUnit>().SetGridState(true, Sides[x], true, Neighbourhood[Sides[x]], Neighbours[x].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
            if (Neighbours[x].GetComponent<GridUnit>().GetNumberOfSpheres() > Neighbours[x].GetComponent<GridUnit>().MaximumCapacity - 1)
            {
                GameObject.Find("GameBoard").GetComponent<ChainReactionScheduler>().ScheduleExplosion(Neighbours[x].GetComponent<GridUnit>().i, Neighbours[x].GetComponent<GridUnit>().j);
            }
        }
        //Then explode and locate the spill players
        for(int x = 0; x < SpillPlayers.Count; x++)
        {
            Neighbours[x].GetComponent<GridUnit>().SetGridState(true, Sides[x], true, SpillPlayers[x], Neighbours[x].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
            if (Neighbours[x].GetComponent<GridUnit>().GetNumberOfSpheres() > Neighbours[x].GetComponent<GridUnit>().MaximumCapacity - 1)
            {
                GameObject.Find("GameBoard").GetComponent<ChainReactionScheduler>().ScheduleExplosion(Neighbours[x].GetComponent<GridUnit>().i, Neighbours[x].GetComponent<GridUnit>().j);
            }

        }
        //Then clear the data space of the cell
        ClearDataSpace();
        //make it a garden because it has exploded
        setGarden(true);
    }

    //Clear all arrays and lists
    public void ClearDataSpace()
    {
        occupied = false;
        ClearOccupants();
        NumberOfSpheres = 0;
        SpillPlayers.Clear();
        ClearNeighbourhood();
    }

    //Check explosion state to avoid repeated explosions
    public bool CheckExplosionState()
    {
        bool explosive = true;
        for(int i = 0; i < occupyingPlayers.Length; i++)
        {
            if (occupyingPlayers[i] == -1)
                explosive = false;
        }
        return explosive;
    }
}
