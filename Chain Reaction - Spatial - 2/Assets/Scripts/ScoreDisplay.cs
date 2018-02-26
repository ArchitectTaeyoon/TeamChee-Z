﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour {

    //The text gameobjects
    public Text playerNames;
    public Text playerScores;

    PlayerData[] PlayerDataArray;

	// Use this for initialization
	void Start () {
        PlayerDataArray = GameObject.Find("GameBoard").GetComponent<PlayerController>().PlayerArray;
        UpdatePlayerNames();
	}
	
	// Update is called once per frame
	void Update () {
        UpdatePlayerScores();
	}

    //Fill the player names
    public void UpdatePlayerNames() {
        playerNames.text = "";
        for (int i = 0; i < PlayerDataArray.Length; i++) {
            playerNames.text += PlayerDataArray[i].PlayerName + "\n";
        }
    }

    //Update scores
    public void UpdatePlayerScores()
    {
        playerScores.text = "";
        foreach (PlayerData player in PlayerDataArray) {
            playerScores.text += player.GetScore() + "\n";
        }
    }
}
