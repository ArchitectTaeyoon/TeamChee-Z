using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PlayerData {
    public string PlayerName;
    public Color PlayerColor;
    [HideInInspector]
    public int PlayerID;
}
public class PlayerController : MonoBehaviour {

    //Variable to hold count of the turns
    int NumberOfTurns;

    //Variable holding Current Player Data
    PlayerData CurrentPlayer;

    //PlayerData Array
    public PlayerData[] PlayerArray;

    //boolean to check whether the player has played
    bool played;

	// Use this for initialization
	void Start () {
        SetPlayerIDs();
        played = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Function to go to the next turn
    public void NextTurn() {
        NumberOfTurns++;
        played = false;
        SetCurrentPlayer();
    }

    //Determining Current Player based on the number of turns
    void SetCurrentPlayer() {
        int currentPlayerId = ((NumberOfTurns % PlayerArray.Length)+(PlayerArray.Length-1))%PlayerArray.Length;
        CurrentPlayer = PlayerArray[currentPlayerId];
        //Debug.Log(CurrentPlayer.PlayerID.ToString());
        Debug.Log(CurrentPlayer.PlayerName + " is playing...");
        gameObject.GetComponent<GameBoardController>().GridColorChange(CurrentPlayer.PlayerColor);
    }

    public PlayerData GetCurrentPlayer() {
        return CurrentPlayer;
    }

    //Set Player IDS
    void SetPlayerIDs(){
        for (int i = 0; i < PlayerArray.Length; i++) {
            PlayerArray[i].PlayerID = i;
        }
    }

    //Turn on played boolean
    public void Played() {
        played = true;
    }

    //get bool played
    public bool GetPlayState() {
        return played;
    }
}
