using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    public RawImage BG;
    public TextMeshProUGUI Place;
    public TextMeshProUGUI Driver;
    public TextMeshProUGUI Time;
    public Color NormalColor;
    public Color HighlightColor;

    public void SetValues(Manager.KartInfo KI)
    {
        BG.gameObject.SetActive(true);
        Place.gameObject.SetActive(true);
        Driver.gameObject.SetActive(true);
        Time.gameObject.SetActive(true);

        Place.text = KI.Place.ToString() + TextFormatter.GetPlaceSuffix(KI.Place);   //TextFormatter.GetPlaceSuffix(KI.Place);
        Driver.text = KI.Controller.ToString(); //TextFormatter.GetDriverName(KI.Controller);
        Time.text = TextFormatter.GetTimeFormatted(KI.Time);

        if(KI.Controller != KartController.Driver.AI)
        {
            BG.color = HighlightColor;
        }else
        {
            BG.color = NormalColor;
        }
    }

    public void SetNotFinished(int DriverPlace)
    {
        BG.gameObject.SetActive(true);
        Place.gameObject.SetActive(true);
        Driver.gameObject.SetActive(true);
        Time.gameObject.SetActive(true);

        Place.text = DriverPlace.ToString() + TextFormatter.GetPlaceSuffix(DriverPlace);  //TextFormatter.GetPlaceSuffix(KI.Place);
        Driver.text = KartController.Driver.AI.ToString(); //TextFormatter.GetDriverName(KI.Controller);
        Time.text = "Did Not Complete";

        BG.color = NormalColor;
    }

    public void SetHidden()
    {
        BG.gameObject.SetActive(false);
        Place.gameObject.SetActive(false);
        Driver.gameObject.SetActive(false);
        Time.gameObject.SetActive(false);
    }
}
