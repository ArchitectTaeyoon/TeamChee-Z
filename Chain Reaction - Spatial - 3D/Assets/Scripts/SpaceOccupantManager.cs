using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpaceOccupantManager : MonoBehaviour
{

    //Array to hold the occupant ids --> The length of the array depends on the size of the space
    public int[] floorOccupantArray;
    int SpaceSize=0;

    //Floor plane material
    Material floorMaterial;

    //Height at which it is invoked
    public int k; 

    // Use this for initialization
    void Start()
    {
        InstantiateArray();
        floorMaterial = GameObject.Find("GameBoard").GetComponent<GameBoardController>().floorPlaneMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        RefreshSpace();
    }

    //To instantiate the floorOccupantArray with the number of spaces the prefab has based on its tag
    public void InstantiateArray(){
        string tag = gameObject.tag;
        if (tag == "Single")
            SpaceSize = 1;
        else if (tag == "Double")
            SpaceSize = 2;
        else if (tag == "Triple")
            SpaceSize = 3;
        floorOccupantArray = new int[SpaceSize];
        SetOccupants();
    }

    //SetOccupants with variable length arguments
    public void SetOccupants()
    {
        for (int i = 0; i < SpaceSize; i++)
        {
            if (transform.parent.gameObject.GetComponent<GridUnit>().k == 1)
                Debug.Log(transform.parent.gameObject.GetComponent<GridUnit>().occupyingPlayers[i]);
            floorOccupantArray[i] = transform.parent.gameObject.GetComponent<GridUnit>().occupyingPlayers[i];
        }
    }

    //Get the space size
    public int GetSpaceSize()
    {
        return SpaceSize;
    }

    //Refresh Materials
    public void RefreshSpace()
    {
        for(int i = 0; i < SpaceSize; i++)
        {
            transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = floorMaterial;
            PlayerData currentOccupant;
            try {
                currentOccupant = GameObject.Find("GameBoard").GetComponent<PlayerController>().PlayerArray[floorOccupantArray[i]];
            }
            catch (Exception)
            {
                Debug.Log(transform.parent.gameObject.GetComponent<GridUnit>().Sides[i]);
                Debug.Log(transform.parent.gameObject.GetComponent<GridUnit>().Neighbourhood[transform.parent.gameObject.GetComponent<GridUnit>().Sides[i]]);
                currentOccupant = GameObject.Find("GameBoard").GetComponent<PlayerController>().PlayerArray[transform.parent.gameObject.GetComponent<GridUnit>().Neighbourhood[transform.parent.gameObject.GetComponent<GridUnit>().Sides[i]]];
            }
            transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", currentOccupant.PlayerColor);
        }
    }
}
