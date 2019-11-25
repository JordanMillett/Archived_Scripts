using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dropdown : MonoBehaviour
{

    public int Index;
    public TextMeshProUGUI Current;
    public List<TextMeshProUGUI> Options;

    public GameObject OptionsTab;
    public GameObject Icon;

    bool Open = false;

    public void Change(int I)
    {

        Index = I;
        Current.text = Options[Index].text;

    }

    public void Toggle()
    {
        Open = !Open;
        OptionsTab.SetActive(Open);
        if(Open)
            Icon.GetComponent<RectTransform>().localEulerAngles = new Vector3(180f,0f,0f);
        else
            Icon.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;

    }
}
