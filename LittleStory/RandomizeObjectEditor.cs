using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(RandomizeObject)), CanEditMultipleObjects]
public class RandomizeObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();

        if(GUILayout.Button("Randomize Object"))
        {
            foreach (RandomizeObject RObject in targets.Cast<RandomizeObject>())
            {
                RObject.Randomize();
            }
        }

        GUILayout.EndHorizontal();

    }
}