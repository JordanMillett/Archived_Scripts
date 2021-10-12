using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    [System.Serializable]
    public struct CreditsImage
    {
        public Texture Image;
        public Vector2 Dimensions;
    }

    List<string> CreditsText = new List<string>()
    {
        "^Programmer/Artist",
        "Jordan Millett",
        "",
        "^Textures",
        "textures.com",
        "thispersondoesnotexist.com",
        "",
        "^Fonts",
        "1001fonts.com",
        "abdulmakesfonts - Kroftsmann Font",
        "",
        "^Audio",
        "freesound.org",
        "ValentinSosnitskiy - Guitar Jam in Ableton Live",
        "ValentinSosnitskiy - Blues midnight Improvisation on Electric-Guitar st1",
        "joshuaempyre - Groovy Guitar - LOOP.wav",
        "frankum - Loop in progression - Dance music",
        "joedeshon - rocker_switch.wav",
        "themfish - glass_house1.wav",
        "MoveAwayPodcast - Gong-like Pot Smash.m4a",
        "InspectorJ - Ambience, Night Wildlife, A.wav",
        "Anton - wind1.wav",
        "qubodup - Car Engine Loop",
        "",
        "^Made possible with",
        "Unity3D (2021.1.11f1 Personal)",
        "Blender (2.90.1)",
        "GIMP (2.10.22)",
        "Audacity (2.4.2)",
        "",
    };

    string EndNote = "Thanks for playing!";

    [HideInInspector]
    public List<RectTransform> CreditsObjects;
    public GameObject TextPrefab;
    public GameObject ImagePrefab;

    IEnumerator ShowCoroutine;

    float ScrollSpeed = 1f;
    float YScroll = 0f;

    public List<CreditsImage> Images;

    void OnEnable()
    {
        ShowCoroutine = StartCredits();
        StartCoroutine(ShowCoroutine);

        GetComponent<AudioSourceController>().Play();
    }

    void OnDisable()
    {
        StopCoroutine(ShowCoroutine);
    }

    IEnumerator StartCredits()
    {
        for(int i = 0; i < CreditsObjects.Count; i++)
            Destroy(CreditsObjects[i].gameObject);

        CreditsObjects.Clear();

        YScroll = 0f;

        for(int i = 0; i < CreditsText.Count; i++)      //TEXTS
        {
            GameObject NewText = Instantiate(TextPrefab, Vector3.zero, Quaternion.identity);
            NewText.transform.SetParent(this.transform);

            CreditsObjects.Add(NewText.GetComponent<RectTransform>());
            CreditsObjects[i].anchoredPosition = new Vector2(0f, YScroll);

            if(CreditsText[i].Length == 0)
            {
                YScroll -= 150f;
            }else
            {   
                NewText.GetComponent<RectTransform>().localScale = Vector3.one;
                if(CreditsText[i][0] == '^')
                {
                    NewText.GetComponent<TextMeshProUGUI>().text = CreditsText[i].Substring(1);
                    NewText.GetComponent<TextMeshProUGUI>().fontSize = NewText.GetComponent<TextMeshProUGUI>().fontSize * 2f;
                    YScroll -= 80f;
                }else
                {
                    NewText.GetComponent<TextMeshProUGUI>().text = CreditsText[i];;
                    YScroll -= 40f;
                }

                
            }
        }

        for(int i = 0; i < Images.Count; i++)             //IMAGES
        {
            GameObject NewImage = Instantiate(ImagePrefab, Vector3.zero, Quaternion.identity);
            NewImage.transform.SetParent(this.transform);

            CreditsObjects.Add(NewImage.GetComponent<RectTransform>());
            CreditsObjects[CreditsText.Count + i].anchoredPosition = new Vector2(0f, YScroll);

            NewImage.GetComponent<RawImage>().texture = Images[i].Image;
            NewImage.GetComponent<RectTransform>().sizeDelta = Images[i].Dimensions;
            NewImage.GetComponent<RectTransform>().localScale = Vector3.one;
            YScroll -= Images[i].Dimensions.y;
        }

        //ENDING NOTE
        YScroll -= 200f;
        GameObject EndText = Instantiate(TextPrefab, Vector3.zero, Quaternion.identity);
        EndText.transform.SetParent(this.transform);
        CreditsObjects.Add(EndText.GetComponent<RectTransform>());
        EndText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, YScroll);
        EndText.GetComponent<RectTransform>().localScale = Vector3.one;
        EndText.GetComponent<TextMeshProUGUI>().text = EndNote;
        EndText.GetComponent<TextMeshProUGUI>().fontSize = EndText.GetComponent<TextMeshProUGUI>().fontSize * 3f;

        while(CreditsObjects[CreditsObjects.Count - 1].anchoredPosition.y < 400f)
        {
            for(int i = 0; i < CreditsObjects.Count; i++)
                CreditsObjects[i].anchoredPosition += new Vector2(0f, ScrollSpeed);

            yield return null;
        }

    
        GameObject.FindWithTag("MenuManager").GetComponent<MenuManager>().GoToLastMenu();

        while(true) //wait to be terminated
            yield return null;
    }
}