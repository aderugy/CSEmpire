using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Game
{
    /// <author> Salma </author>
    /// <summary>
    /// Script that manages the AI of an enemy.
    /// </summary>
    
    public class EnnemyAI : MonoBehaviour
    {
        [Header("References")] 
    
        [SerializeField]
        private Transform player;

        [FormerlySerializedAs("_agent")] [SerializeField]
        private NavMeshAgent agent;

        [Header("Stats")]
    
        [SerializeField]
        private float detectionRadius;

        private bool _hasDestination;

        [Header("Wandering parameters")] 
    
        [SerializeField]
        private float wanderingTimeMin;
    
        [SerializeField]
        private float wanderingTimeMax;
    
        [SerializeField]
        private float wanderingDistanceMin;

        [SerializeField] private float wanderingDistanceMax;

        /// <summary>
        /// Called once per frame.
        /// </summary>
        private void Update()
        {
            if (Vector3.Distance(player.position, transform.position) < detectionRadius)
            {
                agent.SetDestination(player.position);
            }
            else
            {
                if(agent.remainingDistance < 0.75f && !_hasDestination)
                {
                    StopCoroutine(GetNewDestination());
                }
            }
        }

        /// <summary>
        /// Coroutine that gets a new destination for the enemy.
        /// </summary>
        /// <returns>A new destination for the enemy.</returns>
        private IEnumerator GetNewDestination()
        {
            _hasDestination = true;
            yield return new WaitForSeconds(Random.Range(wanderingTimeMin,wanderingTimeMax));

            Vector3 nextDestination = transform.position;
            nextDestination += Random.Range(wanderingDistanceMin,wanderingDistanceMax) * new Vector3(Random.Range(-1f, 1), 0f, Random.Range(-1f, 1)).normalized;

            if (NavMesh.SamplePosition(nextDestination, out NavMeshHit hit, wanderingDistanceMax, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }

            _hasDestination = false;
        }

        /// <summary>
        /// Called when the script is drawn in the editor.
        /// </summary>
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position,detectionRadius);
        }
    }
}
