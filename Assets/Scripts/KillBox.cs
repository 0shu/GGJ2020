using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("KillBox killed: " + other.gameObject.name);
        GameObject.Destroy(other.gameObject);
    }
}
