using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject buttonTemplate = null;

    private ElevatorButton[] buttons;

    void Start()
    {
        buttons = new ElevatorButton[SceneTransitioner.NumberOfLevels];
        for(int i=0; i<buttons.Length; i++) //Create a button for each level and assign them to the array
        {
            buttons[i] = Instantiate(buttonTemplate, this.transform, false).GetComponent<ElevatorButton>();
            buttons[i].SetNumber(i+1);
        }
    }
}
