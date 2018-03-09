using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMode : MonoBehaviour {

    public bool playMode = false;
    public Text gameModeDisplay;
    public Button PlayButton;
    public GameObject ScorePanel;

	// Use this for initialization
	void Start () {
        gameModeDisplay.text = "Build Mode";
        ScorePanel.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {

    }

    public void StartPlay()
    {
        Debug.Log("Clicked!");
        playMode = true;
        gameModeDisplay.text = "Play Mode";
        PlayButton.enabled = false;
        ScorePanel.SetActive(true);

        for(int i = 0; i < GetComponent<GameBoardController>().length; i++)
        {
            for(int j=0;j<GetComponent<GameBoardController>().breadth; j++)
            {
                GameObject current = GetComponent<GameBoardController>().GameBoardArray[i, j];
                
                for (int k = 0; k < current.GetComponent<BoardGrid>().height; k++)
                {
                    current.GetComponent<BoardGrid>().VerticalNeighbourhood[k].GetComponent<GridUnit>().ReadyGrid();
                }
                current.GetComponent<BoardGrid>().InvokeGridUnit(current.GetComponent<BoardGrid>().HighestActive());
                current.GetComponent<BoardGrid>().ReadyBoard();
            }
        }
    }
}
