using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(RandomYSpin)), CanEditMultipleObjects]
public class RandomYSpinEditor : Editor
{
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();

        if(GUILayout.Button("Spin"))
        {
            foreach (RandomYSpin YSpin in targets.Cast<RandomYSpin>())
            {
                YSpin.Spin();
            }
        }

        GUILayout.EndHorizontal();

    }
}