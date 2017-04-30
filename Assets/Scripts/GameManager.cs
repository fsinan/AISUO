using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public GameObject spawnerPrefab;
    public static bool isGamePaused;
    public static bool isGameOver;

    private GameObject gameOverCanvas;
    private GameObject pauseCanvas;
    private GameObject inGameCanvas;
    private AfterGameStuff afterGameStuff;

    [HideInInspector]
    public static int enemiesAlive;
    public static int score;
    static Text scoreText;
    static Text enemiesAliveText;
    static Text nextWaveText;

    public float timeBetweenWaves = 5f;

    GameObject[] enemySpawners;

    int wave;
    int waveSize;
    float timePassedSinceWave;

    void Awake()
    {
        gameOverCanvas = GameObject.Find("GameOverCanvas");
        pauseCanvas = GameObject.Find("PauseCanvas");
        inGameCanvas = GameObject.Find("InGameCanvas");
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        enemiesAliveText = GameObject.Find("EnemiesText").GetComponent<Text>();
        nextWaveText = GameObject.Find("NextWaveText").GetComponent<Text>();
        afterGameStuff = GetComponent<AfterGameStuff>();

        isGameOver = false;
        isGamePaused = false;

        score = 0;
        enemiesAlive = 0;
    }

    // Use this for initialization
    void Start ()
    {
        FindEnemySpawners();

        pauseCanvas.SetActive(false);
        gameOverCanvas.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        
        // Just in case
        Time.timeScale = 1f;

        wave = 0;
        timePassedSinceWave = 0f;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(isGamePaused)
            {
                if(!isGameOver)
                {
                    ResumeGame();
                }
            }
            else
            {
                PauseGame();
            }
        }

        // If no enemy left, start spawning the new wave
        if(enemiesAlive == 0)
        {
            timePassedSinceWave += Time.deltaTime;

            nextWaveText.text = "NEXT WAVE IN " + Mathf.CeilToInt(timeBetweenWaves - timePassedSinceWave).ToString() + "!";
            nextWaveText.enabled = true;

            if (timePassedSinceWave >= timeBetweenWaves)
            {
                wave++;
                waveSize = CalculateWaveSize(wave);

                for (int i = 0; i < waveSize; i++)
                {
                    SpawnEnemy(Random.Range(0, enemySpawners.Length - 1));
                }

                timePassedSinceWave = 0f;
            }
        }
        else
        {
            nextWaveText.enabled = false;
        }
	}

    int CalculateWaveSize(int currentWave)
    {
        if (currentWave <= 2)
            return currentWave;
        if (currentWave <= 10)
            return (currentWave-1) * 2;
        return (currentWave - 10) + 20;
    }

    void FindEnemySpawners()
    {
        enemySpawners = GameObject.FindGameObjectsWithTag("EnemySpawner");
    }

    void SpawnEnemy(int spawnerIndex)
    {
        Object prefab = spawnerPrefab as Object;
        Vector3 pos = enemySpawners[spawnerIndex].transform.position;
        Quaternion rot = spawnerPrefab.transform.rotation;

        Object.Instantiate(prefab, pos, rot);

        enemiesAlive++;
        SetEnemiesText();
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        isGamePaused = true;
        isGameOver = true;

        afterGameStuff.SetGameOverPanels();

        gameOverCanvas.SetActive(true);
        inGameCanvas.SetActive(false);

        // Cursor settings
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isGamePaused = true;

        pauseCanvas.SetActive(true);

        // Cursor settings
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isGamePaused = false;

        pauseCanvas.SetActive(false);

        // Cursor settings
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public static void IncreaseScore(int amount)
    {
        score += amount;

        scoreText.text = "SCORE: " + score.ToString();
    }

    public static void SetEnemiesText()
    {
        enemiesAliveText.text = "ENEMIES ALIVE: " + enemiesAlive.ToString();
    }
}
