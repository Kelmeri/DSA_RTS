using Codice.Client.BaseCommands;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour
{
    [SerializeField] private bool hasTreasure = false; // Boolean to assign the tile as a treasure node
    private GameObject treasure; 
    private Vector2Int tilePosition; // Position of the tile

    [SerializeField] private bool isCastle = false;

    // Public getters and setters
    public bool HasTreasure
    {
        get { return hasTreasure; }
        set { hasTreasure = value; }
    }
    public Vector2Int TilePosition => tilePosition;

    private void Awake()
    {
        Transform treasureT = transform.Find("the treasure");
        if (treasureT != null) { treasure = treasureT.gameObject; }

        tilePosition = GetTilePosition(transform.position); // Gets the position of the tile
        
        RenameTile(tilePosition); // Renames the tile for easier readability
        Hide(); // Hide tiles in the beginning
    }

    private Vector2Int GetTilePosition(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / 3); // Divide by 3 to account for spacing
        int y = Mathf.FloorToInt(worldPos.z / 3);
        return new Vector2Int(x, y);
    }

    private void RenameTile(Vector2Int pos)
    {
        gameObject.name = $"Tile_{pos.x}_{pos.y}";
    }

    public void Reveal()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
            treasure.SetActive(hasTreasure);
        }
    }

    public void Hide()
    {
        foreach (Transform child in transform)
        {
            if (isCastle)
            {
                child.gameObject.SetActive(true);
            }
            else { child.gameObject.SetActive(false); }
        }
    }

    private void OnDrawGizmos()
    {
        if (hasTreasure)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(gameObject.transform.position + (Vector3.up * 5), 0.5f);
        }
    }
}