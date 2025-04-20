using Codice.Client.Common.GameUI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Grid settings")]
    [SerializeField] private List<GameObject> prefabs = new List<GameObject>(); // List of objects to represent nodes
    [SerializeField] public int gridSize = 5; // Size of the grid (N x N)
    [SerializeField] float spacing = 1f; // Spacing of each node
    [SerializeField] private LayerMask groundLayer; // Layer mask for ground
    [Header("Camera settings")]
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private Vector3 cameraOffset;
    
    private List<GameObject> currentNodes = new List<GameObject>(); // List for currently instantiated nodes

    // Dictionary to map layer names to colors
    private Dictionary<string, Color> layerColors = new Dictionary<string, Color>()
    {
        {"NormalTerrain", Color.green},
        {"MediumTerrain", Color.yellow},
        {"RoughTerrain", Color.red},
    };

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

                GameObject node = Instantiate(RandomPrefab(), position, Quaternion.identity, this.transform);
                int randRot = Random.Range(0, 4) * 90;
                node.transform.Rotate(Vector3.up, randRot);
                AdjustToGround(node);
                currentNodes.Add(node);
            }
        }
        AdjustCameraToGrid();
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
    }

    // Visualization for node layers in the editor
    private void OnDrawGizmos()
    {
        if (currentNodes == null) return;

        foreach (GameObject node in currentNodes)
        {
            if (node == null) continue;

            string layerName = LayerMask.LayerToName(node.layer);

            Color gizmoColor = layerColors.ContainsKey(layerName) ? layerColors[layerName] : Color.gray;

            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(node.transform.position + (Vector3.up * 5), 0.5f);
        }
    }

    private void AdjustCameraToGrid()
    {
        if (sceneCamera == null)
        {
            Debug.LogError("Scene camera is not assigned.");
            return;
        }

        // Calculate the center of the grid
        float gridCenterX = (gridSize - 1) * spacing / 2f;
        float gridCenterZ = (gridSize - 1) * spacing / 2f;
        Vector3 gridCenter = new Vector3(gridCenterX, 0, gridCenterZ);

        // Calculate the camera position with offset
        Vector3 cameraPosition = gridCenter + cameraOffset;

        // Move the camera to the calculated position
        sceneCamera.transform.position = cameraPosition;

        // Make the camera look at the center of the grid
        sceneCamera.transform.LookAt(gridCenter);

        // Adjust the camera's orthographic size or field of view to fit the grid
        if (sceneCamera.orthographic)
        {
            // Orthographic camera
            float gridWidth = gridSize * spacing;
            float gridHeight = gridSize * spacing;
            float orthographicSize = Mathf.Max(gridWidth, gridHeight) / 2f;
            sceneCamera.orthographicSize = orthographicSize;
        }
        else
        {
            // Perspective camera
            float distance = Vector3.Distance(sceneCamera.transform.position, gridCenter);
            float fov = 2f * Mathf.Atan((Mathf.Max(gridSize * spacing, gridSize * spacing) / 2f) / distance) * Mathf.Rad2Deg;
            sceneCamera.fieldOfView = fov;
        }
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