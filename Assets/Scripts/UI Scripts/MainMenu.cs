using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        
    }
    public GameObject mainMenu;
    public GameObject optionsMenu;
    //public void PlayButton()
    //{
    //    SceneManager.LoadScene(1);
    //}

    /*public void LevelSelect() //One function for all levels, less code more flexibility. But idk how
    {
       
    }*/
    
    public void Level1()
    {
        SceneManager.LoadScene("Level 1");
    }
    public void Level2()
    {
        SceneManager.LoadScene("Level 2");
    }
    public void Level3()
    {
        SceneManager.LoadScene("Level 3");
    }
    public void Level4()
    {
        SceneManager.LoadScene("Level 4");
    }
    public void Level5()
    {
        SceneManager.LoadScene("Level 5");
    }
    public void Level6()
    {
        SceneManager.LoadScene("Level 6");
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void MainMenuOptions()
    {
        float time = 2f;
        time -= Time.deltaTime;
        if (time > 0)
        {
            SceneManager.LoadScene("MainMenu");

            Debug.Log("MainMenuOptions - Loaded Scene - Time:" /*+ time*/);

        }
        else
        {
            mainMenu.SetActive(false);
            optionsMenu.SetActive(true);
            time = .5f;
            Debug.Log("MainMenuOptions - SetActive Statements");
        }
        Debug.Log(time);
    }
    public void QuitButton()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }
}
