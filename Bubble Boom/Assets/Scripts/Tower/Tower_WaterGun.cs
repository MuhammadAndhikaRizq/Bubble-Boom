using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_WaterGun : Tower
{
    private WaterGun_Viisuals visuals;
    [Header("Water Gun")]
    [SerializeField] private int damage;
    [SerializeField] private Transform gunPoint;

    protected override void Awake()
    {
        visuals = GetComponent<WaterGun_Viisuals>();   
    }

    protected override void Attack()
    {
        Vector3 directionToEnemy = DirectionToEnemy(gunPoint);

        if(Physics.Raycast(gunPoint.position, directionToEnemy, out RaycastHit hitInfo, Mathf.Infinity))
        {
            towerHead.forward = directionToEnemy;

            Enemy enemyTarget = null;
            IDamageAble damageAble = hitInfo.transform.GetComponent<IDamageAble>();

            if(damageAble != null)
            {
                damageAble.TakeDamage(damage);
                enemyTarget = currentEnemy;
            }

            visuals.PlayAttackVisual(gunPoint.position, hitInfo.point, enemyTarget);
        }
    }
}
