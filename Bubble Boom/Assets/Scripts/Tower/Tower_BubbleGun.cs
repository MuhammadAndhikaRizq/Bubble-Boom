using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_BubbleGun : Tower
{
    [Header("Bubble Gun")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int damage;
    [SerializeField] private Transform gunPoint;
    [SerializeField] private float projectileSpeed = 10f;

    protected override void Start()
    {
        
    }

    protected override void Attack()
    {
        Vector3 directionToEnemy = DirectionToEnemy(gunPoint);

        if(currentEnemy != null)
        {
            towerHead.forward = directionToEnemy;

            GameObject bubbleTile =  Instantiate(projectilePrefab, gunPoint.position, Quaternion.LookRotation(directionToEnemy));
            
            BubbleBullet bubbleTileScript = bubbleTile.GetComponent<BubbleBullet>();

            if(bubbleTileScript != null)
            {
                 bubbleTileScript.Initialize(damage, currentEnemy.transform);
            }

            Rigidbody rb = bubbleTile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = directionToEnemy * projectileSpeed;
            }
        }
    }
}
