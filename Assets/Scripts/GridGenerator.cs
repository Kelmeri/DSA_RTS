using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabs = new List<GameObject>(); // List of objects to represent nodes
    [SerializeField] public int gridSize = 5; // Size of the grid (N x N)
    [SerializeField] float spacing = 1f; // Spacing of each node
    [SerializeField] private LayerMask groundLayer; // Layer mask for ground
    
    private List<GameObject> currentNodes = new List<GameObject>(); // List for currently instantiated nodes

    private GameObject RandomPrefab()
    {
        int length = prefabs.Count; //The amount of different prefabs/nodes
        int index = Random.Range(0, length); //Randomly chooses the node to be used

        return prefabs[index]; // Returns a random node from the list
    }

    [ContextMenu("Generate Grid")]
    public void GenerateGrid()
    {
        DeleteGrid(); // Clear any existing grid

        if (prefabs == null) Debug.LogError("Prefab is not assigned");

        // Generate grid
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3 position = new Vector3(x * spacing, 0, y * spacing);

                //GameObject node = Instantiate(RandomPrefab(), position, Quaternion.identity, this.transform);
                //currentNodes.Add(node);
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

    private void AdjustToGround(GameObject obj) 
    {
        Ray rayDown = new Ray(obj.transform.position, Vector3.down);
        Ray rayUp = new Ray(obj.transform.position, Vector3.up);
        RaycastHit hit;

        // Make sure the instantiated object is grounded
        if (Physics.Raycast(rayDown, out hit, 100f, groundLayer))
        {
            Vector3 newPos = obj.transform.position;
            newPos.y = hit.point.y;
            obj.transform.position = newPos;
        }
        else if (Physics.Raycast(rayUp, out hit, 100f, groundLayer))
        {
            Vector3 newPos = obj.transform.position;
            newPos.y = hit.point.y;
            obj.transform.position = newPos;
        }
        else Debug.LogWarning("No ground detected below or above object at position: " + obj.transform.position);
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