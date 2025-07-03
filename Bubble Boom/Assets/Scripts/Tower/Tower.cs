using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Enemy currentEnemy;

    [Header("Time Setup")]
    [SerializeField] protected float attackCooldown =1;
    protected float lastTimeAttacked;

    [Header("Tower Setup")]
    [SerializeField] protected EnemyType enemyPriority = EnemyType.None;
    [SerializeField] protected Transform towerHead;
    private bool canRotate;
    [SerializeField] protected float lookSpeed = 10;
    [SerializeField] protected float attackRange = 2.5f;
    [SerializeField] protected LayerMask whatIsEnemy;

    [Space]
    [Tooltip("Enabling this allows tower to change target between attacks")]
    [SerializeField] private bool dynamicTargetChange;
    private float targetCheckInterval = .1f;
    private float lastTimeChecked;

    protected virtual void Start() // Ganti Awake ke Start atau pastikan GameManager.Instance sudah ada
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterTower(this);
        }
        EnableRotation(true); // Dari kode Anda sebelumnya, dipindah dari Awake
    }

    protected virtual void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterTower(this);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateTargetIfNeeded();
        if (currentEnemy == null)
        {
            currentEnemy = FindClosestEnemy();
            return;
        }

        
        if (CanAttack())
            Attack();

        LooseTargetIfNeeded();
        RotateToEnemy();
    }

    private void LooseTargetIfNeeded()
    {
        if (Vector3.Distance(currentEnemy.CenterPoint(), transform.position) > attackRange)
            currentEnemy = null;
    }

    private void UpdateTargetIfNeeded()
    {
        if(dynamicTargetChange == false)
            return;
        if(Time.time > lastTimeChecked + targetCheckInterval)
        {
            lastTimeChecked = Time.time;
            currentEnemy = FindClosestEnemy();
        }
    }

    protected virtual void Attack()
    {
        
    }

    public virtual float GetAttackStrength()
    {
        return 1f / attackCooldown;;
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

    protected Enemy FindClosestEnemy()
    {
        List<Enemy> priorityTargets = new List<Enemy>();
        List<Enemy> possibleTargets = new List<Enemy>();

        Collider[] enemiesAround = Physics.OverlapSphere(transform.position, attackRange, whatIsEnemy);

        foreach(Collider enemy in enemiesAround)
        {
            Enemy newEnemy = enemy.GetComponent<Enemy>();
            EnemyType newEnemyType = newEnemy.GetEnemyType();

            if(newEnemyType == enemyPriority)
                priorityTargets.Add(newEnemy);
            else
                possibleTargets.Add(newEnemy);;
        }

        if(priorityTargets.Count > 0)
            return GetMostAdvancedEnemy(priorityTargets);
        
        if(possibleTargets.Count > 0)
            return GetMostAdvancedEnemy(possibleTargets);

        return null;
    }

    private Enemy GetMostAdvancedEnemy(List<Enemy> targets)
    {
        Enemy mostAdvanceEnemy = null;
        float minRemainingDistance = float.MaxValue;

        foreach(Enemy enemy in targets)
        {
            float remainingDistance = enemy.DistanceToTarget();

            if(remainingDistance < minRemainingDistance)
            {
                minRemainingDistance = remainingDistance;
                mostAdvanceEnemy = enemy;
            }
        }

        return mostAdvanceEnemy;
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
        
        Vector3 directionEnemy = DirectionToEnemy(towerHead);
        Quaternion lookRotation = Quaternion.LookRotation(directionEnemy);
        Vector3 rotation = Quaternion.Lerp(towerHead.rotation, lookRotation, lookSpeed * Time.deltaTime).eulerAngles;

        towerHead.rotation =Quaternion.Euler(rotation);
    }

    protected Vector3 DirectionToEnemy(Transform startPoint)
    {
        if(currentEnemy == null)
            return Vector3.zero;

        return (currentEnemy.CenterPoint() - startPoint.position).normalized;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
