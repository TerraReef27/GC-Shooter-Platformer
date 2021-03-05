using UnityEngine;

public class LevelChanger : MonoBehaviour
{
    [SerializeField] string loadLevelName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            SceneTransitioner.LoadLevel(loadLevelName);
        }
    }
}
