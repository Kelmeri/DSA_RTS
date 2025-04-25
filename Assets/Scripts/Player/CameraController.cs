using UnityEngine;
using UnityEngine.Assertions;

namespace RTS.Runtime
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private PlayerDataSO _playerData;
        [SerializeField] private Collider _cameraBounds;
        [SerializeField] private PlayerControls _playerControls;

        private void Start()
        {
            Assert.IsNotNull(_cameraBounds, "Camera bounds reference is missing.");
            Assert.IsNotNull(_playerControls, "PlayerControls reference is missing in CameraController.");
            Assert.IsNotNull(_playerData, "PlayerData reference is missing in CameraController.");
            Assert.IsNotNull(_cameraTransform, "Camera transform reference is missing in CameraController.");

            _playerControls.OnMoveActionEvent += MoveAction; // Subscribe to the escape action event
        }
        private void OnDestroy()
        {
            // Unsubscribe from the move action event when the object is destroyed
            _playerControls.OnMoveActionEvent -= MoveAction;
        }
        private void MoveAction(Vector2 moveInput)
        {
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
