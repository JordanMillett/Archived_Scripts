using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tileset))]
public class TilesetEditor : Editor
{
    private SerializedProperty tilesetName;
    private SerializedProperty tiles;

    private void OnEnable() 
    {
        tilesetName = serializedObject.FindProperty("TilesetName");
        tiles = serializedObject.FindProperty("Tiles");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        EditorGUILayout.PropertyField(tilesetName, new GUIContent("Tileset Name"));

        EditorGUILayout.Space();

        for (int i = 0; i < System.Enum.GetValues(typeof(Tile.Types)).Length; i++)
        {
            SerializedProperty tile = tiles.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(tile, new GUIContent(System.Enum.GetName(typeof(Tile.Types), (Tile.Types) i)));
        }

        serializedObject.ApplyModifiedProperties();
    }
}