using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Number", menuName = "Number")]
public class Number : ScriptableObject
{
    public string PhoneNumber;
    public AudioClip AnswerSound;
    public PhoneCheats.Cheat CheatType;
}
