using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float currentHealth = 100;
    private GameObject[] Turrets;
    private GameObject turret;
    public void takeDamage(float Damage)
    {
        if (currentHealth - Damage >= 1)
        {
            currentHealth -= Damage;
        }
        else
        {

            //Turrets = GameObject.FindGameObjectsWithTag("Turret");

            //foreach (GameObject turret in Turrets)
            //{
            //    if(this != turret.GetComponent<Turret>().nearest)
            //    {
            //        print("im not");

            //    }
            //}

            GameObject[] gameObjects;
            gameObjects = GameObject.FindGameObjectsWithTag("Turret");

            foreach (GameObject turret in gameObjects)
            {
                if(turret.GetComponent<Turret>().nearest == this)
                {
                    turret.GetComponent<Turret>().nearest = null;
                    turret.GetComponent<Turret>().Targets.Remove(this);
                }
            }

            Destroy(this.gameObject);

        }

        // Update is called once per frame

    }
}