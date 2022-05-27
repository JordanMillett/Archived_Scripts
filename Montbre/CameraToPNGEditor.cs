using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraToPNG))]
public class CameraToPNGEditor : Editor
{
    float m_LastEditorUpdateTime;

    int PhotoIndex = -1;
    
    bool Next = false;
    bool Charged = false;
    
    int Cooldown = 60;
    int CooldownTime = 60;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CameraToPNG CTPNG = (CameraToPNG) target;
        
        if(GUILayout.Button("Ready"))
        {
            CTPNG.Ready();
        }

        if(GUILayout.Button("Take Photo"))
        {
            Charged = false;
            Cooldown = CooldownTime;
            PhotoIndex = 0;
            Next = true;
        }
    }
 
    protected virtual void OnEnable()
    {
        m_LastEditorUpdateTime = Time.realtimeSinceStartup;
        EditorApplication.update += OnEditorUpdate;
    }

    protected virtual void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }

    protected virtual void OnEditorUpdate()
    {
        CameraToPNG CTPNG = (CameraToPNG) target;

        if(PhotoIndex > -1)
        {
            Cooldown--;
            if(Cooldown == 0)
            {
                if(!Charged)
                {
                    CTPNG.Prepare(PhotoIndex);
                    Charged = true;
                    Cooldown = CooldownTime;
                    return;
                }

                if(Next)
                {
                    Next = CTPNG.Capture(PhotoIndex);
                    Cooldown = CooldownTime;
                    if(Next)
                    {
                        Charged = false;
                        PhotoIndex++;
                    }
                }else
                {
                    Cooldown = CooldownTime;
                    CTPNG.Finish();
                    PhotoIndex = -1;
                    return;
                }
            }
        }
        
    }
}