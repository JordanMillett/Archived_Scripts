using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FactoryMenu : MonoBehaviour
{

    public Factory F;

    public GameObject UnitPrefab;

    float Y_Loc = 150f;
    float Spacing = 100f;

    void Start()
    {
        for(int i = 0; i < F.Units.Length; i++)
            MakeTab(i);
    }

    void MakeTab(int Index)
    {

        GameObject N = Instantiate(UnitPrefab, this.transform.position, Quaternion.identity);
        N.transform.SetParent(this.transform);
        N.name = "tab_" + F.Units[Index].Name;
        N.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Y_Loc);
        Y_Loc -= Spacing;

        N.GetComponent<Button>().onClick.AddListener(delegate {F.MakeUnit(Index);});

        N.transform.GetChild(0).GetComponent<RawImage>().texture = F.Units[Index].Icon; 
        N.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = F.Units[Index].Name;
        N.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = F.Units[Index].Money.ToString();
        N.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = F.Units[Index].Oil.ToString();
        N.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = F.Units[Index].Metal.ToString();

    }


}
