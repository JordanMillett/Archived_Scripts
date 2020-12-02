using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BlueObject))]
public class BlueObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        BlueObject BO = (BlueObject)target;

        if(GUILayout.Button("Generate Distances"))
        {
            BO.GenerateDistances();
        }

    }
}