#if HAS_AI_NAVIGATION
using UnityEngine;
using UnityEngine.AI;

namespace ErmineGames.Utils
{
    public static class NavMeshUtils
    {
        public static Vector3 GetNavMeshPoint(
            Vector3 originalPoint, float maxDistance = 1f, int areaMask = NavMesh.AllAreas)
        {
            if (NavMesh.SamplePosition(originalPoint, out var closestHit, maxDistance, areaMask))
            {
                return closestHit.position;
            }
            
            Debug.LogWarning("NavMesh.GetNavMeshPoint() not found point.");
            return Vector3.zero;
        }
        
        public static void PlaceToNavMesh(
            Transform transform, float maxDistance = 10f, int areaMask = NavMesh.AllAreas)
        {
            if (NavMesh.SamplePosition(transform.position, out var closestHit, maxDistance, areaMask))
            {
                transform.position = closestHit.position;
            }
        }

        public static bool HasReachedDestination(
            NavMeshAgent agent, Vector3 destination, float threshold = 0.1f)
        {
            if (agent.pathPending || (agent.hasPath && agent.stoppingDistance == 0f))
            {
                return false;
            }

            var sqrStoppingDistance = 
                (agent.stoppingDistance + threshold) * 
                (agent.stoppingDistance + threshold);
            
            return (destination - agent.transform.position).sqrMagnitude <= sqrStoppingDistance;
        }
    }
}
#endif
