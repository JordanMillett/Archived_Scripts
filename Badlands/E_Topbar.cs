using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class E_Topbar : MonoBehaviour
{
    public bool WorldMode;

    TextMeshProUGUI Info;

    void Start()
    {
        Info = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        Info.text = "";

        if (WorldMode)
        {
            Info.text += "<pos=0%>" + Game.SaveData.Blueprints + "<color=#" + Game.UIColors["Blueprints"] + "> β</color>";
            Info.text += "<pos=44%>Day " + Game.SaveData.Day;
            Info.text += "<pos=88%>" + Game.SaveData.Credits + "<color=#" + Game.UIColors["Credits"] + "> Ϛ</color>";
        }else
        {
            Info.text += "<pos=0%>" + Game.RunData.CollectedBlueprints + "<color=#" + Game.UIColors["Blueprints"] + "> β</color>";
            Info.text += "<pos=85%>" + Game.RunData.HoldingCredits + "/" + Game.SaveData.MaxHoldingCredits + "<color=#" + Game.UIColors["Credits"] + "> Ϛ</color>";
        }
    }
}
