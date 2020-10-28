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

    //Populate the UI with the respective bullets for each gun
    public void PopulateUI()
    {
        for(int i=0; i<ammoArray.Length; i++)
        {
            ammoArray[i] = Instantiate(ammoTemplate, this.gameObject.transform);
        }
    }

    //Set the values for the ammo UI container
    public void SetValues(int ammoAmount, string newGunName)
    {
        ammoArray = new GameObject[ammoAmount];
        maxAmmo = ammoArray.Length-1;
        ammoLeft = maxAmmo;
        gunName = newGunName;
        PopulateUI();
    }

    //Change the visual of the current bullet to show a fired state
    public void UseBullet()
    {
        if (!PauseMenu.isGamePaused)
        {
            ammoArray[ammoLeft].GetComponent<Image>().color = new Color(1f, 1f, 1f, .2f);

            if (ammoLeft >= 0)
                ammoLeft--;
            else
                Debug.Log("UI Ammo Attempts to go under 0");
        }
    }

    //Reset the UI back to the full, unfired state
    public void ResetAmmo()
    {
        foreach(GameObject ammo in ammoArray)
        {
            ammo.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            ammoLeft = maxAmmo;
        }
    }
}
