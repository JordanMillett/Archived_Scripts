using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EquipSlot : MonoBehaviour
{

    public TextMeshProUGUI Ammo;
    List <GameObject> Weapons = new List<GameObject>();

    public GameObject Throwable;

    public GameObject SpawnLocation;
    public Vector3 DirVector;
    public float Force;

    UIController UIC;

    public WeaponConfig Primary;
    public WeaponConfig Secondary;
    //public equipconfig use throwable

    //public GameObject Showcase;

    //public Stats Primary;
    //public Stats Secondary;
    //public Stats Grenade;

    void Start()
    {

        UIC = GameObject.FindWithTag("Pause").GetComponent<UIController>();

        /* 

        foreach (Transform child in transform)
        {

            Weapons.Add(child.gameObject);

        }

        Set(0);*/

    }

    void Update()
    {
        
        if(!UIC.Paused)
        {

            if(Input.GetKeyDown("1"))
                Set(0);

            if(Input.GetKeyDown("2"))
                Set(1);

            if(Input.GetKeyDown("g"))
                Throw();
        }

    }

    public void InitGuns()
    {
        GameObject P = Instantiate(Primary.DefaultGun, Vector3.zero, Quaternion.identity);
        Weapons.Add(P);
        P.transform.SetParent(this.transform);
        P.transform.localPosition = P.GetComponent<Weapon>().Pos;
        P.transform.localEulerAngles = P.GetComponent<Weapon>().Rot;
        P.GetComponent<Weapon>().PlayerControlled = true;

        GameObject S = Instantiate(Secondary.DefaultGun, Vector3.zero, Quaternion.identity);
        Weapons.Add(S);
        S.transform.SetParent(this.transform);
        S.transform.localPosition = Vector3.zero;
        S.transform.localPosition = S.GetComponent<Weapon>().Pos;
        S.transform.localEulerAngles = S.GetComponent<Weapon>().Rot;
        S.GetComponent<Weapon>().PlayerControlled = true;
        //GameObject G = Instantiate(Showcase.transform.GetChild(0).GetChild(Primary.Index).gameObject, Vector3.zero, Quaternion.identity);

        Set(0);


    }

    void Throw()
    {

        GameObject Projectile = Instantiate(Throwable, SpawnLocation.transform.position, Quaternion.identity);
        Rigidbody r = Projectile.GetComponent<Rigidbody>();
        r.AddForce((SpawnLocation.transform.forward + DirVector) * Force);

    }

    void Set(int Selector)
    {

        for(int i = 0; i < Weapons.Count;i++)
        {

            if(i == Selector)
            {
                //Weapons[i].SetActive(true);
                Weapons[i].GetComponent<Weapon>().enabled = true;
                Weapons[i].transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                //Weapons[i].SetActive(false);
                Weapons[i].GetComponent<Weapon>().enabled = false;
                Weapons[i].transform.GetChild(0).gameObject.SetActive(false);
            }

        }

    }

}
