using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
// using UnityEngine.UIElements; // Tidak terpakai di skrip ini

public class GridBuilder : MonoBehaviour
{
    // Properti untuk NavMeshSurface dengan null check sederhana
    private NavMeshSurface _myNavmesh;
    private NavMeshSurface MyNavmesh
    {
        get
        {
            if (_myNavmesh == null)
            {
                _myNavmesh = GetComponent<NavMeshSurface>();
                if (_myNavmesh == null)
                {
                    Debug.LogError("NavMeshSurface component tidak ditemukan pada GridBuilder!", this);
                }
            }
            return _myNavmesh;
        }
    }

    [SerializeField] private GameObject mainTilePrefab; // Ganti nama agar lebih jelas bahwa ini prefab untuk satu tile
    [SerializeField] private int gridLength = 10;
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private List<GameObject> createdTileGameObjects; // Ganti nama agar lebih jelas isinya GameObject

    /// <summary>
    /// Mengembalikan list GameObject dari tile yang telah dibuat.
    /// </summary>
    public List<GameObject> GetTileGameObjectsSetup() => createdTileGameObjects;

    /// <summary>
    /// Membangun ulang NavMesh berdasarkan tile saat ini.
    /// </summary>
    public void UpdateNavMesh()
    {
        if (MyNavmesh != null)
        {
            MyNavmesh.BuildNavMesh();
        }
        else
        {
            Debug.LogWarning("Tidak bisa update NavMesh karena NavMeshSurface tidak ditemukan.", this);
        }
    }

    [ContextMenu("Build Grid")]
    private void BuildGrid()
    {
        ClearGrid(); // Hapus grid lama sebelum membangun yang baru
        createdTileGameObjects = new List<GameObject>();

        if (mainTilePrefab == null)
        {
            Debug.LogError("Main Tile Prefab belum di-assign di GridBuilder!", this);
            return;
        }

        for (int x = 0; x < gridLength; x++)
        {
            for (int z = 0; z < gridWidth; z++)
            {
                CreateTile(x, z);
            }
        }
        UpdateNavMesh(); // Update NavMesh setelah grid selesai dibangun
    }

    [ContextMenu("Clear Grid")]
    private void ClearGrid()
    {
        // Hati-hati dengan DestroyImmediate di luar Editor scripts.
        // Jika ini dipanggil saat runtime, lebih aman menggunakan Destroy.
        // Untuk ContextMenu (Editor), DestroyImmediate OK.
        if (createdTileGameObjects != null)
        {
            // Iterasi mundur agar aman saat menghapus dari list (meskipun di sini kita Clear setelahnya)
            for (int i = createdTileGameObjects.Count - 1; i >= 0; i--)
            {
                if (createdTileGameObjects[i] != null)
                {
                    if (Application.isPlaying)
                        Destroy(createdTileGameObjects[i]);
                    else
                        DestroyImmediate(createdTileGameObjects[i]);
                }
            }
            createdTileGameObjects.Clear();
        }
        else
        {
            createdTileGameObjects = new List<GameObject>();
        }
    }

    private void CreateTile(float xPosition, float zPosition)
    {
        // Asumsi posisi tile relatif terhadap posisi GridBuilder
        Vector3 newPosition = transform.position + new Vector3(xPosition, 0, zPosition);
        GameObject newTile = Instantiate(mainTilePrefab, newPosition, Quaternion.identity, transform); // Jadikan anak dari GridBuilder
        newTile.name = $"Tile_{xPosition}_{zPosition}"; // Beri nama agar mudah diidentifikasi

        // Pastikan prefab tile memiliki komponen BuildSlot
        if (newTile.GetComponent<BuildSlot>() == null)
        {
            Debug.LogWarning($"Prefab tile '{mainTilePrefab.name}' tidak memiliki komponen BuildSlot. Ini diperlukan.", mainTilePrefab);
        }

        createdTileGameObjects.Add(newTile);
    }

    /// <summary>
    /// Mengembalikan list BuildSlot yang tersedia (aktif, enabled, dan belum ada tower).
    /// Digunakan untuk menemukan tempat membangun tower baru.
    /// </summary>
    public List<BuildSlot> GetAvailableBuildSlots()
    {
        List<BuildSlot> buildSlots = new List<BuildSlot>();
        if (createdTileGameObjects == null) return buildSlots;

        foreach (GameObject tileGO in createdTileGameObjects)
        {
            if (tileGO != null)
            {
                BuildSlot slot = tileGO.GetComponent<BuildSlot>();
                // Cek tambahan apakah slot bisa dibangun secara programatik (jika ada tower, tidak bisa)
                if (slot != null && slot.enabled && slot.gameObject.activeInHierarchy && slot.CanBuildTowerProgrammatically())
                {
                    buildSlots.Add(slot);
                }
            }
        }
        return buildSlots;
    }

    /// <summary>
    /// Mengembalikan list SEMUA komponen BuildSlot dari tile yang telah dibuat.
    /// Berguna untuk GameManager saat mereset semua slot.
    /// </summary>
    public List<BuildSlot> GetAllCreatedBuildSlots()
    {
        List<BuildSlot> allSlots = new List<BuildSlot>();
        if (createdTileGameObjects == null) return allSlots;

        foreach (GameObject tileGO in createdTileGameObjects)
        {
            if (tileGO != null)
            {
                BuildSlot slot = tileGO.GetComponent<BuildSlot>();
                if (slot != null)
                {
                    allSlots.Add(slot);
                }
            }
        }
        return allSlots;
    }
}
