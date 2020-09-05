using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(ClickToSpawn))]
public class ClickToSpawnEditor : Editor
{
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();

        ClickToSpawn CTS = (ClickToSpawn)target;

        if(GUILayout.Button("Toggle"))
        {
            CTS.SpawnOn = !CTS.SpawnOn;
            //CTS.Spawn(Vector3.zero);
        }

        GUILayout.EndHorizontal();
    }

    public void OnSceneGUI()
    {
        ClickToSpawn CTS = (ClickToSpawn)target;

        if(CTS.SpawnOn)
        {
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            SceneView sceneView = SceneView.currentDrawingSceneView;

            Event e = Event.current;
            if((e.type == EventType.MouseDown) && (e.button == 0))
            {
                Ray ray = HandleUtility.GUIPointToWorldRay( Event.current.mousePosition );
                RaycastHit hit;

                if (Physics.Raycast (ray, out hit, 1000f)) 
                {
                    CTS.Spawn(hit.point);
                }

                

                GUIUtility.hotControl = controlId;
                Event.current.Use();
            }

        }



    }
}