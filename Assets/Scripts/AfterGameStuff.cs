using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CSVEditor;

public class AfterGameStuff : MonoBehaviour {

    public GameObject newHighPanel;
    public GameObject boringPanel;
    public GameObject highScoresPanel;
    List<CSVManager.Record> highScores;

    Text yourScoreText;
    Text yourScoreHighText;
    Text highScoresText;
    InputField nameField;

    void Awake()
    {
        yourScoreText = GameObject.Find("YourScoreText").GetComponent<Text>();
        yourScoreHighText = GameObject.Find("YourScoreHighText").GetComponent<Text>();
        highScoresText = GameObject.Find("HighScoresText").GetComponent<Text>();
        nameField = GameObject.Find("NameField").GetComponent<InputField>();
    }

    // Use this for initialization
    void Start ()
    {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void SetGameOverPanels()
    {
        highScores = CSVManager.getTop10Leaderboard();
        string lowestScoreStr = highScores[highScores.Count - 1].score;
        int lowestScore = (lowestScoreStr == null) ? 0 : Convert.ToInt32(lowestScoreStr);

        if (GameManager.score > lowestScore)
        {
            yourScoreHighText.text = GameManager.score.ToString();

            newHighPanel.SetActive(true);
            boringPanel.SetActive(false);
            highScoresPanel.SetActive(false);
        }
        else
        {
            yourScoreText.text = "YOUR SCORE: " + GameManager.score.ToString();

            boringPanel.SetActive(true);
            newHighPanel.SetActive(false);
            highScoresPanel.SetActive(false);
        }
    }

    public void AddNewScore()
    {
        string name = nameField.text;

        if (name == "") name = "Unknown";

        CSVManager.addRecord(name.ToUpper(), GameManager.score);

        ShowHighScores();
    }

    public void ShowHighScores()
    {
        highScores = CSVManager.getTop10Leaderboard();

        string hiScores = "";

        for(int i = 0; i < highScores.Count; i++)
        {
            string n = (highScores[i].name == null) ? "" : highScores[i].name;
            string s = (highScores[i].score == null) ? "" : highScores[i].score;

            string newLine = string.Format("{0, -2}. {1, -41} {2, -6}\n", (i+1).ToString(), n, s);
            hiScores += newLine;
        }

        highScoresText.text = hiScores;

        highScoresPanel.SetActive(true);
        boringPanel.SetActive(false);
        newHighPanel.SetActive(false);
    }
}
