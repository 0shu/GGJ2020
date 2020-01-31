using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace GGJ2020
{
    public class EnemyController : MonoBehaviour
    {
        public float lookRadius = 10f;
        public float lookInterval = 10f;
        float timeWaited = 0;
        bool triggered = false;

        Transform target;
        NavMeshAgent agent;
        EnemyState m_state = EnemyState.Idle;

        void Start()
        {
            target = PlayerManager.instance.player.transform;
            agent = GetComponent<NavMeshAgent>();

        }

        void Update()
        {
            if(triggered)
            {
                timeWaited += Time.deltaTime;

                if (timeWaited > lookInterval)
                {
                    timeWaited = 0;
                    Search();

                }

                float distance = Vector3.Distance(target.position, transform.position);
                agent.SetDestination(target.position);

                if (distance <= agent.stoppingDistance)
                {
                    FaceTarget();
                    //Attack the target
                    //Face the target
                }
            }
        }

        public void TriggerSearch(Transform loc)
        {
            triggered = true;
            
            if (Search() == false)
            {
                target = loc;
            }
        }

        public void StopSearch()
        {
            triggered = false;
            target = gameObject.transform;
        }

        bool Search()
        {
            float distance;
            Transform player = PlayerManager.instance.player.transform;

            //PLAYER
            distance = Vector3.Distance(player.position, transform.position);
            if (distance <= lookRadius)
            {
                target = player;
                return true;
            }
            else
            {
                distance = lookRadius;
                //BUILDINGS
                foreach(GameObject building in PlayerManager.instance.bases)
                {
                    float temp = Vector3.Distance(building.transform.position, transform.position);
                    if(temp < distance)
                    {
                        player = building.transform;
                    }
                }

                //Check if closest building is actually within the radius once more
                distance = Vector3.Distance(player.position, transform.position);
                if (distance <= lookRadius)
                {
                    target = player;
                    return true;
                }
            }
            return false;
        }

        EnemyState GetState()
        {
            return m_state;
        }

        void FaceTarget()
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion LookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, LookRotation, Time.deltaTime * 5f);

        }
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, lookRadius);

        }
    }

    enum EnemyState {
        Idle,
        AttackingPlayer,
        AttackingBuilding,
        GoToLocation
    }

}
