using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoUIScript : MonoBehaviour
{
    private CharacterGunController gunController = null;
    private GunHolder gunHolder = null;
    private RespawnSystem respawn = null;

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
        respawn.HandleNewCheckpoint += Respawn_OnPlayerSetSpawn;
        respawn.HandleOldCheckpoint += Respawn_OnResetAmmo;
    }

    void Start()
    {
        PopulateAmmoArray(gunHolder.GetGuns());
        if (ammoUIObjects.Length > 0)
            SetActiveAmmo(0);
    }

    //Fill the array of AmmoObjects with the repsective guns the player is holding
    private void PopulateAmmoArray(Gun[] guns)
    {
        ammoUIObjects = new GameObject[guns.Length];

        for (int i = 0; i < ammoUIObjects.Length; i++)
        {
            ammoUIObjects[i] = Instantiate(ammoUITemplate, this.gameObject.transform);
            ammoUIObjects[i].GetComponent<AmmoUIObject>().SetValues(guns[i].StartAmmo, guns[i].GunName);
        }
    }

    //Set which gun's ammo is currently being displayed
    private void SetActiveAmmo(int activate)
    {
        if (ammoUIObjects != null && activate < ammoUIObjects.Length)
        {
            ammoUIObjects[currentActiveAmmoUIObject].SetActive(false);
            ammoUIObjects[activate].SetActive(true);

            currentActiveAmmoUIObject = activate;
        }
    }

    //Receive the OnWeaponSwitch event and set the active ammo accordingly
    private void Controller_OnWeaponSwitch(int gunNum, GameObject gunObject)
    {
        SetActiveAmmo(gunNum);
    }

    //Receive the OnWeaponFire event and reduce set a bullet UI to be in the fired state
    private void Controller_OnWeaponFire()
    {
        ammoUIObjects[currentActiveAmmoUIObject].GetComponent<AmmoUIObject>().UseBullet();
    }

    //Receive the OnPlayerRespawnEvent and reset the UI
    private void Respawn_OnPlayerRespawn(Vector3 respawnPoint)
    {
        ResetAmmo();
    }

    //Receive the OnResetAmmo and reset the UI
    private void Respawn_OnResetAmmo()
    {
        ResetAmmo();
    }

    //Receive the OnPlayerSetSpawn event and update the UI to the new guns
    private void Respawn_OnPlayerSetSpawn(GameObject[] guns)
    {
        for(int i=0; i<ammoUIObjects.Length; i++)
        {
            Destroy(ammoUIObjects[i]);
        }

        Gun[] gunComponenets = new Gun[guns.Length];
        for(int i=0; i<guns.Length; i++)
        {
            gunComponenets[i] = guns[i].GetComponent<Gun>();
        }

        PopulateAmmoArray(gunComponenets);
        ResetAmmo();
        SetActiveAmmo(0);
    }

    //Reset the bullet UI for all of the guns
    private void ResetAmmo()
    {
        foreach (GameObject ammoObject in ammoUIObjects)
        {
            ammoObject.GetComponent<AmmoUIObject>().ResetAmmo();
        }
    }
}
