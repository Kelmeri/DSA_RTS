using UnityEngine;
using UnityEngine.InputSystem;

namespace RTS.Runtime
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerControls : MonoBehaviour
    {
        private PlayerInput _playerInput;
        [SerializeField] private InputActionReference _escapeAction;
        [SerializeField] private InputActionReference _moveAction;
        public event System.Action OnEscapeActionEvent; // Event to notify when the escape action is performed
        public event System.Action<Vector2> OnMoveActionEvent; // Event to notify when the move action is performed
        private bool _MoveActionPressed;

        private void Awake()
        {
            // Get the PlayerInput component and assert that it is not null
            _playerInput = GetComponent<PlayerInput>();

            UnityEngine.Assertions.Assert.IsNotNull(_playerInput, "PlayerInput component is missing on this GameObject.");
            UnityEngine.Assertions.Assert.IsNotNull(_escapeAction, "Escape action reference is missing.");
            UnityEngine.Assertions.Assert.IsNotNull(_moveAction, "Move action reference is missing.");
        }
        private void Start()
        {
            _escapeAction.action.performed += OnEscapeAction; // Subscribe to the escape action performed event 
            _moveAction.action.performed += (context) => _MoveActionPressed = true; // Subscribe to the move action performed event
        }
        private void OnDestroy()
        {
            // Unsubscribe from the escape action when the object is destroyed
            _escapeAction.action.performed -= OnEscapeAction;
            _moveAction.action.performed -= (context) => _MoveActionPressed = true; // Unsubscribe from the move action when the object is destroyed
        }
        private void FixedUpdate()
        {
            if (!_MoveActionPressed)
            {
                return; // If the move action is not pressed, exit the method
            }
            // Handle the move action here (e.g., move the player character)
            Vector2 moveInput = _moveAction.action.ReadValue<Vector2>();
            //Not optimal idc
            OnMoveActionEvent?.Invoke(moveInput); // Invoke the move action event with the input value
            if (!_moveAction.action.IsPressed())
            {
                _MoveActionPressed = false; // Reset the move action pressed flag when the action is released
            }
        }

        private void OnEscapeAction(InputAction.CallbackContext context)
        {
            // Invoke the escape action event when the action is performed
            OnEscapeActionEvent?.Invoke();
        }
    }
}
