using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridBuilder : MonoBehaviour
{
    [SerializeField] private GameObject mainPrefab;
    [SerializeField] private int gridLength = 10;
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private List<GameObject> createdList;


    [ContextMenu("Build grid")]
    private void BuildGrid()
    {
        ClearGrid();
        createdList = new List<GameObject>();

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
        foreach(GameObject tile in createdList)
        {
            DestroyImmediate(tile);
        }

        createdList.Clear();
    }

    private void CreateTile(float xPosition, float zPosition)
    {
        Vector3 newPosition = new Vector3(xPosition, 0, zPosition);
        GameObject newTile = Instantiate(mainPrefab, newPosition, Quaternion.identity, transform);

        createdList.Add(newTile);
    }
}
