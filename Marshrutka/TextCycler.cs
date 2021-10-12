using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TextCycler : MonoBehaviour
{   
    public UnityEvent OnLast;

    public List<GameObject> Texts = new List<GameObject>();

    int CurrentIndex = 0;

    void OnEnable()
    {
        CurrentIndex = 0;
        Set();
    }

    public void Next()
    {
        if(CurrentIndex >= Texts.Count - 1)
        {
            OnLast.Invoke();
        }else
        {
            CurrentIndex++;
            Set();
        }
    }

    void Set()
    {
        for(int i = 0; i < Texts.Count; i++)
        {
            if(i == CurrentIndex)
            {
                Texts[i].SetActive(true);
            }else
            {
                Texts[i].SetActive(false);
            }
        }
    }
}