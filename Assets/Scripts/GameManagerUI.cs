using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject gameStartPanel;

    [SerializeField] private GameObject gamePlayingPanel;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private Image timerImage;
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI newHighScoreText;

    private void Start()
    {
        GameManager.Instance.OnGameStart += HandleGameStart;
        GameManager.Instance.OnGamePlaying += HandleGamePlaying;
        GameManager.Instance.OnGameOver += HandleGameOver;
        GameManager.Instance.OnLivesChanged += HandleLivesChanged;
        GameManager.Instance.OnScoreChanged += HandleScoreChanged;
        GameManager.Instance.OnHighScoreChanged += HandleHighScoreChanged;

        livesText.text = new string('.', GameManager.Instance.GetLives());
        timerImage.fillAmount = 1f;
        scoreText.text = "0";
        finalScoreText.text = "0";
        newHighScoreText.gameObject.SetActive(false);

        gameStartPanel.SetActive(true);
        gamePlayingPanel.SetActive(false);
        gameOverPanel.SetActive(false);

    }

    private void Update()
    {
        if (GameManager.Instance.IsGamePlaying())
        {
            float fillAmount = 1 - (GameManager.Instance.GetTimer() / GameManager.Instance.GetMaxTime());
            timerImage.fillAmount = fillAmount;
        }
    }

    private void HandleGameStart(object sender, EventArgs e)
    {
        gameStartPanel.SetActive(true);
        gamePlayingPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    private void HandleGamePlaying(object sender, EventArgs e)
    {
        gameStartPanel.SetActive(false);
        gamePlayingPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    private void HandleGameOver(object sender, EventArgs e)
    {
        gameStartPanel.SetActive(false);
        gamePlayingPanel.SetActive(false);

        finalScoreText.text = "Score:" + GameManager.Instance.GetScore();
        highScoreText.text = "High Score:" + GameManager.Instance.GetHighScore();
        gameOverPanel.SetActive(true);
    }

    private void HandleLivesChanged(object sender, EventArgs e)
    {
        int lives = Mathf.Max(0, GameManager.Instance.GetLives());
        livesText.text = new string('.', lives);
    }

    private void HandleScoreChanged(object sender, EventArgs e)
    {
        scoreText.text = GameManager.Instance.GetScore().ToString();
    }

    private void HandleHighScoreChanged(object sender, EventArgs e)
    {
        newHighScoreText.gameObject.SetActive(true);
    }
}
