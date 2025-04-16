using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Runtime
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; } // Singleton instance of GameManager
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Destroy duplicate instances of GameManager
                return;
            }
            Instance = this; // Set the singleton instance
            DontDestroyOnLoad(gameObject); // Prevent this GameObject from being destroyed when loading a new scene
        }
        [SerializeField] private SettingsSO _settingsSO; // Reference to the Settings ScriptableObject

        private void Start()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_settingsSO, "SettingsSO reference is missing in GameManager.");
            CapFramerate();
        }

        private void CapFramerate()
        {
            Application.targetFrameRate = (int)_settingsSO.FrameCapSetting; // Set the target frame rate based on the settings
        }

    }

}
