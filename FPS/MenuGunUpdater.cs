using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuGunUpdater : MonoBehaviour
{
    public GameObject G;

    public void Confirm(Texture T, string N)
    {

        G.transform.GetChild(0).GetComponent<RawImage>().texture = T;
        G.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = N;

    }
}
