using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType {Basic, Fast, None}
public class Enemy : MonoBehaviour, IDamageAble
{
    private NavMeshAgent agent;
    public int healthPoints = 5;
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private Transform[] waypoints; 
    private int wayPointIndex;

    [Space]
    private float totalDistance;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.avoidancePriority = Mathf.RoundToInt(agent.speed * 10);
    }

    private void Start()
    {
        //Mencari objek pertamaa dengan script waypoint manager
        waypoints = FindFirstObjectByType<WayPointManager>().GetWayPoints();
        CollecTotalDistance();
    }

    private void Update()
    {
        FaceTarget(agent.steeringTarget);
        if(agent.remainingDistance < .5f)
        {
            agent.SetDestination(GetNextWaypoint());
        }
    }
    
    public float DistanceToTarget() => totalDistance + agent.remainingDistance;
    private void CollecTotalDistance()
    {
        for(int i = 0; i < waypoints.Length - 1; i++)
        {
            float distance = Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
            totalDistance = totalDistance + distance;
        }
    }
    private void FaceTarget(Vector3 newTarget)
    {
        Vector3 directionTarget = newTarget - transform.position;
        directionTarget.y = 0;

        Quaternion newRotation = Quaternion.LookRotation(directionTarget);

        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, turnSpeed * Time.deltaTime);
    }

    private Vector3 GetNextWaypoint()
    {
        if(wayPointIndex >= waypoints.Length)
        {
            // wayPointIndex = 0;
            return transform.position;
        }
        Vector3 targetPoint = waypoints[wayPointIndex].position;

        if(wayPointIndex > 0)
        {
            float distance = Vector3.Distance(waypoints[wayPointIndex].position, waypoints[wayPointIndex - 1].position);
            totalDistance = totalDistance - distance;
        }

        wayPointIndex = wayPointIndex + 1;
        return targetPoint;
    }

    public Vector3 CenterPoint() => centerPoint.position;
    public EnemyType GetEnemyType() => enemyType;

    public void TakeDamage(int damage)
    {
        healthPoints = healthPoints - damage;

        if(healthPoints <= 0)
        {
            Destroy(gameObject);
        }

    }


}
