using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusAnimator : MonoBehaviour
{
    [System.Serializable]
    public struct Movable
    {
	    public GameObject Object;
        public Vector3 Start;
        public Vector3 Finish;
        public float Alpha;
    }

    public Movable Steering;
    public Movable Lights;
    public Movable Gear;
    public Movable Brake;
    public Movable Gas;
    public Movable Visor;
    public Movable Roller;

    public Material Glass;
    public Material Broken_Glass;
    public MeshRenderer Body;
    public AudioSourceController ASC_FrontGlass;
    public AudioSourceController ASC_SideGlass;
    public AudioClip BreakSound;
    
    void Start()
    {
        Steering.Object.transform.localEulerAngles = Steering.Start;
        Lights.Object.transform.localEulerAngles = Lights.Start;
        Gear.Object.transform.localEulerAngles = Gear.Start;
        Brake.Object.transform.localEulerAngles = Brake.Start;
        Gas.Object.transform.localEulerAngles = Gas.Start;
        Visor.Object.transform.localEulerAngles = Visor.Start;
        Roller.Object.transform.localEulerAngles = Roller.Start;
    }

    void FixedUpdate()
    {
        Refresh(Steering);
        Refresh(Lights);
        Refresh(Gear);
        Refresh(Brake);
        Refresh(Gas);
        Refresh(Visor);
        Refresh(Roller);
    }

    public void BreakGlass()
    {
        ASC_FrontGlass.SetVolume(1f);
        ASC_SideGlass.SetVolume(1f);

        ASC_FrontGlass.Sound = BreakSound;
        ASC_SideGlass.Sound = BreakSound;

        ASC_FrontGlass.Play();
        ASC_SideGlass.Play();

        Material[] Temp = new Material[Body.materials.Length];
        for(int i = 0; i < Temp.Length; i++)
            Temp[i] = Body.materials[i];
        
        Temp[4] = Broken_Glass;

        Body.materials = Temp;
    }

    public void FixGlass()
    {
        Material[] Temp = new Material[Body.materials.Length];
        for(int i = 0; i < Temp.Length; i++)
            Temp[i] = Body.materials[i];
        
        Temp[4] = Glass;

        Body.materials = Temp;
    }

    void Refresh(Movable M)
    {
        M.Object.transform.localRotation = Quaternion.Slerp(M.Object.transform.localRotation, Quaternion.Euler(Vector3.Lerp(M.Start, M.Finish, M.Alpha)), Time.fixedDeltaTime * 2f);
    }
}
