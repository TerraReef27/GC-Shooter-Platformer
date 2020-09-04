using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionResponses : MonoBehaviour
{
    private RespawnSystem respawn = null; //Reference to the respawn system in the scene
    private Collider2D playerCollider = null;

    [Tooltip("Which collisions to ignore")]
    [SerializeField] private List<string> ignoreList = null;

    [Tooltip("Which collisions to kill object")]
    [SerializeField] private List<string> killList = null;

    void Awake()
    {
        if (respawn == null) respawn = FindObjectOfType<RespawnSystem>();
        playerCollider = GetComponent<Collider2D>();
    }

    //Checks if the object should ignore collisions. Returns true if the collision should be ignored
    public bool CheckIfIgnore(string collisionTag)
    {
        if (ignoreList.Contains(collisionTag))
        {
            return false;
        }

        return true;
    }
    
    //Checks collision to see if it should kill the object
    public bool CheckIfKill(string collisionTag)
    {
        if (killList.Contains(collisionTag))
        {
            return false;
        }

        return true;
    }
}
