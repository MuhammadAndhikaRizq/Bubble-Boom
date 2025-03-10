using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPortal : MonoBehaviour
{

    [SerializeField] private Transform respawn;
    [SerializeField] private float spawnCooldown;
    private float spawnTimer;
    
    public List<GameObject> enemiesToCreate;
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
    }

    private GameObject GetRandomEnemy()
    {
        int randomIndex = Random.Range(0, enemiesToCreate.Count);
        GameObject chooseEnemy = enemiesToCreate[randomIndex];
        enemiesToCreate.Remove(chooseEnemy);

        return chooseEnemy;
    }

    public List<GameObject> GetEnemyList() => enemiesToCreate;
}
