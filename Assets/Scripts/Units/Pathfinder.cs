using System;
using System.Collections;
using System.Collections.Generic;
using RTS.Runtime;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] private Transform _bottomOfUnit;
    [SerializeField] private PointcloudGenerator _pointcloudGenerator;
    [SerializeField] private Transform _target;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _stopDistance = 0.1f; // Distance to stop from the target
    [SerializeField] private float _rotationSpeed = 5f;

    private List<AStarSearch.Pair> _path = new(); // List to store the path points
    private int _currentPathIndex = 0; // Index of the current path point
    private bool _isMoving = false; // Flag to indicate if the object is moving
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
        UnityEngine.Assertions.Assert.IsNotNull(_pointcloudGenerator, "PointcloudGenerator reference is missing in Pathfinder.");
        UnityEngine.Assertions.Assert.IsNotNull(_target, "Target reference is missing in Pathfinder.");

        _pointcloudGenerator.OnPointCloudGenerated += GeneratePath2; // Subscribe to the event when the point cloud is generated
    }

    private void Update()
    {
        if (IsMoving)
        {
            MoveToNextPoint();
        }
    }
    private void GeneratePath2()
    {
        AStarSearch.Pair start = GetClosestNode(transform.position, _pointcloudGenerator.GeneratedPointCloud); // Start point (bottom of the unit)
        AStarSearch.Pair end = GetClosestNode(_target.position, _pointcloudGenerator.GeneratedPointCloud); // End point (target position)
        Debug.Log(_target.position);
        Debug.Log(transform.position);

        _path = AStarSearch.AStar(_pointcloudGenerator.GeneratedPointCloud.Grid, start, end); // Call the A* search algorithm
        IsMoving = true; // Set the moving flag to true

    }
    private static AStarSearch.Pair GetClosestNode(Vector3 position, PointcloudGenerator.PointCloud pointCloud)
    {
        // Find the closest node to the given position
        float closestDistance = Mathf.Infinity;
        AStarSearch.Pair closestNode = new AStarSearch.Pair(0, 0); // Initialize closest node

        foreach (PointcloudGenerator.PointCloudPoint point in pointCloud.Points)
        {
            float distance = Vector3.Distance(position, point.Position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = new AStarSearch.Pair(point.GridX, point.GridY); // Create a new Pair object for the closest node
            }
        }
        return closestNode; // Return the closest node
    }
    private void MoveToNextPoint()
    {
        if (_currentPathIndex >= _path.Count)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero; // Stop the rigidbody movement
                rb.angularVelocity = Vector3.zero; // Stop the rigidbody rotation
            }
            _currentPathIndex = 0; // Reset the path index to the start
            IsMoving = false; // Stop moving if the end of the path is reached
            return;
        }

        AStarSearch.Pair targetPair = _path[_currentPathIndex];
        Vector3 targetPosition = GetCoordinatesFromGrid(targetPair, _pointcloudGenerator.GeneratedPointCloud); // Get the target position from the point cloud

        Vector3 direction = (targetPosition - transform.position).normalized;
        float distance = Vector3.Distance(_bottomOfUnit.transform.position, targetPosition);

        Vector3 lookDirection = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        // lock x and z rotation
        targetRotation.x = 0;
        targetRotation.z = 0;
        // Set the y rotation to the target rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);

        // Move towards the target point
        transform.position += direction * _speed * Time.deltaTime;

        // Check if the object is close enough to the target point
        if (distance < _stopDistance)
        {
            _currentPathIndex++; // Move to the next point in the path
        }
    }
    private static Vector3 GetCoordinatesFromGrid(AStarSearch.Pair pair, PointcloudGenerator.PointCloud pointCloud)
    {
        // Get the coordinates from the grid based on the pair
        foreach (PointcloudGenerator.PointCloudPoint point in pointCloud.Points)
        {
            if (point.GridX == pair.first && point.GridY == pair.second)
            {
                return point.Position; // Return the position of the point
            }
        }
        return Vector3.zero; // Return zero if no matching point is found
    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (_pointcloudGenerator == null || _pointcloudGenerator.GeneratedPointCloud == null || _path == null || _path.Count == 0)
        {
            return; // Exit if there are no point cloud points to visualize
        }
        // Draw the path points in the scene view for debugging
        Gizmos.color = Color.green;
        // Draw a line between the path points
        for (int i = 0; i < _path.Count - 1; i++)
        {
            Vector3 start = GetCoordinatesFromGrid(_path[i], _pointcloudGenerator.GeneratedPointCloud);
            Vector3 end = GetCoordinatesFromGrid(_path[i + 1], _pointcloudGenerator.GeneratedPointCloud);
            Gizmos.DrawLine(start, end); // Draw a line between the points
        }
        //Draw bottom of capsule collider
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_bottomOfUnit.transform.position, 0.1f); // Draw a small sphere at the bottom of the capsule collider


    }
#endif
}
