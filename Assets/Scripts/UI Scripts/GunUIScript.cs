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
    
    void Awake()
    {
        gunHolder = FindObjectOfType<GunHolder>();

        gunHolder.OnWeaponSwitch += Controller_OnWeaponSwitch;
        gunHolder.OnGunsUpdated += Holder_OnGunsUpdate;
    }

    private void Controller_OnWeaponSwitch(int gunNum, GameObject gun)
    {
        if (gunUIObjects != null)
        {
            gunUIObjects[currentSelection].SetDeselected();
            currentSelection = gunNum;
            gunUIObjects[currentSelection].SetSelected();
        }
    }

    private void Holder_OnGunsUpdate(int numGuns)
    {
        if (gunUIObjects != null)
        {
            for (int i = 0; i < gunUIObjects.Length; i++)
            {
                Destroy(gunUIObjects[i].gameObject);
            }
        }

        SetGunUI();
    }

    private void SetGunUI()
    {
        gunUIObjects = new GunUIObject[gunHolder.GetGunObjects().Length];

        for(int i=0; i<gunUIObjects.Length; i++)
        {
            GameObject gunUIObject = Instantiate(gunUIObjectTemplate, this.gameObject.transform, false);
            gunUIObjects[i] = gunUIObject.GetComponent<GunUIObject>();
            gunUIObjects[i].SetValues(gunHolder.GetGunObjects()[i]);
        }

        currentSelection = 0;
        gunUIObjects[currentSelection].SetSelected();
    }
}
