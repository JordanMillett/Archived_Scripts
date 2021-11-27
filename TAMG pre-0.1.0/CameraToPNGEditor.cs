
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraToPNG))]
public class CameraToPNGEditor : Editor
{
    /*
    public override void OnInspectorGUI()
    {
        CameraToPNG CTPNG = (CameraToPNG) target;

        if(CTPNG.isEnabled)
        {
            if(GUILayout.Button("Disable"))
            {
                CTPNG.isEnabled = false;
            }
        }else
        {
            if(GUILayout.Button("Enable"))
            {
                CTPNG.isEnabled = true;
            }
        }
    }
    */
}