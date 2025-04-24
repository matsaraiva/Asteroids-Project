using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Game UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private LivesCounter livesCounter;
    [SerializeField] private GameObject gameUI;

    [Header("Menu UI")]
    [SerializeField] private GameObject menuUI;
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        startButton.onClick.AddListener(StartGame);
        restartButton.onClick.AddListener(RestartGame);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventType.GAME_INITIALIZED, OnGameInitialized);
        EventManager.Instance.RemoveListener(EventType.GAME_STARTED, OnGameStarted);
        EventManager.Instance.RemoveListener(EventType.GAME_OVER, OnGameOver);
        EventManager.Instance.RemoveListener(EventType.SCORE_UPDATED, OnScoreUpdated);
        EventManager.Instance.RemoveListener(EventType.LIVES_UPDATED, OnLivesUpdated);
    }

    private void Start()
    {
        EventManager.Instance.AddListener(EventType.GAME_INITIALIZED, OnGameInitialized);
        EventManager.Instance.AddListener(EventType.GAME_STARTED, OnGameStarted);
        EventManager.Instance.AddListener(EventType.GAME_OVER, OnGameOver);
        EventManager.Instance.AddListener(EventType.SCORE_UPDATED, OnScoreUpdated);
        EventManager.Instance.AddListener(EventType.LIVES_UPDATED, OnLivesUpdated);

        UpdateHighScoreDisplay();
    }

    private void OnGameInitialized(object data)
    {
        menuUI.SetActive(true);
        gameUI.SetActive(false);
        gameOverUI.SetActive(false);
    }

    private void OnGameStarted(object data)
    {
        menuUI.SetActive(false);
        gameUI.SetActive(true);
        gameOverUI.SetActive(false);
    }

    private void OnGameOver(object data)
    {
        gameUI.SetActive(false);
        gameOverUI.SetActive(true);

        int score = GameManager.Instance.CurrentScore;

        

        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (score > highScore)
        {
            finalScoreText.text = $"NEW RECORD! {score:D6}";
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }
        else
        {
            finalScoreText.text = $"SCORE {score:D6}";
        }

        UpdateHighScoreDisplay();
    }

    private void OnScoreUpdated(object score)
    {
        // formato 000000
        scoreText.text = $"{score:D6}";
    }

    private void OnLivesUpdated(object lives)
    {
        livesText.text = $"LIVES: {lives}";

        livesCounter.SetLives((int)lives);
    }

    private void UpdateHighScoreDisplay()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = $"HIGH SCORE {highScore:D6}";
    }

    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }

    public void RestartGame()
    {
        GameManager.Instance.ReturnToMenu();
    }
}