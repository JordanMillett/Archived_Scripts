using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoShop : MonoBehaviour
{
    public Transform CamEmpty;

    public List<Transform> CamPositions;

    //0 is outside view
    //1 is initial inside

    public Transform CarEmpty;
    public Transform CarExit;

    MenuManager MM;

    VehicleController Current;

    bool ShouldLerp = false;
    int CamIndex = 0;

    float SmoothCamSpeed = 2f;

    void Start()
    {
        MM = GameObject.FindWithTag("Camera").GetComponent<MenuManager>();
        SetActiveView(0);
    }

    void OnTriggerEnter(Collider Object) 
    {
        if(!Current)
        {
            try
            {
                VehicleController VC = Object.transform.root.GetComponent<VehicleController>();
        
                if(VC != null)
                {
                    if(VC.Occupied && VC.ID.hasAuthority)
                    {
                        VC.CurrentShop = this;
                        Current = VC;
                        Set(true);
                    }
                }

            }catch{}
        }
    }

    void Update()
    {
        if(ShouldLerp && Current)
        {
            CamEmpty.transform.position = Vector3.Lerp(CamEmpty.transform.position,CamPositions[CamIndex].position, Time.fixedDeltaTime * SmoothCamSpeed);
            CamEmpty.transform.rotation = Quaternion.Slerp(CamEmpty.transform.rotation, CamPositions[CamIndex].rotation, Time.fixedDeltaTime * SmoothCamSpeed);
        }

        /*
        if(Current)
        {
        Slerp between different camera positions 
        }*/
    }

    public void SetActiveView(int Index)
    {
        CamIndex = Index;

        if(!ShouldLerp)
        {
            CamEmpty.position = CamPositions[Index].position;
            CamEmpty.rotation = CamPositions[Index].rotation;
        }
    }

    IEnumerator Enter() //Lerp vehicle into garage
    {
        Current.InShop = true;
        MM.SetScreen("Blank");
        Current.GetComponent<Rigidbody>().isKinematic = true;
        GameObject.FindWithTag("Player").GetComponent<Player>().CamOverride = CamEmpty;
        SetActiveView(0);   //Show outside
        
        //yield return new WaitForSeconds(0.5f);

        float TimeStarted = Time.time;
        float WaitTime = 1.5f;
        float Difference = 0f;

        Vector3 StartPosition = Current.transform.position;
        Quaternion StartRotation = Current.transform.rotation;

        

        while(Difference < WaitTime)
        {
            yield return null;
            Difference = Time.time - TimeStarted;

            Current.transform.position = Vector3.Lerp(StartPosition, CarEmpty.position, Difference/WaitTime);
            Current.transform.rotation = Quaternion.Lerp(StartRotation, CarEmpty.rotation, Difference/WaitTime);
        }
        
        //yield return new WaitForSeconds(0.25f);

        MM.SetScreen("Auto");   //Show inside UI
        SetActiveView(1);   //Show inside
        ShouldLerp = true;
    }

    IEnumerator Exit() //Lerp vehicle into garage
    {
        ShouldLerp = false;
        MM.SetScreen("Blank");
        SetActiveView(0);   //Show outside

        //yield return new WaitForSeconds(0.5f);

        float TimeStarted = Time.time;
        float WaitTime = 1.5f;
        float Difference = 0f;

        Vector3 StartPosition = Current.transform.position;
        Quaternion StartRotation = Current.transform.rotation;

        while(Difference < WaitTime)
        {
            yield return null;
            Difference = Time.time - TimeStarted;

            Current.transform.position = Vector3.Lerp(StartPosition, CarExit.position, Difference/WaitTime);
            Current.transform.rotation = Quaternion.Lerp(StartRotation, CarExit.rotation, Difference/WaitTime);
        }
        
        //yield return new WaitForSeconds(0.25f);
        
        
        MM.SetScreen("HUD");   
        Current.GetComponent<Rigidbody>().isKinematic = false;
        Current.InShop = false;
        GameObject.FindWithTag("Player").GetComponent<Player>().CamOverride = null;
        Current = null;
    }

    public void ExitNoCar()
    {
        ShouldLerp = false;
        MM.SetScreen("HUD");  
        SetActiveView(0); 
        GameObject.FindWithTag("Player").GetComponent<Player>().CamOverride = null;
        Current = null;
    }

    public void Set(bool inUse)
    {
        if(inUse)
        {
            StartCoroutine(Enter());
        }else
        {
            StartCoroutine(Exit());
        }
    }
}
