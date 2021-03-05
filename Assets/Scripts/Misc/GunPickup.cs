using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    RespawnSystem respawner = null;
    GameObject[] guns = new GameObject[1];

    void Awake()
    {
        respawner = FindObjectOfType<RespawnSystem>();
    }

    private void Start()
    {
        foreach(Transform child in this.transform)
        {
            if (child.gameObject.GetComponent<Gun>())
            {
                guns[0] = child.gameObject;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            ActivateCheckpoint(guns);
            Destroy(this.gameObject);
        }
    }

    private void ActivateCheckpoint(GameObject[] gun)
    {
        respawner.SetNewSpawnpoint(this.transform.position + new Vector3(0, 0), gun);
    }
}
