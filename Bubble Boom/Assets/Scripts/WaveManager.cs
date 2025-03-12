using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[System.Serializable]
public class WaveDetails
{
    public int basicEnemy;
    public int fastEnemy;
}
public class WaveManager : MonoBehaviour
{
    public bool waveCompleted;

    public float timerBeetwenWave = 5;
    public float waveTimer;
    [SerializeField] private WaveDetails[] levelWaves;
    private int waveIndex;

    private float checkInterval = .5f;
    private float nextCheckTime;

    [Header ("Enemy Prefab")]
    [SerializeField] private GameObject basicEnemy;
    [SerializeField] private GameObject fastEnemy;

    public List<EnemyPortal> enemyPortals;
    
    private void Awake()
    {
        enemyPortals = new List<EnemyPortal>( FindObjectsOfType<EnemyPortal>() );
    }

    private void Start()
    {
        SetUpNextWave();
    }

    private void Update()
    {
        HandleWave();
        HandleTiming();
    }

    private void HandleWave()
    {
        if(ReadyToCheck() == false)
            return;

        if (waveCompleted == false && AllEnemiesDead())
        {
            waveCompleted = true;
            waveTimer = timerBeetwenWave;
        }
    }

    private void HandleTiming()
    {
        if (waveCompleted)
        {
            waveTimer -= Time.deltaTime;

            if (waveTimer <= 0)
                SetUpNextWave();
        }
    }

    public void ForceNextWave()
    {
        if(AllEnemiesDead() == false)
        {
            return;
        }

        SetUpNextWave();
    }

    [ContextMenu("Setup Up Next Wave")]
    private void SetUpNextWave()
    {
        List<GameObject> newEnemyList = NewEnemyWave();
        int portalIndex = 0;

        if(newEnemyList == null)
            return;

        for(int i = 0; i < newEnemyList.Count; i++)
        {
            GameObject enemyToAdd = newEnemyList[i];
            EnemyPortal portalToReceiveEnemy = enemyPortals[portalIndex];

            portalToReceiveEnemy.AddEnemy(enemyToAdd);

            portalIndex++;

            if(portalIndex >= enemyPortals.Count)
                portalIndex = 0;
        }

        waveCompleted = false;
    }
    private List<GameObject> NewEnemyWave()
    {
        if(waveIndex >= levelWaves.Length)
        {
            return null;
        }
        List<GameObject> newEnemyList = new List<GameObject>();

        for(int i=0; i < levelWaves[waveIndex].basicEnemy; i++)
        {
            newEnemyList.Add(basicEnemy);
        }

        for(int i=0; i < levelWaves[waveIndex].fastEnemy; i++)
        {
            newEnemyList.Add(fastEnemy);
        }

        waveIndex++;

        return newEnemyList;
    }

    private bool AllEnemiesDead()
    {
        foreach(EnemyPortal portal in enemyPortals)
        {
            if(portal.GetActiveEnemies().Count > 0)
                return false;
        }
        return true;
    }

    private bool ReadyToCheck()
    {
        if(Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;
            return true;
        }

        return false;
    }
}
