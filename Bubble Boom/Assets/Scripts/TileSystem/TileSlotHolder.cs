using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSlotHolder : MonoBehaviour
{
    [Header("Main Tiles")]
    public GameObject tileRoad;
    public GameObject tileField;
    public GameObject tileSideway;

    [Header("Corner Tiles")]
    public GameObject tileInnerCorner;
    public GameObject tileOuterCorner;

    [Header("Hill Tiles")]
    public GameObject tileHill_1;
    public GameObject tileHill_2;
    public GameObject tileHill_3;

    [Header("Bridge Tiles")]
    public GameObject tileBridgeRoad;
    public GameObject tileBridgeField;
    public GameObject tileBridgeSideway;
}
