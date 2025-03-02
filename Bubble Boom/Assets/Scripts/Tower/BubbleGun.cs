using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BubbleGun : Tower
{
    private BubbleBullet bullet;
    [SerializeField] private int damage;
    [SerializeField] private Transform gunPoint;

    protected override void Awake()
    {
        bullet = GetComponent<BubbleBullet>();
    }

    protected override void Attack()
    {
        Vector3 directionEnemy = DirectionToEnemy(gunPoint);

        if(Physics.Raycast(gunPoint.position, directionEnemy, out RaycastHit hitInfo, Mathf.Infinity))
        {
            towerHead.forward = directionEnemy;

            bullet.PlayAttack(gunPoint.position, hitInfo.point);

            IDamageAble damagable = hitInfo.transform.GetComponent<IDamageAble>();

            if(damagable != null)
                damagable.TakeDamage(damage);
        }
    }
}
