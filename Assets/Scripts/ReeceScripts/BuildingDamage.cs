using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public class BuildingDamage : MonoBehaviour
    {
        public float BuildingHealth;
        Building building;

        // Start is called before the first frame update
        void Start()
        {
            building = this.gameObject.GetComponent<Building>();

        }

        public void takeDamage(float Damage)
        {
            if (BuildingHealth - Damage >= 1)
            {
                BuildingHealth -= Damage;
            }
            else
            {
                building.Ruin();
            }

        }
    }
}


