using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public float RespawnTime = 3f;
    Manager M;
    bool Respawning = false;
    Item CurrentItem;

    Transform Model;

    void Start()
    {
        M = GameObject.FindWithTag("Manager").GetComponent<Manager>();
        CurrentItem = M.AllItems[Random.Range(0, M.AllItems.Count)];
        Model = transform.GetChild(0);
    }

    void OnTriggerEnter(Collider Col)
    {
        if(Col.GetComponent<KartController>() != null && !Respawning)
        {
            if(!Col.GetComponent<KartController>().ItemReference)
            {
                GameObject ItemInstance = Instantiate(CurrentItem.Prefab, Vector3.zero, Quaternion.identity);
                ItemInstance.transform.SetParent(Col.GetComponent<KartController>().ItemSlot);
                ItemInstance.transform.localPosition = Vector3.zero;
                ItemInstance.transform.localEulerAngles = Vector3.zero;

                ItemInstance.transform.GetComponent<ItemInvoker>().KC = Col.GetComponent<KartController>();

                Col.GetComponent<KartController>().ItemReference = ItemInstance;
            }
                
            Respawning = true;
            Model.gameObject.SetActive(false);
            Invoke("Respawn", RespawnTime);
        }
    }

    void Respawn()
    {
        Respawning = false;
        CurrentItem = M.AllItems[Random.Range(0, M.AllItems.Count)];
        Model.gameObject.SetActive(true);
    }
}
