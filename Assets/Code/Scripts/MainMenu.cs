using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject helpPanel; 

    void Start()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(false); 
        }
    }


    public void PlayGame()
    {
        SceneManager.LoadScene(1); 
    }
    public void PlayMobile()
    {
        SceneManager.LoadScene(2); 
    }

    // Toggle the Help Panel on/off
    public void ToggleHelp()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(!helpPanel.activeSelf);
        }
    }

    // Quit the game
        public void QuitGame()
    {
        #if UNITY_WEBGL
            Application.OpenURL("https://itch.io/");
        #else
            Application.Quit(); // For other platforms
        #endif
    }
}
