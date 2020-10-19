using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    RespawnSystem respawner = null;
    [SerializeField] Collider2D trigger = null;
    private bool newTrigger = true;

    void Awake()
    {
        respawner = FindObjectOfType<RespawnSystem>();
        if (trigger == null)
            Debug.Log("Checkpoint trigger not set!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && newTrigger)
        {
            if(newTrigger)
                ActivateCheckpoint();
        }
    }

    private void ActivateCheckpoint()
    {
        newTrigger = false;
        respawner.SetNewSpawnpoint(this.transform.position);
    }
}
