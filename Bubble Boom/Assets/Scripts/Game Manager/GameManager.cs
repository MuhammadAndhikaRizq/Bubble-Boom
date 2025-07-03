using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private float maxPlayerBaseHealth = 100f;
    private float currentPlayerBaseHealth;
    private int currentWaveIndex = 0;

    private List<Tower> allPlayerTowers = new List<Tower>();
    
    // Tambahkan referensi ke GridBuilder jika memungkinkan, untuk mendapatkan BuildSlots
    // Jika tidak, kita akan menggunakan FindObjectsOfType sebagai fallback, tapi kurang ideal.
    [SerializeField] private GridBuilder mainGridBuilder; // Assign ini di Inspector

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        currentPlayerBaseHealth = maxPlayerBaseHealth;

        // Jika mainGridBuilder tidak di-assign, coba cari
        if (mainGridBuilder == null)
        {
            mainGridBuilder = FindObjectOfType<GridBuilder>();
            if (mainGridBuilder == null)
            {
                Debug.LogError("GameManager: GridBuilder tidak ditemukan atau tidak di-assign!");
            }
        }
    }

    // --- Metode untuk Observasi Agen ---
    public float GetTotalTowerPower()
    {
        float totalPower = 0f;
        foreach (Tower tower in allPlayerTowers)
        {
            if (tower != null) totalPower += tower.GetAttackStrength();
        }
        return totalPower;
    }

    public int GetTowerCount()
    {
        return allPlayerTowers.Count;
    }

    public int GetWaveIndex()
    {
        return currentWaveIndex;
    }
    public void IncrementWaveIndex() { currentWaveIndex++; }
    public void ResetWaveIndexForTraining() { currentWaveIndex = 0; }

    // --- Metode untuk Reward Agen ---
    public float GetPlayerBaseHealthPercentage()
    {
        if (maxPlayerBaseHealth <= 0) return 0f;
        return currentPlayerBaseHealth / maxPlayerBaseHealth;
    }

    public void PlayerBaseTakesDamage(float amount)
    {
        currentPlayerBaseHealth -= amount;
        if (currentPlayerBaseHealth < 0) currentPlayerBaseHealth = 0;
    }

    public void ResetPlayerBaseHealth()
    {
        currentPlayerBaseHealth = maxPlayerBaseHealth;
    }

    // --- Metode untuk Setup Episode Agen ---
    public void AutoBuildTowersForTraining(GridBuilder grid, List<GameObject> towerPrefabs, int towerCount, bool randomizePlacementAndType)
    {
        // Gunakan metode ClearAllTowersAndSlots yang baru untuk memastikan slot juga direset
        ClearAllTowersAndSlots(); 

        // Dapatkan BuildSlot dari GridBuilder yang diberikan sebagai parameter (lebih baik)
        // atau dari mainGridBuilder jika parameter grid null (sebagai fallback)
        GridBuilder targetGrid = grid != null ? grid : mainGridBuilder;
        if (targetGrid == null)
        {
            Debug.LogError("AutoBuildTowersForTraining: Tidak ada GridBuilder valid untuk mendapatkan slot.");
            return;
        }

        List<BuildSlot> availableSlots = targetGrid.GetAvailableBuildSlots();
        if (availableSlots.Count == 0 && towerCount > 0) {
             Debug.LogWarning("AutoBuildTowersForTraining: Tidak ada BuildSlot tersedia di GridBuilder.");
             return;
        }
        if (towerPrefabs.Count == 0 && towerCount > 0) {
            Debug.LogWarning("AutoBuildTowersForTraining: Tidak ada tower prefab yang tersedia.");
            return;
        }


        for (int i = 0; i < towerCount; i++)
        {
            if (availableSlots.Count == 0) break;

            int slotIndex;
            GameObject towerPrefabToBuild;
            
            if (randomizePlacementAndType)
            {
                slotIndex = Random.Range(0, availableSlots.Count);
                towerPrefabToBuild = towerPrefabs[Random.Range(0, towerPrefabs.Count)];
            }
            else
            {
                slotIndex = 0; 
                towerPrefabToBuild = towerPrefabs[0];
            }

            BuildSlot chosenSlot = availableSlots[slotIndex];

            // Gunakan metode programatik dari BuildSlot
            if (chosenSlot.CanBuildTowerProgrammatically())
            {
                GameObject newTowerGO = chosenSlot.BuildTowerProgrammatically(towerPrefabToBuild);
                if (newTowerGO != null)
                {
                    Tower newTowerScript = newTowerGO.GetComponent<Tower>();
                    if (newTowerScript != null)
                    {
                        // Pendaftaran tower ke list allPlayerTowers sebaiknya dilakukan oleh Tower.Start()
                        // atau oleh BuildTowerProgrammatically jika tower tidak mendaftarkan dirinya sendiri.
                        // Untuk saat ini, kita tambahkan di sini untuk memastikan.
                        if (!allPlayerTowers.Contains(newTowerScript))
                        {
                           allPlayerTowers.Add(newTowerScript);
                        }
                    }
                }
            }
            availableSlots.RemoveAt(slotIndex);
        }
        ResetPlayerBaseHealth();
    }

    // Mengganti nama ClearAllTowers menjadi lebih spesifik
    // dan memastikan BuildSlot juga direset.
    public void ClearAllTowersAndSlots()
{
    // 1. Hancurkan GameObject tower
    foreach (Tower tower in allPlayerTowers)
    {
        if (tower != null) Destroy(tower.gameObject);
    }
    allPlayerTowers.Clear();

    // 2. Reset semua BuildSlot menggunakan metode baru dari GridBuilder
    if (mainGridBuilder != null)
    {
        List<BuildSlot> allSlotsInGrid = mainGridBuilder.GetAllCreatedBuildSlots();
        foreach (BuildSlot slot in allSlotsInGrid)
        {
            slot.ClearTowerProgrammatically();
        }
    }
    else
    {
        Debug.LogWarning("ClearAllTowersAndSlots: mainGridBuilder tidak di-assign di GameManager. Mencoba fallback FindObjectsOfType.");
        BuildSlot[] allSlotsInScene = FindObjectsOfType<BuildSlot>();
        foreach (BuildSlot slot in allSlotsInScene)
        {
            slot.ClearTowerProgrammatically();
        }
    }
}

    public void RegisterTower(Tower tower) {
        if (!allPlayerTowers.Contains(tower)) allPlayerTowers.Add(tower);
    }

    public void UnregisterTower(Tower tower) {
        if (allPlayerTowers.Contains(tower)) allPlayerTowers.Remove(tower);
    }
}