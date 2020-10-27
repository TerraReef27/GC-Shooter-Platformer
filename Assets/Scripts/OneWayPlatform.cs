using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    public bool isAbove;

    GameObject physics;
    GameObject player;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            transform.parent.GetComponent<Collider2D>().enabled = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            transform.parent.GetComponent<Collider2D>().enabled = isAbove;
        }
    }

}
