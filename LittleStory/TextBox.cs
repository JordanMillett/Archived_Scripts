using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{

    public bool isActive = false;
    public GameObject Empty;
    public TextMeshProUGUI TextField;
    public TextMeshProUGUI NameField;
    public List<TextMeshProUGUI> Choices;
    public RawImage Selection;
    public int SelectionIndex = 0;

    public Color SelectionColor;

    IEnumerator PrintCoroutine;
    public bool isPrinting = false;
    public float PrintDelay = 0.02f;

    public Voice CurrentVoice;
    public AudioSource CurrentSource;

    string Alphabet = "abcdefghijklmnopqrstuvwxzy";

    public void DisplayText(string Text)
    {
        if(isPrinting)
            StopCoroutine(PrintCoroutine);

        if(!isActive)
            Toggle();

        Selection.color = Color.clear;
        foreach(TextMeshProUGUI T in Choices)
            T.text = "";
        SelectionIndex = 0;
        Selection.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Choices[0].GetComponent<RectTransform>().anchoredPosition.y);

        PrintCoroutine = PrintText(Text);
        StartCoroutine(PrintCoroutine);

    }

    public void ShiftSelection(bool Up, int Size)
    {

        if(Up && SelectionIndex >= 1)
        {
            SelectionIndex--;
        }
        else if(!Up && SelectionIndex < (Size - 1))
        {
            SelectionIndex++;
        }

        Selection.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Choices[SelectionIndex].GetComponent<RectTransform>().anchoredPosition.y);

    }

    public void FastForward(string Text)
    {

        if(isPrinting)
            StopCoroutine(PrintCoroutine);

        isPrinting = false;

        TextField.text = Text;

        

    }

    public void UpdateChoices(List<string> Texts)
    {

        
        if(isPrinting)
            StopCoroutine(PrintCoroutine);
        TextField.text = "";
            
        Selection.color = SelectionColor;
        foreach(TextMeshProUGUI T in Choices)
            T.text = "";

        for(int i = 0; i < Texts.Count; i++)
        {   
            Choices[i].text = Texts[i];
        }

    }

    IEnumerator PrintText(string Text)
    {
        isPrinting = true;
        int TotalChars = Text.Length + 1;

        for(int i = 0; i < TotalChars; i++)
        {
            yield return new WaitForSeconds(PrintDelay);
            TextField.text = Text.Substring(0, i);
            if(!CurrentSource.isPlaying && i < Text.Length)
                PlaySound(Text[i]);
        }

        isPrinting = false;

    }

    void PlaySound(char Letter)
    {

        Letter = char.ToLower(Letter);

        for(int i = 0; i < Alphabet.Length; i++)
        {

            if(Letter == Alphabet[i])
            {
                CurrentSource.clip = CurrentVoice.Letters[i];
                CurrentSource.Play();
            }

        }

    }

    public void SetName(string Text)
    {

        NameField.text = Text;

    }

    public void Toggle()
    {

        isActive = !isActive;
        if(!isActive)
        {
            if(isPrinting)
                StopCoroutine(PrintCoroutine);
            isPrinting = false;
            TextField.text = "";
            NameField.text = "";
            CurrentVoice = null;
            CurrentSource = null;
            Selection.color = Color.clear;
            SelectionIndex = 0;
            Selection.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Choices[0].GetComponent<RectTransform>().anchoredPosition.y);
            foreach(TextMeshProUGUI T in Choices)
                T.text = "";
        }   
        Empty.SetActive(isActive);
        

    }

}
