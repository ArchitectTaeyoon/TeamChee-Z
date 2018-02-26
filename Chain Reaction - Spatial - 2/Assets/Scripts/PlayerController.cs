using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Struct that carries data about the players
[System.Serializable]
public struct PlayerData {
    public string PlayerName;
    public Color PlayerColor;
    [HideInInspector]
    public int PlayerID;
    private int PlayerScore;

    public void AddScore(int _score) {
        PlayerScore += _score;
    }

    public void SetScore(int _score)
    {
        PlayerScore =_score;
    }

    public int GetScore() {
        return PlayerScore;
    }
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

    //Gameboard Array
    GameObject[,] GameBoardArray;
    int length;
    int breadth;

	// Use this for initialization
	void Start () {
        SetPlayerIDs();
        played = false;
        GameBoardArray = gameObject.GetComponent<GameBoardController>().GameBoardArray;
        length = gameObject.GetComponent<GameBoardController>().length;
        breadth = gameObject.GetComponent<GameBoardController>().breadth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Function to go to the next turn
    public void NextTurn() {
        ScoreCalculator();
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

    //To calculate scores of each player
    public void ScoreCalculator()
    {
        ResetScores();
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < breadth; j++)
            {
                if (GameBoardArray[i, j].GetComponent<GridUnit>().IsOccupied())
                {
                    for(int x = 0; x < GameBoardArray[i,j].GetComponent<GridUnit>().MaximumCapacity; x++)
                    {
                        int currentPlayer = GameBoardArray[i, j].GetComponent<GridUnit>().occupyingPlayers[x];
                        if (currentPlayer != -1)
                        {
                            PlayerArray[currentPlayer].AddScore(1);
                        }
                    }
                }
            }
        }
    }

    //Reset scores
    public void ResetScores() {
        for(int i = 0; i < PlayerArray.Length; i++)
        {
            PlayerArray[i].SetScore(0);
        }
    }
}
