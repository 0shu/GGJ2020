using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public class Bullet : MonoBehaviour
    {
        public const float movementSpeed = 50f;
        public float damage = 20f;
        // Update is called once per frame
        void Start()
        {
            Destroy(this.gameObject, 3);
        }
        void Update()
        {
            transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed);
        }

        void OnTriggerEnter(Collider col)
        {
            Debug.Log(col.tag);
            if (col.CompareTag("Enemy"))
            {
                col.GetComponent<Enemy>().takeDamage(damage);
                Destroy(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }

        }
        void OnCollisionEnter(Collision collision)
        {
            Destroy(this.gameObject);
        }
    }
}