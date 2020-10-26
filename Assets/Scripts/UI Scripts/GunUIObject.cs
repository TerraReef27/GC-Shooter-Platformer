using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunUIObject : MonoBehaviour
{
    [SerializeField] private Image pannel = null;
    [SerializeField] private Image gunSprite = null;

    [SerializeField] private Color regularPannelColor = Color.white;
    [SerializeField] private Color selectedPannelColor = Color.black;

    private GameObject gun;
    private bool isSelected;
    
    public void SetSelected()
    {
        isSelected = true;
        pannel.color = selectedPannelColor;
    }
    public void SetDeselected()
    {
        isSelected = false;
        pannel.color = selectedPannelColor;
    }

    public void SetValues(GameObject gunObject)
    {
        gun = gunObject;
        gunSprite.sprite = gun.GetComponent<SpriteRenderer>().sprite;
    }
}
