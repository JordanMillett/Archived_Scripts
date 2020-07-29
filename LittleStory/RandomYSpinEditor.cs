using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(RandomYSpin)), CanEditMultipleObjects]
public class RandomYSpinEditor : Editor
{
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        if(GUILayout.Button("Spin"))
        {
            foreach (RandomYSpin YSpin in targets.Cast<RandomYSpin>())
            {
                YSpin.Spin();
            }
        }

        if(GUILayout.Button("Scale"))
        {
            foreach (RandomYSpin YSpin in targets.Cast<RandomYSpin>())
            {
                YSpin.Scale();
            }
        }

    }
}