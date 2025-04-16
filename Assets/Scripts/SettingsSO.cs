using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Runtime
{
    [CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObjects/Settings", order = 1)]
    public class SettingsSO : ScriptableObject
    {
        [SerializeField] private int _masterVolume = 100; // Master volume level (0-100)
        public int MasterVolume
        {
            get => _masterVolume; // Getter for master volume
            set => _masterVolume = Mathf.Clamp(value, 0, 100); // Setter for master volume with clamping
        }
        [SerializeField] private int _musicVolume = 100; // Music volume level (0-100)
        public int MusicVolume
        {
            get => _musicVolume; // Getter for music volume
            set => _musicVolume = Mathf.Clamp(value, 0, 100); // Setter for music volume with clamping
        }
        [SerializeField] private FrameCap _frameCap = FrameCap._60FPS; // Frame cap setting
        public FrameCap FrameCapSetting
        {
            get => _frameCap; // Getter for frame cap setting
            set => _frameCap = value; // Setter for frame cap setting
        }


        public enum FrameCap
        {
            _30FPS = 30,
            _60FPS = 60,
            _90FPS = 90,
            _120FPS = 120,
            _144FPS = 144,
            _240FPS = 240,
            Uncapped = 0

        }

    }

}
