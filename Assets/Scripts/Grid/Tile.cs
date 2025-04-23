using Codice.Client.BaseCommands;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour
{
    private Vector2Int tilePosition;

    private void Awake()
    {
        tilePosition = GetTilePosition(transform.position);
        RenameTile(tilePosition);
        Hide(); // Hide tiles in the beginning
    }

    private Vector2Int GetTilePosition(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x);
        int y = Mathf.FloorToInt(worldPos.z);
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
        }
    }

    public void Hide()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}