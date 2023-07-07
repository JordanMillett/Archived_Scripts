using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Faction))]
public class FactionEditor : Editor
{
    Faction F;

    public override void OnInspectorGUI()
    {
        F = (Faction) target;

        base.OnInspectorGUI();

        DrawLevelStats(1);
        DrawLevelStats(5);
        DrawLevelStats(10);
        DrawLevelStats(20);

    }
    
    void DrawLevelStats(int Level)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Level " + Level + " - " + Mathf.Floor((Game.RampUpTime * (Level - 1))/60f)+" Minutes "+ (Game.RampUpTime * (Level - 1))%60 + " Seconds");
        EditorGUILayout.LabelField("Health - " + F.GetHealth(Level));
        EditorGUILayout.LabelField("Shields - " + F.GetShields(Level));
        EditorGUILayout.LabelField("Total - " + (F.GetHealth(Level) + F.GetShields(Level)));
    }
}