using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WeaponInfo))]
public class WeaponInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        WeaponInfo Reference = (WeaponInfo) target;

        GUIStyle HeaderStyle = new GUIStyle(GUI.skin.label) 
        {
            alignment = TextAnchor.MiddleCenter,
            fixedHeight = 30,
            fontSize = 20,
            fontStyle = FontStyle.Bold
        };

        EditorGUILayout.LabelField(
            (Reference.Automatic ? "Auto" : "Semi") + " " + 
            (!Reference.Explosive ? "Normal" : Reference.Flak ? "Flak" : "Explosive")
            , HeaderStyle);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("UseCase.AntiInfantry"), new GUIContent("Anti Infantry"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("UseCase.AntiArmor"), new GUIContent("Anti Armor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("UseCase.AntiAir"), new GUIContent("Anti Air"));
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Damage"), new GUIContent("Damage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("RPM"), new GUIContent("RPM"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MagazineSize"), new GUIContent(Reference.MagazineSize == 0 ? "Magazine Size (∞)" : "Magazine Size"));
        if(Reference.MagazineSize != 0)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ReloadTime"), new GUIContent("Reload Time"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Accuracy"), new GUIContent("Accuracy"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Automatic"), new GUIContent("Automatic"));
        if(Reference.Automatic)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AccurateTime"), new GUIContent("Accurate Time"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MuzzleVelocity"), new GUIContent("Muzzle Velocity"));
        EditorGUILayout.Space();
        if(Reference.MagazineSize == 0)
            EditorGUILayout.LabelField("");
        if(!Reference.Automatic)    
            EditorGUILayout.LabelField("");

        EditorGUILayout.LabelField("First Person", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("FOV"), new GUIContent("FOV"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("RecoilMultiplier"), new GUIContent("Recoil Multiplier"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("AnimationIndex"), new GUIContent("Animation Index"));
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Graphics", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("DecalScale"), new GUIContent("Decal Scale"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Caliber"), new GUIContent("Caliber"));
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Audio", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("FirePitch"), new GUIContent("Fire Pitch"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("FireSounds"), new GUIContent("Fire Sound Group"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("DecalVolume"), new GUIContent("Decal Volume"));
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Prefabs", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Prefab"), new GUIContent("Prefab"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ProjectilePrefab"), new GUIContent("Projectile Prefab"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("DecalPrefab"), new GUIContent("Decal Prefab"));
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("Explosive"), new GUIContent("Explosive"));
        if(Reference.Explosive)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ExplosiveSize"), new GUIContent("Explosive Size"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ExplosiveDamage"), new GUIContent("Explosive Damage"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Flak"), new GUIContent("Flak"));
            if(Reference.Flak)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("FlakDistanceMin"), new GUIContent("Flak Distance Min"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("FlakDistanceMax"), new GUIContent("Flak Distance Max"));
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}