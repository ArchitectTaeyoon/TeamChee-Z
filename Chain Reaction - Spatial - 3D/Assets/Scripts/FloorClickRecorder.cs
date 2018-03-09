using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorClickRecorder : MonoBehaviour {

    public bool ClickEnabled = false;

    //Grid parent
    GameObject grid;

    //grid address
    int i;
    int j;
    int k;

	// Use this for initialization
	void Start () {
        grid = transform.parent.gameObject.transform.parent.gameObject;
        i = grid.GetComponent<GridUnit>().i;
        j = grid.GetComponent<GridUnit>().j;
        k = transform.parent.GetComponent<SpaceOccupantManager>().k;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    /*
    public void OnMouseDown()
    {
        if (ClickEnabled)
        {
            if (GameObject.Find("GameBoard").GetComponent<GameBoardController>().ChainReactionOver == true && !grid.GetComponent<GridUnit>().isGarden)
            {
                //Two possibilities:
                string preference = GameObject.Find("GameBoard").GetComponent<GameBoardController>().sharedSpacePreference;
                bool allow = false; //if this turns true, allow the player to expand
                switch (preference)
                {
                    //A. If at least one floor in the grid unit is held by the current player, he can expand in it
                    case "atleastOne":
                        for (int i = 0; i < transform.parent.gameObject.GetComponent<GridUnit>().MaximumCapacity; i++)
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
        }
    }*/
}
