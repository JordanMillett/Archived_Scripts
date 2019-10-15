using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceController : MonoBehaviour
{
    public int Money;
    public int Oil;
    public int Metal;

    public GameObject TopBar;

    void Start()
    {

        Refresh();

    }

    void Refresh()
    {
        TopBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Money.ToString();
        TopBar.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Oil.ToString();
        TopBar.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Metal.ToString();
    }

    public void Change(int Index, int Amount)
    {

        switch (Index)
        {
            case 0 : Money  += Amount; break;
            case 1 : Oil    += Amount; break;
            case 2 : Metal  += Amount; break;
        }

        Refresh();

    }
}
