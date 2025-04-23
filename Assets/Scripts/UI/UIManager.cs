using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Input Fields (TextMeshPro)")]
    public TMP_InputField widthInput;
    public TMP_InputField heightInput;

    [Header("Buttons")]
    public Button generateButton;
    public Button startPathButton; // Start the game
    public Button resetButton;
    public Button quitButton;
    public Button mainMenuButton; // Reference to the Main Menu button in Pause Menu
    public Button resumeButton;   // Reference to the Start/Resume button in Pause Menu

    [Header("Info Text (TextMeshPro)")]
    public TMP_Text timerText;

    [Header("Script References")]
    public GridGenerator gridGenerator;
    public Pathfinder pathfinder;

    [Header("Pause Menu")]
    public GameObject pauseMenuCanvas; // Reference to the Pause Menu Canvas GameObject
    private bool isPaused = false;

    private void Start()
    {
        // Adding button listeners
        if (generateButton != null)
        {
            generateButton.onClick.AddListener(OnGenerateClicked);
        }

        if (startPathButton != null) // Starts the game
        {
            startPathButton.onClick.AddListener(OnStartGame); // New method to start the game
        }
        else
        {
            Debug.LogError("Start Button is not assigned in UIManager!");
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(OnResetScene);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitGame);
        }

        // Adding listeners for Pause Menu buttons
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnLoadMainMenu); // Call this method
        }
        else
        {
            Debug.LogError("Main Menu Button is not assigned in UIManager!");
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(TogglePauseMenu); // Use TogglePauseMenu for resuming
        }
        else
        {
            Debug.LogError("Start/Resume Button is not assigned in UIManager!");
        }

        // Ensure the pause menu is initially hidden
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);
        }
        else
        {
            Debug.LogError("Pause Menu Canvas is not assigned in UIManager!");
        }
    }

    private void Update()
    {
        // Check for Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    // Method to start the game (load MainRTS scene and start pathfinding)
    public void OnStartGame()
    {
        Time.timeScale = 1f; // Ensure game time is not paused
        SceneManager.LoadScene("MainRTS"); // Load the main game scene

        // Start pathfinding
        if (pathfinder != null && gridGenerator != null && gridGenerator.gridSize > 0)
        {
            
            pathfinder.GeneratePath();
        }
        else
        {
            Debug.LogWarning("Pathfinder ei ole asetettu tai ruudukkoa ei ole luotu, polunhakua ei aloiteta.");
        }
    }

    // Method to show and hide the Pause Menu
    public void TogglePauseMenu()
    {
        if (pauseMenuCanvas != null)
        {
            isPaused = !isPaused;
            pauseMenuCanvas.SetActive(isPaused);
            Time.timeScale = isPaused ? 0f : 1f; // Pause/resume game time
        }
    }

    // Method to load the Main Menu scene (for the Pause Menu)
    public void OnLoadMainMenu()
    {
        Time.timeScale = 1f; // Ensure game time is not paused
        SceneManager.LoadScene("MainMenuSceneName"); // Replace with your Main Menu scene name
    }

    // Grid Generation (keeping existing functionality)
    void OnGenerateClicked()
    {
        if (gridGenerator == null)
        {
            Debug.LogError("GridGenerator is not assigned in UIManager!");
            return;
        }

        if (widthInput != null && heightInput != null)
        {
            if (int.TryParse(widthInput.text, out int width) && float.TryParse(heightInput.text, out float spacingValue))
            {
                gridGenerator.gridSize = Mathf.Max(1, width);
                gridGenerator.spacing = Mathf.Max(0.1f, spacingValue);
                gridGenerator.GenerateGrid();
            }
            else
            {
                Debug.LogWarning("Width or spacing value is not a valid number.");
            }
        }
        else
        {
            Debug.LogError("Width or height input field is not assigned in UIManager!");
        }
    }

    // Reseting Scene (keeping existing functionality)
    void OnResetScene()
    {
        Time.timeScale = 1f; // Ensure game time is not paused
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Quitting the game (keeping existing functionality)
    void OnQuitGame()
    {
        Application.Quit();
    }
}