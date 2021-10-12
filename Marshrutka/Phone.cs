using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Phone : MonoBehaviour
{
    //DATATYPES

    //PUBLIC COMPONENTS
    public AudioClip BackspaceSound;
    public AudioClip CallSound;
    public AudioClip HangUpSound;
    public AudioClip PhoneCallSound;
    public TextMeshProUGUI PhoneScreen;
    public Voice TowVoice;
    public Voice RobotVoice;
    public Canvas C;

    //PUBLIC VARS
    public bool InCall = false;
    public string PhoneNumber = "";
    public Vector2 PhoneCallRandomness = new Vector2(15f, 60f);

    //PUBLIC LISTS
    public List<Number> SecretNumbers;
    public List<Caller> SpamCallers;
    public List<AudioClip> NumberSounds;

    //COMPONENTS
    AudioSourceController ASC;

    //VARS

    //LISTS

    void Start()
    {
        ASC = GetComponent<AudioSourceController>();
        C.worldCamera = GameObject.FindWithTag("Camera").GetComponent<Camera>();
        StartCoroutine(RandomCalls());
    }

    void Update()
    {
        ASC.Refresh();
    }

    IEnumerator RandomCalls()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(PhoneCallRandomness.x, PhoneCallRandomness.y));
            int RandomCaller = Random.Range(0, SpamCallers.Count);
            TextBox TB = null;
            while(!TB)
            {
                yield return new WaitForSeconds(0.5f);
                TB = GameObject.FindWithTag("TextBox").GetComponent<TextBox>();
            }

            if(!GetComponent<DashObject>().PickedUp && !GetComponent<DashObject>().Locked && !InCall && !TB.isActive)
            {
                if(GameObject.FindWithTag("Manager").GetComponent<Manager>().TimeIndex != 0 && GameObject.FindWithTag("Manager").GetComponent<Manager>().TimeIndex < 600)
                {

                    ASC.Sound = PhoneCallSound;
                    ASC.SetVolume(1f);
                    ASC.AS.loop = true;
                    ASC.Play();

                    PhoneNumber = "";
                    PhoneScreen.text = "Incoming : " + SpamCallers[RandomCaller].CallerID;
                    DashObject DO = GetComponent<DashObject>();

                    int Frames = 0;
                    bool Answered = false;
                    while(Frames < (7 * 60))
                    {
                        yield return null;
                        Frames++;

                        if(DO.PickedUp)
                        {
                            Answered = true;
                            Frames = 1000;
                        }
                    }

                    if(Answered)
                    {
                        PhoneScreen.text = SpamCallers[RandomCaller].CallerID;
                        InCall = true;
                        
                        ASC.AS.loop = false;
                        ASC.Stop();

                        
                        TB.CurrentVoice = SpamCallers[RandomCaller].CallerVoice;
                        TB.CurrentSource = ASC;
                        ASC.SetVolume(1f);

                        int Index = 0;

                        bool HungUp = false;

                        while(Index < SpamCallers[RandomCaller].Lines.Count && HungUp == false)
                        {
                            TB.DisplayText(SpamCallers[RandomCaller].Lines[Index]);
                            
                            while(TB.isPrinting && HungUp == false)
                            {
                                yield return null;

                                if(!DO.PickedUp)
                                {
                                    HungUp = true; 
                                }
                            }

                            /*
                            Frames = 0;
                            while(!HungUp && Frames < 30)
                            {
                                yield return null;
                                Frames++;
                                if(!DO.PickedUp)
                                {
                                    HungUp = true; 
                                }
                            }*/
                                
                            Index++;
                        }
                        
                        TB.Toggle();
                        ASC.SetPitch(1f);
                        
                        if(!HungUp)
                            GetComponent<DashObject>().RemoveFromHands();
                        
                        Clear();
                        InCall = false;
                    }
                    
                    ASC.AS.loop = false;
                    ASC.Stop();

                    PhoneScreen.text = "";
                    

                }
            }
        }
    }

    public void NumberPressed(int Number)
    {
        if(!InCall && GetComponent<DashObject>().PickedUp)
        {
            ASC.Sound = NumberSounds[Number];
            ASC.SetVolume(1f);
            ASC.Play();

            PhoneNumber += Number.ToString();
            PhoneScreen.text = PhoneNumber;
        }
    }

    public void Backspace()
    {
        if(!InCall && GetComponent<DashObject>().PickedUp)
        {
            ASC.Sound = BackspaceSound;
            ASC.SetVolume(1f);
            ASC.Play();

            if(PhoneNumber.Length > 0)
            {
                PhoneNumber = PhoneNumber.Substring(0, PhoneNumber.Length - 1);
                PhoneScreen.text = PhoneNumber;
            }
        }
    }

    public void Call()
    {
        if(!InCall && GetComponent<DashObject>().PickedUp)
        {
            if(PhoneNumber == GameObject.FindWithTag("Manager").GetComponent<Manager>().TowNumber)
            {
                StartCoroutine(PhoneCall());
            }else
            {
                if(PhoneNumber.Length > 0)
                {
                    if(isSecretNumber(PhoneNumber))
                        StartCoroutine(PhoneCallSecret());
                    else
                        StartCoroutine(PhoneCallWrong());
                }
            }
        }
            
    }

    bool isSecretNumber(string Num)
    {
        for(int i = 0; i < SecretNumbers.Count; i++)
        {
            if(Num == SecretNumbers[i].PhoneNumber)
            {
                return true;
            }
        }

        return false;
    }

    AudioClip GetSecretSound(string Num)
    {
        for(int i = 0; i < SecretNumbers.Count; i++)
        {
            if(Num == SecretNumbers[i].PhoneNumber)
            {
                if(SecretNumbers[i].CheatType != PhoneCheats.Cheat.none)
                    GetComponent<PhoneCheats>().ActivateCheat(SecretNumbers[i].CheatType);

                return SecretNumbers[i].AnswerSound;
            }
        }

        return BackspaceSound;
    }

    IEnumerator PhoneCall()
    {
        InCall = true;
        ASC.Sound = CallSound;
        ASC.SetVolume(1f);
        ASC.AS.loop = true;
        ASC.Play();

        yield return new WaitForSeconds(2f);
        ASC.AS.loop = false;
        ASC.Stop();

        TextBox TB = GameObject.FindWithTag("TextBox").GetComponent<TextBox>();
        TB.CurrentVoice = TowVoice;
        TB.CurrentSource = ASC;
        ASC.SetVolume(1f);
        TB.DisplayText(GameSettings.TowCompanyAnswerText);
        yield return new WaitForSeconds(3f);
        TB.Toggle();
        ASC.SetPitch(1f);

        Clear();
        InCall = false;

        GetComponent<DashObject>().RemoveFromHands();
        GetComponent<DashObject>().Locked = true;

        yield return new WaitForSeconds(4f);

        GameObject.FindWithTag("Manager").GetComponent<Manager>().MUI.ShowResultsMenu();

        GetComponent<DashObject>().Locked = false;
    }

    IEnumerator PhoneCallSecret()
    {
        InCall = true;
        ASC.Sound = CallSound;
        ASC.SetVolume(1f);
        ASC.AS.loop = true;
        ASC.Play();

        yield return new WaitForSeconds(2f);
        ASC.AS.loop = false;
        ASC.Stop();

        AudioClip SecretSound = GetSecretSound(PhoneNumber);
        ASC.Sound = SecretSound;
        ASC.SetVolume(1f);
        ASC.Play();
        yield return new WaitForSeconds(SecretSound.length + 1f);

        Clear();
        InCall = false;
    }

    IEnumerator PhoneCallWrong()
    {
        InCall = true;
        ASC.Sound = CallSound;
        ASC.SetVolume(1f);
        ASC.AS.loop = true;
        ASC.Play();

        yield return new WaitForSeconds(4f);
        ASC.AS.loop = false;
        ASC.Stop();

        TextBox TB = GameObject.FindWithTag("TextBox").GetComponent<TextBox>();
        TB.CurrentVoice = RobotVoice;
        TB.CurrentSource = ASC;
        ASC.SetVolume(1f);
        TB.DisplayText("This number cannot be reached at this time.");
        yield return new WaitForSeconds(3f);
        TB.Toggle();
        ASC.SetPitch(1f);

        Clear();
        InCall = false;
    }

    public void Clear()
    {
        ASC.Sound = HangUpSound;
        ASC.SetVolume(1f);
        ASC.Play();

        PhoneNumber = "";
        PhoneScreen.text = PhoneNumber;
    }
}
