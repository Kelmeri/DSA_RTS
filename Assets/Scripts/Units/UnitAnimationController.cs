using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RTS.Runtime
{
    /// <summary>
    /// This class is responsible for controlling the animation of units.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class UnitAnimationController : MonoBehaviour
    {
        [SerializeField] private UnitMovementController _unitMovementController; // Reference to the UnitMovementController component
        private Animator _animator; // Reference to the Animator component

        void Start()
        {
            _animator = GetComponent<Animator>(); // Get the Animator component

            UnityEngine.Assertions.Assert.IsNotNull(_animator, "Animator reference is missing in UnitAnimationController.");
            UnityEngine.Assertions.Assert.IsNotNull(_unitMovementController, "UnitMovementController reference is missing in UnitAnimationController.");
            _unitMovementController.OnMovingStatusChanged += UpdateAnimationState; // Subscribe to the OnMovingStatusChanged event
        }

        private void UpdateAnimationState(bool isMoving)
        {
            _animator.SetBool("IsMoving", isMoving); // Set the isMoving parameter in the animator based on the moving status
        }
    }
}
