using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame(); // Wait for the end of the frame 
            GeneratePointCloud(); // Call the method to generate the point cloud
        }

        // private void Update()
        // {
        //     //TODO: Implement better
        //     //dont do if exists
        //     if (GeneratedPointCloud != null)
        //         return;
        //     if (Input.GetKeyDown(KeyCode.G)) // Press G to regenerate the point cloud 
        //     {
        //         GeneratePointCloud(); // Call the method to generate the point cloud
        //     }
        // }

        private void GeneratePointCloud()
        {
            // Clear the previous point cloud coordinates
            List<PointCloudPoint> pointCloudPoints = new(); // Initialize the list to store point cloud points
            GeneratedPointCloud = new PointCloud(new int[GRIDSIZEX, GRIDSIZEY], pointCloudPoints); // Initialize the point cloud with an empty grid and points

            // Generate the point cloud coordinates
            for (int x = 0; x < GRIDSIZEX; x++)
            {
                for (int y = 0; y < GRIDSIZEY; y++)
                {
                    Vector3 position = new(x * _gridSpacingMeters, _heightToCastFromMeters, y * _gridSpacingMeters);

                    // RaycastHit hit = Physics.Raycast(position, Vector3.down, _heightToCastFromMeters);
                    if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, _heightToCastFromMeters))
                    {
                        switch (hit.collider.gameObject.layer) // Check the layer of the hit object
                        {
                            case 6: // Layer 3 (NormalTerrain layer)
                                UnityEngine.Debug.Log("Hit default object: " + hit.collider.gameObject.name); // Log the name of the default object hit
                                pointCloudPoints.Add(new PointCloudPoint(hit.point, x, y)); // Add the hit point to the list
                                GeneratedPointCloud.Grid[x, y] = 1; // Mark the grid cell as walkable
                                break;
                            case 7: // Layer 6 (rough terrain layer)
                                pointCloudPoints.Add(new PointCloudPoint(hit.point, x, y)); // Add the hit point to the list¨
                                GeneratedPointCloud.Grid[x, y] = 2; // Mark the grid cell as rough terrain
                                UnityEngine.Debug.Log("Hit rough terrain object: " + hit.collider.gameObject.name); // Log the name of the rough terrain object hit
                                break;
                                // {

                                //     default:
                                // }
                        }
                        // else
                        // {
                        //     GeneratedPointCloud.Grid[x, y] = 0; // Mark the grid cell as not walkable
                        // }
                        // switch ()
                        // {

                        //     default:
                        // }

                        // foreach (RaycastHit hit1 in hits)
                        // {
                        //     switch (hit1.collider.gameObject.layer) // Check the layer of the hit object
                        //     {
                        //         case 6: // Layer 3 (NormalTerrain layer)
                        //             Debug.Log("Hit default object: " + hit1.collider.gameObject.name); // Log the name of the default object hit
                        //             pointCloudPoints.Add(new PointCloudPoint(hit1.point, x, y)); // Add the hit point to the list
                        //             GeneratedPointCloud.Grid[x, y] = 1; // Mark the grid cell as walkable
                        //             break;
                        //         case 7: // Layer 6 (rough terrain layer)
                        //         pointCloudPoints.Add(new PointCloudPoint(hit1.point, x, y)); // Add the hit point to the list¨
                        //             GeneratedPointCloud.Grid[x, y] = 2; // Mark the grid cell as rough terrain
                        //             Debug.Log("Hit rough terrain object: " + hit1.collider.gameObject.name); // Log the name of the rough terrain object hit
                        //             break;
                        //     }
                        //     // if (hit1.collider.gameObject.tag == "PointCloudable")
                        //     // {
                        //     //     Debug.Log("Hit static object: " + hit1.collider.gameObject.name); // Log the name of the static object hit
                        //     //     pointCloudPoints.Add(new PointCloudPoint(hit1.point, x, y)); // Add the hit point to the list
                        //     // }
                        //     // break; // Exit the loop after the first hit
                        // }
                    }
                }
                // GeneratedPointCloud = new PointCloud(new int[GRIDSIZEX, GRIDSIZEY], pointCloudPoints); // Initialize the point cloud with the generated points

                // for (int x = 0; x < GRIDSIZEX; x++)
                // {
                //     for (int y = 0; y < GRIDSIZEY; y++)
                //     {
                //         GeneratedPointCloud.Grid[x, y] = 1; // mark all grid cells as walkable
                //     }
                // }
                // Notify subscribers that the point cloud has been generated
            }
            string debug = "";
            // print all the grid values
            for (int i = 0; i < GRIDSIZEX; i++)
            {
                for (int j = 0; j < GRIDSIZEY; j++)
                {
                    debug += GeneratedPointCloud.Grid[i, j] + " ";
                }
                debug += "\n";
            }
            UnityEngine.Debug.Log(debug); // Log the grid values for debugging
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
            // points where grid value is 1
            var points = GeneratedPointCloud.Points.Where(p => GeneratedPointCloud.Grid[p.GridX, p.GridY] == 1).ToList();
            foreach (PointCloudPoint point in points)
            {
                Gizmos.DrawSphere(point.Position, 0.1f); // Draw a sphere at each point in the point cloud
            }
            // points where grid value is 2
            var points2 = GeneratedPointCloud.Points.Where(p => GeneratedPointCloud.Grid[p.GridX, p.GridY] == 2).ToList();
            Gizmos.color = Color.yellow; // Set the color for the point cloud
            foreach (PointCloudPoint point in points2)
            {
                Gizmos.DrawSphere(point.Position, 0.1f); // Draw a sphere at each point in the point cloud
            }


            // foreach (PointCloudPoint point in GeneratedPointCloud.Points)
            // {
            //     Gizmos.DrawSphere(point.Position, 0.1f); // Draw a sphere at each point in the point cloud
            // }
        }
#endif
    }
}
