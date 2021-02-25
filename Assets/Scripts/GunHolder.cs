using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHolder : MonoBehaviour
{
    #region Variables
    private RespawnSystem respawn = null;

    [SerializeField] private List<GameObject> guns = new List<GameObject>();
    public GameObject activeGun;

    private KeyCode[] numKeys =
    {
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6,
         KeyCode.Alpha7,
         KeyCode.Alpha8,
         KeyCode.Alpha9,
         KeyCode.Alpha0
    };

    public event OnWeaponSwitchDelegate OnWeaponSwitch; //Create an event for when the gun is switched
    public delegate void OnWeaponSwitchDelegate(int gunNum, GameObject gunObject); //Delegate to pass on gun switching information
    public event OnGunsUpdatedDelegate OnGunsUpdated; //Create an event for when the player dies
    public delegate void OnGunsUpdatedDelegate(int numOfGuns); //Delegate to pass on respawn information

    #endregion Variables

    void Awake()
    {
        respawn = FindObjectOfType<RespawnSystem>();
        respawn.OnPlayerRespawn += Respawn_OnPlayerRespawn;
        respawn.HandleNewCheckpoint += Respawn_OnPlayerSetSpawn;
        respawn.HandleOldCheckpoint += Respawn_OnPlayerResetAmmo;
    }

    void Start()
    {
        GetGunsInChild();
        if (guns != null)
        {
            foreach(GameObject gun in guns)
            {
                gun.SetActive(false);
            }

            SwitchGuns(0);
        }
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        for (int i = 0; i < numKeys.Length; i++)
        {
            if (Input.GetKeyDown(numKeys[i]))
            {
                SwitchGuns(i);
            }
        }
    }

    private void SwitchGuns(int newGun)
    {
        if (guns.Count > newGun && guns[newGun] != null)
        {
            if(activeGun)
                activeGun.SetActive(false);

            activeGun = guns[newGun];
            activeGun.SetActive(true);
            
            OnWeaponSwitch?.Invoke(newGun, activeGun);

            Debug.Log("Switching to gun: " + newGun);
        }
    }

    private void GetGunsInChild()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<Gun>())
                guns.Add(child.gameObject);
        }
    }

    public Gun[] GetGuns()
    {
        Gun[] gunArray = new Gun[guns.Count];
        for(int i=0; i<gunArray.Length; i++)
        {
            gunArray[i] = guns[i].GetComponent<Gun>();
        }
        return gunArray;
    }
    
    public GameObject[] GetGunObjects()
    {
        return guns.ToArray();
    }

    private void LoadNewGuns(GameObject[] newGuns)
    {
        if (guns != null)
        {
            foreach (GameObject gun in guns)
            {
                Destroy(gun);
            }
        }

        if (newGuns != null)
        {
            GameObject[] checkpointGuns = new GameObject[newGuns.Length];

            for (int i = 0; i < checkpointGuns.Length; i++)
            {
                checkpointGuns[i] = Instantiate(newGuns[i], this.transform, false);
            }

            guns = new List<GameObject>(checkpointGuns);
            SwitchGuns(0);

            OnGunsUpdated?.Invoke(guns.Count);
        }
    }

    private void ReloadGuns()
    {
        foreach (GameObject gun in guns)
        {
            gun.GetComponent<Gun>().RefillAmmo();
        }
    }

    private void Respawn_OnPlayerRespawn(Vector3 respawnPos) //Called when player dies
    {
        ReloadGuns();
    }

    private void Respawn_OnPlayerSetSpawn(GameObject[] newGuns)
    {
        LoadNewGuns(newGuns);
    }

    private void Respawn_OnPlayerResetAmmo()
    {
        ReloadGuns();
    }
}
