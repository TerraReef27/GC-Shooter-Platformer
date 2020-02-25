using UnityEngine;

public class PlayerGunController : MonoBehaviour
{
    [SerializeField] Camera cam;
    private Vector2 mousePos;
    private Rigidbody2D rb;

    [Tooltip("The gun the object is to use")]
    [SerializeField] private Gun gun;

    private RespawnSystem respawn = null; //Reference to the respawn system in the scene

    void Awake()
    {
        if (cam == null) cam = FindObjectOfType<Camera>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if(respawn == null) respawn = FindObjectOfType<RespawnSystem>();
        respawn.OnPlayerRespawn += Respawn_OnPlayerRespawn;
    }

    private void Respawn_OnPlayerRespawn(Vector3 respawnPos) //Called when player dies
    {
        gun.RefillAmmo();
    }

    void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition); //Gets the mouse position relative to world space

        if(Input.GetButtonDown("Fire1") && gun != null && gun.CurrentClip > 0) //Gets the mouse postion and adds a force to the player in the opposite direction of the mouse relative to the player. Amount of force is determined by the recoilForce
        {
            Debug.DrawLine(transform.position, mousePos, Color.red, 1f);
            Vector2 forceTo = new Vector2(transform.position.x - mousePos.x, transform.position.y - mousePos.y);
            rb.AddRelativeForce(forceTo.normalized * gun.RecoilForce);
            gun.Shoot();
        }

        if(Input.GetKeyDown(KeyCode.R)) //For reloading gun
        {
            gun.Reload();
        }
    }
}
