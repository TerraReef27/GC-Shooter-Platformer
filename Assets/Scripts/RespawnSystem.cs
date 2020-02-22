using UnityEngine;

public class RespawnSystem : MonoBehaviour
{
    private Vector3 baseRespawnPoint;
    [Tooltip("The object to respawn when called")]
    [SerializeField] GameObject targetObject;

    void Start()
    {
        baseRespawnPoint = targetObject.transform.position;
    }

    //Respawns the object at the base position
    public void Respawn()
    {
        targetObject.transform.position = baseRespawnPoint;
    }
    //Respawns the object at the designated position
    public void Respawn(Vector3 respawnPosition)
    {
        targetObject.transform.position = respawnPosition;
    }
}
