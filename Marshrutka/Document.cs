using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Document : MonoBehaviour
{
    //DATATYPES
    [System.Serializable]
    public struct Information
    {
        public FakedPart Fake;
        public bool Male;
        public string FirstName;
        public string LastName;
        public int Weight;
        public int HeightFoot;
        public int HeightInch;
        public int ExpirationDay;
        public GameSettings.Month ExpirationMonth;
        public int ExpirationYear;
        public string Citizenship;
        public Texture2D Photo;
    }

    public enum FakedPart
    {
        None,
        Gender,
        FirstName,
        LastName,
        Weight,
        Height,
        Expiration,
        Citizenship,
        Photo
    }

    //PUBLIC COMPONENTS
    public Passenger P;
    public Information Generated;
    public TextMeshProUGUI Gender;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Weight;
    public TextMeshProUGUI Height;
    public TextMeshProUGUI Expiration;
    public TextMeshProUGUI Citizenship;
    public RawImage Photo;

    //PUBLIC VARS

    //PUBLIC LISTS

    //COMPONENTS

    //VARS

    //LISTS

    void Start()
    {
        Generated = GameObject.FindWithTag("Manager").GetComponent<Manager>().GenerateInformation(P);
        P.Faked = Generated.Fake;

        Gender.text = "Gender : " + (Generated.Male ? "M" : "F");
        Name.text = Generated.LastName + ", " + Generated.FirstName;
        Weight.text = Generated.Weight + "KG";
        Height.text = Generated.HeightFoot + "\'" + Generated.HeightInch + "\"";

        Expiration.text = "Expires : " + Generated.ExpirationMonth.ToString() + ". " + 
        Generated.ExpirationDay.ToString() + GameSettings.GetPlaceSuffix(Generated.ExpirationDay) + ", " + 
        Generated.ExpirationYear.ToString();

        Citizenship.text = "Republic of " + Generated.Citizenship;

        Photo.texture = Generated.Photo;
    }

    public void PromptPlayer()
    {
        GameObject.FindWithTag("Player").GetComponent<PlayerController>().TogglePrompt();
    }
}