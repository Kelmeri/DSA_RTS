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
    public Button startPathButton;
    public Button resetButton;
    public Button quitButton;

    [Header("Info Text (TextMeshPro)")]
    public TMP_Text timerText;

    [Header("Script References")]
    public GridGenerator gridGenerator;
    public Pathfinder pathfinder;

    private void Start()
    {
        // Adding buttons
        if (generateButton != null)
        {
            generateButton.onClick.AddListener(OnGenerateClicked);
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(OnResetScene);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitGame);
        }
    }

    // Grid Generation
    void OnGenerateClicked()
    {
        if (gridGenerator != null && widthInput != null && int.TryParse(widthInput.text, out int width))
        {
            gridGenerator.gridSize = Mathf.Max(1, width);
            gridGenerator.GenerateGrid();
        }
    }

    // Reseting Scene
    void OnResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Quitting the game
    void OnQuitGame()
    {
        Application.Quit();
    }
}