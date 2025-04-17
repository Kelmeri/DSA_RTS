using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabs = new List<GameObject>(); // List of objects to represent nodes
    [SerializeField] private int gridSize = 5; // Size of the grid (N x N)
    [SerializeField] float spacing = 1f; // Spacing of each node

    private List<GameObject> currentNodes = new List<GameObject>(); // List for currently instantiated nodes

    private void RandomPrefab()
    {
        // Rando stuff here
    }

    [ContextMenu("Generate Grid")]
    public void GenerateGrid()
    {
        DeleteGrid(); // Clear any existing grid

        if (prefabs == null) Debug.LogError("Prefab is not assigned");

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3 position = new Vector3(x * spacing, 0, y * spacing);
                GameObject node = Instantiate(prefab, position, Quaternion.identity, this.transform);
                currentNodes.Add(node);
            }
        }
    }

    [ContextMenu("Delete Grid")]
    public void DeleteGrid()
    {
        foreach (GameObject node in currentNodes)
        {
            if (node != null)
            {
                DestroyImmediate(node);
            }
        }
        currentNodes.Clear();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GridGenerator))]
public class GridGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridGenerator gridGenerator = (GridGenerator)target;

        if (GUILayout.Button("Generate Grid"))
        {
            gridGenerator.GenerateGrid();
        }

        if (GUILayout.Button("Delete Grid"))
        {
            gridGenerator.DeleteGrid();
        }
    }
}
#endif