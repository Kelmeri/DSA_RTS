using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Runtime
{
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class LoadSceneButton : MonoBehaviour
    {
        [SerializeField] private int _sceneToLoad; // The index of the scene to load
        private UnityEngine.UI.Button _button;

        private void Awake()
        {
            // Get the button component and add a listener to it
            _button = GetComponent<UnityEngine.UI.Button>();
            UnityEngine.Assertions.Assert.IsNotNull(_button, "Button component is missing on this GameObject.");
            _button.onClick.AddListener(LoadScene);
        }

        public void LoadScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneToLoad);
        }
    }
    
}
