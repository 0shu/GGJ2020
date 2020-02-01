using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace GGJ2020
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField]
        float checkInterval = 3.0f;

        public enum EnemyState
        {
            Idle,
            AttackingPlayer,
            AttackingBuilding,
            MovingToLocation
        }

        EnemyState m_state = EnemyState.Idle;

        public float lookRadius = 10f;
        public float lookInterval = 10f;
        public float stopDistance = 2.0f;

        private Transform player;

        Vector3 m_currentTarget;
        Vector3 m_longTermTarget;
        bool m_objectiveCheckTrigger = true;
        NavMeshAgent m_agent;
        bool m_aggressive = false;

        IEnumerator m_objectiveCheck()
        {
            yield return new WaitForSeconds(checkInterval);
            m_objectiveCheckTrigger = true;
            print("Triggered enemycheck");
        }
        void Start()
        {
            player = PlayerManager.instance.player.transform;
            m_agent = GetComponent<NavMeshAgent>();
            m_longTermTarget = this.transform.position;
            m_currentTarget = m_longTermTarget;
    }

        void Update()
        {
            



            /*//Calculate distance to player
            float distancetoPlayer = Vector3.Distance(Player.position, transform.position);
            //If its nearby go to them
            if (distancetoPlayer <= lookRadius)
            {
                Debug.Log("Going to Player");
                m_agent.SetDestination(Player.position);
                //If you are close to the player (update rotation)
                if (distancetoPlayer <= m_agent.stoppingDistance)
                {
                    FaceTarget();
                }
            }
            else
            {
                //Check if you are in the range of closest building
                float DistancetoClosestBuilding = Vector3.Distance(Closestbuilding.position, transform.position);
                
                if (DistancetoClosestBuilding <= lookRadius)
                {
                    Debug.Log("Going to Building");
                    Debug.Log(target.position);

                    agent.SetDestination(target.position);
                    Debug.Log(agent.SetDestination(target.position));
                    
                }
                else
                {
                    FindClosestBuilding();
                }

            }*/
        }

        void FixedUpdate()
        {
            if (m_objectiveCheckTrigger)
            {
                float sqrDistanceToPlayer = (player.position - transform.position).sqrMagnitude;
                var nearestBuilding = ResourceManager.GetClosestActiveBuildingTo(transform.position);
                print(nearestBuilding);
                if (sqrDistanceToPlayer <= (lookRadius * lookRadius))
                {
                    m_state = EnemyState.AttackingPlayer;
                    m_currentTarget = player.position;
                    print("Triggering player");
                }
                else if (nearestBuilding != null && ((nearestBuilding.transform.position - transform.position).sqrMagnitude) <= (lookRadius * lookRadius))
                {
                    m_state = EnemyState.AttackingBuilding;
                    //m_currentTarget = nearestBuilding.transform.position;
                    m_agent.SetDestination(new Vector3(nearestBuilding.transform.position.x,
                                                        0f,
                                                        nearestBuilding.transform.position.z));
                    print("Triggering building");
                }
                else
                {
                    print("Triggering else");
                    if (m_aggressive)
                    {
                        m_state = EnemyState.MovingToLocation;
                        m_currentTarget = m_longTermTarget;
                        m_agent.SetDestination(m_currentTarget);
                        print("Triggering aggressive");
                    }
                    else
                    {
                        m_state = EnemyState.Idle;
                        m_longTermTarget = this.transform.position;
                        m_currentTarget = m_longTermTarget;
                        m_agent.SetDestination(m_currentTarget);
                        print("Triggering idle");
                    }
                }


                m_objectiveCheckTrigger = false;
                StartCoroutine("m_objectiveCheck");
            }

            switch (m_state)
            {
                case EnemyState.AttackingPlayer:
                    m_currentTarget = player.position;
                    m_agent.SetDestination(m_currentTarget);
                    break;
                case EnemyState.AttackingBuilding:
                    
                    break;
                case EnemyState.MovingToLocation:
                    if ((transform.position - m_longTermTarget).sqrMagnitude <= (stopDistance * stopDistance))
                    {
                        m_aggressive = false;
                        StopCoroutine("m_objectiveCheck");
                        m_objectiveCheckTrigger = true;
                    }
                    break;
                case EnemyState.Idle:
                default:
                    break;

            }
        }
        //Transform target;
        //Transform FindClosestBuilding()
        //{

        //    //Otherwise Find the closest building#
        //    float temp = 2000;
        //    foreach (GameObject building in PlayerManager.instance.bases)
        //    {
        //        float DistanceToBuilding = Vector3.Distance(building.transform.position, transform.position);
        //        if (DistanceToBuilding < temp)
        //        {
        //            target = building.transform;
        //            temp = DistanceToBuilding;
        //        }
        //    }
        //    return target;
        //}

        EnemyState GetState()
        {
            return m_state;
        }

        void FaceTarget()
        {
            Vector3 direction = (m_currentTarget - transform.position).normalized;
            Quaternion LookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, LookRotation, Time.deltaTime * 5f);
        }
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, lookRadius);
        }

        public void MakeAggressive(Vector3 destinationLocation)
        {
            m_aggressive = true;
            m_longTermTarget = destinationLocation;
        }
    }

}

