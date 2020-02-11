using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialRotationFreeze : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("WaitToUnlock");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitToUnlock()
    {
        yield return new WaitForSeconds(5f);
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb!=null) { rb.constraints = 0; }
    }
}
