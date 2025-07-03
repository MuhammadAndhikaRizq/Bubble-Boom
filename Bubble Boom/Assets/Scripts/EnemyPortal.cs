using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyPortal : MonoBehaviour
{
    [SerializeField] private float spawnCooldownBase = 1f; // Cooldown dasar
    private float currentSpawnCooldown; // Cooldown yang di-set oleh agen
    [SerializeField] private float spawnRange = 2.4f;
    private float spawnTimer;
    [Space]

    [SerializeField] private List<Waypoint> waypoints;
    [SerializeField] public List<GameObject> enemyVariants; // Jadikan public agar bisa dicek dari Agent

    private List<GameObject> enemiesToSpawnQueue = new List<GameObject>(); // Ganti nama agar lebih jelas
    private List<GameObject> activeEnemies = new List<GameObject>(); // Ganti nama agar lebih jelas

    private void Awake()
    {
        CollectWaypoints();
        currentSpawnCooldown = spawnCooldownBase; // Set default cooldown
    }

    private void Update()
    {
        if (CanSpawnEnemy()) // Ganti nama method
        {
            SpawnNextEnemyInQueue(); // Ganti nama method
        }
    }

    private bool CanSpawnEnemy()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0 && enemiesToSpawnQueue.Count > 0)
        {
            // spawnTimer = currentSpawnCooldown; // Cooldown di-set saat PrepareSpawn
            return true;
        }
        return false;
    }

    private void SpawnNextEnemyInQueue()
    {
        if (enemiesToSpawnQueue.Count == 0) return;

        GameObject enemyPrefabToSpawn = enemiesToSpawnQueue[0]; // Ambil yang pertama di antrian
        enemiesToSpawnQueue.RemoveAt(0); // Hapus dari antrian

        GameObject newEnemyGO = Instantiate(enemyPrefabToSpawn, transform.position, Quaternion.identity);
        Enemy enemyScript = newEnemyGO.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.SetUpEnemy(waypoints, this);
        }
        activeEnemies.Add(newEnemyGO);

        spawnTimer = currentSpawnCooldown; // Reset timer untuk musuh berikutnya *setelah* spawn
    }

    // Method ini tidak lagi diperlukan jika PrepareSpawn mengisi queue dengan tipe yang sudah ditentukan
    // private GameObject GetRandomEnemy() { ... }

    // Method ini tidak lagi diperlukan jika agen langsung memanggil PrepareSpawn
    // public void AddEnemy(GameObject enemyToAdd) => enemiesToSpawnQueue.Add(enemyToAdd);

    public void RemoveActivateEnemy(GameObject enemyToRemove)
    {
        if (activeEnemies.Contains(enemyToRemove))
            activeEnemies.Remove(enemyToRemove);
    }

    public List<GameObject> GetActiveEnemies() => activeEnemies;
    public bool AreAllEnemiesInQueueSpawned() => enemiesToSpawnQueue.Count == 0;


    public bool IsValidEnemyTypeIndex(int index)
    {
        return index >= 0 && index < enemyVariants.Count;
    }

    public void PrepareSpawn(int count, int enemyTypeIndex, float delayBetweenSpawns)
    {
        currentSpawnCooldown = Mathf.Max(0.1f, delayBetweenSpawns); // Pastikan delay tidak terlalu kecil
        enemiesToSpawnQueue.Clear();

        if (!IsValidEnemyTypeIndex(enemyTypeIndex))
        {
            Debug.LogWarning($"EnemyPortal: Menerima enemyTypeIndex tidak valid ({enemyTypeIndex}). Menggunakan tipe 0 (default).");
            enemyTypeIndex = 0; // Default ke tipe pertama jika tidak valid
        }
        
        if (enemyVariants.Count == 0) {
            Debug.LogError("EnemyPortal: enemyVariants kosong! Tidak bisa spawn musuh.");
            return;
        }

        GameObject selectedEnemyPrefab = enemyVariants[enemyTypeIndex];

        for (int i = 0; i < count; i++)
        {
            enemiesToSpawnQueue.Add(selectedEnemyPrefab);
        }

        spawnTimer = 0; // Mulai spawn segera untuk musuh pertama di antrian
    }

    public void ClearAllEnemies()
    {
        foreach (GameObject e in activeEnemies)
        {
            if (e != null) Destroy(e);
        }
        activeEnemies.Clear();
        enemiesToSpawnQueue.Clear();
    }

    [ContextMenu("Collect all waypoint")]
    private void CollectWaypoints()
    {
        waypoints = new List<Waypoint>();
        // Gunakan transform.GetChild(i) untuk iterasi yang lebih aman jika ada child lain
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Waypoint waypoint = child.GetComponent<Waypoint>();
            if (waypoint != null)
            {
                waypoints.Add(waypoint);
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRange);
    }
}
