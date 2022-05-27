using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CameraToPNG : MonoBehaviour
{  
    public Transform Holder;

    List<GameObject> Children = new List<GameObject>();

    #if UNITY_EDITOR

    public void Ready()
    {
        Children.Clear();
        for(int i = 0; i < Holder.childCount; i++)
            Children.Add(Holder.GetChild(i).gameObject);
        foreach(GameObject G in Children)
            G.transform.localPosition = new Vector3(0f, 1f, 0f);
        Debug.Log(Children.Count + " Ready");
    }

    public void Prepare(int Index)
    {
        if(!Holder)
            return;
        if(Children.Count == 0)
            return;

        foreach(GameObject G in Children)
            G.transform.localPosition = new Vector3(0f, 1f, 0f);

        Children[Index].transform.localPosition = Children[Index].GetComponent<Weapon>().Photo_Offset;
        GetComponent<Camera>().orthographicSize = Children[Index].GetComponent<Weapon>().Photo_CameraSize;
    }

    public bool Capture(int Index)
    {
        string path = Application.dataPath + "/Factions/" + Children[Index].name.Split('_')[0] + "/Weapons/Images/" + 
        Children[Index].name + ".png";
            
        ScreenCapture.CaptureScreenshot(path, 1);
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log("Screenshot Taken " + path);

        if(Index == Children.Count - 1)
            return false;
        else
            return true;
    }

    public void Finish()
    {
        UnityEditor.AssetDatabase.Refresh();
        foreach(GameObject G in Children)
            G.transform.localPosition = new Vector3(0f, 1f, 0f);
    }

    #endif
}