using System;
using UnityEngine;

public class RespawnSystem : MonoBehaviour
{
    //private Checkpoint[] resapwnPoints;
    private Vector3 currentRespawnPoint;
    [Tooltip("The object to respawn when called")]
    [SerializeField] GameObject targetObject = null;

    public event OnPlayerRespawnDelegate OnPlayerRespawn; //Create an event for when the player dies
    public delegate void OnPlayerRespawnDelegate(Vector3 respawnPos); //Delegate to pass on respawn information

    public event OnActivateNewCheckpointDelegate HandleNewCheckpoint; //Create an event for when the player dies
    public delegate void OnActivateNewCheckpointDelegate(); //Delegate to pass on respawn information

    void Start()
    {
        //resapwnPoints = FindObjectsOfType<Checkpoint>();
        currentRespawnPoint = targetObject.transform.position;
    }

    //Respawns the object at the base position
    public void Respawn()
    {
        OnPlayerRespawn?.Invoke(currentRespawnPoint); //Send out event to all subscribers
    }

    public void SetNewSpawnpoint(Vector3 newRespawnPos)
    {
        currentRespawnPoint = newRespawnPos;
        HandleNewCheckpoint?.Invoke();
    }
}
