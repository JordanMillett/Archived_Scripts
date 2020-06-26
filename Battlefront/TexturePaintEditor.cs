using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TexturePaintTarget))]
public class TexturePaintEditor : Editor
{
    /*Tool lastTool = Tool.None;
 
    void OnEnable()
    {
        lastTool = Tools.current;
        Tools.current = Tool.None;
    }
    
    void OnDisable()
    {
        Tools.current = lastTool;
    }*/

    public override void OnInspectorGUI()
    {
        
        /*
        TexturePaintTarget TPT = (TexturePaintTarget)target;

        base.OnInspectorGUI();

        if(GUILayout.Button("Create new Texture"))
        {
            TPT.NewTexture();
        }
        */
        //Debug.Log("Inspector Working");
    }
   
    public void OnSceneGUI()
    {
        TexturePaintTarget TPT = (TexturePaintTarget)target;
 
        //Debug.Log("Scene Working");

        if(TPT.editing)
        {
            int controlId = GUIUtility.GetControlID(FocusType.Passive);

            SceneView sceneView = SceneView.currentDrawingSceneView;

            TPT.UpdateMousePosition(sceneView);

            if(TPT.isHovered)
            {
                Event e = Event.current;
                if((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && (e.button == 0))
                {
                    if(TPT.Layers.Count > 0)
                        TPT.PaintPixels(TPT.mousePosition, 0);

                    //e.Use();
                    GUIUtility.hotControl = controlId;
                    Event.current.Use();
                }
                    
            }

            

        }


        // int controlId = GUIUtility.GetControlID(FocusType.Passive);
        // GUIUtility.hotControl = controlId;   

        //Gizmos.color = TPT.brushColor;
        //Gizmos.DrawWireSphere(TPT.mousePosition, TPT.brushSize);

    }

    /*public void Awake()
    {
        TexturePaintTarget TPT = (TexturePaintTarget)target;
        TPT.Initialize();
    }*/

     /*void OnFocus() 
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
    }

    void OnDestroy() 
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }*/
    

    //SceneView sceneView

    /*[DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    static void DrawGizmoForMyScript(TexturePaintTarget scr, GizmoType gizmoType)
    {
        Debug.Log("Gizmos Called");

        //Vector3 position = scr.transform.position;

        //if (Vector3.Distance(position, Camera.current.transform.position) > 10f)
            //Gizmos.DrawIcon(position, "MyScript Gizmo.tiff");
    }*/

    /*void OnDrawGizmosSelected()
    {
        #if UNITY_EDITOR
            Debug.Log("Gizmos Called");
        #endif

    }*/
    

    
    /*
    void OnFocus() 
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
    }
 
    void OnDestroy() 
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }*/
    
    /*[CustomEditor(typeof(GameObject))]
public class SceneGUITest : Editor
{
    [DrawGizmo(GizmoType.NotSelected)]
    static void RenderCustomGizmo(Transform objectTransform, GizmoType gizmoType)
    {
        //Draw here
    }
}*/

}
