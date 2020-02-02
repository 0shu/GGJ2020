using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Returner : MonoBehaviour
{
    public float m_WaitTime = 30f;          //The amount of time to wait from staring before returning
    public string m_ReturnName = "main";    //The name of the scene to return to


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("ReturnHome");
    }

    IEnumerator ReturnHome()
    {
        Cursor.lockState = CursorLockMode.None;
        yield return new WaitForSeconds(m_WaitTime);            //Waits for the amount of time
        Debug.Log("Changing scene back to : " + m_ReturnName);  //Lets us know what it's doing
        SceneManager.LoadScene(m_ReturnName);                   //Loads the other scene
    }
}
