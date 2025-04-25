using System;
using System.Collections.Generic;
using RTS.Runtime;
using UnityEngine;
using UnityEngine.Assertions;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] private Transform _bottomOfUnit;
    [SerializeField] private PointcloudGenerator _pointcloudGenerator;
    [SerializeField] private Transform _target; // Goal target
    [SerializeField] private Explorer _explorer; // Reference to explorer script

    private Rigidbody _rigidbody; // Reference to the Rigidbody component
    private List<AStarSearch.Pair> _path = new(); // List to store the path points
    private Vector3 _lastTargetPosition;  // Store the last target position
    private float _pathRecalculationThreshold = 1.0f;  // Minimum distance to recalculate the path
    public event Action<List<AStarSearch.Pair>> OnPathGenerated; // Event to notify when the path is generated

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to the object

        Assert.IsNotNull(_pointcloudGenerator, "PointcloudGenerator reference is missing in Pathfinder.");
        Assert.IsNotNull(_target, "Target reference is missing in Pathfinder.");
        Assert.IsNotNull(_bottomOfUnit, "BottomOfUnit reference is missing in Pathfinder.");
        Assert.IsNotNull(_rigidbody, "Rigidbody reference is missing in Pathfinder.");
        Assert.IsNotNull(_explorer, "Explorer reference is missing in Pathfinder.");

        _pointcloudGenerator.OnPointCloudGenerated += GeneratePath; // Subscribe to the event when the point cloud is generated
    }
    public void GeneratePath()
    {
        Debug.Log("Generating path..."); // Log the path generation process

        Transform newTarget = _explorer.GetNextTarget().transform; // Get target position from explorer

        AStarSearch.Pair start = GetClosestNode(transform.position, _pointcloudGenerator.GeneratedPointCloud); // Start point (bottom of the unit)
        AStarSearch.Pair end = GetClosestNode(newTarget.position, _pointcloudGenerator.GeneratedPointCloud); // End point (target position)
        
        Debug.Log($"Attempting to generate path from {start.first},{start.second} to {end.first},{end.second}");

        _path = AStarSearch.AStar(_pointcloudGenerator.GeneratedPointCloud.Grid, start, end); // Call the A* search algorithm

        if (_path == null || _path.Count == 0)
        {
            Debug.LogWarning("A* did not return a valid path.");
        }
        else
        {
            Debug.Log($"Generated path with {_path.Count} nodes.");
        }

        OnPathGenerated?.Invoke(_path); // Invoke the event when the path is generated
    }
    public AStarSearch.Pair GetClosestNode(Vector3 position, PointcloudGenerator.PointCloud pointCloud)
    {
        // Find the closest node to the given position
        float closestDistance = Mathf.Infinity;
        AStarSearch.Pair closestNode = new(0, 0); // Initialize closest node

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

    // Check if unit has reached current target
    private void Update()
    {
        // Check if the target position has changed significantly enough to recalculate the path
        if (Vector3.Distance(_lastTargetPosition, _target.position) > _pathRecalculationThreshold)
        {
            GeneratePath();
            _lastTargetPosition = _target.position;  // Update the last target position
        }
    }

    public void SetTarget(Transform target)
    {
        _target = target;
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
            Vector3 start = GridUtils.GetCoordinatesFromGrid(_path[i], _pointcloudGenerator.GeneratedPointCloud);
            Vector3 end = GridUtils.GetCoordinatesFromGrid(_path[i + 1], _pointcloudGenerator.GeneratedPointCloud);
            Gizmos.DrawLine(start, end); // Draw a line between the points
        }
        //Draw bottom of capsule collider
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_bottomOfUnit.transform.position, 0.1f); // Draw a small sphere at the bottom of the capsule collider


    }
#endif
}
