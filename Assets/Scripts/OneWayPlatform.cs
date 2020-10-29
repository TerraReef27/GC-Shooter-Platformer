using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    public bool isAbove;
    public bool isBelow;
    bool isGrounded;
    //public bool isGrounded;

    public GameObject groundCheck;
    public GameObject platform;
    GameObject player;
    private float time = 2f;
    private void Start()
    {
        platform.tag = "Untagged";
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S) && isGrounded)
        {
            platform.tag = "OneWayPlatform";
        }
        if (platform.tag.Equals("OneWayPlatform") && time > 0)
        {
            time -= Time.fixedDeltaTime;
        }
        else
        {
            platform.tag = "Untagged";
            time = 2f;
        }
        //Debug.Log(time);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(isAbove)
        {
            platform.tag = "Untagged";
        }
        if(isBelow)
        {
            platform.tag = "OneWayPlatform";
        }
        if(groundCheck)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

}
