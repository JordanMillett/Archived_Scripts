using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutManager : MonoBehaviour
{

    public GameObject Player;
    public GameObject MenuCam;
    public GameObject GunRoom;
    public GameObject Enemys;

    Camera Cam;
    PlayerController PC;
    GameObject EquipSlot;

    void Start()
    {
        Cam = Player.transform.GetChild(0).GetComponent<Camera>();       
        PC = Player.GetComponent<PlayerController>();
        EquipSlot = Cam.transform.GetChild(1).gameObject;

        Enable(false); 
    }   

    public void Enable(bool On)
    {

        Cam.enabled = On;
        PC.enabled = On;
        EquipSlot.SetActive(On);

        MenuCam.SetActive(!On);
        GunRoom.SetActive(!On);
        Enemys.SetActive(On);

        Cursor.visible = !On;

    }

    void Update()
    {
        
    }
}
