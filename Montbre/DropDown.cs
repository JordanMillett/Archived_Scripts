using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DropDown : MonoBehaviour
{
    static DropDown OpenedInstance;

    public GameObject ListItemPrefab;
    string[] Options;

    TextMeshProUGUI Selected;
    GameObject ListItems;

    public bool Open = false;

    public int CurrentIndex = 0;

    public string Prefix = "";
    public string Suffix = "";

    bool Inited = false;

    void OnEnable()
    {
        if(Open)
        {
            Toggle();
        }
    }

    public void Initialize(string[] PassedOptions, int StartingValue)
    {
        if(!Inited)
        {
            Inited = true;
            Selected = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            ListItems = transform.GetChild(1).gameObject;
            Options = PassedOptions;

            int Yloc = -50;
            for(int i = 0; i < Options.Length; i++)
            {
                GameObject ListItem = Instantiate(ListItemPrefab, Vector3.zero, Quaternion.identity);
                ListItem.transform.SetParent(ListItems.transform);
                ListItem.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, Yloc, 0f);
                ListItem.GetComponent<RectTransform>().transform.localScale = Vector3.one;
                ListItem.transform.GetComponentInChildren<TextMeshProUGUI>().text = Prefix + Options[i] + Suffix;
                AddListener(ListItem.transform.GetComponentInChildren<Button>(), i);

                Yloc -= 50;
            }

            CurrentIndex = StartingValue;
            Select(CurrentIndex);
            ListItems.SetActive(false);
        }
    }

    void AddListener(Button B, int Value)
    {
        B.onClick.AddListener(delegate {Select(Value);});
        B.onClick.AddListener(delegate {MenuManager.MM.PlaySound();});
    }

    public void Toggle()
    {
        MenuManager.MM.PlaySound();
        Open = !Open;
        ListItems.SetActive(Open);

        if(Open)
        {
            if(OpenedInstance)
                if(OpenedInstance != this && OpenedInstance.Open)
                    OpenedInstance.Toggle();
            
            OpenedInstance = this;
        }
    }

    public void Select(int Index)
    {
        Selected.text = Prefix + Options[Index] + Suffix;
        CurrentIndex = Index;
        Toggle();
    }

    public void Set(int Index)
    {
        Selected.text = Prefix + Options[Index] + Suffix;
        CurrentIndex = Index;
    }
}