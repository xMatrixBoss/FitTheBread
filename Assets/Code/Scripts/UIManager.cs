using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{
    public GameObject menuPanel; // Assign the menu panel in the Inspector
    private Timer timer; // Reference to the Timer script

    void Start()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false); // Ensure the menu starts as hidden
        }
        
        timer = FindObjectOfType<Timer>(); // Find Timer in the scene
    }

    public void ToggleMenu()
    {
        if (menuPanel != null)
        {
            bool isMenuActive = !menuPanel.activeSelf;
            menuPanel.SetActive(isMenuActive);

            if (timer != null)
            {
                timer.ToggleTimer(isMenuActive); // Pause when menu is active, resume when hidden
            }
        }
    }

    public void ExitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
