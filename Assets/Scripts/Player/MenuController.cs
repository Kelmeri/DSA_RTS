using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;


namespace RTS.Runtime
{
    public class MenuController : MonoBehaviour
    {

        [SerializeField] private GameObject _menuUI;
        [SerializeField] private PlayerControls _playerControls;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _MainMenuButton;
        [SerializeField] private int _MainMenuIndex;

        private void Start()
        {
            Assert.IsNotNull(_menuUI, "Menu UI reference is missing.");
            Assert.IsNotNull(_playerControls, "PlayerControls reference is missing in MenuController.");
            Assert.IsNotNull(_resumeButton, "Resume button reference is missing.");
            Assert.IsNotNull(_MainMenuButton, "Main menu button reference is missing.");

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
