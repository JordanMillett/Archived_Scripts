using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using UnityEditor.Callbacks;
using UnityEditor;

[CreateAssetMenu(fileName ="New Dialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    [System.Serializable]
    public struct Talk
    {
	    public List<string> Reply;
        public List<Choice> Choices;
    }

    [System.Serializable]
    public struct Choice
    {
        public string Text;
        public int ReturnValue;
    }

    //public TextAsset LineLookupFile;
    public string Name;
    public List<Talk> Conversation;
    public List<string> Idle_Responses;
    public List<Vector2> NodePositions;
    
    [OnOpenAssetAttribute(1)]
    public static bool OnOpen(int instanceID, int line)
    {
        if (Selection.activeObject as Dialogue != null) 
        {
            DialogueNodeEditor window = EditorWindow.GetWindow<DialogueNodeEditor>();
            window.D = Selection.activeObject as Dialogue;
            return true; //catch open file
        }        
     
        return false; // let unity open the file
    }
}
