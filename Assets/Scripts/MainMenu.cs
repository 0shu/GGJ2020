using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string m_CreditsName = "CREDITS";
    public string m_MainName = "MAIN";

    public void PlayGame()
    {
        SceneManager.LoadScene(m_MainName);
    }
    public void PlatCredits()
    {
        SceneManager.LoadScene(m_CreditsName);
    }

    public void Quit()
    {
        Debug.Log("Quit!");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        
        Application.Quit();
    }


}

