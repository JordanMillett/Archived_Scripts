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

    IEnumerator PrintCoroutine;
    public bool isPrinting = false;
    public float PrintDelay = 0.02f;

    public Voice CurrentVoice;
    public AudioSourceController CurrentSource;

    string Alphabet = "abcdefghijklmnopqrstuvwxzy";

    //Presets for (Person, Player, and Radio)

    public void DisplayText(string Text)
    {
        if(isPrinting)
            StopCoroutine(PrintCoroutine);

        if(!isActive)
            Toggle();

        PrintCoroutine = PrintText(Text);
        StartCoroutine(PrintCoroutine);

    }

    public void FastForward(string Text)
    {

        if(isPrinting)
            StopCoroutine(PrintCoroutine);

        isPrinting = false;

        TextField.text = Text;

        

    }

    IEnumerator PrintText(string Text)
    {
        isPrinting = true;
        int TotalChars = Text.Length + 1;
        Text += ' ';

        for(int i = 0; i < TotalChars; i++)
        {
        
            //while(Time.timeScale == 0f)
                //yield return null;

            TextField.text = Text.Substring(0, i + 1);
            if(!CurrentSource.AS.isPlaying && i < Text.Length)
                PlaySound(Text[i]);

            if(Text[i] == '.' || Text[i] == '?')
            {
                yield return new WaitForSeconds(0.4f);
            }else if(Text[i] == ',')
            {
                yield return new WaitForSeconds(0.2f);
            }else
            {
                yield return new WaitForSeconds(PrintDelay);
            }
        }

        yield return new WaitForSeconds(0.75f); //END UP MESSAGE PAUSE

        isPrinting = false;

    }

    void PlaySound(char Letter)
    {

        Letter = char.ToLower(Letter);

        for(int i = 0; i < Alphabet.Length; i++)
        {

            if(Letter == Alphabet[i])
            {
                CurrentSource.Sound = CurrentVoice.Letters[i];
                CurrentSource.SetPitch(CurrentVoice.Pitch);
                CurrentSource.Play();
            }

        }

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
            CurrentVoice = null;
            CurrentSource = null;
        }   
        Empty.SetActive(isActive);
        

    }

}