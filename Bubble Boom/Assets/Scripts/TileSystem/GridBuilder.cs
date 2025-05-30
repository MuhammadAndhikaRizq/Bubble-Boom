using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class GridBuilder : MonoBehaviour
{
    private NavMeshSurface myNavmesh => GetComponent<NavMeshSurface>();
    [SerializeField] private GameObject mainPrefab;
    [SerializeField] private int gridLength = 10;
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private List<GameObject> createdTiles;

    public List<GameObject> GetTileSetup() => createdTiles;
    public void UpdateNavMesh() => myNavmesh.BuildNavMesh();

    [ContextMenu("Build grid")]
    private void BuildGrid()
    {
        ClearGrid();
        createdTiles = new List<GameObject>();

        for(int x = 0; x < gridLength; x++)
        {
            for(int z = 0; z < gridWidth; z++)
            {
                CreateTile(x,z);
            }
            
        }
    }

    [ContextMenu("Clear grid")]
    private void ClearGrid()
    {
        foreach(GameObject tile in createdTiles)
        {
            DestroyImmediate(tile);
        }

        createdTiles.Clear();
    }

    private void CreateTile(float xPosition, float zPosition)
    {
        Vector3 newPosition = new Vector3(xPosition, 0, zPosition);
        GameObject newTile = Instantiate(mainPrefab, newPosition, Quaternion.identity, transform);

        createdTiles.Add(newTile);
    }

    public List<BuildSlot> GetAvailableBuildSlots()
    {
        List<BuildSlot> buildSlots = new List<BuildSlot>();

        foreach (GameObject tile in createdTiles)
        {
            BuildSlot slot = tile.GetComponent<BuildSlot>();
            if (slot != null && slot.enabled && slot.gameObject.activeInHierarchy)
            {
                buildSlots.Add(slot);
            }
        }

        return buildSlots;
    }
}
