using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CameraToPNG : MonoBehaviour
{   
    public bool isEnabled = false;

    #if UNITY_EDITOR

    void Update()
    {
        if(Input.GetKeyDown("l"))
            Screenshot();
    }

    void Screenshot()
    {
        string path = "C:\\" + "EditorScreenshot.png";
        ScreenCapture.CaptureScreenshot(path, 1);
        Debug.Log("Screenshot Taken " + path);
        UnityEditor.AssetDatabase.Refresh();
    }

    #endif

    /*
    #if UNITY_EDITOR
    private float m_LastEditorUpdateTime;
 
    protected virtual void OnEnable()
    {
        m_LastEditorUpdateTime = Time.realtimeSinceStartup;
        UnityEditor.EditorApplication.update += OnEditorUpdate;
    }
 
    protected virtual void OnDisable()
    {
        UnityEditor.EditorApplication.update -= OnEditorUpdate;
    }
 
    protected virtual void OnEditorUpdate()
    {
        if(isEnabled && Input.GetKeyDown("p"))
        {
            Screenshot();
        }
    }

    void Screenshot()
    {
        Debug.Log("Screenshot Taken");

        string path = Application.dataPath + "EditorScreenshot.png";

        Texture2D screenImage = new Texture2D(Screen.width, Screen.height);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        byte[] imageBytes = screenImage.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, imageBytes);

        UnityEditor.AssetDatabase.Refresh();
    }

    #endif*/
}
