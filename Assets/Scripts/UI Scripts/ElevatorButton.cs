using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ElevatorButton : MonoBehaviour
{
    private Button button = null;
    
    private int buttonNumber;

    void Awake()
    {
        button = FindObjectOfType<Button>();
    }

    public void SetNumber(int num)
    {
        buttonNumber = num;
        GetComponent<TextMeshPro>().text = buttonNumber.ToString();
    }

    private void ChageToScene()
    {
        SceneTransitioner.LoadLevel(buttonNumber); 
    }
}
