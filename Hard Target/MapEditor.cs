using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    private SerializedProperty mapName;
    private SerializedProperty dimensions;
    private SerializedProperty offset;

    private void OnEnable() 
    {
        mapName = serializedObject.FindProperty("MapName");
        dimensions = serializedObject.FindProperty("Dimensions");
        offset = serializedObject.FindProperty("Offset");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();
        
        EditorGUILayout.PropertyField(mapName, new GUIContent("Map Name"));
        EditorGUILayout.LabelField("Map Dimensions : " + dimensions.vector2IntValue.x + " x " + dimensions.vector2IntValue.y);
        EditorGUILayout.LabelField("Map Offset : " + offset.vector2IntValue.x + ", " + offset.vector2IntValue.y);

        serializedObject.ApplyModifiedProperties();
    }
}