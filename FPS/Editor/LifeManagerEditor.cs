using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LifeManager))]
public class LifeManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        LifeManager LM = (LifeManager)target;

        //GUILayout.Label("Controls");

        //DrawDefaultInspector();

        //EditorGUILayout.FloatField(Max_HP);

        GUILayout.BeginHorizontal();

        if(GUILayout.Button("Kill"))
        {
            LM.Kill();
        }

        if(GUILayout.Button("Respawn"))
        {
            LM.Respawn();
        }

        GUILayout.EndHorizontal();

    }
}
