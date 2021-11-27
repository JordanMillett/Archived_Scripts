using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

[EditorTool("Paint Tool")]
public class PaintTool : EditorTool
{
    //[SerializeField]
    public GameObject PaintPrefab;

    //[SerializeField]
    public Transform ParentEmpty;

    Transform InnerParent;

    [SerializeField]
    int ParentAmount = 100;

    [SerializeField]
    int yLevel = 0;

    int CurrentParentAmount = 101;

    public void OnEnable()
    {
        CurrentParentAmount = 101;
        InnerParent = null;
    }

    public override void OnToolGUI(EditorWindow window)
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        Paint();
    }

    void Paint()
    {
        int controlId = GUIUtility.GetControlID(FocusType.Passive);
        Event e = Event.current;

        RaycastHit mouseHit;
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        if(!Physics.Raycast(mouseRay, out mouseHit))
        {
            if((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && (e.button == 0))
            {
                MakeTile(new Vector3(Mathf.Round(mouseRay.origin.x/16)*16, yLevel, Mathf.Round(mouseRay.origin.z/16)*16));

                GUIUtility.hotControl = controlId;
                Event.current.Use();
            }
        }
    }

    void MakeTile(Vector3 Position)
    {
        CurrentParentAmount++;

        if(CurrentParentAmount >= ParentAmount)
        {
            InnerParent = new GameObject().transform;
            InnerParent.SetParent(ParentEmpty);
            InnerParent.position = Vector3.zero;
            InnerParent.localEulerAngles = Vector3.zero;
            InnerParent.localScale = new Vector3(1f, 1f, 1f);

            CurrentParentAmount = 0;
        }

        GameObject Copy = (GameObject) PrefabUtility.InstantiatePrefab(PaintPrefab) as GameObject;
        Copy.transform.position = Position;
        Copy.transform.SetParent(InnerParent);
        Copy.transform.localScale = new Vector3(1f, 1f, 1f);

        

        //Debug.Log(Position);
    }
}
