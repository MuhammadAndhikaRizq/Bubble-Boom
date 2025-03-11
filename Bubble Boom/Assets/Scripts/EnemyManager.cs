using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveDetails
{
    public int basicEnemy;
    public int fastEnemy;
}
public class EnemyManager : MonoBehaviour
{
    public bool waveCompleted;

    public float timerBeetwenWave = 10;
    public float waveTimer;
    [SerializeField] private WaveDetails[] levelWaves;
    private int waveIndex;

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
        if(waveCompleted == false && AllEnemiesDead())
        {
            waveCompleted = true;
            waveTimer = timerBeetwenWave;
        }
        
        if(waveCompleted)
        {
            waveTimer -= Time.deltaTime;

            if(waveTimer <= 0)
                SetUpNextWave();
        }
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
}
