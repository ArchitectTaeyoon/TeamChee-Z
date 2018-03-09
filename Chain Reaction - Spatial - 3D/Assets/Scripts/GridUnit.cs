using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GridUnit : MonoBehaviour {

    #region VARIABLES
    //Boolean to check if the grid is Garden
    public bool isGarden = false;
    public bool visible = true;
    public bool click = false;
    public bool buildable = true;
    public bool externallyBuilt = false;
    public bool terrace = false;

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


    //Stores the grid address
    [Header("Grid address")]
    public int i;
    public int j;
    public int k;

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
    public List<GameObject> VerticalNeighbours;

    //List to hold the spilt players, those beyond the Maximum capacity
    public List<int> SpillPlayers;
    public int numberOfSpill;

    public bool gridReady = false;

    #endregion

    // Use this for initialization
    void Start () {

    }
	
    public void ReadyGrid()
    {
        if (!gridReady)
        {
            //Instantiate the prefabs
            SingleSpace = GameObject.Find("GameBoard").GetComponent<GameBoardController>().SingleSpace;
            DoubleSpace = GameObject.Find("GameBoard").GetComponent<GameBoardController>().DoubleSpace;
            TripleSpace = GameObject.Find("GameBoard").GetComponent<GameBoardController>().TripleSpace;
            //Instantiate the Neighbourhood dictionary to hold the occupants
            Neighbourhood = new Dictionary<string, int>();
            Neighbours = new List<GameObject>();
            VerticalNeighbours = new List<GameObject>();
            ComputeNeighbourhood();
            //Instantiate the occupying players array
            //occupyingPlayers = new int[MaximumCapacity];
            ClearOccupants(); 
            //Instantiate the Spill Players list
            SpillPlayers = new List<int>();
        }
        gridReady = true;
    }

	// Update is called once per frame
	void Update () {

        currentPlayer = GameObject.Find("GameBoard").GetComponent<PlayerController>().GetCurrentPlayer();
        RefreshGrid();
        CheckBuilt();
        numberOfSpill = SpillPlayers.Count;
        DuplicationCheck();
	}

    //Inform board on mouse click
    void OnMouseDown()
    {
        if (GameObject.Find("GameBoard").GetComponent<GameMode>().playMode)
        {
            if (k < GameObject.Find("GameBoard").GetComponent<GameBoardController>().height-2)
            {
                if (click)
                {
                    if (GameObject.Find("GameBoard").GetComponent<GameBoardController>().ChainReactionOver == true && !isGarden)
                    {
                        if (k > 0 || occupied)
                        {
                            //Two possibilities:
                            string preference = GameObject.Find("GameBoard").GetComponent<GameBoardController>().sharedSpacePreference;
                            bool allow = false; //if this turns true, allow the player to expand
                            if (!occupied && terrace)
                            {
                                allow = true;
                            }
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
                                //C. Anyone can initiate anywhere
                                case "anyone":
                                    allow = true;
                                    break;
                                default:
                                    break;

                            }
                            if (allow)
                            {
                                Debug.Log("Grid(" + i.ToString() + ", " + j.ToString() + ", " + k.ToString() + ") clicked.");
                                GameObject.Find("GameBoard").GetComponent<GameBoardController>().TriggerChainReaction(i, j, k);
                                GameObject.Find("GameBoard").GetComponent<PlayerController>().Played();
                            }
                            else
                            {
                                Debug.Log("Permission failed.");
                            }
                        }
                        else
                        {
                            if (GameObject.Find("GameBoard").GetComponent<CostManager>().CheckFund(currentPlayer.PlayerID, transform.parent.gameObject.GetComponent<BoardGrid>().GridCost))
                            {
                                currentPlayer.Money -= transform.parent.gameObject.GetComponent<BoardGrid>().GridCost;
                                GameObject.Find("GameBoard").GetComponent<CostManager>().DistributeMoney(currentPlayer.PlayerID, transform.parent.gameObject.GetComponent<BoardGrid>().GridCost);
                                transform.parent.gameObject.GetComponent<BoardGrid>().Bought();
                                Debug.Log("Grid(" + i.ToString() + ", " + j.ToString() + ", " + k.ToString() + ") clicked.");
                                GameObject.Find("GameBoard").GetComponent<GameBoardController>().TriggerChainReaction(i, j, k);
                                GameObject.Find("GameBoard").GetComponent<PlayerController>().Played();
                            }
                            else
                            {
                                Debug.Log("Insufficient funds.");
                            }
                        }
                    }
                }
            
            }
        }
        else
        {
            if (buildable)
            {
                buildable = false;
                if (k == 0)
                {
                    transform.parent.gameObject.GetComponent<BoardGrid>().Bought();
                }
            }
            else
            {
                externallyBuilt = true;
                buildable = false;
                GameObject external = Instantiate(GameObject.Find("GameBoard").GetComponent<GameBoardController>().ExternallyBuiltSpace, transform.position+Vector3.up*0.125f, Quaternion.identity);
                external.transform.parent = transform;
                transform.parent.GetComponent<BoardGrid>().InvokeGridUnit();
                transform.parent.GetComponent<BoardGrid>().VerticalNeighbourhood[transform.parent.GetComponent<BoardGrid>().HighestActive() - 1].GetComponent<GridUnit>().terrace = true;
                
            }
        }
    }

    #region GRID STATE FUNCTIONS
    //Set Grid State - 6 arguments 
    public void SetGridState(bool _isExplosion, string _sideOfInstantiation, bool _occupied, int _occupyingPlayer, int _NumberOfSpheres, bool _isGarden) {
        MakeVisible();
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
        MakeVisible();
        isExplosion = _isExplosion;
        SideOfInstantiation = InverseKey(_sideOfInstantiation);
        occupied = _occupied;
        NumberOfSpheres = _NumberOfSpheres;
        InsertOccupant(_occupyingPlayer);
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
                    RotationAngle = 0;
                    break;
                default:
                    break;
            }

            //Instantiate the embeddedSpace
            GameObject currentSpace = Instantiate(currentSpacePrefab, new Vector3(i, k*transform.parent.gameObject.GetComponent<BoardGrid>().clearHeight, j), Quaternion.Euler(0, RotationAngle, 0));
            //Set the space as a child
            currentSpace.transform.parent = transform;
            built = true;
        }

        DuplicationCheck();
    }
    #endregion

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

    //Setter for the grid address
    public void SetAddress(int _i, int _j, int _k){
        i = _i;
        j = _j;
        k = _k;
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

    //Enable click
    public void EnableClick()
    {
        click = true;
    }

    //Disable click
    public void DisableClick() {
        click = false;
    }

    //Make Visible
    public void MakeVisible()
    {
        visible = true;
    }

    //Make Invisible
    public void MakeInvisible()
    {
        visible = false;
    }
    #endregion

    #region FUNCTIONS CALLED IN UPDATE

    //Refresh grid materials
    public void RefreshGrid()
    {
        if (visible)
        {
            GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
        if (isGarden)
        {
            visible = true;
            GetComponent<MeshRenderer>().material = grass;
        }
        else
        {
            GetComponent<MeshRenderer>().material = defaultMaterial;
        }
        if (!buildable)
        {
            visible = true;
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<MeshRenderer>().material =GameObject.Find("GameBoard").GetComponent<GameBoardController>().unbuildableGridMaterial;
        }

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
        }
    }
    #endregion

    #region OCCUPANTS FUNCTIONS
    //Insert Occupant into the Occupant array
    public void InsertOccupant(int occupant)
    {
        if (NumberOfSpheres <= MaximumCapacity)
        {
            if (k == 1)
            {
                Debug.Log("Max Capacity is " + MaximumCapacity);
                Debug.Log("Size of Occupant Array: " + occupyingPlayers.Length);
            }
            for (int i = MaximumCapacity - 1; i > 0; i--)
            {
                
                occupyingPlayers[i] = occupyingPlayers[i - 1];
            }
            Debug.Log("Grid(" + i + "," + j + "," + k + "): Occupant "+ occupant.ToString()+ "inserted.");
            occupyingPlayers[0] = occupant;
            Debug.Log("Grid(" + i + "," + j + "," + k + "): Occupant " + occupyingPlayers[0].ToString() + "inserted.");
        }
        else
        {
            SpillPlayers.Add(occupant);
            Debug.Log("Grid(" + i + "," + j + "," + k + "): Spill player added. Spill count is " + SpillPlayers.Count);
        }
    }

    //Clear occupant array
    public void ClearOccupants()
    {
        if(k==1)
            Debug.Log("Grid(" + i.ToString() + "," + j.ToString() + "," + k.ToString() + "): Occupants mysteriously cleared!");
        for (int i = 0; i < MaximumCapacity; i++)
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
            neighbour = GameObject.Find("GameBoard").GetComponent<GameBoardController>().GameBoardArray[i - 1, j].GetComponent<BoardGrid>().VerticalNeighbourhood[k];
            if (neighbour.GetComponent<GridUnit>().buildable)
            {
                Neighbours.Add(neighbour);
                Neighbourhood.Add("left", -1);
            }
        }
        catch (Exception) { }
        try
        {
            neighbour = GameObject.Find("GameBoard").GetComponent<GameBoardController>().GameBoardArray[i, j + 1].GetComponent<BoardGrid>().VerticalNeighbourhood[k];
            if (neighbour.GetComponent<GridUnit>().buildable)
            {
                Neighbours.Add(neighbour);
                Neighbourhood.Add("up", -1);
            }
        }
        catch (Exception) { }
        try
        {
            neighbour = GameObject.Find("GameBoard").GetComponent<GameBoardController>().GameBoardArray[i + 1, j].GetComponent<BoardGrid>().VerticalNeighbourhood[k];
            if (neighbour.GetComponent<GridUnit>().buildable)
            {
                Neighbours.Add(neighbour);
                Neighbourhood.Add("right", -1);
            }
        }
        catch (Exception) { }
        try
        {
            neighbour = GameObject.Find("GameBoard").GetComponent<GameBoardController>().GameBoardArray[i, j - 1].GetComponent<BoardGrid>().VerticalNeighbourhood[k];
            if (neighbour.GetComponent<GridUnit>().buildable)
            {
                Neighbours.Add(neighbour);
                Neighbourhood.Add("down", -1);
            }
        }
        catch (Exception) { }

        //Initiate the Sides array from the dictionary length
        if (k == 1)
        {
            Debug.Log(Neighbourhood.Keys.Count);
        }
        Sides = new string[Neighbourhood.Keys.Count];
        Neighbourhood.Keys.CopyTo(Sides, 0);
        //Calculate the Maximum capacity from the number of sides
        MaximumCapacity = Sides.Length;
        occupyingPlayers = new int[MaximumCapacity];
        //Initiate the Vertical Neighbourhood
        try
        {
            neighbour = transform.parent.gameObject.GetComponent<BoardGrid>().VerticalNeighbourhood[k + 1];
            if (neighbour.GetComponent<GridUnit>())
            {
                VerticalNeighbours.Add(neighbour);
            }
        }
        catch (Exception) { }
        try
        {
            neighbour = transform.parent.gameObject.GetComponent<BoardGrid>().VerticalNeighbourhood[k - 1];
            if (neighbour.GetComponent<GridUnit>())
            {
                VerticalNeighbours.Add(neighbour);
            }
        }
        catch (Exception) { }
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
            if (k == 1)
            {
                Debug.Log("Entered update Neighbourhood function!!");

            }
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
                    Debug.Log("Grid(" + i.ToString() + "," + j.ToString() + "," + k.ToString() + "): Player " + occupyingPlayers[0] + " is placed in the Dictionary at the index " + SideOfInstantiation);
                }
                else
                {
                    int index = Array.IndexOf(Sides, SideOfInstantiation);
                    Debug.Log("Grid(" + i.ToString() + "," + j.ToString() + "," + k.ToString() + "): Index to be inserted in: " + index);
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

                    Debug.Log("Grid(" + i.ToString() + ", " + j.ToString() + "," + k.ToString() + "): " + "Clockwise cycled.");
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
                Debug.Log("Grid(" + i.ToString() + "," + j.ToString() + "," + k.ToString() +  "): Player " + Neighbourhood[Sides[index]] + " is placed in the Dictionary at the index " + Sides[index]);
            }
        }
    }

    //Clear Neighbourhood
    public void ClearNeighbourhood()
    {
        if (k == 1)
        {
            Debug.Log("Grid(" + i.ToString() + "," + j.ToString() + "," + k.ToString() + "): Neighbourhood mysteriously cleared!");
        }
        for(int index = 0; index < Sides.Length; index++)
        {
            Neighbourhood[Sides[index]] = -1;
        }
    }

    //Print Neighbourhood
    public void PrintNeighbourhood()
    {
        string printString = "Grid(" + i + "," + j + "," + k + ") Neighbourhood: ";
        for(int i = 0; i < Sides.Length; i++)
        {
            printString += Sides[i] + ": ";
            printString += Neighbourhood[Sides[i]] + "; ";
        }
        Debug.Log(printString);
    }

    //Recompute Neighbourhood when new neighbours get initiated
    public void RecomputeNeighbourhood()
    {
        //First check if it is occupied
        if (NumberOfSpheres == 0)
        {
            try
            {
                //Neighbourhood.Clear();
                //Neighbours.Clear();
                //VerticalNeighbours.Clear();
            }
            catch (Exception) {
                //Neighbourhood = new Dictionary<string, int>();
                //SingleSpace = GameObject.Find("GameBoard").GetComponent<GameBoardController>().SingleSpace;
                //DoubleSpace = GameObject.Find("GameBoard").GetComponent<GameBoardController>().DoubleSpace;
                //TripleSpace = GameObject.Find("GameBoard").GetComponent<GameBoardController>().TripleSpace;
                /*
                //Instantiate the prefabs
                SingleSpace = GameObject.Find("GameBoard").GetComponent<GameBoardController>().SingleSpace;
                DoubleSpace = GameObject.Find("GameBoard").GetComponent<GameBoardController>().DoubleSpace;
                TripleSpace = GameObject.Find("GameBoard").GetComponent<GameBoardController>().TripleSpace;
                //Instantiate the Neighbourhood dictionary to hold the occupants
                Neighbourhood = new Dictionary<string, int>();
                Neighbours = new List<GameObject>();
                VerticalNeighbours = new List<GameObject>();
                //Instantiate the occupying players array
                occupyingPlayers = new int[MaximumCapacity];
                //Instantiate the Spill Players list
                SpillPlayers = new List<int>();*/
            }
            Neighbourhood = new Dictionary<string, int>();
            SingleSpace = GameObject.Find("GameBoard").GetComponent<GameBoardController>().SingleSpace;
            DoubleSpace = GameObject.Find("GameBoard").GetComponent<GameBoardController>().DoubleSpace;
            TripleSpace = GameObject.Find("GameBoard").GetComponent<GameBoardController>().TripleSpace;
            ComputeNeighbourhood();
        }
        else
        {
            //Create Duplicates
            Dictionary<string, int> TempNeighbourhood = Neighbourhood;
            string[] TempSides = Sides;

            ComputeNeighbourhood();

            for(int i = 0; i < TempNeighbourhood.Keys.Count; i++)
            {
                try
                {
                    Neighbourhood[TempSides[i]] = TempNeighbourhood[TempSides[i]];
                }
                catch (Exception) { }
            }
        }
    }
    #endregion

    #region EXPLOSION FUNCTIONS

    //Explosion function
    public void Explode()
    {
        DestroyBuilt();
        PrintNeighbourhood();
        //First explode the occupants with floor spaces i.e. those within the occupants array
        for(int x = 0; x < Neighbours.Count; x++)
        {
            Neighbours[x].SetActive(true);
            Neighbours[x].GetComponent<GridUnit>().MakeVisible();
            //if (Neighbours[x].GetComponent<GridUnit>().GetNumberOfSpheres() == 0&&k>0)
            //    Neighbours[x].GetComponent<GridUnit>().RecomputeNeighbourhood();
            Neighbours[x].GetComponent<GridUnit>().ReadyGrid();
            Neighbours[x].GetComponent<GridUnit>().SetGridState(true, Sides[x], true, Neighbourhood[Sides[x]], Neighbours[x].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
            
            Neighbours[x].GetComponent<GridUnit>().EnableClick();
            if (k==0&&!Neighbours[x].transform.parent.GetComponent<BoardGrid>().bought)
                Neighbours[x].transform.parent.gameObject.GetComponent<BoardGrid>().Bought();
            if (Neighbours[x].GetComponent<GridUnit>().GetNumberOfSpheres() > Neighbours[x].GetComponent<GridUnit>().MaximumCapacity - 1)
            {
                GameObject.Find("GameBoard").GetComponent<ChainReactionScheduler>().ScheduleExplosion(Neighbours[x].GetComponent<GridUnit>().i, Neighbours[x].GetComponent<GridUnit>().j, k);
            }
        }
        //Then explode and locate the spill players
        for(int x = 0; x < SpillPlayers.Count; x++)
        {
            x = x % VerticalNeighbours.Count;
            VerticalNeighbours[x].SetActive(true);
            VerticalNeighbours[x].GetComponent<GridUnit>().MakeVisible();
            VerticalNeighbours[x].GetComponent<GridUnit>().ReadyGrid();
            VerticalNeighbours[x].GetComponent<GridUnit>().SetGridState(true, "random", true, SpillPlayers[x], VerticalNeighbours[x].GetComponent<GridUnit>().GetNumberOfSpheres() + 1, false);
            //VerticalNeighbours[x].GetComponent<GridUnit>().UpdateGrid();
            //VerticalNeighbours[x].GetComponent<GridUnit>().UpdateNeighbourhood();
            VerticalNeighbours[x].GetComponent<GridUnit>().EnableClick();

            if (VerticalNeighbours[x].GetComponent<GridUnit>().GetNumberOfSpheres() > VerticalNeighbours[x].GetComponent<GridUnit>().MaximumCapacity - 1)
            {
                GameObject.Find("GameBoard").GetComponent<ChainReactionScheduler>().ScheduleExplosion(VerticalNeighbours[x].GetComponent<GridUnit>().i, VerticalNeighbours[x].GetComponent<GridUnit>().j, VerticalNeighbours[x].GetComponent<GridUnit>().k);
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

    #endregion
}
