using UnityEngine;
using UnityEditor;
using System.Collections;

public class KeyFramer : EditorWindow
{

    //bool Action = true;
    CameraAnimator CA;

    int ClipIndex = 0;
    int KeyFrameIndex = 0;

    string Name = "Untitled";

    [MenuItem ("Window/KeyFramer")]
    public static void ShowWindow() 
    {
        EditorWindow.GetWindow(typeof(KeyFramer));
    }

    void OnGUI() 
    {
        CA = EditorGUILayout.ObjectField(CA, typeof(CameraAnimator), true) as CameraAnimator;

        if(CA != null)
        {
            EditorGUILayout.LabelField("Total Clips : " +  CA.Scenes.Count);

            if(CA.Scenes.Count > 1)
                ClipIndex = EditorGUILayout.IntSlider("Clip Number : ", ClipIndex, 0, CA.Scenes.Count - 1);
            
            if(CA.Scenes.Count != 0)
            {
                Name = CA.Scenes[ClipIndex].Name;
                Name = EditorGUILayout.TextField ("Name : ", Name);
                CA.SetName(ClipIndex, Name);
            }

            GUILayout.BeginHorizontal();

                if(GUILayout.Button("Make New Clip"))
                {
                    CA.NewClip(ClipIndex);

                    if(ClipIndex < CA.Scenes.Count - 1) //push clip index slider over when making a new clip
                        ClipIndex++;

                }
                
                if(CA.Scenes.Count > 1)
                    if(GUILayout.Button("Delete Current Clip"))
                    {
                        CA.DeleteClip(ClipIndex);
                        if(ClipIndex != 0)
                            ClipIndex--;
                    }

                
            GUILayout.EndHorizontal();

            if(CA.Scenes.Count > 0)
            {
                if(CA.Scenes[ClipIndex].Positions.Count > 1)
                    KeyFrameIndex = EditorGUILayout.IntSlider("Keyframe Number : ", KeyFrameIndex, 0, CA.Scenes[ClipIndex].Positions.Count - 1);

                GUILayout.BeginHorizontal();

                    if (GUILayout.Button("New Keyframe"))
                    {
                        CA.NewFrame(ClipIndex, KeyFrameIndex);
                        if(!(KeyFrameIndex >= CA.Scenes[ClipIndex].Positions.Count - 1))
                            KeyFrameIndex++;
                    }

                    if(CA.Scenes[ClipIndex].Positions.Count > 1)
                        if (GUILayout.Button("Delete Current Keyframe"))
                        {
                            CA.DeleteFrame(ClipIndex, KeyFrameIndex);
                            if(KeyFrameIndex != 0)
                                KeyFrameIndex--;
                        }
                    
                GUILayout.EndHorizontal();

                if(CA.Scenes[ClipIndex].Positions.Count > 0)
                {
                    EditorGUILayout.LabelField("Position : " +  CA.Scenes[ClipIndex].Positions[KeyFrameIndex]);
                    EditorGUILayout.LabelField("Rotation : " +  CA.Scenes[ClipIndex].Rotations[KeyFrameIndex]);
                }
            }


        }
           
    }
}
