using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneTransitioner
{
    private static AssetBundle levelAssets = AssetBundle.LoadFromFile("Assets/Scenes/Levels");
    private static string[] levels = levelAssets.GetAllScenePaths();

    public static void LoadLevel(int levelNum)
    {
        SceneManager.LoadScene(levels[levelNum]);
    }
    
    public static int GetNumLevels()
    {
        return levels.Length;
    }
}
