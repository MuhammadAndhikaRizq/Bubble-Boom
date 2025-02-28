using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Transform currentEnemy;

    [Header("Time Setup")]
    [SerializeField] protected float attackCooldown =1;
    protected float lastTimeAttacked;

    [Header("Tower Setup")]
    [SerializeField] protected Transform towerHead;
    private bool canRotate;
    [SerializeField] protected float lookSpeed = 10;
    [SerializeField] protected float attackRange = 2.5f;
    [SerializeField] protected LayerMask whatIsEnemy;

    protected virtual void Awake()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(currentEnemy == null)
        {
            currentEnemy = FindClosestEnemy();
            return;
        }

        if(Vector3.Distance(currentEnemy.position, transform.position) < attackRange)
            currentEnemy = null;

        if(CanAttack())
            Attack();
        
        RotateToEnemy();
    }

   

    protected virtual void Attack()
    {
        
    }

    protected virtual bool CanAttack()
    {
        if(Time.time > lastTimeAttacked + attackCooldown)
        {

            lastTimeAttacked = Time.time;
            return true;
        }
        return false;
    }

    protected Transform FindClosestEnemy()
    {
        List<Transform> possibleTargets = new List<Transform>();
        Collider[] enemiesAround = Physics.OverlapSphere(transform.position, attackRange, whatIsEnemy);

        foreach(Collider enemy in enemiesAround)
        {
            possibleTargets.Add(enemy.transform);
        }

        int randomIndex = Random.Range(0, possibleTargets.Count);

        if(possibleTargets.Count <= 0)
            return null;
        
        return possibleTargets[randomIndex];
    }

    public void EnableRotation(bool enable)
    {
        canRotate = enable;
    }

    protected void RotateToEnemy()
    {
        if(canRotate == false)
            return;
        if(currentEnemy == null)
            return;
        
        Vector3 directionEnemy = currentEnemy.position - towerHead.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionEnemy);
        Vector3 rotation = Quaternion.Lerp(towerHead.rotation, lookRotation, lookSpeed * Time.deltaTime).eulerAngles;

        towerHead.rotation =Quaternion.Euler(rotation);
    }

    protected Vector3 DirectionToEnemy(Transform startPoint)
    {
        if(currentEnemy == null)
            return Vector3.zero;

        return (currentEnemy.position - startPoint.position).normalized;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
