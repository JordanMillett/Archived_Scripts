using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class CraftGUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    string nam;
    bool over = false;
    Inventory inv;

    void Start()
    {
        GameObject Player = GameObject.FindWithTag("Player");
        inv = Player.gameObject.GetComponent<Inventory>();

        nam = gameObject.name;
        nam = nam.Replace("craftable_", "");
    }

    void Update()
    {
        Craft();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        over = true;
        
    }

    public void Craft()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (over)
            {
                inv.Craft(int.Parse(nam));
            }
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        over = false;
    }

    public void OnEnable()
    {
        over = false;
    }

}

