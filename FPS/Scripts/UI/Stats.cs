using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{

    public bool Weapon = true;

    public Weapon[] Guns;
    public Explode[] Grenades;

    public int Index = 0;

    //public Weapon W;
    //public Explode E;

    Simple_Bar RPM;
    Simple_Bar Damage;
    Simple_Bar MagSize;
    Simple_Bar Recoil;
    Simple_Bar Accuracy;

    Simple_Bar Radius;
    Simple_Bar Magnitude;
    

    void Start()
    {   

        Init();
        Disable();

        UpdateBars();

        


    }

    public void ChangeIndex(int i)
    {

        Index = i;
        UpdateBars();

    }

    void Disable()
    {
        
        if(Weapon)
        {

            for(int i = 0; i < Guns.Length; i++)
            {

                if(i != 0)
                    Guns[i].transform.gameObject.SetActive(false);

            }

        }else
        {
            for(int i = 0; i < Grenades.Length; i++)
            {

                if(i != 0)
                    Grenades[i].transform.gameObject.SetActive(false);

            }
        }

        

    }

    void UpdateBars()
    {

        if(Weapon)
        {

            RPM.Max = 2000f;
            RPM.Current = Guns[Index].WC.RPM;

            Damage.Max = 30f;
            Damage.Current = Guns[Index].WC.Damage;

            MagSize.Max = 40f;
            MagSize.Current = Guns[Index].WC.MagSize;

            Recoil.Max = 2f;
            Recoil.Current = Guns[Index].WC.RecoilAmount;

            Accuracy.Max = 100f;
            Accuracy.Current = Guns[Index].WC.Accuracy;
        }else
        {

            Radius.Max = 10f;
            Radius.Current = Grenades[Index].Radius;

            Damage.Max = 200f;
            Damage.Current = Grenades[Index].Damage;

            Magnitude.Max = 2000f;
            Magnitude.Current = Grenades[Index].Magnitude;
        }

    }

    void Init()
    {

        if(Weapon)
        {
            RPM = transform.GetChild(0).GetComponent<Simple_Bar>();
            Damage = transform.GetChild(1).GetComponent<Simple_Bar>();
            MagSize = transform.GetChild(2).GetComponent<Simple_Bar>();
            Recoil = transform.GetChild(3).GetComponent<Simple_Bar>();
            Accuracy = transform.GetChild(4).GetComponent<Simple_Bar>();
        }else
        {
            Radius = transform.GetChild(0).GetComponent<Simple_Bar>();
            Damage = transform.GetChild(1).GetComponent<Simple_Bar>();
            Magnitude = transform.GetChild(2).GetComponent<Simple_Bar>();
        }

    }

}
