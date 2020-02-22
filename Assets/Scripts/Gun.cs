using UnityEngine;

public class Gun : MonoBehaviour
{
    [Tooltip("The amount of ammo that the gun starts with")]
    [SerializeField] private int maxAmmo = 1;
    private int currentAmmo = 1; //The amount ofammo that the gun currently has

    [Tooltip("How much the gun will push the holder back when fired")]
    [SerializeField] private float recoilForce = 100f;
    public float RecoilForce { get { return recoilForce; } private set { recoilForce = value; } }

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    public void Shoot() //Should be called when the object fires the gun. Handles ammo and effects
    {
        currentAmmo--;
    }
}
