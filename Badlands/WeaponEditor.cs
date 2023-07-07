using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor
{
    Weapon W;

    public override void OnInspectorGUI()
    {
        W = (Weapon) target;

        base.OnInspectorGUI();
        
        EditorGUILayout.Space();

        int mult = W.BurstWaves;
        
        if(W.AllAtOnce)
            mult *= (((W.SprayMax * 2) / W.SprayInterval) + 1);

        EditorGUILayout.LabelField("Damage Per Shot - " + (W.Damage * mult));
        EditorGUILayout.LabelField("Damage Per Second - " + ((W.RPM/60f) * (W.Damage * mult)));
        if(W.Firemode == FireModes.Semi)
            EditorGUILayout.LabelField("Max Damage Per Second - " + (((W.RPM * 1.25f/60f)) * (W.Damage * mult)));
        
        /*
        if(W.Firemode == Weapon.Firemodes.Semi)
            EditorGUILayout.LabelField("Player Max Fire Rate - " + (W.RPM * 2f));
        else
            EditorGUILayout.LabelField("Player Fire Rate - " + (W.RPM));
        EditorGUILayout.LabelField("AI Fire Rate - " + (W.RPM));*/
    }
}