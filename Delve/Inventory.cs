using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public PlayerController PC;

    public GameObject InventoryWindow;
    public GameObject SkillsWindow;

    public bool isOpen = false;

    public List<InvInfo> Contents;

    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = isOpen;

    }

    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Tab) && (PC.Frozen == isOpen))
        {

            Toggle();

        }

        if(Input.GetKeyDown("o"))
        {

            disableDecals();

        }

        if(Input.GetKeyDown("p"))
        {

            ScreenCapture.CaptureScreenshot("ScreenShot.png",1);

        }

    }

    void disableDecals()    //move to different script, options script
    {

        GameObject[] Decals = GameObject.FindGameObjectsWithTag("Decal");

        foreach (GameObject D in Decals)
        {
            D.SetActive(false);
        }

    }

    public void Toggle()
    {
        isOpen = !isOpen;

        Cursor.visible = isOpen;
        if(!isOpen)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;

        PC.Frozen = isOpen;
        InventoryWindow.SetActive(isOpen);
        SkillsWindow.SetActive(isOpen);

    }
}
