using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionResponses : MonoBehaviour
{
    [Tooltip("Which collisions to ignore")]
    [SerializeField] private List<string> ignoreList;

    //Checks if the object should ignore collisions. Returns true if the collision should be ignored
    public bool CheckIfIgnore(string collisionTag)
    {
        if (ignoreList.Contains(collisionTag))
            return false;

        return true;
    }
}
