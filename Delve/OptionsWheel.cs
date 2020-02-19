using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsWheel : MonoBehaviour
{
    public GameObject Option_Prefab;

    int CurrentIndex = 0;

    //public List<String>() OR public List<Options>()
    List<RectTransform> Options = new List<RectTransform>();

    void Start()
    {
        MakeWheel();
    }

    void MakeWheel()
    {

        CurrentIndex = 1;

        GameObject one = Instantiate(Option_Prefab, Vector3.zero, Quaternion.identity);
        one.transform.SetParent(this.transform);
        one.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 50f, 0f);
        one.GetComponent<RectTransform>().transform.localScale = Vector3.one;
        Options.Add(one.GetComponent<RectTransform>());
        

        GameObject two = Instantiate(Option_Prefab, Vector3.zero, Quaternion.identity);
        two.transform.SetParent(this.transform);
        two.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 0f, 0f);
        two.GetComponent<RectTransform>().transform.localScale = Vector3.one;
        Options.Add(two.GetComponent<RectTransform>());

        GameObject three = Instantiate(Option_Prefab, Vector3.zero, Quaternion.identity);
        three.transform.SetParent(this.transform);
        three.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -50f, 0f);
        three.GetComponent<RectTransform>().transform.localScale = Vector3.one;
        Options.Add(three.GetComponent<RectTransform>());

        UpdateColors();

    }

    void Update()
    {
        if(Options.Count > 0)
        {
            if(Input.mouseScrollDelta.y > 0f)
                Shift(true);
            else if(Input.mouseScrollDelta.y < 0f)
                Shift(false);
        }else
        {

            Debug.LogError("ERROR : No Options in the interact dialogue");

        }
        
    }

    void Shift(bool Up)     //unselected ones should be dimmed //#C8C8C8
    {

        if(Up && CurrentIndex > 0)
        {

            CurrentIndex--;

            //foreach(RectTransform RT in Options)
            for(int i = 0; i < Options.Count; i++)
                Options[i].anchoredPosition += new Vector2(0f, -50f);

        }else if(!Up && CurrentIndex < Options.Count - 1)
        {

            CurrentIndex++;

            for(int i = 0; i < Options.Count; i++)
                Options[i].anchoredPosition += new Vector2(0f, 50f);

        }

        UpdateColors();

    }

    void UpdateColors()
    {

        for(int i = 0; i < Options.Count; i++)
        {

            if(i == CurrentIndex)
                Options[i].gameObject.GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
            else
                Options[i].gameObject.GetComponent<TextMeshProUGUI>().color = new Color32(150, 150, 150, 255);

        }

    }

    void Select()
    {
        
        //if mousepress get active send to dialogue script

    }
}
