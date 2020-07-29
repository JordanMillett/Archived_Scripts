using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine.IMGUIModule;

/*
TODO

Double click dialogue to open this editor and automatically pass reference
expandable node that can handle any amount of choices or atleast 0 - 3
Drag and drop connections between nodes
Save position and layout of nodes to be loaded if the file is re opened
load a pre existing dialogue into the editor and have it properly formatted
Connect lines together if they're heading to the same target (or snap them side by side neatly)

*/

public class DialogueNodeEditor : EditorWindow
{
    public Dialogue D;
    
    static Color BackGroundColor = new Color(.2f, .2f, .2f);
    static Color GridColor = new Color(.1f, .1f, .1f);
    static Color NodeColor = new Color(.4f, .4f, .4f);
    static Color LineColor = new Color(1f, 1f, 1f);
    
    static Texture2D BackgroundTexture;
    static Texture2D NodeTexture;
    static Texture2D LineTexture;
    public Texture2D ConnectorTexture;
    public Texture2D AddChoice;
    public Texture2D RemoveChoice;
    public Texture2D DeleteButton;

    public Texture2D TextBackground;

    public Texture2D NodeTop;
    public Texture2D NodeMid;
    public Texture2D NodeBottom;

    GUIStyle ButtonStyle;
    GUIStyle TextStyle;

    Vector2 OffsetVector = Vector2.zero;
    Vector2 InitialMousePos = Vector2.zero;
    Vector2 InitialOffsetVector = Vector2.zero;

    bool InConnectMode = false;
    bool Initialized = false;

    Vector2Int ConnectStart = Vector2Int.zero;
    int ConnectEnd = 0;

    float LineThickness = 5f;
    int LineCount = 15;

    [MenuItem ("Window/Dialogue Node Editor")]
    static void ShowWindow() 
    {
        EditorWindow.GetWindow(typeof(DialogueNodeEditor));
    }

    void OnEnable()
    {
        InitializeTextures();
    }

    void OnGUI() 
    {
        if(Initialized)
        {
            
            MouseControls();
            GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), BackgroundTexture, ScaleMode.StretchToFill);

            DrawLines();

            if(D.NodePositions.Count == 0)
            {
                D.NodePositions.Add(new Vector2(200, 200));
            }

            for(int i = 0; i < D.NodePositions.Count; i++)
            {
                DrawNode(D.NodePositions[i], i);
            }
            
            
            
        }
    }

    void Update()
    {
        Repaint();
    }

    void MouseControls()
    {
        Event e = Event.current;
        
        if((e.type == EventType.MouseDown))
        {
            InitialMousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            InitialOffsetVector = OffsetVector;
        }

        if((e.type == EventType.MouseDrag) && (e.button == 0))
        {
            OffsetVector = GUIUtility.GUIToScreenPoint(Event.current.mousePosition) - InitialMousePos + InitialOffsetVector;
        }

        if((e.type == EventType.MouseDown) && (e.button == 1))
        {
            if(InConnectMode)
            {
                InConnectMode = false;
            }else
            {
                D.NodePositions.Add(InitialMousePos - InitialOffsetVector - new Vector2(this.position.x, this.position.y) + new Vector2(-100f, 25f));
            }
        }
    }

    void DrawLines()
    {   
        /*
        Iterate between all choices that don't have -1 as a return value. 
        If it has a value then draw a line to where ever the node is at
        */

        for(int x = 0; x < D.Conversation.Count; x++)
        {
            for(int y = 0; y < D.Conversation[x].Choices.Count; y++)
            {
                if(D.Conversation[x].Choices[y].ReturnValue != -1)
                {
                    DrawLine(D.NodePositions[x] + OffsetVector + 
                    new Vector2(190, (y + 1) * 25f), 
                    D.NodePositions[D.Conversation[x].Choices[y].ReturnValue] + new Vector2(-15f, 0f) + OffsetVector);
                }
            }
        }

        if(InConnectMode)
        {
            DrawLine(D.NodePositions[ConnectStart.x] + OffsetVector + new Vector2(190, (ConnectStart.y + 1) * 25f), 
            GUIUtility.GUIToScreenPoint(Event.current.mousePosition) - new Vector2(this.position.x, this.position.y) + new Vector2(-5f, -30f));
        }
    }

    void DrawLine(Vector2 StartPosition, Vector2 EndPosition)
    {   

        //GUI.DrawTexture(new Rect(EndPosition.x, EndPosition.y, 25, 25), LineTexture);
        //var svMat: Matrix4x4 = GUI.matrix;
        //GUI.DrawTexture(new Rect(StartPosition.x, StartPosition.y, LineThickness, LineThickness), LineTexture);
        //GUI.DrawTexture(new Rect(EndPosition.x, EndPosition.y, LineThickness, LineThickness), LineTexture);

        //Vector2 MidPoint = new Vector2((StartPosition.x + EndPosition.x)/2f, (StartPosition.y + EndPosition.y)/2f);

        //GUI.DrawTexture(new Rect(MidPoint.x, MidPoint.y, LineThickness, LineThickness), LineTexture);

        float Slope = (EndPosition.y - StartPosition.y)/(EndPosition.x - StartPosition.x);
        //Debug.Log(Slope);
        //float xDist = Mathf.Abs(StartPosition.x - EndPosition.x)/Mathf.Max(StartPosition.x, EndPosition.x);
        //Debug.Log(xDist);

        
        

        float Distance = Mathf.Abs(StartPosition.x - EndPosition.x);
        float Increment = 100f/LineCount;
        float Travelled = 0f;

        if(StartPosition.x < EndPosition.x)
        {

        while(Travelled < Distance)
        {
            Travelled += Increment;
            GUI.DrawTexture(new Rect(Travelled + StartPosition.x, (Slope * Travelled) + StartPosition.y + 10f, LineThickness, LineThickness), LineTexture);
        }
        
        }else
        {

        while(Mathf.Abs(Travelled) < Distance)
        {
            Travelled -= Increment;
            GUI.DrawTexture(new Rect(Travelled + StartPosition.x, (Slope * Travelled) + StartPosition.y + 10f, LineThickness, LineThickness), LineTexture);
        }

        }
        
        
        //
        

        //MidPoint = new Vector2((StartPosition.x + EndPosition.x)/1.5f, (StartPosition.y + EndPosition.y)/.75f);
        //GUI.DrawTexture(new Rect(MidPoint.x, MidPoint.y, LineThickness, LineThickness), LineTexture);
        
        //Slope = (Slope/360f) * 100f;

        //UIUtility.RotateAroundPivot(Slope, MidPoint);
        //GUI.DrawTexture(new Rect(MidPoint.x, MidPoint.y, 25, 25), LineTexture);
        //GUIUtility.RotateAroundPivot(-Slope, MidPoint);
        //GUI.martrix = svMat;
    }

    void DrawNode(Vector2 Pos, int Index)
    {
        bool RemoveAtEnd = false;
        Pos += OffsetVector;

        if(Index == D.Conversation.Count)
        {
            Dialogue.Choice C = new Dialogue.Choice();
            C.Text = "New Choice";
            C.ReturnValue = -1;
            Dialogue.Talk T = new Dialogue.Talk();
            T.Reply = new List<string>();
            T.Reply.Add("New Reply");
            T.Choices = new List<Dialogue.Choice>();
            T.Choices.Add(C);
            D.Conversation.Add(T);
        }

        //GUI.DrawTexture(new Rect(Pos.x, Pos.y - 50, 200, 75 + (D.Conversation[Index].Choices.Count * 25)), NodeTexture);
        GUI.DrawTexture(new Rect(Pos.x, Pos.y - 50, 200, 50), NodeTop);
        GUI.DrawTexture(new Rect(Pos.x, Pos.y, 200, 25), NodeMid);

        /*if(GUI.Button(new Rect(Pos.x, Pos.y - 40, 200, 20), AddChoice, ButtonStyle)) draggable window
        {

        }*/

        if(GUI.Button(new Rect(75 + Pos.x, Pos.y - 25, 25, 25), AddChoice, ButtonStyle))
        {
            Dialogue.Choice C = new Dialogue.Choice();
            C.Text = "New Choice";
            C.ReturnValue = -1;
            D.Conversation[Index].Choices.Add(C);
        }
    
        if(GUI.Button(new Rect(75 + Pos.x + 25, Pos.y - 25, 25, 25), RemoveChoice, ButtonStyle))
        {
            if(D.Conversation[Index].Choices.Count >= 1)
            {
                D.Conversation[Index].Choices.RemoveAt(D.Conversation[Index].Choices.Count - 1);
            }
        }

        if(Index != 0)
        {
            if(GUI.Button(new Rect(75 + Pos.x + 75, Pos.y - 25, 25, 25), DeleteButton, ButtonStyle))
            {
                RemoveAtEnd = true;
            }
        }

        //GUI.DrawTexture(new Rect(Pos.x - 15, Pos.y, 25, 25), ConnectorTexture);
        if(GUI.Button(new Rect(Pos.x - 15, Pos.y, 25, 25), ConnectorTexture, ButtonStyle))
        {
            if(InConnectMode)
            {
                ConnectEnd = Index;
                InConnectMode = false;

                Dialogue.Choice C = new Dialogue.Choice();
                C.Text = D.Conversation[ConnectStart.x].Choices[ConnectStart.y].Text;
                C.ReturnValue = ConnectEnd;
                D.Conversation[ConnectStart.x].Choices[ConnectStart.y] = C;
            }
        }
        
        //GUI.Label(new Rect(Pos.x + 15f, Pos.y - 45f, 100, 100), D.Conversation[Index].Reply[0]);
        GUI.DrawTexture(new Rect(Pos.x + 25, Pos.y, 150f, 25f), TextBackground); //Max length
        D.Conversation[Index].Reply[0] = GUI.TextField(new Rect(Pos.x + 25, Pos.y, 150f, 25f), D.Conversation[Index].Reply[0], 25, TextStyle); //Max length

        for(int i = 0; i < D.Conversation[Index].Choices.Count; i++)
        {
            GUI.DrawTexture(new Rect(Pos.x,  Pos.y + ((i + 1) * 25f), 200f, 25f), NodeMid);
            Dialogue.Choice C = new Dialogue.Choice();
            GUI.DrawTexture(new Rect(Pos.x + 25, Pos.y + ((i + 1) * 25f), 150f, 25f), TextBackground); 
            C.Text = GUI.TextField(new Rect(Pos.x + 25, Pos.y + ((i + 1) * 25f), 150f, 25f), D.Conversation[Index].Choices[i].Text, 25, TextStyle);
            C.ReturnValue = D.Conversation[Index].Choices[i].ReturnValue;

            D.Conversation[Index].Choices[i] = C;
            //GUI.DrawTexture(new Rect(Pos.x + 190, Pos.y + ((i + 1) * 25f), 25, 25), ConnectorTexture, ScaleMode.StretchToFill);
            if(GUI.Button(new Rect(Pos.x + 190, Pos.y + ((i + 1) * 25f), 25, 25), ConnectorTexture, ButtonStyle))
            {
                ConnectStart = new Vector2Int(Index, i);
                InConnectMode = true;
            }
        }

        GUI.DrawTexture(new Rect(Pos.x,  Pos.y + ((D.Conversation[Index].Choices.Count + 1) * 25f), 200, 25), NodeBottom);

        if(RemoveAtEnd)
        {
            D.NodePositions.RemoveAt(Index);
            D.Conversation.RemoveAt(Index); 

            for(int x = 0; x < D.Conversation.Count; x++)           //Re shuffles lines if node is removed
            {
                for(int y = 0; y < D.Conversation[x].Choices.Count; y++)
                {
                    if(D.Conversation[x].Choices[y].ReturnValue == Index)
                    {
                        Dialogue.Choice C = new Dialogue.Choice();
                        C.Text = D.Conversation[x].Choices[y].Text;
                        C.ReturnValue = -1;
                        D.Conversation[x].Choices[y] = C;
                    }

                    if(D.Conversation[x].Choices[y].ReturnValue > Index)
                    {
                        Dialogue.Choice C = new Dialogue.Choice();
                        C.Text = D.Conversation[x].Choices[y].Text;
                        C.ReturnValue = D.Conversation[x].Choices[y].ReturnValue - 1;
                        D.Conversation[x].Choices[y] = C;
                    }
                }
            }
        }
    }

    void InitializeTextures()
    {
        BackgroundTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        BackgroundTexture.SetPixel(0, 0, BackGroundColor);
        BackgroundTexture.Apply();

        NodeTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        NodeTexture.SetPixel(0, 0, NodeColor);
        NodeTexture.Apply();

        LineTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        LineTexture.SetPixel(0, 0, LineColor);
        LineTexture.Apply();
        
        ButtonStyle = new GUIStyle(GUI.skin.label);
        ButtonStyle.margin = new RectOffset(0, 0, 0, 0);
        ButtonStyle.padding = new RectOffset(0, 0, 0, 0);
        ButtonStyle.stretchHeight = true;
        ButtonStyle.stretchWidth = true;

        TextStyle = new GUIStyle(GUI.skin.label);
        TextStyle.fontSize = 14;
        TextStyle.normal.textColor = Color.white;
        TextStyle.focused.textColor = Color.white;
        GUI.skin.settings.cursorColor = Color.white;
        
        Initialized = true;
    }
}
