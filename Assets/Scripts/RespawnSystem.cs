using System;
using UnityEngine;

public class RespawnSystem : MonoBehaviour
{
    private Vector3 baseRespawnPoint;
    [Tooltip("The object to respawn when called")]
    [SerializeField] GameObject targetObject;
    
    public event OnPlayerRespawnDelegate OnPlayerRespawn; //Create an event for when the player dies
    public delegate void OnPlayerRespawnDelegate(Vector3 respawnPos); //Delegate to pass on respawn information

    void Start()
    {
        baseRespawnPoint = targetObject.transform.position;
    }

    //Respawns the object at the base position
    public void Respawn()
    {
        OnPlayerRespawn?.Invoke(baseRespawnPoint); //Send out event to all subscribers
    }
}
