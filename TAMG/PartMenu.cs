using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartMenu : MonoBehaviour
{
    CarMenu CM;

    public GameObject PartOptionPrefab;
    public GameObject BackOptionPrefab;

    public List<GameObject> Tabs;

    public VehiclePart.Location PartLoc = VehiclePart.Location.Front;

    bool MenuCreated = false;

    /*
    public void SetIndex(int Index)
    {
        if(PlayerInfo.Balance >= VPs[Index].Cost)
        {
            PlayerInfo.Balance -= VPs[Index].Cost;
            PartIndex = Index;
            CM.ApplyEngine();
            CM.SetScreen(0);
        }
    }*/

    public void Init(CarMenu _CM)
    {
        CM = _CM;

        if(!MenuCreated)
        {
            CreateMenu(VehiclePart.Location.Front);
            CreateMenu(VehiclePart.Location.Middle);
            CreateMenu(VehiclePart.Location.End);
            MenuCreated = true;
        }

        SetScreen(0);
    }

    public void SetScreen(int Index)
    {
        for(int i = 0; i < Tabs.Count; i++)
        {
            if(i == Index)
            {
                Tabs[i].SetActive(true);
            }
            else
            {
                Tabs[i].SetActive(false);
            }
        }
    }

    public void SetPart(VehiclePart VP)  //Apply a purchased part
    {
        if(PlayerInfo.Balance >= VP.Cost)
        {
            PlayerInfo.Balance -= VP.Cost;
            CM.ApplyPart(PartLoc, VP, true);
        }
    }

    public void OpenTab(int Loc)   //Open a tab duh
    {
        PartLoc = (VehiclePart.Location) Loc;
        SetScreen(Loc + 1);
    }

    public void HoverPart(VehiclePart VP)    //Show part on hover
    {
        CM.ApplyPart(PartLoc, VP, false);
    }

    void CreateMenu(VehiclePart.Location Type)
    {
        Transform NewParent = null;

        switch (Type)
        {
            case VehiclePart.Location.Front :
                NewParent = Tabs[1].transform;
            break;
            case VehiclePart.Location.Middle :
                NewParent = Tabs[2].transform;
            break;
            case VehiclePart.Location.End :
                NewParent = Tabs[3].transform;
            break;
        }
        
        int Len = CM.VC.Config.InstallableParts.Count;
        int YCount = 0;

        for(int i = 0; i < Len; i++)
        {
            if(CM.VC.Config.InstallableParts[i].PartLocation == Type)
            {
                GameObject NewOption = Instantiate(PartOptionPrefab, Vector3.zero, Quaternion.identity);
                NewOption.transform.SetParent(NewParent);
                RectTransform RT = NewOption.GetComponent<RectTransform>();
                RT.transform.localScale = Vector3.one;
                RT.anchoredPosition = new Vector2(0f, (YCount * -35f) + 70f);
                
                NewOption.GetComponent<PartOption>().Init(this, CM.VC.Config.InstallableParts[i].PartName, CM.VC.Config.InstallableParts[i], CM.VC.Config.InstallableParts[i].Cost.ToString());
                YCount++;
            }
        }

        GameObject EndOption = Instantiate(BackOptionPrefab, Vector3.zero, Quaternion.identity);
        EndOption.transform.SetParent(NewParent);
        RectTransform RTE = EndOption.GetComponent<RectTransform>();
        RTE.transform.localScale = Vector3.one;
        RTE.anchoredPosition = new Vector2(0f, (YCount * -35f) + 70f);
                
        EndOption.GetComponent<ListEndOption>().Init(this);
    }

    public void Back()
    {
        //CM.VC.SetParts(VC.CurrentParts);
        SetScreen(0);
        CM.SetScreen(6);
    }
}
