using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BubbleGun : Tower
{
    private VisualBullet visual;
    [SerializeField] private int damage;
    [SerializeField] private Transform gunPoint;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Attack()
    {
        Vector3 directionEnemy = DirectionToEnemy(gunPoint);

        if(Physics.Raycast(gunPoint.position, directionEnemy, out RaycastHit hitInfo, Mathf.Infinity))
        {
            towerHead.forward = directionEnemy;

            visual.PlayAttack(gunPoint.position, hitInfo.point);

            // IDamagable damagable = hitInfo.transform.GetComponent<IDamagable>();

            // if(damagable != null)
            //     damagable.TakeDamage(damage);
        }
    }
}
