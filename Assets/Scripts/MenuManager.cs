using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CSVEditor;

public class MenuManager : MonoBehaviour {

    public GameObject menuPanel;
    public GameObject highScorePanel;
    public GameObject creditsPanel;
    public Text highScoresText;

    List<CSVManager.Record> highScores;

    // Use this for initialization
    void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void BackToMenuPanel()
    {
        highScorePanel.SetActive(false);
        creditsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void GoToHighScorePanel()
    {
        highScores = CSVManager.getTop10Leaderboard();

        string hiScores = "";

        for (int i = 0; i < highScores.Count; i++)
        {
            string n = (highScores[i].name == null) ? "" : highScores[i].name;
            string s = (highScores[i].score == null) ? "" : highScores[i].score;

            string newLine = string.Format("{0, -2}. {1, -41} {2, -6}\n", (i + 1).ToString(), n, s);
            hiScores += newLine;
        }

        highScoresText.text = hiScores;

        creditsPanel.SetActive(false);
        menuPanel.SetActive(false);
        highScorePanel.SetActive(true);
    }

    public void GoToCreditsPanel()
    {
        menuPanel.SetActive(false);
        highScorePanel.SetActive(false);
        creditsPanel.SetActive(true);
    }
}
