using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject helpPanel; // Assign the Help Panel in the Inspector

    void Start()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(false); // Ensure Help Panel starts hidden
        }
    }

    // Load the level scene using scene index (1)
    public void PlayGame()
    {
        SceneManager.LoadScene(1); // Scene index 1 should be your level scene
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
