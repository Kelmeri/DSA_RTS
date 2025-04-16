using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Runtime
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform; // Reference to the camera transform
        [SerializeField] private PlayerDataSO _playerData; // Reference to the PlayerDataSO scriptable object
        [SerializeField] private Collider _cameraBounds;
        [SerializeField] private PlayerControls _playerControls; // Reference to the PlayerControls script

        private void Start()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_cameraBounds, "Camera bounds reference is missing.");
            UnityEngine.Assertions.Assert.IsNotNull(_playerControls, "PlayerControls reference is missing in CameraController.");
            UnityEngine.Assertions.Assert.IsNotNull(_playerData, "PlayerData reference is missing in CameraController.");
            UnityEngine.Assertions.Assert.IsNotNull(_cameraTransform, "Camera transform reference is missing in CameraController.");

            _playerControls.OnMoveActionEvent += MoveAction; // Subscribe to the escape action event
        }
        private void OnDestroy()
        {
            // Unsubscribe from the move action event when the object is destroyed
            _playerControls.OnMoveActionEvent -= MoveAction;
        }
        private void MoveAction(Vector2 moveInput)
        {
            Debug.Log($"Move Input: {moveInput}"); // Log the move input for debugging
            // Handle the camera movement here (e.g., move the camera based on the input)
            Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y); // Convert to 3D direction
            _cameraTransform.position += moveDirection * Time.deltaTime * _playerData.CameraSpeed; // Move the camera based on input and time
        }
        private void FixedUpdate()
        {
            if (_cameraBounds == null)
            {
                return;
            }
            // Clamp the camera position within the bounds defined by the collider
            Vector3 clampedPosition = new Vector3(
                Mathf.Clamp(_cameraTransform.position.x, _cameraBounds.bounds.min.x, _cameraBounds.bounds.max.x),
                _cameraTransform.position.y,
                Mathf.Clamp(_cameraTransform.position.z, _cameraBounds.bounds.min.z, _cameraBounds.bounds.max.z)
            );
            _cameraTransform.position = clampedPosition; // Update the camera position
        }

    }

}
