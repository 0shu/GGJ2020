using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Player")
        {
            Debug.Log("KillBox killed: " + other.gameObject.name);
            GameObject.Destroy(other.gameObject);
        }
    }
}
