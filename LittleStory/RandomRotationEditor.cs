using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(RandomRotation)), CanEditMultipleObjects]
public class RandomRotationEditor : Editor
{
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        if(GUILayout.Button("Rotate"))
        {
            foreach (RandomRotation Rotation in targets.Cast<RandomRotation>())
            {
                Rotation.Rotate();
            }
        }
    }
}
