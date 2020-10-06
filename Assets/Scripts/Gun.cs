using UnityEngine;

public class Gun : MonoBehaviour
{
    [Tooltip("Amount of ammo the player starts with")]
    [SerializeField] private int startAmmo = 1;

    private int currentAmmo = 1; //The amount ofammo that the gun currently has
    public int CurrentAmmo { get { return currentAmmo; } set { value = currentAmmo; } }

    [Tooltip("How much the gun will push the holder back when fired")]
    [SerializeField] private float recoilForce = 100f;
    public float RecoilForce { get { return recoilForce; } private set { recoilForce = value; } }

    [Tooltip("The name of the gun")]
    [SerializeField] private string gunName = "gun";

    [SerializeField] private GameObject bullet = null;
    [SerializeField] private float projectileSpeed = 3f;

    [SerializeField] private GameObject bulletCollecter = null;

    private bool isInverted = false;
    [SerializeField] private Transform firePoint = null;

    private SpriteRenderer sprite;
    public SpriteRenderer Sprite { get { return sprite; } private set { sprite = value; } }
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        if(bulletCollecter == null)
            bulletCollecter = GameObject.FindGameObjectWithTag("Collecter");
    }

    void Start()
    {
        currentAmmo = startAmmo;
    }

    public void Shoot(Vector2 shootDir) //Should be called when the object fires the gun. Handles ammo and effects
    {
        GameObject bull = Instantiate(bullet, firePoint.position, Quaternion.Euler(0, 0, 0), bulletCollecter.transform);
        bull.GetComponent<Projectile>().Shot(projectileSpeed, shootDir.normalized);
        currentAmmo--;
    }
    
    public void RefillAmmo()
    {
        currentAmmo = startAmmo;
    }

    public void InvertFirePoint()
    {
        firePoint.localPosition = new Vector2(-firePoint.localPosition.x, firePoint.localPosition.y);
    }
}
