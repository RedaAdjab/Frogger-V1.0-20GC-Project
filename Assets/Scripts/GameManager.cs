using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private static int highScore = 0;

    public event EventHandler OnGameStart;
    public event EventHandler OnGamePlaying;
    public event EventHandler OnGameOver;
    public event EventHandler OnLivesChanged;
    public event EventHandler OnScoreChanged;
    public event EventHandler OnHighScoreChanged;

    public enum GameState
    {
        GameStart,
        GamePlaying,
        GameOver
    }

    public GameState CurrentState { get; private set; } = GameState.GameStart;
    private int lives = 4;
    private int score;
    private int lillyPadCount = 0;
    private int maxLillyPads = 5; 
    private float maxTime = 30f; //for each life
    private float timer = 0f;
    private float gameTimer = 0f;
    private int initialLives = 4;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CurrentState = GameState.GameStart;
        OnGameStart?.Invoke(this, EventArgs.Empty);
        timer = 0f;
        gameTimer = 0f;
        highScore = GenericPlayerPrefs.Load<int>("HighScore", 0);
        score = 0;
        lillyPadCount = 0;
        lives = initialLives;
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case GameState.GameStart:
                if (Input.anyKeyDown)
                {
                    StartGame();
                }
                break;

            case GameState.GamePlaying:
                gameTimer += Time.deltaTime;
                timer += Time.deltaTime;
                if (timer >= maxTime)
                {
                    RemoveLife();
                }
                break;

            case GameState.GameOver:
                if (Input.GetKeyDown(KeyCode.R))
                {
                    RestartGame();
                }
                break;
        }
    }

    public void StartGame()
    {
        CurrentState = GameState.GamePlaying;
        OnGamePlaying?.Invoke(this, EventArgs.Empty);
    }

    public void GameOver()
    {
        if (CurrentState == GameState.GameOver) return;
        CurrentState = GameState.GameOver;
        OnGameOver?.Invoke(this, EventArgs.Empty);
        if (lillyPadCount >= maxLillyPads)
        {
            AddScore(100 + lives * 50 + GetEarlyBonus());
        }
        if (score > highScore)
        {
            OnHighScoreChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void RestartGame()
    {
        if (score > highScore)
        {
            highScore = score;
            GenericPlayerPrefs.Save("HighScore", highScore);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddLillyPad()
    {
        lillyPadCount++;
        AddScore(100);
        timer = 0f;
        if (lillyPadCount >= maxLillyPads)
        {
            GameOver();
            OnGameOver?.Invoke(this, EventArgs.Empty);
        }
    }

    public void RemoveLife()
    {
        lives--;
        timer = 0f;
        OnLivesChanged?.Invoke(this, EventArgs.Empty);
        if (lives <= 0)
        {
            GameOver();
            OnGameOver?.Invoke(this, EventArgs.Empty);
        }
    }

    private int GetEarlyBonus()
    {
        float time = gameTimer;
        float _maxTime = initialLives * (int)maxTime;
        float minTime = maxTime;
        float normalizedTime = Mathf.Clamp01((time - minTime) / (_maxTime - minTime));

        float score = 100f * Mathf.Pow(1f - normalizedTime, 2f);
        int finalScore = Mathf.RoundToInt(score);
        return finalScore;
    }

    public void AddScore(int amount)
    {
        score += amount;
        OnScoreChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetScore()
    {
        return score;
    }

    public int GetLives()
    {
        return lives;
    }

    public float GetTimer()
    {
        return timer;
    }

    public float GetMaxTime()
    {
        return maxTime;
    }

    public int GetHighScore()
    {
        return highScore;
    }

    public bool IsGameStart()
    {
        return CurrentState == GameState.GameStart;
    }

    public bool IsGamePlaying()
    {
        return CurrentState == GameState.GamePlaying;
    }
    
    public bool IsGameOver()
    {
        return CurrentState == GameState.GameOver;
    }
}
