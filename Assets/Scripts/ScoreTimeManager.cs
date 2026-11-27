using UnityEngine;
using TMPro;

public class ScoreTimeManager : MonoBehaviour
{
    public static ScoreTimeManager Instance { get; private set; }

    [Header("HUD Text Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    [Header("Records Display")]
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI bestTimeText;

    [Header("Game Over")]
    public GameObject gameOverPanel;

    private int currentScore = 0;
    private int highScore = 0;
    private float elapsedTime = 0f;
    private float bestTime = 0f;
    private bool timerCanRun = true;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }

        highScore = PlayerPrefs.GetInt("High Score", 0);
        bestTime = PlayerPrefs.GetFloat("Best Time", 0f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1f;
        UpdateScoreDisplay();
        UpdateTimerDisplay();

        if (highScoreText != null) highScoreText.gameObject.SetActive(false);
        if (bestTimeText != null) bestTimeText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (timerCanRun)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    public void StartTimer() => timerCanRun = true;
    public void StopTimer() => timerCanRun = false;

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreDisplay();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayScorePointsSound();
        }

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("High Score", highScore);
            PlayerPrefs.Save();
        }
    }

    public void StopTimerPermanently()
    {
        timerCanRun = false;

        // Check Points Record
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("High Score", highScore);
        }

        // Check Time Record (Best Time)
        if (elapsedTime > bestTime)
        {
            bestTime = elapsedTime;
            PlayerPrefs.SetFloat("Best Time", bestTime);
        }

        if (highScoreText != null)
        {
            highScoreText.gameObject.SetActive(true);
            highScoreText.text = "HIGH SCORE: " + highScore.ToString("000000");
        }

        if (bestTimeText != null)
        {
            bestTimeText.gameObject.SetActive(true);
            UpdateBestTimeDisplay();
        }

        PlayerPrefs.Save();
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + currentScore.ToString("000000");
        }
    }

    void UpdateTimerDisplay()
    {
        if (timerText == null) return;
        int hours = Mathf.FloorToInt(elapsedTime / 3600);
        int minutes = Mathf.FloorToInt(elapsedTime / 60) % 60;
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = $"TIME: {hours:00}:{minutes:00}:{seconds:00}";
    }

    void UpdateBestTimeDisplay()
    {
        if (bestTimeText == null) return;
        int hours = Mathf.FloorToInt(bestTime / 3600);
        int minutes = Mathf.FloorToInt(bestTime / 60) % 60;
        int seconds = Mathf.FloorToInt(bestTime % 60);
        bestTimeText.text = $"BEST TIME: {hours:00}:{minutes:00}:{seconds:00}";
    }
}
