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
            MovingToLocation,
            Exploading,
            Death
        }

        EnemyState m_state = EnemyState.Idle;

        public float lookRadius = 10f;
        public float lookInterval = 10f;
        public float stopDistance = 2.0f;

        public float explosionDelay = 2f;
        public float explosionRadius = 15f;
        public float explosionForce = 700f;
        public float hurtFrameLength = 0.1f;

        public GameObject explosionEffect;
        public AudioSource audio;
        public List<AudioClip> m_sounds;
        public List<Material> m_materials;
        private Transform player;

        Vector3 m_currentTarget;
        Vector3 m_longTermTarget;
        bool m_objectiveCheckTrigger = true;
        NavMeshAgent m_agent;
        bool m_aggressive = false;
        bool goingToPlayer = false;
        bool hurting = false;
        bool dying = false;

        private float distanceToBuilding;

        IEnumerator m_objectiveCheck()
        {
            yield return new WaitForSeconds(checkInterval);
            m_objectiveCheckTrigger = true;
            //print("Triggered enemycheck");
        }
        void Start()
        {
            player = GameObject.FindWithTag("Player").transform;
            m_agent = GetComponent<NavMeshAgent>();
            m_longTermTarget = this.transform.position;
            m_currentTarget = m_longTermTarget;
            StartCoroutine(ReenableNavMesh());
        }

        void Update()
        {
            if (m_state == EnemyState.AttackingPlayer)
            {
                float DistanceToPlayer = (Vector3.Distance(transform.position, player.transform.position));
                if (DistanceToPlayer <= stopDistance)
                {
                    TriggerFuse();
                    //Explode();
                }
            }

            
        }

        public void InterruptSound(int sound)
        {
            audio.clip = m_sounds[sound];
            audio.Play();
        }

        public void QueueSound(int sound)
        {
            if(m_sounds.Count > sound)
            {
                if(audio.isPlaying)
                {
                    float delay = audio.clip.length - audio.time;
                    audio.clip = m_sounds[sound];
                    audio.PlayDelayed(delay);
                }
                else
                {
                    audio.clip = m_sounds[sound];
                    audio.Play();
                }
            }
            else
            {
                Debug.Log("Not Enough sounds on enemy to play specified piece! " + sound + " vs " + m_sounds.Count);
            }
        }

        public void TriggerFuse()
        {
            if (m_state != EnemyState.Exploading)
            {
                m_state = EnemyState.Exploading;
                StartCoroutine(Fuse());
            }
        }

        void FixedUpdate()
        {

            if (m_objectiveCheckTrigger)
            {
                
                float sqrDistanceToPlayer = (player.position - transform.position).sqrMagnitude; 
                var nearestBuilding = ResourceManager.GetClosestActiveBuildingTo(transform.position);
                if (sqrDistanceToPlayer <= (lookRadius * lookRadius))
                {
                    if(m_state != EnemyState.AttackingPlayer) 
                    {
                        QueueSound(1);
                    }
                    m_state = EnemyState.AttackingPlayer;
                    
                    
                    m_currentTarget = player.position;
                    

                }
                else if (nearestBuilding != null && ((nearestBuilding.transform.position - transform.position).sqrMagnitude) <= (lookRadius * lookRadius))
                {
                    //print("Triggering Attacking Building");

                    distanceToBuilding = Vector3.Distance(transform.position, nearestBuilding.transform.position);
                    if(distanceToBuilding <= 10f)
                    {
                        //Explode();
                        TriggerFuse();
                    }
                    m_state = EnemyState.AttackingBuilding;
                    //m_currentTarget = nearestBuilding.transform.position;
                    if(m_agent.isOnNavMesh) m_agent.SetDestination(new Vector3(nearestBuilding.transform.position.x,
                                                        0f,
                                                        nearestBuilding.transform.position.z));

                }
                else
                {

                    if (m_aggressive)
                    {
                        m_state = EnemyState.MovingToLocation;
                        m_currentTarget = m_longTermTarget;
                        if(m_agent.isOnNavMesh) m_agent.SetDestination(m_currentTarget);

                    }
                    else
                    {
                        m_state = EnemyState.Idle;
                        m_longTermTarget = this.transform.position;
                        m_currentTarget = m_longTermTarget;
                        if(m_agent.isOnNavMesh) m_agent.SetDestination(m_currentTarget);
                        //print("Triggering idle");
                    }
                }


                m_objectiveCheckTrigger = false;
                StartCoroutine("m_objectiveCheck");
            }

            switch (m_state)
            {
                case EnemyState.AttackingPlayer:
                    m_currentTarget = player.position;
                    if(m_agent.isOnNavMesh) m_agent.SetDestination(m_currentTarget);
                    goingToPlayer = true;
                    break;
                case EnemyState.AttackingBuilding:
                    
                    break;
                case EnemyState.Exploading:
                    transform.Rotate(Vector3.up, 10.0f, Space.World);

                    break;
                case EnemyState.MovingToLocation:
                    if ((transform.position - m_longTermTarget).sqrMagnitude <= (stopDistance * stopDistance))
                    {
                        m_aggressive = false;
                        StopCoroutine("m_objectiveCheck");
                        m_state = EnemyState.Idle;
                        m_objectiveCheckTrigger = true;
                    }
                    break;
                case EnemyState.Idle:
                default:
                    goingToPlayer = false;
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

        public void Die()
        {
            InterruptSound(2);
            gameObject.GetComponent<MeshCollider>().enabled = false;
            dying = true;
            if(!hurting) gameObject.GetComponent<MeshRenderer>().material = m_materials[2];
            Destroy(m_agent);
            Destroy(this.gameObject, m_sounds[2].length);
        }

        public void Hurt()
        {
            if(hurting) StopCoroutine(HurtFrame());
            StartCoroutine(HurtFrame());
        }

        IEnumerator HurtFrame()
        {
            hurting = true;
            gameObject.GetComponent<MeshRenderer>().material = m_materials[1];
            yield return new WaitForSeconds(hurtFrameLength);
            if(dying) gameObject.GetComponent<MeshRenderer>().material = m_materials[2];
            else if(hurting) gameObject.GetComponent<MeshRenderer>().material = m_materials[0];
            hurting = false;
        }

        public void Explode()
        {
            InterruptSound(0);
            //print("Exploding");
            m_agent.enabled = false;
            m_objectiveCheckTrigger = false;
            m_state = EnemyState.Exploading; 

            GameObject clone = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(clone, 1.0f);

            Rigidbody minerigid = this.GetComponent<Rigidbody>();

            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider nearbyObject in colliders)
            {
                Debug.Log("Detected hit.");
                Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                    rb.AddForce(Vector3.up * (explosionForce/1.5f));

                    var pc = nearbyObject.GetComponent<playerController>();
                    if (pc != null) { pc.takeDamage(10f); }
                }

                BuildingDamage bd = nearbyObject.GetComponent<BuildingDamage>();
                if(bd != null) bd.takeDamage(1f);
            }
            //minerigid.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            StartCoroutine(Reenable());
        }

        IEnumerator Reenable()
        {
            m_state = EnemyState.Exploading;
            yield return new WaitForSeconds(5f);
            m_agent.enabled = true;
            m_objectiveCheckTrigger = true;
        }

        IEnumerator Fuse()
        {
            m_agent.enabled = false;
            m_state = EnemyState.Exploading;
            yield return new WaitForSeconds(3f);
            m_agent.enabled = true;
            Explode();
        }

        IEnumerator ReenableNavMesh()
        {
            m_agent.enabled = false;
            yield return new WaitForSeconds(10f);
            m_agent.enabled = true;
        }
    }

}

