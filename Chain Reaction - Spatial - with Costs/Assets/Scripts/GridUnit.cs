using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridUnit : MonoBehaviour {

    //Corner/ Edge/ Inner booleans
    bool corner;
    bool edge;
    bool inner;

    //Dictionary to hold the neighbourhood
    Dictionary<string, int> Neighbourhood;
    string[] Sides;

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

    //Array containing the choices of the Rotation indices of the space instantiation
    int[] RotationArray = { 0, 90, 180, 270 };

    // Use this for initialization
    void Start () {
        //instantiate the occupant array with the maximum number the grid can hold
        Neighbourhood = new Dictionary<string, int>();
        ComputeNeighbourhood();
        cost = transform.GetChild(0).GetComponent<RectTransform>().GetChild(0).gameObject.GetComponent<Text>();
        SetCost();
        occupyingPlayers = new int[MaximumCapacity];
        ClearOccupants(); 
        SingleSpace = GameObject.Find("GameBoard").GetComponent<GameBoardController>().SingleSpace;
        DoubleSpace = GameObject.Find("GameBoard").GetComponent<GameBoardController>().DoubleSpace;
        TripleSpace = GameObject.Find("GameBoard").GetComponent<GameBoardController>().TripleSpace;
        
    }
	
	// Update is called once per frame
	void Update () {
        currentPlayer = GameObject.Find("GameBoard").GetComponent<PlayerController>().GetCurrentPlayer();
        //currentPlayer = GetComponentInParent<PlayerController>().GetCurrentPlayer();
        CheckBuilt(); //Check if the grid is built every frame
        RefreshGrid();
	}

    //Getter & Setters for the three booleans ---> Corner/ Edge/ Inner
#region GETTER SETTER FOR CORNER / EDGE / INNER
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
#endregion

    //Setter for the grid address
    public void SetAddress(int _i, int _j) {
        i = _i;
        j = _j;
    }

    //Set Grid State - 6 arguments 
    public void SetGridState(bool _isExplosion, string _sideOfInstantiation, bool _occupied, int _occupyingPlayer, int _NumberOfSpheres, bool _isGarden) {
        isExplosion = _isExplosion;
        SideOfInstantiation = _sideOfInstantiation;
        occupied = _occupied;
        NumberOfSpheres = _NumberOfSpheres;
        InsertOccupant(_occupyingPlayer);
        isGarden = _isGarden;
        UpdateGrid();
    }

    //Set Grid State - 5 arguments
    public void SetGridState(bool _isExplosion, string _sideOfInstantiation, bool _occupied, int _occupyingPlayer, int _NumberOfSpheres)
    {
        isExplosion = _isExplosion;
        SideOfInstantiation = _sideOfInstantiation;
        occupied = _occupied;
        InsertOccupant(_occupyingPlayer);
        NumberOfSpheres = _NumberOfSpheres;
        UpdateGrid();
    }

    //Empty a grid
    public void EmptyGrid() {
        occupied = false;
        ClearOccupants();
        NumberOfSpheres = 0;
        Destroy(transform.GetChild(0).gameObject);
    }
    
    //Update the grid based on the increasing numbers
    public void UpdateGrid() {
        if (NumberOfSpheres<MaximumCapacity)
        {
            if (built)
            {
                Destroy(transform.GetChild(0).gameObject);
            }

            //Gameobject to be instatiated based on the number of spaces
            GameObject currentSpacePrefab;
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

            //Determine the rotation in which the embedded space needs to be instantiated
            int RotationAngle = 0;
            switch (SideOfInstantiation)
            {
                case "left":
                    RotationAngle = RotationArray[0];
                    break;
                case "up":
                    RotationAngle = RotationArray[1];
                    break;
                case "right":
                    RotationAngle = RotationArray[2];
                    break;
                case "down":
                    RotationAngle = RotationArray[3];
                    break;
                case "random":
                    RotationAngle = RotationArray[Random.Range(0, 3)];
                    break;
                default:
                    break;
            }

            //Instantiate the embeddedSpace
            GameObject currentSpace = Instantiate(currentSpacePrefab, new Vector3(i, 0, j), Quaternion.Euler(0, RotationAngle, 0));
            //Set the space as a child
            currentSpace.transform.parent = transform;
        }
    }
    
    //Inform board on mouse click
    void OnMouseDown()
    {
        if (GameObject.Find("GameBoard").GetComponent<GameBoardController>().ChainReactionOver==true&&!isGarden) {
            if (occupied)
            {
                //Two possibilities:
                string preference = GameObject.Find("GameBoard").GetComponent<GameBoardController>().sharedSpacePreference;
                bool allow = false; //if this turns true, allow the player to expand
                switch (preference)
                {
                    //A. If at least one floor in the grid unit is held by the current player, he can expand in it
                    case "atleastOne":
                        for(int i = 0; i < MaximumCapacity; i++)
                        {
                            int occupant = occupyingPlayers[i];
                            if (currentPlayer.PlayerID == occupant)
                                allow = true;
                        }
                        break;
                    //B. If the player has 50% or more spaces in the grid, he can expand in it
                    case "halfOrAbove":
                        int count = 0; // count of the current player's spaces in the grid
                        for(int i = 0; i < MaximumCapacity; i++)
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
                    Debug.Log("Grid(" + i.ToString() + "," + j.ToString() + ") clicked.");
                    GameObject.Find("GameBoard").GetComponent<GameBoardController>().TriggerChainReaction(i, j);
                    GameObject.Find("GameBoard").GetComponent<PlayerController>().Played();
                }
                else
                {
                    Debug.Log("Permission failed.");
                }
            }
            else {
                if (GameObject.Find("GameBoard").GetComponent<CostManager>().CheckFund(currentPlayer.PlayerID,GridCost))
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

    //Return number of occupying spheres
    public int GetNumberOfSpheres() {
        return NumberOfSpheres;
    }

    //check if grid is occupied
    public bool IsOccupied()
    {
        return occupied;
    }

    //set the boolean isGarden
    public void setGarden(bool _isGarden)
    {
        isGarden = _isGarden;
    }

    //Check built status automatically
    public void CheckBuilt()
    {
        if (NumberOfSpheres > 0)
        {
            built = true;
            //if Grid cost is greater than zero, make it zero and delete the cost panel
            if (GridCost > 0)
            {
                GridCost = 0;
                Destroy(transform.GetChild(0).gameObject);
            }
        }
        else
        {
            built = false;
        }
    }

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

    //Insert Occupant into the Occupant array
    public void InsertOccupant(int occupant)
    {
        for(int i = MaximumCapacity-1; i > 0; i--)
        {
            occupyingPlayers[i] = occupyingPlayers[i - 1];
        }
        occupyingPlayers[0] = occupant;
    }

    //Clear occupant array
    public void ClearOccupants()
    {
        for(int i = 0; i < MaximumCapacity; i++)
        {
            occupyingPlayers[i] = -1;
        }
    }


    //Update cost
    public void SetCost()
    {
        GridCost = GameObject.Find("GameBoard").GetComponent<CostManager>().basicCost / MaximumCapacity;
        cost.text = "$ "+ GridCost.ToString();
    }

    //Compute neighbourhood
    public void ComputeNeighbourhood()
    {
        if (GameObject.Find("GameBoard").GetComponent<GameBoardController>().GameBoardArray[i - i, j])
        {
            Neighbourhood.Add("left", -1);
        }
        if (GameObject.Find("GameBoard").GetComponent<GameBoardController>().GameBoardArray[i, j + 1])
        {
            Neighbourhood.Add("up", -1);
        }
        if (GameObject.Find("GameBoard").GetComponent<GameBoardController>().GameBoardArray[i + i, j])
        {
            Neighbourhood.Add("right", -1);
        }
        if (GameObject.Find("GameBoard").GetComponent<GameBoardController>().GameBoardArray[i, j - 1])
        {
            Neighbourhood.Add("down", -1);
        }
        Sides = new string[Neighbourhood.Keys.Count];
        Neighbourhood.Keys.CopyTo(Sides, 0);
        MaximumCapacity = Sides.Length;
    }

    //Update Neighbourhood
    public void UpdateNeighbourhood()
    {

    }
}
