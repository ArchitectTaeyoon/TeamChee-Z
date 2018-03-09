using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class BoardGrid : MonoBehaviour {

    public int height = 0;
    public float clearHeight = 0.35f;
    public float clearance = 0.1f;
    public bool bought = false;

    //Address
    public int i;
    public int j;


    //Neighbourhood informations
    public int numberOfNeighbours;
    public Dictionary<string, GameObject> Neighbourhood;
    public string[] Sides;

    //cost
    public int GridCost;
    Text cost;

    //Array to hold the vertical Neighbourhood
    public GameObject[] VerticalNeighbourhood;

	// Use this for initialization
	void Start () {

        //Instantiate the cost panel
        GameObject costBubble = Instantiate(GameObject.Find("GameBoard").GetComponent<GameBoardController>().costBubblePrefab);
        costBubble.GetComponent<RectTransform>().SetPositionAndRotation(new Vector3(i, 0.06f, j), Quaternion.Euler(90, 0, 0));
        costBubble.GetComponent<RectTransform>().SetParent(transform);

        cost = transform.GetChild(0).GetComponent<RectTransform>().GetChild(0).gameObject.GetComponent<Text>();
        costBubble.SetActive(false);
        VerticalNeighbourhood = new GameObject[GameObject.Find("GameBoard").GetComponent<GameBoardController>().height];
        InstantiateVerticalNeighbourhood();
        //Invoke the first bottom grid
        InvokeGridUnit();

        VerticalNeighbourhood[0].GetComponent<GridUnit>().MakeInvisible();

    }
	
    public void ReadyBoard()
    {
        //Compute the horizontal Neighbourhood
        ComputeNeighbourhood();
        //Create the Vertical Neighbourhood
        
        //VerticalNeighbourhood[0].GetComponent<GridUnit>().EnableClick();
        SetCost();
    }

    // Update is called once per frame
    void Update () {
        RefreshGrid();
	}

    #region HORIZONTAL NEIGHBOURHOOD
    //Set address
    public void SetAddress(int _i, int _j)
    {
        i = _i;
        j = _j;
    }

    //Compute the nrighbourhood
    public void ComputeNeighbourhood()
    {
        Neighbourhood = new Dictionary<string, GameObject>();
        GameObject neighbour;
        try
        {
            neighbour = GameObject.Find("GameBoard").GetComponent<GameBoardController>().GameBoardArray[i - 1, j];
            if (!neighbour.GetComponent<BoardGrid>().bought)
            {
                Neighbourhood.Add("left", neighbour);
            }
        }
        catch (Exception) { }
        try
        {
            neighbour = GameObject.Find("GameBoard").GetComponent<GameBoardController>().GameBoardArray[i, j + 1];
            if (!neighbour.GetComponent<BoardGrid>().bought)
            {
                Neighbourhood.Add("up", neighbour);
            }
        }
        catch (Exception) { }
        try
        {
            neighbour = GameObject.Find("GameBoard").GetComponent<GameBoardController>().GameBoardArray[i + 1, j];
            if (!neighbour.GetComponent<BoardGrid>().bought)
            {
                Neighbourhood.Add("right", neighbour);
            }
        }
        catch (Exception) { }
        try
        {
            neighbour = GameObject.Find("GameBoard").GetComponent<GameBoardController>().GameBoardArray[i, j - 1];
            if (!neighbour.GetComponent<BoardGrid>().bought)
            {
                Neighbourhood.Add("down", neighbour);
            }
        }
        catch (Exception) { }

        //Initiate the Sides array from the dictionary length
        Sides = new string[Neighbourhood.Keys.Count];
        Neighbourhood.Keys.CopyTo(Sides, 0);
        //Calculate the Maximum capacity from the number of sides
        numberOfNeighbours = Sides.Length;
    }
    #endregion

    #region COST FUNCTIONS
    //Update cost
    public void SetCost()
    {
        if (!bought)
        {
            try
            {
                transform.GetChild(0).gameObject.SetActive(true);
                GridCost = GameObject.Find("GameBoard").GetComponent<CostManager>().basicCost / numberOfNeighbours;
                cost.text = "$ " + GridCost.ToString();
            }
            catch (DivideByZeroException)
            {
                GameObject.Find("GameBoard").GetComponent<GameBoardController>().NeglectCosts();
            }
        }

    }

    //Set Cost externally
    public void Bought()
    {
        bought = true;
            GridCost = 0;
            DeleteCostPanel();

    }

    //DeleteCostPanel
    public void DeleteCostPanel()
    {
        Destroy(transform.GetChild(0).gameObject);
    }
    #endregion

    #region VERTICAL NEGIHBOURHOOD FUNCTIONS
    //Instantiate Vertical neighbourhood
    public void InstantiateVerticalNeighbourhood()
    {
        for(int k=0;k< GameObject.Find("GameBoard").GetComponent<GameBoardController>().height; k++)
        {
            GameObject gridUnit = Instantiate(GameObject.Find("GameBoard").GetComponent<GameBoardController>().GridUnitPrefab,new Vector3(i,(k*clearHeight)+0.06f,j),Quaternion.identity);
            gridUnit.name = "Grid Unit (" + i.ToString() + ", " + j.ToString() + ", " + k.ToString() + ")";
            gridUnit.GetComponent<GridUnit>().SetAddress(i, j, k);
            gridUnit.transform.parent = transform;
            gridUnit.SetActive(false);
            VerticalNeighbourhood[k] = gridUnit;
        }
    }

    //Get the highest active grid
    public int HighestActive()
    {
        return height;
    }

    //Invoke grids
    public void InvokeGridUnit()
    {
        if(height<= GameObject.Find("GameBoard").GetComponent<GameBoardController>().height)
        {
            height++;
            VerticalNeighbourhood[height - 1].SetActive(true);
            VerticalNeighbourhood[height - 1].GetComponent<GridUnit>().EnableClick();
        }
    }

    public void InvokeGridUnit(int h)
    {
        VerticalNeighbourhood[h - 1].SetActive(true);
        VerticalNeighbourhood[h - 1].GetComponent<GridUnit>().EnableClick();
    }

    //Check grid if its active
    public bool CheckGridActive(int _height)
    {
        bool active = false;
        try
        {
            GameObject grid = VerticalNeighbourhood[_height-1];
            if (grid.GetComponent<GridUnit>().isActiveAndEnabled)
            {
                active = true;
            }
            else
            {
                active = false;
            }
        }
        catch (Exception)
        {
            active = false;
        }
        return active;
    }
    #endregion

    //Refresh grid material
    public void RefreshGrid()
    {

            GetComponent<MeshRenderer>().material = GameObject.Find("GameBoard").GetComponent<GameBoardController>().buildableGridMaterial;

    }
}
