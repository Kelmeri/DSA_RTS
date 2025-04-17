using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Runtime
{
    public class PointcloudGenerator : MonoBehaviour
    {
        private int _heightToCastFromMeters = 500;
        private const int GRIDSIZEY = 100;
        private const int GRIDSIZEX = 100;
        private float _gridSpacingMeters = 1f;
        public PointCloud GeneratedPointCloud { get; private set; } // List to store point cloud points
        
        public event Action OnPointCloudGenerated; // Event to notify when the point cloud is generated

        private void Start()
        {
            GeneratePointCloud();
        }

        private void GeneratePointCloud()
        {
            // Clear the previous point cloud coordinates
            List<PointCloudPoint> pointCloudPoints = new(); // Initialize the list to store point cloud points

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
                            pointCloudPoints.Add(new PointCloudPoint(hit1.point, x, y)); // Add the hit point to the list
                        }
                        break; // Exit the loop after the first hit
                    }
                }
            }
            GeneratedPointCloud = new PointCloud(new int[GRIDSIZEX, GRIDSIZEY], pointCloudPoints); // Initialize the point cloud with the generated points

            for (int x = 0; x < GRIDSIZEX; x++)
            {
                for (int y = 0; y < GRIDSIZEY; y++)
                {
                    GeneratedPointCloud.Grid[x, y] = 1; // mark all grid cells as walkable
                }
            }
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
            // Visualize the point cloud
            Gizmos.color = Color.red; // Set the color for the point cloud
            foreach (PointCloudPoint point in GeneratedPointCloud.Points)
            {
                Gizmos.DrawSphere(point.Position, 0.1f); // Draw a sphere at each point in the point cloud
            }
        }
#endif
    }
}
