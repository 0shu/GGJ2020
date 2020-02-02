using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public class attackEnemy : MonoBehaviour
    {
        public Transform playerCamera;
        public float detectionDistance;
        bool detected;

        public void AttackEnemy()
        {
            Ray raycast = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;

            if (Physics.Raycast(raycast, out hit, detectionDistance))
            {
                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    hit.collider.gameObject.GetComponent<Enemy>().takeDamage(25f);
                    print("Enemy hit");
                }
            }
        }

    }
        
}

