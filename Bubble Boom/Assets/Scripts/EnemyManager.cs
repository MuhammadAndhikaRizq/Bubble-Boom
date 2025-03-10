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
    public List<EnemyPortal> enemyPortals;
    [SerializeField] private WaveDetails currentWave;

    [Header ("Enemy Prefab")]
    [SerializeField] private GameObject basicEnemy;
    [SerializeField] private GameObject fastEnemy;
    
    private void Awake()
    {
        enemyPortals = new List<EnemyPortal>( FindObjectsOfType<EnemyPortal>() );
    }

    private void Start()
    {
        SetUpNextWave();
    }

    [ContextMenu("Setup Up Next Wave")]
    private void SetUpNextWave()
    {
        List<GameObject> newEnemyList = NewEnemyWave();
        int portalIndex = 0;

        for(int i = 0; i < newEnemyList.Count; i++)
        {
            GameObject enemyToAdd = newEnemyList[i];
            EnemyPortal portalToReceiveEnemy = enemyPortals[portalIndex];

            portalToReceiveEnemy.AddEnemy(enemyToAdd);

            portalIndex++;

            if(portalIndex >= enemyPortals.Count)
                portalIndex = 0;
        }
    }
    private List<GameObject> NewEnemyWave()
    {
        List<GameObject> newEnemyList = new List<GameObject>();

        for(int i=0; i < currentWave.basicEnemy; i++)
        {
            newEnemyList.Add(basicEnemy);
        }

        for(int i=0; i < currentWave.fastEnemy; i++)
        {
            newEnemyList.Add(fastEnemy);
        }

        return newEnemyList;
    }
}
