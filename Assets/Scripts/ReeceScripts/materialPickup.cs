using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public class materialPickup : MonoBehaviour
    {
        // type of the resource (enum from Stefan)

        GameObject player;
        public float maxDistance;
        public float speed;

        public float m_fMoveSpeed;
        public float m_fDirectionChangeTime;
        public Vector3 m_MoveDirection;

        public Vector3 rotationAxis;
        public float rotationSpeed;

        bool m_bMoveUp;

        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            StartCoroutine(MoveUp());
        }

        // Update is called once per frame
        void Update()
        {

            if (Vector3.Distance(transform.position, player.transform.position) < maxDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
            }
            else
            {
                if (m_bMoveUp == true)
                {
                    transform.position += m_MoveDirection * m_fMoveSpeed * Time.deltaTime;
                }
                else
                {
                    transform.position -= m_MoveDirection * m_fMoveSpeed * Time.deltaTime;
                }

                transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
            }
        }

        void OnTriggerEnter(Collider coll)
        {
            if(coll.CompareTag("Player"))
            {
                // add resources
                print("Collected");
                Destroy(gameObject);
            }
        }

        IEnumerator MoveUp()
        {
            yield return new WaitForSeconds(m_fDirectionChangeTime);
            m_bMoveUp = true;

            yield return new WaitForSeconds(m_fDirectionChangeTime);
            m_bMoveUp = false;

            StartCoroutine(MoveUp());
        }

        public IEnumerator MoveToTarget(Vector3 targetPos, float flightHeight, float flightDuration)
        {
            Vector3 startPos = transform.position;
            float normalizedTime = 0.0f;

            while (normalizedTime < 1.0f)
            {
                if (gameObject != null)
                {
                    float yAxisOffset = flightHeight * (normalizedTime - normalizedTime * normalizedTime);
                    transform.position = Vector3.Lerp(startPos, targetPos, normalizedTime) + yAxisOffset * Vector3.up;
                    normalizedTime += Time.deltaTime / flightDuration;
                    yield return null;
                }
                else break;
            }
        }
    }
}
