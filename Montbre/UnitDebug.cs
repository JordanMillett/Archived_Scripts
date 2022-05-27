using UnityEngine;
using System.Collections;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(Unit))]
class UnitDebug : Editor
{
    Texture2D Attack;
    Texture2D Defend;

    void OnEnable()
    {
        Attack = new Texture2D(64, 64);
        for (int y = 0; y < Attack.height; y++)
        {
            for (int x = 0; x < Attack.width; x++)
            {
                Attack.SetPixel(x, y, Color32.Lerp(Game.EnemyColor, Color.black, 0.6f));
            }
        }
        Attack.Apply();

        Defend = new Texture2D(64, 64);
        for (int y = 0; y < Defend.height; y++)
        {
            for (int x = 0; x < Defend.width; x++)
            {
                Defend.SetPixel(x, y, Color32.Lerp(Game.FriendlyColor, Color.black, 0.6f));
            }
        }
        Defend.Apply();
    }

    void OnSceneGUI()
    {
        Unit each = (Unit) target;
        if(each == null)
            return;

        GUIStyle style = new GUIStyle();
        style.normal.background = each.Team == Game.TeamTwo ? Attack : Defend;
        style.normal.textColor = Color.white; 
        style.alignment = TextAnchor.MiddleCenter;
        
        if(each.inf)
        {
            Handles.Label(each.Target.position,
                "<b><size=12>" + "Infantry</size></b>" + "\n" +
                "<size=11>" + each.inf.State.ToString() + "</size>",
                style
            );
        }
        if(each.tan)
        {
            Handles.Label(each.Target.position,
                "<b><size=12>" + "Tank</size></b>" + "\n" +
                "<size=11>" + each.tan.State.ToString() + "</size>",
                style
            );
        }
        if(each.pla)
        {
            Handles.Label(each.Target.position,
            "<b><size=12>" + each.pla.Type + "</size></b>" + "\n" +
            "<size=11>" + each.pla.State.ToString() + "</size>",
            style
            );
        }
    }

    /*
     void OnSceneGUI()
    {
        return;

        Handles.color = new Color(0f, 0f, 0f, 0f);

        Plane each = (Plane) target;
        if(each == null)
            return;

        GUIStyle style = new GUIStyle();
        style.normal.background = each.U.Team == Game.TeamTwo ? Attack : Defend;
        style.normal.textColor = Color.white; 
        style.alignment = TextAnchor.MiddleCenter;
        
        //Manager.M.Factions[(int) each.Team].Abb +

        Handles.Label(each.transform.position,
            "<b><size=12>" + each.Type + "</size></b>" + "\n" +
            "<size=11>" + each.State.ToString() + "</size>",
            style
        );
    }*/
}