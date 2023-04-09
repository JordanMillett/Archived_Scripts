using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu_Game : MonoBehaviour
{
    public Transform Popups;
    public RectTransform Canvas;

    public SimpleBar Health;
    public SimpleBar Shields;
    public SimpleBar Stamina;
    public SimpleBar Healing;
    
    public Element_Minimap E_Minimap;
    
    public GameObject ContainerPopupPrefab;
    public GameObject EnemyPopupPrefab;
    public List<GameObject> PopupPrefabs;

    public Popup_Crosshair Crosshair;
    public Transform CrosshairTarget;

    public TextMeshProUGUI Primary;
    public TextMeshProUGUI Secondary;

    Life _life;
    
    void Start()
    {
        _life = Player.P.gameObject.GetComponent<Life>();
    } 
    
    void Update()
    {
        Health.Current = _life.Health;
        Health.Max = _life.MaxHealth;
        Shields.Current = _life.Shields;
        Shields.Max = _life.MaxShields;
        Stamina.Current = Player.P.Stamina;
        Stamina.Max = Game.RunData.ActiveRaider.MaxStamina;
        Healing.Current = Game.RunData.HealingMatter;
        Healing.Max = Game.SaveData.MaxHealingMatter;
        
        
        Cursor.visible = false;
        CrosshairTarget.transform.position = Player.P.AimAt;
        Crosshair.Primary.Max = Player.P.Primary.Info.GetRPMDelay(true);
        Crosshair.Primary.Current = Player.P.Primary.Cooldown;
        
        if(Player.P.Secondary)
        {
            Crosshair.Secondary.Max = Player.P.Secondary.Info.GetRPMDelay(true);
            Crosshair.Secondary.Current = Player.P.Secondary.Cooldown;
        }else
        {
            Crosshair.Secondary.Max = 1f;
            Crosshair.Secondary.Current = 0f;
        }

        Primary.text = Player.P.Primary.Info.Name;

        if(Player.P.Secondary)
            Secondary.text = Player.P.Secondary.Info.Name;
        else
            Secondary.text = "";
    }
    
    public Popup_Container CreateContainerPopup()
    {
        return CreatePopupGameObject(ContainerPopupPrefab).GetComponent<Popup_Container>();
    }
    
    public Popup_Enemy CreateEnemyPopup()
    {
        return CreatePopupGameObject(EnemyPopupPrefab).GetComponent<Popup_Enemy>();
    }

    public Popup CreatePopup(Interactable.Types Type)
    {
        return CreatePopupGameObject(PopupPrefabs[(int)Type]).GetComponent<Popup>();
    }
    
    GameObject CreatePopupGameObject(GameObject Prefab)
    {
        GameObject PopupInstance = Instantiate(Prefab , Vector3.zero, Quaternion.identity);
        PopupInstance.transform.SetParent(Popups.transform);
        PopupInstance.transform.localScale = Vector3.one;
        PopupInstance.transform.localPosition = Vector3.zero;
        PopupInstance.transform.localEulerAngles = Vector3.zero;
        PopupInstance.GetComponent<Popup>().Cam = this.transform.parent.parent.gameObject.GetComponent<Camera>();
        PopupInstance.GetComponent<RectTransform>().localPosition = new Vector2(5000f, 5000f);

        return PopupInstance;
    }
}
