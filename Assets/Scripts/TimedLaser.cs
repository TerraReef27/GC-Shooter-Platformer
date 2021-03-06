﻿using System.Collections;
using UnityEngine;

public class TimedLaser : MonoBehaviour
{
    [Tooltip("The amount of the time that the laser beam stays active and on")]
    [SerializeField] private float activeDuration = 3;
    [Tooltip("The amount of time that the laser beam is off before the next activation")]
    [SerializeField] private float pauseDuration = 20;

    [Tooltip("The physics for the obsticle. For things like colliders")]
    [SerializeField] private GameObject physics = null;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        StartCoroutine(disableBeam()); //When the scene starts, the beam is disabled
    }

    IEnumerator activateBeam() //Sets the components of the laser beam to be active and begins a countdown to call disableBeam
    {
        animator.SetBool("IsActive", true);
        physics.SetActive(true);
        yield return new WaitForSeconds(activeDuration);
        StartCoroutine(disableBeam());
    }
    IEnumerator disableBeam() //Sets the components of the laser beam to be disabled and begins a countdown to call activateBeam
    {
        animator.SetBool("IsActive", false);
        physics.SetActive(false);
        yield return new WaitForSeconds(pauseDuration);
        StartCoroutine(activateBeam());
    }

}
