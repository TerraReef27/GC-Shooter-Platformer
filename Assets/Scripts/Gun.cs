﻿using UnityEngine;

public class Gun : MonoBehaviour
{
    [Tooltip("The amount of ammo that the gun starts with")]
    [SerializeField] private int maxAmmo = 1;
    private int currentAmmo = 1; //The amount ofammo that the gun currently has
    public int CurrentAmmo { get { return currentAmmo; } set { value = currentAmmo; } }

    [Tooltip("The amount of ammo the gun can store without having to reload")]
    [SerializeField] private int clipSize = 1;
    private int currentClip = 1; //The amount of bullets currently in the clip
    public int CurrentClip { get { return currentClip; } private set { value = currentClip; } }

    [Tooltip("How much the gun will push the holder back when fired")]
    [SerializeField] private float recoilForce = 100f;
    public float RecoilForce { get { return recoilForce; } private set { recoilForce = value; } }

    [Tooltip("The name of the gun")]
    [SerializeField] private string gunName = "gun";

    void Start()
    {
        currentAmmo = maxAmmo;
        currentClip = clipSize;
    }

    public void Shoot() //Should be called when the object fires the gun. Handles ammo and effects
    {
        currentAmmo--;
        currentClip--;
    }

    public void Reload() //Reloads the current clip
    {
        if (currentAmmo >= clipSize)
            currentClip = clipSize;
        else
            currentClip = currentAmmo;
    }

    public void RefillAmmo()
    {
        currentAmmo = maxAmmo;
        currentClip = clipSize;
    }
}