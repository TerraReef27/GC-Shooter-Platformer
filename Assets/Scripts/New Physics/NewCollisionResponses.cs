using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCollisionResponses : MonoBehaviour
{
    private RespawnSystem respawn = null; //Reference to the respawn system in the scene
    private Collider2D playerCollider = null;

    [Tooltip("Which collisions to ignore")]
    [SerializeField] private List<LayerMask> ignoreList = null;

    [Tooltip("Which collisions to kill object")]
    [SerializeField] private List<LayerMask> killList = null;

    void Awake()
    {
        if (respawn == null) respawn = FindObjectOfType<RespawnSystem>();
        playerCollider = GetComponent<Collider2D>();
    }

    public void CheckReaction(Collider2D collision)
    {
        int layer = collision.gameObject.layer;
        for (int i = 0; i < killList.Count; i++)
        {
            if (MaskContains(killList[i], layer))
            {
                Debug.Log("Die");
            }
        }
    }
    
    private bool MaskContains(LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer)); //Check the bits of the layermask to see if the collision it true or false on that layer
    }
}
