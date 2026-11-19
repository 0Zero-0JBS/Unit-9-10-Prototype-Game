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
    public GameObject returnToMenuPanel;

    [Header("Controller Focus Buttons")]
    public GameObject menuButtonSelected;
    public GameObject settingsButtonSelected;
    public GameObject quitButtonSelected;
    public GameObject returnButtonSelected;

    private MenuState currentMenuState = MenuState.MainMenu;

    [Header("Records Display (In Main Menu)")]
    public TextMeshProUGUI mainMenuHighScoreText;
    public TextMeshProUGUI mainMenuBestTimeText;

    private Coroutine selectionCoroutine;

    public void SwitchMenuState(MenuState newState)
    {
        if (mainMenu != null) mainMenu.SetActive(false);
        if (settingsMenu != null) settingsMenu.SetActive(false);
        if (quitMenu != null) quitMenu.SetActive(false);
        if (returnToMenuPanel != null) returnToMenuPanel.SetActive(false);

        currentMenuState = newState;

        if (selectionCoroutine != null)
        {
            StopCoroutine(selectionCoroutine);
        }

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        switch (currentMenuState)
        {
            case MenuState.MainMenu:
                if (mainMenu != null) mainMenu.SetActive(true);
                if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(menuButtonSelected);
                break;

            case MenuState.Settings:
                if (settingsMenu != null) settingsMenu.SetActive(true);
                if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(settingsButtonSelected);
                break;

            case MenuState.QuitConfirmation:
                if (quitMenu != null) quitMenu.SetActive(true);
                if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(quitButtonSelected);
                break;

            case MenuState.ReturnToMenuConfirmation:
                if (returnToMenuPanel != null) returnToMenuPanel.SetActive(true);
                if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(returnButtonSelected);
                break;
        }
    }

    private IEnumerator SetFocusNextFrame(GameObject targetButton)
    {
        if (EventSystem.current == null) yield break;

        // Clear focus first to completely reset the input module state
        EventSystem.current.SetSelectedGameObject(null);

        yield return null;

        // Apply new focus once the new UI elements are fully active
        if (targetButton != null)
        {
            EventSystem.current.SetSelectedGameObject(targetButton);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1f;

        int savedHigh = PlayerPrefs.GetInt("HighScore", 0);
        if (mainMenuHighScoreText != null) mainMenuHighScoreText.text = "HIGH SCORE: " + savedHigh.ToString("0000");

        float savedBestTime = PlayerPrefs.GetFloat("BestTime", 0f);
        if (mainMenuBestTimeText != null)
        {
            int minutes = Mathf.FloorToInt(savedBestTime / 60);
            int seconds = Mathf.FloorToInt(savedBestTime % 60);
            mainMenuBestTimeText.text = $"BEST TIME: {minutes:00}:{seconds:00}";
        }

        SwitchMenuState(MenuState.MainMenu);
    }

    public void OpenSettingsMenu() => SwitchMenuState(MenuState.Settings);
    public void CloseSettingsMenu() => SwitchMenuState(MenuState.MainMenu);

    public void OpenQuitMenu() => SwitchMenuState(MenuState.QuitConfirmation);
    public void CloseQuitMenu() => SwitchMenuState(MenuState.MainMenu);

    public void OpenReturnToMenuPanel() => SwitchMenuState(MenuState.ReturnToMenuConfirmation);
    public void CloseReturnToMenuPanel() => SwitchMenuState(MenuState.MainMenu);

    public void PlayGame() => SceneManager.LoadScene("Level");
    public void OpenTutorial() => SceneManager.LoadScene("Tutorial");
    public void ToMainMenu() => SceneManager.LoadScene("Main Menu");
    public void RestartLevel() => SceneManager.LoadScene("Level");
    public void QuitGame() => Application.Quit();

    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey("HighScore");
        PlayerPrefs.DeleteKey("BestTime");

        if (mainMenuHighScoreText != null) mainMenuHighScoreText.text = "HIGH SCORE: 0000";
        if (mainMenuBestTimeText != null) mainMenuBestTimeText.text = "BEST TIME: 00:00";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
