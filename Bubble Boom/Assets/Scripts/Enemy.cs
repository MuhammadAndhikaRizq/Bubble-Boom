using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    public int healthPoints = 5;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private Transform[] waypoints; 
    private int wayPointIndex;

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
    }

    private void Update()
    {
        FaceTarget(agent.steeringTarget);
        if(agent.remainingDistance < .5f)
        {
            agent.SetDestination(GetNextWaypoint());
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
        wayPointIndex++;
        return targetPoint;
    }

    public void TakeDamage(int damage)
    {
        healthPoints = healthPoints - damage;

        if(healthPoints <= 0)
        {
            Destroy(gameObject);
        }

    }


}
