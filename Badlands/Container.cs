using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public Item[] Contents = new Item[4];
    public Transform PopupFollow;
    
    Popup_Container PopupReference;

    bool Activated = false;

    public GameObject Effect;

    public void Activate()
    {
        Activated = true;
        Effect.SetActive(true);
    }
    
    void OnTriggerEnter(Collider col)
    {
        if(!Activated)
            return;

        try{
            Player P = col.transform.root.transform.gameObject.GetComponent<Player>();
            if (P)
            {
                if(!PopupReference)
                {
                    PopupReference = UIManager.UI.M_Game.CreateContainerPopup();
                    PopupReference.Target = PopupFollow;
                    PopupReference.C = this;
                    for (int i = 0; i < Contents.Length; i++)
                        PopupReference.SetItem(i, Contents[i]);
                }else
                {
                    PopupReference.gameObject.SetActive(true);
                }
            }
        }catch{}
    }

    void OnTriggerExit(Collider col)
    {
        if(!Activated)
            return;
        
        try{
            Player P = col.transform.root.transform.gameObject.GetComponent<Player>();
            if (P)
            {
                PopupReference.gameObject.SetActive(false);
            }
        }catch{}
    }
}
