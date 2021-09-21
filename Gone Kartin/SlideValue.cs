using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SlideValue : MonoBehaviour
{
    public Slider S;
    TextMeshProUGUI T;

    void Start()
    {
        T = GetComponent<TextMeshProUGUI>();
    }

    public void Refresh()
    {
        T.text = S.value.ToString();
    }
}
