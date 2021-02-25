﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private GameObject[] guns = null;
    RespawnSystem respawner = null;
    [SerializeField] Collider2D trigger = null;
    private bool newTrigger = true;
    private float ybuffer = .5f;

    void Awake()
    {
        respawner = FindObjectOfType<RespawnSystem>();
        if (trigger == null)
            Debug.Log("Checkpoint trigger not set!");
    }

    void Start()
    {
        int gunNumber = this.transform.childCount;
        if (gunNumber > 0)
        {
            guns = new GameObject[gunNumber];
            for (int i = 0; i < guns.Length; i++)
            {
                guns[i] = this.transform.GetChild(i).gameObject;
                guns[i].SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (newTrigger)
                ActivateCheckpoint(guns);
            else
                RefillAmmo();
        }
    }

    private void ActivateCheckpoint(GameObject[] guns)
    {
        newTrigger = false;
        respawner.SetNewSpawnpoint(this.transform.position + new Vector3(0, ybuffer), guns);
    }

    private void RefillAmmo()
    {
        respawner.ResetAmmo();
    }
}
