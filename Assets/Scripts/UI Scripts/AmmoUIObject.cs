using UnityEngine;
using UnityEngine.UI;

public class AmmoUIObject : MonoBehaviour
{
    Image[] ammoArray;
    string gunName;

    public AmmoUIObject(int ammoAmount, string newGunName)
    {
        ammoArray = new Image[ammoAmount];
        gunName = newGunName;
    }

    public void PopulateUI()
    {
        foreach(Image image in ammoArray)
        {

        }
    }
}
