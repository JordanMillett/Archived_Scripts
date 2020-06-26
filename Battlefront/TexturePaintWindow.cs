using UnityEngine;
using UnityEditor;
using System.Collections;

public class TexturePaintWindow : EditorWindow
{

    TexturePaintTarget TPT;

    [MenuItem ("Window/Texture Paint Window")]
    public static void ShowWindow() 
    {
        EditorWindow.GetWindow(typeof(TexturePaintWindow));
    }

    public void OnGUI() 
    {
        //TPT = EditorGUILayout.ObjectField(TPT, typeof(TexturePaintTarget), true) as TexturePaintTarget;

        try
        {
            TPT = Selection.activeGameObject.GetComponent<TexturePaintTarget>();
        }catch{}
        

        if(TPT != null)
        {

            EditorGUILayout.LabelField("Object Name : " +  TPT.gameObject.name);
            
            if(GUILayout.Button("Toggle Edit Mode"))
                TPT.editing = !TPT.editing;
            
            if(TPT.editing)
            {
                TPT.editing = true;

                TPT.brushSize = EditorGUILayout.Slider("Brush Size : ", TPT.brushSize, 1f, 6f);
                TPT.brushColor = EditorGUILayout.ColorField("Brush Color : ", TPT.brushColor);
                TPT.currentBrushShape = (TexturePaintTarget.Shapes) EditorGUILayout.EnumPopup("Current Brush Shape : ", TPT.currentBrushShape);
                TPT.textureSize = EditorGUILayout.Vector2IntField("Texture Size : ", TPT.textureSize);

                if(GUILayout.Button("Create new Texture"))
                {
                    TPT.NewTexture();
                }

            }

        }
    }
}
