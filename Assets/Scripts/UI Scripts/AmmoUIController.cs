using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    private CharacterGunController gunController = null;
    private GunHolder gunHolder = null;

    private AmmoUIObject[] ammoUIObjects;

    void Awake()
    {
        gunController = FindObjectOfType<CharacterGunController>();
        gunHolder = FindObjectOfType<GunHolder>();

        gunController.OnWeaponFire += Controller_OnWeaponFire;
        gunHolder.OnWeaponSwitch += Controller_OnWeaponSwitch;
    }

    void Start()
    {
        PopulateAmmoArray(gunHolder.GetGuns());
    }

    private void PopulateAmmoArray(Gun[] guns)
    {
        ammoUIObjects = new AmmoUIObject[guns.Length];

        for(int i=0; i<ammoUIObjects.Length; i++)
        {
            ammoUIObjects[i] = new AmmoUIObject(guns[i].StartAmmo, guns[i].GunName);
        }
    }

    private void Controller_OnWeaponSwitch(int gunNum, GameObject gunObject)
    {

    }

    private void Controller_OnWeaponFire()
    {

    }
}
