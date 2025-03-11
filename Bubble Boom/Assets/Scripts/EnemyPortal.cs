using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyPortal : MonoBehaviour
{
    [SerializeField] private float spawnCooldown;
    private float spawnTimer;
    [Space]

    [SerializeField] private List<Waypoint> waypoints;
    
    private List<GameObject> enemiesToCreate = new List<GameObject>();
    private List<GameObject> activateEnemies = new List<GameObject>();

    private void Awake()
    {
        CollectWaypoints();
    }
    private void Update()
    {
        if (CanMakeEnemy())
        {
            CreateEnemy();
        }
    }

    private bool CanMakeEnemy()
    {
        spawnTimer -= Time.deltaTime;

        if(spawnTimer <= 0 && enemiesToCreate.Count > 0)
        {
            spawnTimer = spawnCooldown;
            return true;
        }

        return false;
    }

    private void CreateEnemy()
    {
        GameObject randomEnemy = GetRandomEnemy();
        GameObject newEnemy = Instantiate (randomEnemy, transform.position, Quaternion.identity);

        Enemy enemyScript = newEnemy.GetComponent<Enemy>();
        enemyScript.SetUpEnemy(waypoints, this);

        activateEnemies.Add(newEnemy);
    }

    private GameObject GetRandomEnemy()
    {
        int randomIndex = Random.Range(0, enemiesToCreate.Count);
        GameObject chooseEnemy = enemiesToCreate[randomIndex];
        enemiesToCreate.Remove(chooseEnemy);

        return chooseEnemy;
    }

    public void AddEnemy(GameObject enemyToAdd) => enemiesToCreate.Add(enemyToAdd);
    public void RemoveActivateEnemy(GameObject enemyToRemove)
    {
        if(activateEnemies.Contains(enemyToRemove))
            activateEnemies.Remove(enemyToRemove);
    }
    public List<GameObject> GetActiveEnemies() => activateEnemies;

    [ContextMenu("Collect all waypoint")]
    private void CollectWaypoints()
    {
        waypoints = new List<Waypoint>();

        foreach(Transform child in transform)
        {
            Waypoint waypoint = child.GetComponent<Waypoint>();

            if(waypoint != null)
            {
                waypoints.Add(waypoint);
            }
        }
    }
}
