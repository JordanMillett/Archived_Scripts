using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlot : MonoBehaviour
{
    //DATATYPES

    //PUBLIC COMPONENTS
    public GameObject CheaterImage;
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI NightText;
    public TextMeshProUGUI MoneyText;
    public GameObject NewGame;
    public GameObject DeleteButton;

    //PUBLIC VARS
    public int Index;

    //PUBLIC LISTS

    //COMPONENTS

    //VARS

    //LISTS

    void OnEnable()
    {
        Refresh();
    }

    void Refresh()
    {
        if(GameSettings.SC.Saves[Index].New)
        {
            CheaterImage.transform.gameObject.SetActive(false);
            TitleText.transform.gameObject.SetActive(false);
            NightText.transform.gameObject.SetActive(false);
            MoneyText.transform.gameObject.SetActive(false);
            NewGame.transform.gameObject.SetActive(true);
            DeleteButton.transform.gameObject.SetActive(false);
        }else
        {
            TitleText.transform.gameObject.SetActive(true);
            NightText.transform.gameObject.SetActive(true);
            MoneyText.transform.gameObject.SetActive(true);
            NewGame.transform.gameObject.SetActive(false);
            DeleteButton.transform.gameObject.SetActive(true);

            NightText.text = "Night " + GameSettings.SC.Saves[Index].Night.ToString();
            MoneyText.text = GameSettings.SC.Saves[Index].Money.ToString() + ".00";

            CheaterImage.transform.gameObject.SetActive(GameSettings.SC.Saves[Index].Cheating);
        }
    }

    public void DeleteSave()
    {
        GameSettings.SC.DeleteSave(Index);
        Refresh();
    }
}