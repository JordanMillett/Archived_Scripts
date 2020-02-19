using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropDown : MonoBehaviour
{
    public TextMeshProUGUI Current;
    public GameObject List;
    TextMeshProUGUI[] listItems;

    public bool Open = false;

    void Start()
    {
        listItems = List.transform.GetComponentsInChildren<TextMeshProUGUI>();
        List.SetActive(false);
    }

    public void Toggle()
    {
        Open = !Open;
        List.SetActive(Open);
    }

    public void Select(int Index)
    {
        Current.text = listItems[Index].text;

        Toggle();
    }
}
