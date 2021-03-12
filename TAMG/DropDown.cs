using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DropDown : MonoBehaviour
{
    public GameObject ListItemPrefab;
    string[] Options;

    TextMeshProUGUI Selected;
    GameObject ListItems;

    bool Open = false;

    public int CurrentIndex = 0;

    public string Prefix = "";
    public string Suffix = "";

    bool Inited = false;

    MenuManager MM;

    public void Initialize(string[] PassedOptions, int StartingValue)
    {
        if(!Inited)
        {
            MM = GameObject.FindWithTag("Camera").GetComponent<MenuManager>();
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
                //ListItem.transform.GetComponentInChildren<Button>().onClick.AddListener(delegate {Select(i);});

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
        B.onClick.AddListener(delegate {MM.PlaySound();});
    }

    public void Toggle()
    {
        ListItems.SetActive(Open);
        Open = !Open;
    }

    public void Select(int Index)
    {
        Selected.text = Prefix + Options[Index] + Suffix;
        CurrentIndex = Index;
        Toggle();
    }

    /*
    public GameObject List;
    public TextMeshProUGUI Selected;
    TextMeshProUGUI[] listItems;

    public int Index;
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
        Selected.text = listItems[Index].text;

        Toggle();
    }
    */
}