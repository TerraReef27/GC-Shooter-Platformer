using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoUIScript : MonoBehaviour
{
    private CharacterGunController gunController = null;
    private GunHolder gunHolder = null;
    private RespawnSystem respawn = null;

    //private AmmoUIObject[] ammoUIObjects;
    private GameObject[] ammoUIObjects;
    [SerializeField] GameObject ammoUITemplate = null;
    private int currentActiveAmmoUIObject = 0;

    void Awake()
    {
        gunController = FindObjectOfType<CharacterGunController>();
        gunHolder = FindObjectOfType<GunHolder>();
        respawn = FindObjectOfType<RespawnSystem>();

        gunController.OnWeaponFire += Controller_OnWeaponFire;
        gunHolder.OnWeaponSwitch += Controller_OnWeaponSwitch;
        respawn.OnPlayerRespawn += Respawn_OnPlayerRespawn;
    }

    void Start()
    {
        PopulateAmmoArray(gunHolder.GetGuns());
        if (ammoUIObjects.Length > 0)
            SetActiveAmmo(0);
    }

    private void PopulateAmmoArray(Gun[] guns)
    {
        ammoUIObjects = new GameObject[guns.Length];

        for (int i = 0; i < ammoUIObjects.Length; i++)
        {
            ammoUIObjects[i] = Instantiate(ammoUITemplate, this.gameObject.transform);
            ammoUIObjects[i].GetComponent<AmmoUIObject>().SetValues(guns[i].StartAmmo, guns[i].GunName);
        }
    }

    private void SetActiveAmmo(int activate)
    {
        if (ammoUIObjects != null && activate < ammoUIObjects.Length)
        {
            ammoUIObjects[currentActiveAmmoUIObject].SetActive(false);
            ammoUIObjects[activate].SetActive(true);

            currentActiveAmmoUIObject = activate;
        }
    }
    
    private void Controller_OnWeaponSwitch(int gunNum, GameObject gunObject)
    {
        SetActiveAmmo(gunNum);
    }

    private void Controller_OnWeaponFire()
    {
        ammoUIObjects[currentActiveAmmoUIObject].GetComponent<AmmoUIObject>().UseBullet();
    }

    private void Respawn_OnPlayerRespawn(Vector3 respawnPoint)
    {
        foreach (GameObject ammoObject in ammoUIObjects)
        {
            ammoObject.GetComponent<AmmoUIObject>().ResetAmmo();
        }
    }
}
