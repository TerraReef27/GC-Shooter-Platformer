using UnityEngine;
using UnityEngine.UI;

public class AmmoUIObject : MonoBehaviour
{
    [SerializeField] private GameObject ammoTemplate = null;
    private GameObject[] ammoArray;
    private string gunName;
    private GridLayoutGroup grid;
    
    private int maxAmmo;
    private int ammoLeft;

    void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
    }

    public void PopulateUI()
    {
        for(int i=0; i<ammoArray.Length; i++)
        {
            ammoArray[i] = Instantiate(ammoTemplate, this.gameObject.transform);
        }
    }

    public void SetValues(int ammoAmount, string newGunName)
    {
        ammoArray = new GameObject[ammoAmount];
        maxAmmo = ammoArray.Length-1;
        ammoLeft = maxAmmo;
        gunName = newGunName;
        PopulateUI();
    }

    public void UseBullet()
    {
        ammoArray[ammoLeft].GetComponent<Image>().color = new Color(1f, 1f, 1f, .2f);

        if (ammoLeft > 0)
            ammoLeft--;
        else
            Debug.Log("UI Ammo Attempts to go under 0");
    }

    public void ResetAmmo()
    {
        foreach(GameObject ammo in ammoArray)
        {
            ammo.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            ammoLeft = maxAmmo;
        }
    }
}
