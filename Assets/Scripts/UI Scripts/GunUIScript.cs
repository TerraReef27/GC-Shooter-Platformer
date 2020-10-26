using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunUIScript : MonoBehaviour
{
    private GunUIObject[] gunUIObjects = null;
    int currentSelection = 0;
    [SerializeField] private GameObject gunUIObjectTemplate = null;

    private GunHolder gunHolder = null;
    private RespawnSystem respawn = null;
    
    void Awake()
    {
        respawn = FindObjectOfType<RespawnSystem>();
        gunHolder = FindObjectOfType<GunHolder>();

        gunHolder.OnWeaponSwitch += Controller_OnWeaponSwitch;
        respawn.HandleNewCheckpoint += Respawn_OnPlayerSetSpawn;
    }

    void Start()
    {
        SetGunUI();
        
        currentSelection = 0;
    }

    private void Controller_OnWeaponSwitch(int gunNum, GameObject gun)
    {
        gunUIObjects[currentSelection].SetDeselected();
        currentSelection = gunNum;
        gunUIObjects[currentSelection].SetSelected();
    }

    private void Respawn_OnPlayerSetSpawn(GameObject[] guns)
    {
        for (int i = 0; i < gunUIObjects.Length; i++)
        {
            Destroy(gunUIObjects[i]);
        }

        SetGunUI();
    }

    private void SetGunUI()
    {
        gunUIObjects = new GunUIObject[gunHolder.GetGuns().Length];

        for(int i=0; i<gunUIObjects.Length; i++)
        {
            GameObject gunUIObject = Instantiate(gunUIObjectTemplate, this.gameObject.transform, false);
            gunUIObjects[i] = gunUIObject.GetComponent<GunUIObject>();
            gunUIObjects[i].SetValues(gunHolder.GetGunObjects()[i]);
        }
    }
}
