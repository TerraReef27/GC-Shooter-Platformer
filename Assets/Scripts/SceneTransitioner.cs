using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneTransitioner
{
    private static int levelStartIndex = 2;
    public static int LevelStartIndex { get { return levelStartIndex; } }

    private static int numberOfLevels = 6;
    public static int NumberOfLevels { get { return numberOfLevels; } }

    //private static AssetBundle levelAssets = AssetBundle.LoadFromFile("Assets/Scenes/Levels");
    //private static string[] levels = levelAssets.GetAllScenePaths();
    

    public static void LoadLevel(int levelNum)
    {
        if (levelNum <= numberOfLevels)
            SceneManager.LoadScene("Level " + levelNum);
    }
}
