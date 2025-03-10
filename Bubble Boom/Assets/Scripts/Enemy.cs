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
    [SerializeField] private List<Transform> myWaypoints; 
    private int nextWayPointIndex;
    private int currentWayPointIndex;   

    [Space]
    private float totalDistance;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.avoidancePriority = Mathf.RoundToInt(agent.speed * 10);
    }

    public void SetUpEnemy(List<Waypoint> newWaypoints)
    {
        myWaypoints = new List<Transform>();

        foreach(var point in newWaypoints)
        {
            myWaypoints.Add(point.transform);
        }

        CollecTotalDistance();
    }

    private void Update()
    {
        FaceTarget(agent.steeringTarget);
        if(ShouldChangeWaypoint())
        {
            agent.SetDestination(GetNextWaypoint());
        }
    }

    private bool ShouldChangeWaypoint()
    {
        if(nextWayPointIndex >= myWaypoints.Count)
            return false;
        
        if(agent.remainingDistance < .5f)
            return true;

        Vector3 currentWaypoint = myWaypoints[currentWayPointIndex].position;
        Vector3 nexWaypoint = myWaypoints[nextWayPointIndex].position;

        float distanceToNextWaypoint = Vector3.Distance(transform.position, nexWaypoint);
        float ditanceBeetwenWaypoint = Vector3.Distance(currentWaypoint, nexWaypoint);

        
        return ditanceBeetwenWaypoint > distanceToNextWaypoint;
        
    }
    
    public float DistanceToTarget() => totalDistance + agent.remainingDistance;
    private void CollecTotalDistance()
    {
        for(int i = 0; i < myWaypoints.Count - 1; i++)
        {
            float distance = Vector3.Distance(myWaypoints[i].position, myWaypoints[i + 1].position);
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
        if(nextWayPointIndex >= myWaypoints.Count)
        {
            // nextWayPointIndex = 0;
            return transform.position;
        }
        Vector3 targetPoint = myWaypoints[nextWayPointIndex].position;

        if(nextWayPointIndex > 0)
        {
            float distance = Vector3.Distance(myWaypoints[nextWayPointIndex].position, myWaypoints[nextWayPointIndex - 1].position);
            totalDistance = totalDistance - distance;
        }

        nextWayPointIndex = nextWayPointIndex + 1;
        currentWayPointIndex = nextWayPointIndex - 1;
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
