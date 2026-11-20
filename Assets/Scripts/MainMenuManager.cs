using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public enum MenuState
    {
        MainMenu,
        Settings,
        QuitConfirmation,
        ReturnToMenuConfirmation
    }

    [Header("Menu Panels")]
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject quitMenu;
    
    [Header("Continue Button")]
    public GameObject mainContinueButton;

    private MenuState currentMenuState = MenuState.MainMenu;

    [Header("Records Display (In Main Menu)")]
    public TextMeshProUGUI mainMenuHighScoreText;
    public TextMeshProUGUI mainMenuBestTimeText;

    public void SwitchMenuState(MenuState newState)
    {
        if (mainMenu != null) mainMenu.SetActive(false);
        if (settingsMenu != null) settingsMenu.SetActive(false);
        if (quitMenu != null) quitMenu.SetActive(false);

        currentMenuState = newState;

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        switch (currentMenuState)
        {
            case MenuState.MainMenu:
                if (mainMenu != null) mainMenu.SetActive(true);
                break;

            case MenuState.Settings:
                if (settingsMenu != null) settingsMenu.SetActive(true);
                break;

            case MenuState.QuitConfirmation:
                if (quitMenu != null) quitMenu.SetActive(true);
                break;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1f;

        int savedHigh = PlayerPrefs.GetInt("HighScore", 0);
        if (mainMenuHighScoreText != null) mainMenuHighScoreText.text = "HIGH SCORE: " + savedHigh.ToString("000000");

        float savedBestTime = PlayerPrefs.GetFloat("BestTime", 0f);
        if (mainMenuBestTimeText != null)
        {
            int hours = Mathf.FloorToInt(savedBestTime / 3600);
            int minutes = Mathf.FloorToInt(savedBestTime / 60) % 60;
            int seconds = Mathf.FloorToInt(savedBestTime % 60);
            mainMenuBestTimeText.text = $"BEST TIME: {hours:00}:{minutes:00}:{seconds:00}";
        }

        SwitchMenuState(MenuState.MainMenu);
    }

    public void OpenSettingsMenu() => SwitchMenuState(MenuState.Settings);
    public void CloseSettingsMenu() => SwitchMenuState(MenuState.MainMenu);

    public void OpenQuitMenu() => SwitchMenuState(MenuState.QuitConfirmation);
    public void CloseQuitMenu() => SwitchMenuState(MenuState.MainMenu);

    public void PlayGame() => SceneManager.LoadScene("Game");
    public void ToMainMenu() => SceneManager.LoadScene("MainMenu");
    public void RestartLevel() => SceneManager.LoadScene("Game");
    public void QuitGame() => Application.Quit();

    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey("HighScore");
        PlayerPrefs.DeleteKey("BestTime");

        if (mainMenuHighScoreText != null) mainMenuHighScoreText.text = "HIGH SCORE: 000000";
        if (mainMenuBestTimeText != null) mainMenuBestTimeText.text = "BEST TIME: 00:00:00";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
