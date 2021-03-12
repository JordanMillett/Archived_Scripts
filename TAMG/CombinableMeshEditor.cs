
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CombinableMesh)), CanEditMultipleObjects]
public class CombinableMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {

        //base.OnInspectorGUI();
        Object[] OBJS = targets;
        List<CombinableMesh> CMS = new List<CombinableMesh>();

        for(int i = 0; i < OBJS.Length; i++) 
        {
            if(OBJS[i] as CombinableMesh != null)
            {
                CMS.Add(OBJS[i] as CombinableMesh);
            }
        }

        int GenStatus = AllGenerated(CMS);

        if(GenStatus != -1)
        {

            if(GenStatus == 1 && CMS.Count == 1)
            {
                GUILayout.Label("Total Vertices : " + CMS[0].TotalVerts);
                GUILayout.Label("Total Objects : " + CMS[0].TotalObjects);
            }else if(GenStatus == 1)
            {
                GUILayout.Label("Different Number of Vertices");
                GUILayout.Label("Different Number of Objects");
            }

            GUILayout.BeginHorizontal();

            if(GenStatus == 0)
            {
                if(GUILayout.Button("Generate"))
                {
                    foreach(CombinableMesh CM in CMS)
                    {
                        CM.Generate();
                    }
                }

            }else
            {
                if(GUILayout.Button("Split"))
                {
                    foreach(CombinableMesh CM in CMS)
                    {
                        CM.Split();
                    }
                }

                if(GUILayout.Button("Regenerate"))
                {
                    foreach(CombinableMesh CM in CMS)
                    {
                        CM.Regenerate(false);
                    }
                }
            }

            GUILayout.EndHorizontal();

            if(GenStatus == 1)
            {
                if(GUILayout.Button("Regenerate All"))
                {
                    CMS[0].RegenerateAll(false);
                }

                if(GUILayout.Button("Split All"))
                {
                    CMS[0].RegenerateAll(true);
                }
            }

            GUILayout.BeginHorizontal();

            if(GUILayout.Button("Add Tag"))
            {
                foreach(CombinableMesh CM in CMS)
                {
                    CM.AddTag();
                }
            }

            if(GUILayout.Button("Remove Tag"))
            {
                foreach(CombinableMesh CM in CMS)
                {
                    CM.RemoveTag();
                }
            }

            GUILayout.EndHorizontal();
        }else
        {
            GUILayout.Label("Conflicting Variables");
        }
    }

    int AllGenerated(List<CombinableMesh> CMS)
    {
        bool isGenerated = CMS[0].Generated;
        foreach(CombinableMesh CM in CMS)
        {
            if(CM.Generated != isGenerated)
            {
                return -1;
            }
        }

        if(isGenerated)
        {
            return 1;
        }else
        {
            return 0;
        }
    }
}