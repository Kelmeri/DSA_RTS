using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

namespace RTS.Runtime
{
    public class UnitMovementController : MonoBehaviour
    {
        [SerializeField] private Pathfinder _pathfinder; // Reference to the Pathfinder component
        [SerializeField] private PointcloudGenerator _pointcloudGenerator; // Reference to the PointcloudGenerator component
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _stopDistance = 0.1f; // Distance to stop from the target
        private readonly float _rotationSpeed = 100f;
        private bool _isMoving = false; // Flag to indicate if the object is moving
        private int _currentPathIndex = 0; // Index of the current path point
        private Vector3 _targetPosition; // Target position to move towards
        private List<AStarSearch.Pair> _path = new(); // List to store the path points
        public bool IsMoving
        {
            get => _isMoving; // Property to get the moving status
            private set
            {
                if (_isMoving != value)
                {
                    _isMoving = value; // Set the moving status
                    OnMovingStatusChanged?.Invoke(_isMoving); // Invoke the event if the status changes
                }
            }
        }
        public event Action<bool> OnMovingStatusChanged; // Event to notify when the moving status changes

        private void Start()
        {
            UnityEngine.Assertions.Assert.IsNotNull(_pathfinder, "Pathfinder reference is missing in UnitMovementController.");
            UnityEngine.Assertions.Assert.IsNotNull(_pointcloudGenerator, "PointcloudGenerator reference is missing in UnitMovementController.");
            _pathfinder.OnPathGenerated += path =>
            {
                _path = path; // Store the generated path
                _targetPosition = GridUtils.GetCoordinatesFromGrid(path[0], _pointcloudGenerator.GeneratedPointCloud); // Get the target position from the path
                IsMoving = true; // Set the moving status to true when the path is generated

            };
        }

        private void FixedUpdate()
        {
            if (!IsMoving) // Check if the object is moving
                return; // Exit if not moving

            MoveTowards(transform, _targetPosition, _speed); // Move towards the target position
            RotateTowards(_targetPosition);

            if (!(Vector3.Distance(transform.position, _targetPosition) <= _stopDistance)) // Check if the object is close enough to stop
            {
                return;
            }
            _currentPathIndex++; // Increment the path index
            if (_currentPathIndex >= _path.Count) // Check if the path index is out of bounds
            {
                IsMoving = false; // Stop moving when the path is completed
                _currentPathIndex = 0; // Reset the path index
                return;
            }
            _targetPosition = GridUtils.GetCoordinatesFromGrid(_path[_currentPathIndex], _pointcloudGenerator.GeneratedPointCloud); // Get the next target position from the path
        }

        private void RotateTowards(Vector3 targetPosition)
        {
            Vector3 lookDirection = (targetPosition - transform.position).normalized; // Calculate the direction to the target
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection); // Create a rotation towards the target
            // lock x and z rotation
            targetRotation.x = 0;
            targetRotation.z = 0;
            // Set the y rotation to the target rotation
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed); // Rotate towards the target position
        }
        private static void MoveTowards(Transform objectToMove, Vector3 positionToMoveTo, float speed = 5f)
        {

            Vector3 direction = (positionToMoveTo - objectToMove.position).normalized; // Calculate the direction to the target
            objectToMove.position += direction * speed * Time.deltaTime; // Move towards the target position
        }
    }
}
