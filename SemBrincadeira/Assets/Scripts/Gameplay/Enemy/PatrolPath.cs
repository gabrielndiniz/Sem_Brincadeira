using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace FPHorror.Gameplay.Enemy
{
    public class PatrolPath : MonoBehaviour
    {
        private const float waypointGizmoRadius = 0.3f;
        [SerializeField] private float findPlayerRadius = 5f;
        private List<Transform> childTransforms = new List<Transform>();
        [SerializeField] private float distanceTolerance = 5f;

        private void OnEnable()
        {
            childTransforms = transform.Cast<Transform>().ToList();
        }

        public bool IsPlayerInSight(Vector3 enemyPosition, out Transform player)
        {
            player = null;
            Collider[] hits = Physics.OverlapSphere(enemyPosition, findPlayerRadius);

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    player = hit.transform;
                    return true;
                }
            }

            return false;
        }

        public int GetNextIndex(int i)
        {
            if (i + 1 == transform.childCount)
                return 0;
            else
                return i + 1;
        }

        public Vector3 GetWaypoint(int i)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.GetChild(i).position, out hit, distanceTolerance, NavMesh.AllAreas))
            {
                return hit.position;
            }

            return transform.GetChild(i).position;
        }

        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(GetWaypoint(i), waypointGizmoRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }
        }
    }
}