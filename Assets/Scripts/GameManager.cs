using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum MenuState
    {
        None,
        PauseMenu,
        Settings,
        QuitConfirmation,
        ReturnToMenuConfirmation,
        GameOver
    }

    [Header("UI Panels")]
    public GameObject gamePauseMenu;
    public GameObject gameSettingsMenu;
    public GameObject gameOverPanel;
    public GameObject quitGamePanel;
    public GameObject gameReturnToMenuPanel;
    
    private MenuState currentMenuState = MenuState.None;
    private MenuState previousMenuState = MenuState.None;

    //The Tetrominos are those colourful blocks that come in different shapes/arrangements.
    //Tetrominos and Grid code.
    public GameObject[] Tetrominoes;
    public GridScript fromGridScript;
    public float movementFrequency = 0.8f;
    private float passedTime = 0;
    private GameObject currentTetromino;

    //Touchscreen controls code.
    private Vector2 startPos;
    public float swipeLimit = 75f;
    private bool isSwiping = false;

    void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnTetromino();
    }

    // Update is called once per frame
    void Update()
    {
        passedTime += Time.deltaTime;
        if (passedTime >= movementFrequency)
        {
            passedTime -= movementFrequency;
            MoveTetromino(Vector3.down);
        }

        UserInput();
    }

    void UserInput()
    {
        if (currentMenuState != MenuState.None) return;

        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                isSwiping = false;
                return;
            }

            isSwiping = true;
            startPos = touch.position;
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            if (!isSwiping) return;
            Vector2 swipe = touch.position - startPos;

            //Tap = rotate Tetrominos by 90 degrees.
            if (swipe.magnitude < swipeLimit)
            {
                RotateTetromino();
            }
            else if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
            {
                if (swipe.x > 0)
                {
                    //Swipe right = move Tetrominos right by how much swiped.
                    MoveTetromino(Vector3.right);
                }
                else
                {
                    //Swipe left = move Tetrominos left by how much swiped.
                    MoveTetromino(Vector3.left);
                }
            }
            else if (swipe.y < 0)
            {
                //Swipe down = move Tetrominos down by how much swiped.
                MoveTetromino(Vector3.down);
            }

            isSwiping = false;
        }
    }

    public void OnPauseButtonPressed()
    {
        if (currentMenuState == MenuState.GameOver) return;

        if (currentMenuState == MenuState.None)
        {
            Pause();
        }
        else if (currentMenuState == MenuState.PauseMenu)
        {
            Resume();
        }
        else
        {
            CloseSubMenu();
        }
    }

    public void SwitchMenuState(MenuState newState)
    {
        gamePauseMenu.SetActive(false);
        gameSettingsMenu.SetActive(false);
        gameOverPanel.SetActive(false);
        quitGamePanel.SetActive(false);
        gameReturnToMenuPanel.SetActive(false);

        if (currentMenuState == MenuState.PauseMenu || currentMenuState == MenuState.GameOver)
        {
            previousMenuState = currentMenuState;
        }

        currentMenuState = newState;

        switch (currentMenuState)
        {
            case MenuState.None:
                Time.timeScale = 1f;
                previousMenuState = MenuState.None;
                break;

            case MenuState.PauseMenu:
                gamePauseMenu.SetActive(true);
                Time.timeScale = 0f;
                break;

            case MenuState.Settings:
                gameSettingsMenu.SetActive(true);
                break;

            case MenuState.GameOver:
                gameOverPanel.SetActive(true);
                Time.timeScale = 0f;
                break;

            case MenuState.QuitConfirmation:
                quitGamePanel.SetActive(true);
                break;

            case MenuState.ReturnToMenuConfirmation:
                gameReturnToMenuPanel.SetActive(true);
                break;
        }
    }

    void RotateTetromino()
    {
        currentTetromino.transform.Rotate(0, 0, -90);
        if(!IsValidPosition())
        {
            currentTetromino.transform.Rotate(0, 0, 90);
        }
    }

    void SpawnTetromino()
    {
        int index = Random.Range(0, Tetrominoes.Length);
        GameObject prefab = Tetrominoes[index];

        // My grid cells (4, 18) converted to my 0.34 world coordinates
        float spawnX = fromGridScript.gridOrigin.x + (4 * fromGridScript.cellSize);
        float spawnY = fromGridScript.gridOrigin.y + (18 * fromGridScript.cellSize);

        // The Square Tetromin piece uses 0.17 half-cell offsets so shift it by 0.17
        if (prefab.transform.childCount > 0 && Mathf.Approximately(Mathf.Abs(prefab.transform.GetChild(0).localPosition.x), 0.17f))
        {
            spawnX += fromGridScript.cellSize * 0.5f;
            spawnY += fromGridScript.cellSize * 0.5f;
        }

        currentTetromino = Instantiate(Tetrominoes[index], new Vector3(6f, 18f, 0), Quaternion.identity);

        if (!IsValidPosition())
        {
            ShowGameOver();
        }
    }

    void MoveTetromino(Vector3 direction)
    {
        Vector3 step = direction * fromGridScript.cellSize;
        currentTetromino.transform.position += step;

        if(!IsValidPosition())
        {
            currentTetromino.transform.position -= step;

            if(direction == Vector3.down)
            {
                GetComponent<GridScript>().UpdateGrid(currentTetromino.transform);
                CheckForLines();
                SpawnTetromino();
            }
        }
    }

    bool IsValidPosition()
    {
        GetComponent<GridScript>().IsValidPosition(currentTetromino.transform);

        foreach (Transform block in currentTetromino.transform)
        {
            Vector2Int pos = fromGridScript.Round(block.position);

            if (!fromGridScript.IsInsideBorder(pos))
            {
                return false;
            }

            if (pos.y < 20 && fromGridScript.grid[pos.x, pos.y] != null && fromGridScript.grid[pos.x, pos.y].parent != currentTetromino.transform)
            {
                return false;
            }
        }
        return true;
    }

    void CheckForLines()
    {
        
    }

    public void Resume()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.buttonPress != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonPress);

        SwitchMenuState(MenuState.None);
    }

    void Pause() => SwitchMenuState(MenuState.PauseMenu);

    public void ShowGameOver()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.gameOverSound != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.gameOverSound);

        SwitchMenuState(MenuState.GameOver);
    }

    public void CloseSubMenu()
    {
        if (previousMenuState == MenuState.None) SwitchMenuState(MenuState.PauseMenu);
        else SwitchMenuState(previousMenuState);
    }

    public void OpenSettings() => SwitchMenuState(MenuState.Settings);
    public void CloseSettings() => CloseSubMenu();
    public void OpenReturnToMenuPanel() => SwitchMenuState(MenuState.ReturnToMenuConfirmation);
    public void CloseReturnToMenuPanel() => CloseSubMenu();
    public void OpenQuitConfirmation() => SwitchMenuState(MenuState.QuitConfirmation);
    public void CloseQuitConfirmation() => CloseSubMenu();

    public void RestartLevel()
    {
        PlayTransitionAudio();
        Time.timeScale = 1f;
        Instance = null;

        if (ScoreTimeManager.Instance != null) Destroy(ScoreTimeManager.Instance.gameObject);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMainMenu()
    {
        PlayTransitionAudio();
        Time.timeScale = 1f;
        Instance = null;

        if (AudioManager.Instance != null && AudioManager.Instance.mainMenuMusic != null)
        {
            AudioManager.Instance.TransitionToMusic(AudioManager.Instance.mainMenuMusic, 0.75f);
        }

        SceneManager.LoadScene("MainMenu");
    }

    private void PlayTransitionAudio()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.buttonPress != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonPress);
        }
    }

    public void SaveToDisk() => PlayerPrefs.Save();
    public void QuitGame() { PlayTransitionAudio(); Application.Quit(); }
}
