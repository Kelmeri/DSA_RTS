using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Runtime
{
    public class PointcloudGenerator : MonoBehaviour
    {
        private int _heightToCastFromMeters = 500;
        // private int _heightToCastToMeters = 0;
        private const int GRIDSIZEY = 100;
        private const int GRIDSIZEX = 100;
        private float _gridSpacingMeters = 1f;


        // coordinates of the point cloud
        // private List<Vector3> _pointCloudCoordinates = new();
        // //getter setter
        // public List<Vector3> PointCloudCoordinates
        // {
        //     get => _pointCloudCoordinates;
        //     private set => _pointCloudCoordinates = value;
        // }
        public PointCloud GeneratedPointCloud { get; private set; } // List to store point cloud points

        // private int[,] _grid = new int[GRIDSIZEX, GRIDSIZEY]; // 2D array to represent the grid
        // public int[,] Grid
        // {
        //     get => _grid;
        //     private set => _grid = value;
        // }

        
        public event Action OnPointCloudGenerated; // Event to notify when the point cloud is generated

        private void Start()
        {
            GeneratePointCloud();
        }

        private void GeneratePointCloud()
        {
            // Clear the previous point cloud coordinates
            // _pointCloudCoordinates.Clear();
            List<PointCloudPoint> pointCloudPoints = new(); // Initialize the list to store point cloud points
            // List<Vector3> pointCloudCoordinates = new(); // Initialize the list to store point cloud coordinates
            // GeneratedPointCloud = new PointCloud(new int[GRIDSIZEX, GRIDSIZEY], new List<PointCloudPoint>()); // Initialize the point cloud

            // Generate the point cloud coordinates
            for (int x = 0; x < GRIDSIZEX; x++)
            {
                for (int y = 0; y < GRIDSIZEY; y++)
                {
                    Vector3 position = new(x * _gridSpacingMeters, _heightToCastFromMeters, y * _gridSpacingMeters);

                    RaycastHit[] hits = Physics.RaycastAll(position, Vector3.down, _heightToCastFromMeters);

                    foreach (RaycastHit hit1 in hits)
                    {
                        if (hit1.collider.gameObject.isStatic)
                        {
                            // make the grid walkable
                            pointCloudPoints.Add(new PointCloudPoint(hit1.point, x, y)); // Add the hit point to the list

                            // pointCloudCoordinates.Add(hit1.point); // Add the hit point to the list
                        }
                        break; // Exit the loop after the first hit
                    }
                    // if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, _heightToCastFromMeters, _layerMask))
                    // {
                    //     Debug.Log($"Hit point: {hit.point}"); // Log the hit point for debugging
                    //     _pointCloudCoordinates.Add(hit.point);
                    // }
                }
            }
            // _grid = new int[GRIDSIZEX, GRIDSIZEY]; // Initialize the grid array
            // for (int x = 0; x < GRIDSIZEX; x++)
            // {
            //     for (int y = 0; y < GRIDSIZEY; y++)
            //     {
            //         _grid[x, y] = 0; // Initialize the grid with zeros
            //     }
            // }
            // // Populate the grid with point cloud coordinates
            // foreach (Vector3 point in _pointCloudCoordinates)
            // {
            //     int gridX = Mathf.FloorToInt(point.x / _gridSpacingMeters);
            //     int gridY = Mathf.FloorToInt(point.z / _gridSpacingMeters);

            //     if (gridX >= 0 && gridX < GRIDSIZEX && gridY >= 0 && gridY < GRIDSIZEY)
            //     {
            //         _grid[gridX, gridY] = 1; // Mark the grid cell as occupied
            //     }
            // }

            // PointCloud.Clear(); // Clear the previous point cloud points
            // // Populate the point cloud with coordinates and grid indices
            // for (int i = 0; i < _pointCloudCoordinates.Count; i++)
            // {
            //     Vector3 point = _pointCloudCoordinates[i];
            //     int gridX = Mathf.FloorToInt(point.x / _gridSpacingMeters);
            //     int gridY = Mathf.FloorToInt(point.z / _gridSpacingMeters);

            //     PointCloudPoint pointCloudPoint = new(point, gridX, gridY); // Create a new point cloud point
            //     PointCloud.Add(pointCloudPoint); // Add the point to the point cloud
            // }

            // GeneratedPointCloud = new PointCloud(new int[GRIDSIZEX, GRIDSIZEY], new List<PointCloudPoint>()); // Initialize the point cloud
            // // Populate the point cloud with coordinates and grid indices
            // for (int i = 0; i < pointCloudCoordinates.Count; i++)
            // {
            //     Vector3 point = pointCloudCoordinates[i];
            //     int gridX = Mathf.FloorToInt(point.x / _gridSpacingMeters);
            //     int gridY = Mathf.FloorToInt(point.z / _gridSpacingMeters);

            //     PointCloudPoint pointCloudPoint = new(point, gridX, gridY); // Create a new point cloud point
            //     GeneratedPointCloud.Points.Add(pointCloudPoint); // Add the point to the point cloud
            // }
            GeneratedPointCloud = new PointCloud(new int[GRIDSIZEX, GRIDSIZEY], pointCloudPoints); // Initialize the point cloud with the generated points

            for (int x = 0; x < GRIDSIZEX; x++)
            {
                for (int y = 0; y < GRIDSIZEY; y++)
                {
                    GeneratedPointCloud.Grid[x, y] = 1; // mark all grid cells as walkable
                }
            }
            // debug grid
            string debugstr = ""; // Initialize a string to store debug information
            for (int x = 0; x < GRIDSIZEX; x++)
            {
                for (int y = 0; y < GRIDSIZEY; y++)
                {
                    // debugstr += _grid[x, y] + " "; // Append the grid value to the debug string
                    debugstr += GeneratedPointCloud.Grid[x, y] + " "; // Append the grid value to the debug string

                }

            }
            Debug.Log(debugstr); // Log the debug string for the grid
            // Notify subscribers that the point cloud has been generated
            OnPointCloudGenerated?.Invoke();
        }
        public class PointCloud
        {
            public int[,] Grid { get; set; } // 2D array representing the grid
            public List<PointCloudPoint> Points { get; set; } // List of points in the point cloud
            public PointCloud(int[,] grid, List<PointCloudPoint> points)
            {
                Grid = grid;
                Points = points;
            }   
        }
        public class PointCloudPoint
        {
            public Vector3 Position { get; set; } // Position of the point in the point cloud
            public int GridX { get; set; } // X coordinate in the grid
            public int GridY { get; set; } // Y coordinate in the grid

            public PointCloudPoint(Vector3 position, int gridX, int gridY)
            {
                Position = position;
                GridX = gridX;
                GridY = gridY;
            }
        }
#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (GeneratedPointCloud == null || GeneratedPointCloud.Points == null || GeneratedPointCloud.Points.Count == 0)
            {
                return; // Exit if there are no point cloud points to visualize
            }
            // if (_pointCloudCoordinates == null || _pointCloudCoordinates.Count == 0)
            // {
            //     return; // Exit if there are no point cloud coordinates to visualize
            // }
            // Visualize the point cloud
            Gizmos.color = Color.red; // Set the color for the point cloud
            foreach (PointCloudPoint point in GeneratedPointCloud.Points)
            {
                Gizmos.DrawSphere(point.Position, 0.1f); // Draw a sphere at each point in the point cloud
            }
            // {
            //     Gizmos.DrawSphere(point, 0.1f); // Draw a sphere at each point in the point cloud
            // }
        }
#endif
    }
}
