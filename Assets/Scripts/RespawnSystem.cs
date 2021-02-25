using UnityEngine;

public class RespawnSystem : MonoBehaviour
{
    //private Checkpoint[] resapwnPoints;
    private Vector3 currentRespawnPoint;
    [Tooltip("The object to respawn when called")]
    [SerializeField] GameObject targetObject = null;

    public event OnPlayerRespawnDelegate OnPlayerRespawn; //Create an event for when the player dies
    public delegate void OnPlayerRespawnDelegate(Vector3 respawnPos); //Delegate to pass on respawn information

    public event OnActivateNewCheckpointDelegate HandleNewCheckpoint; //Create an event for when the player activates a new checkpoint
    public delegate void OnActivateNewCheckpointDelegate(GameObject[] guns); //Delegate to pass on respawn information

    public event OnActivateOldCheckpointDelegate HandleOldCheckpoint; //Create an event for when the player enters an old checkpoint
    public delegate void OnActivateOldCheckpointDelegate(); //Delegate to pass on respawn information

    void Start()
    {
        //resapwnPoints = FindObjectsOfType<Checkpoint>();
        currentRespawnPoint = targetObject.transform.position;
        Debug.Log("Set spawn at: " + currentRespawnPoint);
    }

    //Respawns the object at the base position
    public void Respawn()
    {
        Debug.Log("Respawning Player at: " + currentRespawnPoint);
        OnPlayerRespawn?.Invoke(currentRespawnPoint); //Send out event to all subscribers
    }

    public void SetNewSpawnpoint(Vector3 newRespawnPos, GameObject[] newGuns)
    {
        currentRespawnPoint = newRespawnPos;
        HandleNewCheckpoint?.Invoke(newGuns);
        Debug.Log("Set spawn at: " + currentRespawnPoint);
    }

    public void ResetAmmo()
    {
        HandleOldCheckpoint?.Invoke();
    }
}
