using UnityEngine;
using UnityEngine.UI;

public class AmmoUIObject : MonoBehaviour
{
    Image[] ammoArray;
    string gunName;
    GridLayoutGroup grid;

    void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
    }

    public AmmoUIObject(int ammoAmount, string newGunName)
    {
        ammoArray = new Image[ammoAmount];
        gunName = newGunName;
        PopulateUI();
    }

    public void PopulateUI()
    {
        foreach(Image image in ammoArray)
        {
            Instantiate(image, this.gameObject.transform);
        }
    }
}
