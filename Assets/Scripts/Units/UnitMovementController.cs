using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace RTS.Runtime
{
    public class UnitMovementController : MonoBehaviour
    {
        [SerializeField] private Pathfinder _pathfinder; // Reference to the Pathfinder component
        [SerializeField] private PointcloudGenerator _pointcloudGenerator; // Reference to the PointcloudGenerator component
        [SerializeField] private Explorer _explorer; // Reference to Explorer
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
            Assert.IsNotNull(_pathfinder, "Pathfinder reference is missing in UnitMovementController.");
            Assert.IsNotNull(_pointcloudGenerator, "PointcloudGenerator reference is missing in UnitMovementController.");
            Assert.IsNotNull(_explorer, "Explorer reference is missing in UnitMovementController.");

            _pathfinder.OnPathGenerated += path =>
            {
                _path = path; // Store the generated path
                if (_path.Count > 0)
                {
                    _targetPosition = GridUtils.GetCoordinatesFromGrid(path[0], _pointcloudGenerator.GeneratedPointCloud); // Get the target position from the path
                    IsMoving = true; // Set the moving status to true when the path is generated
                    _currentPathIndex = 0;
                }
            };
        }

        private void FixedUpdate()
        {
            if (!IsMoving) // Check if the object is moving
                return; // Exit if not moving

            // Move towards the current target position
            MoveTowards(transform, _targetPosition, _speed);
            RotateTowards(_targetPosition);

            // Check if the unit has reached the current target
            if (Vector3.Distance(transform.position, _targetPosition) <= _stopDistance)
            {
                // Get the grid position corresponding to the target
                Vector2Int targetGridPosition = new Vector2Int(Mathf.FloorToInt(_targetPosition.x), Mathf.FloorToInt(_targetPosition.z));
                Tile currentTile = _explorer.grid[targetGridPosition.x / 3, targetGridPosition.y / 3];

                // If the current target has treasure, mark it as collected
                if (currentTile != null && currentTile.HasTreasure)
                {
                    currentTile.HasTreasure = false;  // Collect the treasure
                    Debug.Log("Treasure collected at: " + _targetPosition);
                }

                // If we reached the current target, move to the next one in the path
                _currentPathIndex++;

                // Check if there are more targets in the path
                if (_currentPathIndex < _path.Count)
                {
                    _targetPosition = GridUtils.GetCoordinatesFromGrid(_path[_currentPathIndex], _pointcloudGenerator.GeneratedPointCloud);
                }
                else
                {
                    // If the path is completed, stop the movement
                    IsMoving = false;
                    _currentPathIndex = 0; // Reset the path index for the next round of pathfinding
                    Debug.Log("Path completed.");

                    Tile nextTargetTile = _explorer.GetNextTarget();
                    // If the target is not null, recalculate the path
                    if (nextTargetTile != null)
                    {
                        _pathfinder.GeneratePath(); // Call pathfinder to generate a new path
                    }
                }
            }
        }

        /// <summary>
        /// Rotate the object towards the target position
        /// </summary>
        /// <param name="targetPosition"></param>
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
        /// <summary>
        /// Move the object towards the target position
        /// </summary>
        /// <param name="objectToMove"></param>
        /// <param name="positionToMoveTo"></param>
        /// <param name="speed"></param>
        private static void MoveTowards(Transform objectToMove, Vector3 positionToMoveTo, float speed = 5f)
        {

            Vector3 direction = (positionToMoveTo - objectToMove.position).normalized; // Calculate the direction to the target
            objectToMove.position += direction * speed * Time.deltaTime; // Move towards the target position
        }
    }
}
