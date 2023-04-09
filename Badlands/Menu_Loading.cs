using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu_Loading : MonoBehaviour
{
    public SimpleBar LoadingBar;
    public TextMeshProUGUI Splash;

    public StationGenerator SG;

    public void LoadStation(Texture2D Map)
    {
        SG.Begin(Map);
    }

    public void UpdateStatus(float Percent, string Info)
    {
        LoadingBar.Current = Percent;
        Splash.text = Info;
    }
}
