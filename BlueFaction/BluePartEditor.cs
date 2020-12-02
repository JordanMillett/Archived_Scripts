using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BluePart)), CanEditMultipleObjects]
public class BluePartEditor : Editor
{
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        BluePart BP = (BluePart)target;

        if(GUILayout.Button("Break"))
        {
            BP.Break();
        }

    }
}