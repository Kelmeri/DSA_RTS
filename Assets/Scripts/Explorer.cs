using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explorer : MonoBehaviour
{
    public Tile[,] grid;
    
    private Vector2Int gridSize; // Grid size NxN
    
    [SerializeField] private Vector2Int startNodePos; // Starting position [x,z]
    [SerializeField] private Vector2Int goalNodePos; // Goal position [x,z]

    private Tile goalTile; // Goal tile
    private Tile currentTarget; // Current target

    private Pathfinder _pf; // Reference to pathfinder

    private void Start()
    {
        // Initialize the grid
        InitializeGrid();


        _pf = GetComponent<Pathfinder>(); // Get pathfinder reference
        currentTarget = GetNextTarget(); // Initialize first target
        goalTile = grid[goalNodePos.x / 3, goalNodePos.y / 3]; // Initialize the goal tile

        _pf.SetTarget(currentTarget.transform);
    }

    private void Update()
    {
        // If the unit has reached the target, update the target to the next treasure or goal
        if (Vector3.Distance(transform.position, currentTarget.transform.position) < 1f)  // Threshold distance to consider the target "reached"
        {
            if (currentTarget.HasTreasure)
            {
                Debug.Log("Treasure found, moving to the next one.");
                currentTarget.HasTreasure = false;
            }
            else
            {
                Debug.Log("Goal reached.");
            }

            // Update target to the next treasure or goal
            currentTarget = GetNextTarget();

            //// If the current target is a goal, we stop
            //if (currentTarget == goalTile)
            //{
            //    Debug.Log("Final destination reached (Goal).");
            //    return;  // Stop the process
            //}

            // Set the new target for the Pathfinder and regenerate the path
            _pf.SetTarget(currentTarget.transform);
        }
    }

    private void InitializeGrid()
    {
        // Find all tiles
        Tile[] allTiles = FindObjectsOfType<Tile>();

        // Find the bounds of the grid
        int minX = int.MaxValue, maxX = int.MinValue, minY = int.MaxValue, maxY = int.MinValue;

        // Find min/max to determine grid size
        foreach (Tile tile in allTiles)
        {
            Vector2Int position = tile.TilePosition;
            minX = Mathf.Min(minX, position.x);
            maxX = Mathf.Max(maxX, position.x);
            minY = Mathf.Min(minY, position.y);
            maxY = Mathf.Max(maxY, position.y);
        }

        // Set the grid size
        gridSize = new Vector2Int(maxX - minX + 1, maxY - minY + 1);

        // Initialize the grid with the determined size
        grid = new Tile[gridSize.x, gridSize.y];

        // Populate the grid
        foreach (Tile tile in allTiles)
        {
            Vector2Int position = tile.TilePosition;
            int gridX = position.x - minX; // Normalize to start at (0, 0)
            int gridY = position.y - minY;

            if (gridX >= 0 && gridX < gridSize.x && gridY >= 0 && gridY < gridSize.y)
            {
                grid[gridX, gridY] = tile;
            }
            else
            {
                Debug.LogError($"Tile at position {tile.TilePosition} is out of grid bounds. Skipping tile.");
            }

        }

        Debug.Log($"Grid initialized with size {gridSize.x}x{gridSize.y}");
    }

    public Tile GetNextTarget()
    {
        Tile treasureTile = FindNearestTreasure();

        if (treasureTile != null)
        {
            Debug.Log("Found treasure at: " + treasureTile.TilePosition);
            return treasureTile; // Return treasure tile
        }
        else
        {
            Debug.Log("No treasure found, heading to goal.");
            return goalTile; // If no treasure found, return the goal tile instead
        }
    }

    // BFS to find nearest treasure node
    private Tile FindNearestTreasure()
    {
        bool[,] visited = new bool[gridSize.x, gridSize.y];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(startNodePos);
        visited[startNodePos.x, startNodePos.y] = true;

        // Directions
        Vector2Int[] directions = {
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0)
    };

        // Debug: Log the start position
        Debug.Log($"Starting BFS from {startNodePos}");

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            // Ensure we are not accessing null tile
            if (current.x < 0 || current.x >= gridSize.x || current.y < 0 || current.y >= gridSize.y)
            {
                Debug.LogError($"Invalid grid position: {current}");
                continue; // Skip invalid grid positions
            }

            Tile currentTile = grid[current.x, current.y];

            // Ensure the tile is not null
            if (currentTile == null)
            {
                if (currentTile == goalTile)
                {
                    return currentTile;
                }
                else
                {
                    Debug.LogError($"Tile at {current} is null!");
                    continue; // Skip null tiles
                }
            }

            // If the tile is a treasure node, return it
            if (currentTile.HasTreasure)
            {
                Debug.Log("Treasure found at: " + currentTile.TilePosition);
                return currentTile;
            }

            // Check all neighboring tiles
            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighbor = current + direction;

                if (IsValidPosition(neighbor) && !visited[neighbor.x, neighbor.y])
                {
                    queue.Enqueue(neighbor);
                    visited[neighbor.x, neighbor.y] = true;
                }
            }
        }

        // If no treasure is found, return null
        Debug.Log("No treasure found.");
        return null;
    }


    // Check if position is withing bounds
    private bool IsValidPosition(Vector2Int pos) 
    {
        return pos.x >= 0 && pos.x < gridSize.x && pos.y >= 0 && pos.y < gridSize.y;
    }
}
