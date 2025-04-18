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
    
    private Rigidbody _rigidbody; // Reference to the Rigidbody component

    private List<AStarSearch.Pair> _path = new(); // List to store the path points
    // private int _currentPathIndex = 0; // Index of the current path point
    
    public event Action<List<AStarSearch.Pair>> OnPathGenerated; // Event to notify when the path is generated

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to the object

        UnityEngine.Assertions.Assert.IsNotNull(_pointcloudGenerator, "PointcloudGenerator reference is missing in Pathfinder.");
        UnityEngine.Assertions.Assert.IsNotNull(_target, "Target reference is missing in Pathfinder.");
        UnityEngine.Assertions.Assert.IsNotNull(_bottomOfUnit, "BottomOfUnit reference is missing in Pathfinder.");
        UnityEngine.Assertions.Assert.IsNotNull(_rigidbody, "Rigidbody reference is missing in Pathfinder.");

        _pointcloudGenerator.OnPointCloudGenerated += GeneratePath; // Subscribe to the event when the point cloud is generated
    }
    private void GeneratePath()
    {
        Debug.Log("Generating path..."); // Log the path generation process
        AStarSearch.Pair start = GetClosestNode(transform.position, _pointcloudGenerator.GeneratedPointCloud); // Start point (bottom of the unit)
        AStarSearch.Pair end = GetClosestNode(_target.position, _pointcloudGenerator.GeneratedPointCloud); // End point (target position)
        Debug.Log(_target.position);
        Debug.Log(transform.position);

        _path = AStarSearch.AStar(_pointcloudGenerator.GeneratedPointCloud.Grid, start, end); // Call the A* search algorithm
        OnPathGenerated?.Invoke(_path); // Invoke the event when the path is generated
        // IsMoving = true; // Set the moving flag to true
        Debug.Log("Path generated with " + _path.Count + " points."); // Log the number of points in the path

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
