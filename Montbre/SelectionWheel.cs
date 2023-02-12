using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionWheel : MonoBehaviour
{
    public bool Show = false;

    public WheelPart Up;
    public WheelPart Down;
    public WheelPart Left;
    public WheelPart Right;

    List<Vector2> PastMouse = new List<Vector2>();

    public string Selected = "";

    public Texture2D Buy_Man;
    public Texture2D Buy_Artillery;
    public Texture2D Buy_Plane;
    public Texture2D Buy_Equipment;

    public Texture2D Control;
    public Texture2D Clear;
    public Texture2D Follow;
    public Texture2D Stay;

    public Texture2D Return;
    
    public Texture2D Drop;

    public Texture2D None;


    void Start()
    {
        for(int i = 0; i < 20; i++)
        {
            PastMouse.Add(Vector2.zero);
        }
    }

    public void SetMenu(int Index)
    {
        if(Index == 0)  //Buying
        {
            Up.Set("Men", Buy_Man, Game.Defense_CargoPlaneCost.ToString());
            Down.Set("Air", Buy_Plane, Game.Defense_FighterPlaneCost.ToString());
            //Left.Set("Equipment", Buy_Equipment, Game.Defense_EquipmentCost.ToString());
            Left.Set("Equipment", Buy_Equipment, "NA");
            Right.Set("Artillery", Buy_Artillery, Game.Defense_ArtilleryCost.ToString());
        }else if(Index == 1)    //Selecting men as man
        {
            Up.Set("Control", Control, "Control");
            Down.Set("Clear", Clear, "Clear");
            Left.Set("Follow", Follow, "Follow");
            Right.Set("Stay", Stay, "Stay");
        }else if(Index == 2)    //Selecting plane as man
        {
            Up.Set("Control", Control, "Control");
            Down.Set("", None, "");
            Left.Set("", None, "");
            Right.Set("", None, "");
        }else if(Index == 3)    //anything as plane
        {
            Up.Set("Return", Return, "Return");
            Down.Set("Drop", Drop, "Drop");
            Left.Set("", None, "");
            Right.Set("", None, "");
        }else if(Index == 4)    //anything as tank
        {
            Up.Set("Return", Return, "Return");
            Down.Set("", None, "");
            Left.Set("", None, "");
            Right.Set("", None, "");
        }else if(Index == 5)    //Selecting tank as man
        {
            Up.Set("Control", Control, "Control");
            Down.Set("", None, "");
            Left.Set("", None, "");
            Right.Set("", None, "");
        }
    }

    void Update()
    {
        if(Show)
            UpdateList(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
        else
            UpdateList(Vector2.zero);

        Game.MouseInputLocked = Show;

        if(Show)
        {
            Activate(true);

            Vector2 MouseData = new Vector2(
                Mathf.Abs(GetAverage().x) > 0.25f ? GetAverage().x : 0, 
                Mathf.Abs(GetAverage().y) > 0.25f ? GetAverage().y : 0);

            if(Mathf.Abs(MouseData.x) > Mathf.Abs(MouseData.y))
                MouseData.y = 0;
            else
                MouseData.x = 0;

            if(MouseData.x == 0 && MouseData.y == 0)
            {
                Highlight(-1);
            }else
            {
                if(MouseData.x > 0)
                    Highlight(3);
                else if(MouseData.x < 0)
                    Highlight(2);
                if(MouseData.y > 0)
                    Highlight(0);
                else if(MouseData.y < 0)
                    Highlight(1);
            }   


        }else
        {
            Activate(false);
        }
    }

    void UpdateList(Vector2 M)
    {
        PastMouse.Add(M);
        PastMouse.RemoveAt(0);
    }

    Vector2 GetAverage()
    {
        Vector2 Sum = new Vector2(0f, 0f);

        for(int i = 0; i < PastMouse.Count; i++)
            Sum += PastMouse[i];

        return Sum /= new Vector2(PastMouse.Count, PastMouse.Count);

        //Vector2 MouseData = new Vector2(
                //Mathf.Abs(Input.GetAxis("Mouse X")) > 0.25f ? Input.GetAxis("Mouse X") : 0, 
                //Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.25f ? Input.GetAxis("Mouse Y") : 0);
        
    }

    void Highlight(int Index)
    {
        Up.Highlight = Index == 0;
        Down.Highlight = Index == 1;
        Left.Highlight = Index == 2;
        Right.Highlight = Index == 3;

        Selected = "";
        Selected += Up.Highlight ? Up.Title : "";
        Selected += Down.Highlight ? Down.Title : "";
        Selected += Left.Highlight ? Left.Title : "";
        Selected += Right.Highlight ? Right.Title : "";
    }

    void Activate(bool On)
    {
        Up.transform.gameObject.SetActive(On);
        Down.transform.gameObject.SetActive(On);
        Left.transform.gameObject.SetActive(On);
        Right.transform.gameObject.SetActive(On);
    }
}
