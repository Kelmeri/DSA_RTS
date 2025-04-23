using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    [SerializeField] private int gridSize; // Size of the grid
    [SerializeField] private Transform tilesParent; // Parent object of all the tiles in the grid
    [SerializeField] private Transform unit; // Character unit
    [SerializeField] private int revealRadius = 3; // Radius in which tiles are revealed

    private bool[,] revealed;

    public event System.Action OnTilesRevealed; // Event to notify when tiles are revealed

    private void Start()
    {
        revealed = new bool[gridSize, gridSize];
    }

    private void Update()
    {
        if (unit != null)
        {
            Vector2Int unitPos = GetTilePosition(unit.position);
            if (RevealTilesAroundUnit(unitPos))
            {
                OnTilesRevealed?.Invoke(); // Trigger tiles revealed event
            }
        }
    }

    private Vector2Int GetTilePosition(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x);
        int y = Mathf.FloorToInt(worldPos.z);
        return new Vector2Int(x, y);
    }

    private bool RevealTilesAroundUnit(Vector2Int unitPos)
    {
        bool newTilesRevealed = false;
        for (int x = -revealRadius;  x <= revealRadius; x++)
        {
            for (int y = -revealRadius; y <= revealRadius; y++)
            {
                Vector2Int tilePosition = unitPos + new Vector2Int(x, y);
                if (IsWithinGrid(tilePosition))
                {
                    if (RevealTile(tilePosition))
                    {
                        newTilesRevealed = true;
                    }
                }
            }
        }
        return newTilesRevealed;
    }

    private bool IsWithinGrid(Vector2Int tilePosition)
    {
        return tilePosition.x >= 0 && tilePosition.x < gridSize && tilePosition.y >= 0 && tilePosition.y < gridSize;
    }

    private bool RevealTile(Vector2Int tile)
    {
        if (revealed[tile.x, tile.y]) return false;

        revealed[tile.x, tile.y] = true;
        Transform tileTransform = tilesParent.Find($"Tile_{tile.x}_{tile.y}");
        if (tileTransform != null)
        {
            tileTransform.GetComponent<Tile>().Reveal();
            return true;
        }
        return false;
    }
}
