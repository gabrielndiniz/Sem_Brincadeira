using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace FPHorror.Gameplay.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float timeBetweenAttacks = 2f;
        [SerializeField] private float detectionRadius = 10f;
        private NavMeshAgent agent;
        private Transform player;
        private int currentWaypointIndex;
        private bool isPlayerDetected;
        private float timeSinceLastAttack;

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            currentWaypointIndex = 0;
            MoveToNextWaypoint();
        }

        private void Update()
        {
            if (patrolPath.IsPlayerInSight(transform.position, out player) && !IsPlayerHidden())
            {
                isPlayerDetected = true;
                ChasePlayer();
            }
            else if (isPlayerDetected && Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                AttackPlayer();
            }
            else if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (isPlayerDetected)
                {
                    isPlayerDetected = false;
                    MoveToNextWaypoint();
                }
                else
                {
                    Patrol();
                }
            }
        }

        private void Patrol()
        {
            agent.destination = patrolPath.GetWaypoint(currentWaypointIndex);
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private void MoveToNextWaypoint()
        {
            if (patrolPath == null) return;
            Patrol();
        }

        private void ChasePlayer()
        {
            if (player != null)
            {
                agent.destination = player.position;
            }
        }

        private void AttackPlayer()
        {
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                // Implementar lógica de ataque
                timeSinceLastAttack = 0f;
            }
            timeSinceLastAttack += Time.deltaTime;
        }

        private bool IsPlayerHidden()
        {
            return player != null && player.CompareTag("HideSpot");
        }

        public void IncreaseTimeBetweenAttacks(float amount)
        {
            timeBetweenAttacks += amount;
        }
    }
}
