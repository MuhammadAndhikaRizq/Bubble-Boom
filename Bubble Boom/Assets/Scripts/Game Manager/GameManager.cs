using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // [SerializeField] private PlayerBase player;
    [SerializeField] private List<Tower> activeTowers;
    [SerializeField] private int currentWave;

    private void Awake()
    {
        Instance = this;
    }

    public float TotalTowerPower()
    {
        float total = 0;
        foreach(var tower in activeTowers)
        {
            total += tower.GetAttackStrength();
        }

        return total;
    }

    public int GetWaveIndex()
    {
        return currentWave;
    }

    public void NextWave()
    {
        currentWave++;
    }

    public void AutoBuildTowersForTraining(GridBuilder grid, List<GameObject> towerPrefabs, int maxTower = 3)
    {
        List<BuildSlot> slots = grid.GetAvailableBuildSlots();

        for (int i = 0; i < maxTower && slots.Count > 0; i++)
        {
            int randIndex = Random.Range(0, slots.Count);
            BuildSlot selectedSlot = slots[randIndex];

            // Instantiate tower
            GameObject towerPrefab = towerPrefabs[Random.Range(0, towerPrefabs.Count)];
            GameObject newTower = GameObject.Instantiate(towerPrefab, selectedSlot.transform.position, Quaternion.identity);
            
            newTower.transform.SetParent(selectedSlot.transform); // optional
            slots.RemoveAt(randIndex); // remove so no duplicate
        }
    }
    
}
