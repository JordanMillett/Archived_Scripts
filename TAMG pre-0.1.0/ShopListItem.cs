using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopListItem : MonoBehaviour
{
    TextMeshProUGUI ItemTitle;
    public TextMeshProUGUI Cost;

    public void Init(string Title, int Amount)
    {
        ItemTitle = GetComponent<TextMeshProUGUI>();
        ItemTitle.text = Title;
        Cost.text = "-$" + Amount.ToString();
    }

    public void Remove()
    {

    }
}
