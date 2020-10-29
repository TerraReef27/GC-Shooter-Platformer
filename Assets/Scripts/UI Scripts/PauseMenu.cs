using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool isGamePaused = false;

    RespawnSystem respawn;
    MainMenu mainMenu;
    public GameObject pauseMenu;
    


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isGamePaused)
            {
                Resume();
                Debug.Log("Resuming");
            }
            else
            {
                Pause();
                Debug.Log("Game is paused");
            }
        }
    }

     void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
    }

    public void Options()
    {
        mainMenu.MainMenuOptions();

    }
    public void LastCheckPoint()
    {
        respawn.Respawn();
    }
}
