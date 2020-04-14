using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGunController : MonoBehaviour
{
    private Camera cam;
    private Vector3 mousePos;

    [Tooltip("The gun the object is to use")]
    [SerializeField] private Gun gun = null;

    private RespawnSystem respawn = null; //Reference to the respawn system in the scene
    private PlayerController playerMove = null; //Reference to the playerMovement script

    private float angle = 0f;

    void Awake()
    {
        if (cam == null) cam = FindObjectOfType<Camera>();
        if (respawn == null) respawn = FindObjectOfType<RespawnSystem>();
        respawn.OnPlayerRespawn += Respawn_OnPlayerRespawn;
        playerMove = GetComponent<PlayerController>();
    }

    private void Respawn_OnPlayerRespawn(Vector3 respawnPos) //Called when player dies
    {
        gun.RefillAmmo();
    }

    void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition); //Gets the mouse position relative to world space

        if (Input.GetButtonDown("Fire1") && gun != null && gun.CurrentClip > 0) //Gets the mouse postion and adds a force to the player in the opposite direction of the mouse relative to the player. Amount of force is determined by the recoilForce
        {
            Debug.DrawLine(transform.position, mousePos, Color.red, 1f);
            Vector2 forceTo = new Vector2(transform.position.x - mousePos.x, transform.position.y - mousePos.y);
            playerMove.ChangeMoveDirection(forceTo.normalized * gun.RecoilForce);
            gun.Shoot(Vector2.SignedAngle(mousePos, transform.position) + 90);
        }

        if (Input.GetKeyDown(KeyCode.R) && playerMove.Grounded) //For reloading gun
        {
            gun.Reload();
        }
        /*
        Vector3 facing = mousePos - transform.position;
        angle = Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg - 90f;
        gun.transform.rotation = Quaternion.Euler(0, 0, angle);
        */
    }
}
