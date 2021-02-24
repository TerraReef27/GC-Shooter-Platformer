using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerGunController : MonoBehaviour
{
    #region Variables

    private Camera cam;
    private Vector3 mousePos;

    private GunHolder holder;
    private GameObject gunObject = null;
    private Gun gun = null;

    private RespawnSystem respawn = null; //Reference to the respawn system in the scene
    private PlayerMover playerMove = null; //Reference to the playerMovement script

    private float angle = 0f;

    private SpriteRenderer sprite;
    private bool isFlipped = false;

    public event OnWeaponFireDelegate OnWeaponFire; //Create an event for when the player dies
    public delegate void OnWeaponFireDelegate(); //Delegate to pass on respawn information
    #endregion Variables

    //public GameObject rightArmSolverTarget;

    void Awake()
    {
        if (cam == null) cam = FindObjectOfType<Camera>();
        if (respawn == null) respawn = FindObjectOfType<RespawnSystem>();
        playerMove = GetComponent<PlayerMover>();
        sprite = GetComponent<SpriteRenderer>();
        holder = FindObjectOfType<GunHolder>();

        //respawn.OnPlayerRespawn += Respawn_OnPlayerRespawn;
        holder.OnWeaponSwitch += Holder_OnWeaponChange;
    }

    void Start()
    {
        if (holder.activeGun != null)
            gunObject = holder.activeGun;
    }

    private void Holder_OnWeaponChange(int gunNumber, GameObject gunObj)
    {
        gunObject = gunObj;
        gun = gunObj.GetComponent<Gun>();
    }

    /*
    private void Respawn_OnPlayerRespawn(Vector3 respawnPos) //Called when player dies
    {
        gun.RefillAmmo();
    }
    */

    void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition); //Gets the mouse position relative to world space

        HandleFiring();
        RotateSprites();


        if (Input.GetKeyDown(KeyCode.F)) //For respawning. I didn't know where else to put this but this will do for now
        {
            respawn.Respawn();
            Debug.Log("Respawning");
        }
    }

    private void HandleFiring()
    {
        if (!PauseMenu.isGamePaused)
        {


            if (Input.GetButtonDown("Fire1") && gun != null && gun.CurrentAmmo > 0) //Gets the mouse postion and adds a force to the player in the opposite direction of the mouse relative to the player. Amount of force is determined by the recoilForce
            {
                //Debug.DrawLine(transform.position, mousePos, Color.red, 1f);
                Vector2 forceTo = new Vector2(transform.position.x - mousePos.x, transform.position.y - mousePos.y);
                playerMove.AddForce(forceTo.normalized * gun.RecoilForce);
                //gun.Shoot(Vector2.Angle(mousePos.normalized, transform.position.normalized));
                //Vector2 shotAngle = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);
                Vector3 facing = mousePos - transform.position;
                angle = Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg;
                gun.Shoot(-forceTo);

                OnWeaponFire?.Invoke();
            }
        }
    }

    private void RotateSprites() //Rotate the player and gun sprites according to the mouse position
    {
        Vector3 facing = GetCurrentMousePosition() - transform.position;

        angle = Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg;

        if (angle > 90)
        {
            sprite.flipX = true;
            gun.Sprite.flipX = true;
            angle += 180;
            if (!isFlipped)
            {
                gun.InvertFirePoint();
                isFlipped = true;
            }
        }
        else if (angle < -90)
        {
            sprite.flipX = true;
            gun.Sprite.flipX = true;
            angle -= 180;
            if (!isFlipped)
            {
                gun.InvertFirePoint();
                isFlipped = true;
            }
        }
        else
        {
            sprite.flipX = false;
            gun.Sprite.flipX = false;
            if (isFlipped)
            {
                gun.InvertFirePoint();
                isFlipped = false;
            }
        }
        gun.transform.rotation = Quaternion.Euler(0, 0, angle);
        //gun.transform.position = rightArmSolverTarget.transform.position; //Clay
    }

    //Function for translating the mouse position into a 2d vector
    private Vector3 GetCurrentMousePosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var plane = new Plane(Vector3.forward, Vector3.zero);

        float rayDistance;
        plane.Raycast(ray, out rayDistance);

        return ray.GetPoint(rayDistance);
    }
}
