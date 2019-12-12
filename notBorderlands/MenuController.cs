using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    GameObject MainCamera;
    GameObject Inventory;

    public bool InventoryOpen = false;

    void Start()
    {
        MainCamera = transform.GetChild(0).gameObject;
        Inventory = transform.GetChild(2).gameObject;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = InventoryOpen;
        MainCamera.SetActive(!InventoryOpen);
        Inventory.SetActive(InventoryOpen);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
            OpenInventory();
    }

    void OpenInventory()
    {
        InventoryOpen = !InventoryOpen;
        GetComponent<PlayerController>().enabled = !InventoryOpen; 
        Cursor.visible = InventoryOpen;
        MainCamera.SetActive(!InventoryOpen);
        Inventory.SetActive(InventoryOpen);
    }
}
