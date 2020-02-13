using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

namespace GGJ2020
{
    public class Turret : MonoBehaviour
    {
        public delegate void Death(Enemy e);
        public float fireTimer = 0.5f;
        public float Damage = 20f;
        public Death Enemy_DeathEvent;
        public List<Enemy> Targets = new List<Enemy>();
        public Enemy nearest;
        //public Enemy Current_Target;
        private bool targetLocked;
        private bool shotReady;
        private GameObject cannon;
        public GameObject bullet;
        public GameObject bulletSpawnPoint;
        public ParticleSystem particleLauncher;
        public AudioSource audio;

        // Start is called before the first frame update
        void Start()
        {
            cannon = this.gameObject.transform.GetChild(0).gameObject;
            shotReady = true;
        }

        // Update is called once per frame
        void Update()
        {


        }

        void FixedUpdate()
        {
            if (Targets.Count > 0)
            {
                nearest = Targets[0];
                float closestDistSq = 100000000.0f;
                foreach (var e in Targets)
                {
                    if(e == null) Targets.Remove(e);
                    else
                    {
                        float distSq = (e.transform.position - transform.position).sqrMagnitude;
                        if (distSq < closestDistSq)
                        {
                            nearest = e;
                            closestDistSq = distSq;
                        }
                    }
                    
                }

                float distToEnemy = (nearest.transform.position - bulletSpawnPoint.transform.position).magnitude;
                float timeToEnemy = distToEnemy / Bullet.movementSpeed;


                Vector3 enemyMovement = nearest.gameObject.GetComponent<NavMeshAgent>().velocity;
                Vector3 aimPoint = nearest.transform.position + (timeToEnemy * enemyMovement);

                {
                    Vector3 dir = aimPoint - cannon.transform.position;
                    Quaternion rot = Quaternion.LookRotation(dir);
                    // slerp to the desired rotation over time

                    cannon.transform.rotation = Quaternion.Slerp(cannon.transform.rotation, rot, 5 * Time.deltaTime);
                }
                //cannon.transform.LookAt(aimPoint);

                if (shotReady)
                {
                    Shoot();
                }
            }
        }
        /*public void SetCurrentTarget(Enemy e)
        {
            Current_Target = e;
        }*/

        void Shoot()
        {
            audio.Play();
            Transform _bullet = Instantiate(bullet.transform, bulletSpawnPoint.transform.position, Quaternion.identity);
            particleLauncher.Emit(1);
            _bullet.transform.rotation = bulletSpawnPoint.transform.rotation;
            shotReady = false;
            StartCoroutine(FireRate());

        }

        IEnumerator FireRate()
        {
            yield return new WaitForSeconds(fireTimer);
            shotReady = true;
        }
        void OnTriggerEnter(Collider target)
        {


            if (target.CompareTag("Enemy"))
            {
                /*if(Current_Target == null)
                {
                    SetCurrentTarget(target.gameObject.GetComponent<Enemy>());
                }*/

                Targets.Add(target.gameObject.GetComponent<Enemy>());
                //targetLocked = true;
            }
        }
        void OnTriggerExit(Collider target)
        {
            if (target.CompareTag("Enemy"))
            {
                Enemy e = target.GetComponent<Enemy>();
                Targets.Remove(e);
                /*if (Current_Target == e) {
                    if (Targets != null)
                    {
                        SetCurrentTarget(Targets[0]);
                        targetLocked = true;
                    }
                    else
                    {
                        print("HELLO DARKNES MY OLD FRIEND");
                        Current_Target = null;
                        targetLocked = false;
                    }
                }*/




            }
        }
    }
}