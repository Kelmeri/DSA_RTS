using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace RTS.Runtime
{
    public class MenuController : MonoBehaviour
    {

        [SerializeField] private GameObject _menuUI; // Reference to the menu UI GameObject
        [SerializeField] private PlayerControls _playerControls; // Reference to the PlayerControls script
        [SerializeField] private Button _resumeButton; // Reference to the resume button in the menu UI
        [SerializeField] private Button _MainMenuButton; // Reference to the main menu button in the menu UI
        [SerializeField] private int _MainMenuIndex;

        private void Start()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_menuUI, "Menu UI reference is missing.");
            UnityEngine.Assertions.Assert.IsNotNull(_playerControls, "PlayerControls reference is missing in MenuController.");
            UnityEngine.Assertions.Assert.IsNotNull(_resumeButton, "Resume button reference is missing.");
            UnityEngine.Assertions.Assert.IsNotNull(_MainMenuButton, "Main menu button reference is missing.");

            _menuUI.SetActive(false); // Hide the menu UI at the start
            _playerControls.OnEscapeActionEvent += ToggleMenu; // Subscribe to the escape action event
            _resumeButton.onClick.AddListener(() => _menuUI.SetActive(false)); // Add listener to resume button to hide the menu UI
            _MainMenuButton.onClick.AddListener(() => OnMainMenuButtonClicked()); // Add listener to main menu button 
        }

        private void OnDestroy()
        {
            // Unsubscribe from the escape action event when the object is destroyed
            _playerControls.OnEscapeActionEvent -= ToggleMenu;
        }

        private void ToggleMenu()
        {
            // Toggle the menu UI visibility when the escape action is performed
            _menuUI.SetActive(!_menuUI.activeSelf);
        }

        private void OnMainMenuButtonClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(_MainMenuIndex); // Load the main menu scene
        }

    }
}
