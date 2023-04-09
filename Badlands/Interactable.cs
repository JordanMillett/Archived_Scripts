using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum Types
    {
        Lab,
        Escape,
        Vault,
        Power
    }

    public Types Type;

    Popup PopupReference;
    
    void OnTriggerEnter(Collider col)
    {
        try{
            Player P = col.transform.root.transform.gameObject.GetComponent<Player>();
            if (P)
            {
                if(!PopupReference)
                {
                    PopupReference = UIManager.UI.M_Game.CreatePopup(Type);
                    PopupReference.Target = this.transform;
                }else
                {
                    PopupReference.gameObject.SetActive(true);
                }
            }
        }catch{}
    }

    void OnTriggerExit(Collider col)
    {
        try{
            Player P = col.transform.root.transform.gameObject.GetComponent<Player>();
            if (P)
            {
                PopupReference.gameObject.SetActive(false);
            }
        }catch{}
    }
}
