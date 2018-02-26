using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostManager : MonoBehaviour {

    //Basic cost
    public int basicCost;


	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {

	}
    /*
    public void ComputeCost()
    {
        for (int i = 0; i < gameObject.GetComponent<GameBoardController>().length; i++)
        {
            for (int j = 0; j < gameObject.GetComponent<GameBoardController>().breadth; j++)
            {
                int GridCost = basicCost / gameObject.GetComponent<GameBoardController>().GameBoardArray[i, j].GetComponent<GridUnit>().MaximumCapacity;
                gameObject.GetComponent<GameBoardController>().GameBoardArray[i, j].GetComponent<GridUnit>().SetCost(GridCost);
            }
        }
    }*/

    //Function to distribute money
    public void DistributeMoney(int id, int money)
    {
        for(int i = 0; i < GetComponent<PlayerController>().PlayerArray.Length; i++)
        {
            if (i == id)
            {
                GetComponent<PlayerController>().PlayerArray[i].Money -= money;
                continue;
            }
            int distribution = money / (GetComponent<PlayerController>().PlayerArray.Length - 1);
            GetComponent<PlayerController>().PlayerArray[i].Money += distribution;
        }
    }

    public bool CheckFund(int id, int money)
    {
        bool allow;
        int playerMoney = GetComponent<PlayerController>().PlayerArray[id].Money;
        if (playerMoney >= money)
        {
            allow = true;
        }
        else
        {
            allow = false;
        }
        return allow;
    }
}
