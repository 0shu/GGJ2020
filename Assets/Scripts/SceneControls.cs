using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GGJ2020 {
    public class SceneControls : MonoBehaviour
    {
        public GameObject m_menu;
        public bool m_enabled = false;

        public string m_menuName = "MAIN";
        public string m_playName = "PLAY";
        public string m_creditsName = "CREDITS";

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            if(Input.GetKeyDown("tab"))
            {
                Debug.Log("Cancel pressed!");

                m_enabled = !m_enabled;
                if(m_enabled) 
                {
                    Cursor.lockState = CursorLockMode.None;
                    Time.timeScale = 0;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Time.timeScale = 1;
                }
                m_menu.SetActive(m_enabled);
            }
        }

        public void SwapTo(string sceneName)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(sceneName);
        }

        public void BackToMenu()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(m_menuName);
        }

        public void BackToPlay()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(m_playName);
        }

        public void BackToCredits()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(m_creditsName);
        }

        public void Quit()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif

            Application.Quit();
        }
    }
}