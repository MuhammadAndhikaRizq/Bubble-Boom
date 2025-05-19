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
}
