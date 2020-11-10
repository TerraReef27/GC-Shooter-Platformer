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
        button.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        Debug.Log("Going to level " + buttonNumber);
        ChageToScene();
    }

    public void SetNumber(int num)
    {
        buttonNumber = num;
        GetComponentInChildren<TextMeshProUGUI>().text = buttonNumber.ToString();
    }

    private void ChageToScene()
    {
        SceneTransitioner.LoadLevel(buttonNumber); 
    }
}
