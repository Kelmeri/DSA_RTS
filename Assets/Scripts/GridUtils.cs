using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Runtime
{
    public static class GridUtils
    {
        public static Vector3 GetCoordinatesFromGrid(AStarSearch.Pair pair, PointcloudGenerator.PointCloud pointCloud)
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
    }
}
