using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderController : MonoBehaviour
{
    private BoxCollider2D activeCollider = null;
    public BoxCollider2D ActiveCollider { get { return activeCollider; } private set { activeCollider = ActiveCollider; } }

    [SerializeField] BoxCollider2D normalCollider = null;
    [SerializeField] BoxCollider2D slideCollider = null;

    void Awake()
    {
        activeCollider = normalCollider;
        normalCollider.gameObject.SetActive(true);
        slideCollider.gameObject.SetActive(false);
    }

    public void SwitchActive()
    {
        if(activeCollider == normalCollider)
        {
            activeCollider = slideCollider;
            normalCollider.gameObject.SetActive(false);
            slideCollider.gameObject.SetActive(true);
        }
        else
        {
            activeCollider = normalCollider;
            normalCollider.gameObject.SetActive(true);
            slideCollider.gameObject.SetActive(false);
        }
    }
}
