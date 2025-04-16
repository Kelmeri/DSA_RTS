using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RTS.Runtime
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
    public class PlayerDataSO : ScriptableObject
    {
        [SerializeField] private int _cameraSpeed = 10; // Speed of the camera movement
        public int CameraSpeed
        {
            get => _cameraSpeed; // Getter for camera speed
            set => _cameraSpeed = value; // Setter for camera speed
        }
    }

}
