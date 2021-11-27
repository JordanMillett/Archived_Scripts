using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorOptionList : MonoBehaviour
{
    public CarMenu CM;
    MenuManager MM;

    public GameObject OptionPrefab;

    public GameObject MainListTab;
    public GameObject SubListTab;
    public GameObject MainList;
    public GameObject SubList;

    List<RectTransform> MainListContents = new List<RectTransform>();
    List<GameObject> SubLists = new List<GameObject>();

    bool MenuCreated = false;

    int MainTopY = 35;
    int MainBottomY = -140;
    int MainTotalOffset = 0;

    int SubTopY = 70;

    void OnEnable()
    {
        MM = GameObject.FindWithTag("Camera").GetComponent<MenuManager>();

        if(!MenuCreated)
            InitializeMenu();

        MainListTab.SetActive(true);
        SubListTab.SetActive(false);
    }

    void InitializeMenu()
    {
        int Len = ColorLookup.CustomColors.Count;

        for(int i = 0; i < Len; i++)
        {
            GameObject NewOption = Instantiate(OptionPrefab, Vector3.zero, Quaternion.identity);
            NewOption.transform.SetParent(MainList.transform);
            RectTransform RT = NewOption.GetComponent<RectTransform>();
            RT.transform.localScale = Vector3.one;
            RT.anchoredPosition = new Vector2(0f, (i * -35f) + MainTopY);

            NewOption.GetComponent<ColorOption>().Init(this, ColorLookup.CustomColors[i].Name, i, false, ColorLookup.CustomColors[i].Cost.ToString());

            MainListContents.Add(RT);

            MakeSubList(i);
        }

        Hide();

        MenuCreated = true;
    }

    void MakeSubList(int Index)
    {
        GameObject Folder = new GameObject();
        Folder.transform.SetParent(SubList.transform);
        Folder.transform.localPosition = Vector3.zero;
        Folder.transform.localScale = Vector3.one;
        Folder.name = ColorLookup.CustomColors[Index].Name;

        int Len = ColorLookup.CustomColors[Index].SubShades.Count;

        GameObject FirstOption = Instantiate(OptionPrefab, Vector3.zero, Quaternion.identity);
        FirstOption.transform.SetParent(Folder.transform);
        RectTransform FirstRT = FirstOption.GetComponent<RectTransform>();
        FirstRT.transform.localScale = Vector3.one;
        FirstRT.anchoredPosition = new Vector2(0f, SubTopY);
        FirstOption.GetComponent<ColorOption>().Init(this, ColorLookup.CustomColors[Index].Name, -1, true, ColorLookup.CustomColors[Index].Cost.ToString());

        for(int i = 0; i < Len; i++)
        {
            GameObject NewOption = Instantiate(OptionPrefab, Vector3.zero, Quaternion.identity);
            NewOption.transform.SetParent(Folder.transform);
            RectTransform RT = NewOption.GetComponent<RectTransform>();
            RT.transform.localScale = Vector3.one;
            RT.anchoredPosition = new Vector2(0f, (i * -35f) + 35f);

            NewOption.GetComponent<ColorOption>().Init(this, ColorLookup.CustomColors[Index].SubShades[i].Name, i, true, ColorLookup.CustomColors[Index].SubShades[i].Cost.ToString());
        }

        SubLists.Add(Folder);
    }

    public void Back()
    {
        MainListTab.SetActive(true);
        SubListTab.SetActive(false);
    }

    void Update()
    {
        if(Input.mouseScrollDelta.y > 0f)
        {
            Scroll(true);
        }

        if(Input.mouseScrollDelta.y < 0f)
        {
            Scroll(false);
        }
    }

    public void PlaySound()
    {
        MM.PlaySound();
    }

    public void OpenColorMenu(int Index)
    {
        CM.SetColorMenu(Index);

        MainListTab.SetActive(false);
        SubListTab.SetActive(true);

        for(int i = 0; i < SubLists.Count; i++)
        {
            if(i == Index)
            {
                SubLists[i].SetActive(true);
            }else
            {
                SubLists[i].SetActive(false);
            }
        }
    }

    public void HoverColor(int Index, bool Sub)
    {
        CM.HoverColor(Index, Sub);
    }

    public void SetColor(int Index)
    {
        CM.SetColor(Index);
    }

    public void Scroll(bool Up)
    {
        int Offset = 0;
        if(Up)
        {
            if(MainTotalOffset >= 35)
            {
                Offset = -35;
                MainTotalOffset += Offset;
            }
        }
        else
        {
            if(MainTotalOffset < ((MainListContents.Count * 35) - (35 * 6)))
            {
                Offset = 35;
                MainTotalOffset += Offset;
            }
        }

        foreach (RectTransform RT in MainListContents)
        {
            RT.anchoredPosition += new Vector2(0f, Offset);
        }

        Hide();
    }

    public void Hide()
    {
        foreach (RectTransform RT in MainListContents)
        {
            if(RT.anchoredPosition.y > MainTopY || RT.anchoredPosition.y < MainBottomY)
                RT.gameObject.SetActive(false);
            else
                RT.gameObject.SetActive(true);
        }
    }
}
