using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapBuilder))]
public class MapBuilderEditor : Editor
{
    private SerializedProperty mapData;
    
    private SerializedProperty tileset;
    private SerializedProperty generated;
    private SerializedProperty editing;

    private SerializedProperty paint;

    void OnEnable()
    {
        Tools.hidden = true;
        
        Initialize();
    }
    
    void OnDisable()
    {
        Tools.hidden = false;
    }
    
    bool Initialize()
    {
        if(mapData != null && mapData.objectReferenceValue && tileset.objectReferenceValue)
            return true;

        mapData = serializedObject.FindProperty("MapData");
        
        tileset = serializedObject.FindProperty("TileSet");
        generated = serializedObject.FindProperty("Generated");
        editing = serializedObject.FindProperty("Editing");
        
        paint = serializedObject.FindProperty("Paint");
        
        if(!mapData.objectReferenceValue || !tileset.objectReferenceValue)
            return false;

        return true;
    }
    
    public override void OnInspectorGUI()
    {   
        serializedObject.UpdateIfRequiredOrScript();
        
        EditorGUILayout.PropertyField(mapData, new GUIContent("Map Data"));
        EditorGUILayout.PropertyField(tileset, new GUIContent("Tileset"));
        
        if(!Initialize())
        {
            (serializedObject.targetObject as MapBuilder).ToDefaults();
            serializedObject.ApplyModifiedProperties();
            return;
        }
        
        if(!EditorApplication.isPlaying)
        {
            FileButtons();
        }

        serializedObject.ApplyModifiedProperties();
    }
    
    public void OnSceneGUI()
    {
        MapBuilder mapBuilder = target as MapBuilder;

        if (mapBuilder.Editing)
        {
            Vector3 offset = new Vector3(mapBuilder.Offset.x, 0f, mapBuilder.Offset.y);
            Vector2Int dim = mapBuilder.TileData.GetDimensions();

            for (int x = -1; x < dim.x + 1; x++)
            {
                for (int y = -1; y < dim.y + 1; y++)
                {
                    Handles.color = Color.green;
                    if(mapBuilder.InBounds(x, y))
                        Handles.color = Color.white;
                    
                    if(!mapBuilder.IsCorner(x, y))
                        if (Handles.Button(mapBuilder.transform.position + new Vector3(x, 0f, y) + offset, Quaternion.Euler(90f, 0f, 0f), 0.5f, 0.5f, Handles.RectangleHandleCap))
                            mapBuilder.Set(x, y);
                }
            }
            /*
            Color color = new Color(1, 0.8f, 0.4f, 1);
            Handles.color = color;
            Handles.DrawWireDisc(mapBuilder.transform.position, mapBuilder.transform.up, 1.0f);
            GUI.color = color;
            Handles.Label(mapBuilder.transform.position, "BRUH");*/
        }
    }
    
    void ToolButtons()
    { 
        EditorGUILayout.PropertyField(paint, new GUIContent("Paint"));
    }
    
    void FileButtons()
    {
        if (generated.boolValue)
        {                
            if (editing.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Exit"))
                {
                    editing.boolValue = false;
                }

                if (GUILayout.Button("Save"))
                {
                    (serializedObject.targetObject as MapBuilder).Save();
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space();
                ToolButtons();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Unload"))
                {
                    (serializedObject.targetObject as MapBuilder).UnloadMap();
                }
                
                if (GUILayout.Button("Edit"))
                {
                    editing.boolValue = true;
                }
                EditorGUILayout.EndHorizontal();
            }
        }else
        {
            if (GUILayout.Button("Load"))
            {
                (serializedObject.targetObject as MapBuilder).LoadMap();
            }
        }
    }
}