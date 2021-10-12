using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Beeper : MonoBehaviour
{
    //DATATYPES

    //PUBLIC COMPONENTS
    public TextMeshProUGUI WarningText;
    public AudioClip WarningSound;
    public AudioClip AlarmSound;

    //PUBLIC VARS

    //PUBLIC LISTS

    //COMPONENTS
    AudioSourceController ASC;

    //VARS
    IEnumerator WarnCoroutine;
    public bool isWarning = false;

    //LISTS

    void Start()
    {
        ASC = GetComponent<AudioSourceController>();
    }

    public void PlayAlarm()
    {
        if(isWarning)
            StopCoroutine(WarnCoroutine);

        WarnCoroutine = Alarm();
        StartCoroutine(WarnCoroutine);
    }

    IEnumerator Alarm()
    {
        ASC.AS.loop = true;
        ASC.Sound = AlarmSound;
        ASC.Play();

        WarningText.fontSize = 0.01f;
        WarningText.text = "NIGHT OVER";

        yield return new WaitForSeconds(3f);

        ASC.Stop();
    }

    public void ShowWarning(bool ApprovedWrong)
    {
        //Debug.Log(ApprovedWrong);

        if(isWarning)
            StopCoroutine(WarnCoroutine);

        isWarning = true;
        WarnCoroutine = Warn(ApprovedWrong);
        StartCoroutine(WarnCoroutine);
    }

    IEnumerator Warn(bool ApprovedWrong)
    {
        ASC.AS.loop = false;
        ASC.Sound = WarningSound;
        ASC.Play();

        WarningText.fontSize = 0.007f;
        if(ApprovedWrong)
            WarningText.text = "YOU HAVE BEEN FINED FOR APPROVING FORGED DOCUMENTS.";
        else
            WarningText.text = "YOU HAVE BEEN FINED FOR REJECTING VALID DOCUMENTS.";
        
        yield return new WaitForSeconds(0.25f);
        ASC.Play();
        
        yield return new WaitForSeconds(0.25f);
        ASC.Play();

        yield return new WaitForSeconds(0.25f);
        ASC.Play();

        yield return new WaitForSeconds(0.25f);
        ASC.Play();

        yield return new WaitForSeconds(0.25f);
        ASC.Play();

        yield return new WaitForSeconds(0.25f);
        ASC.Play();

        yield return new WaitForSeconds(0.25f);
        ASC.Play();

        
        
        yield return new WaitForSeconds(5f);
        
        isWarning = false;
    }
}