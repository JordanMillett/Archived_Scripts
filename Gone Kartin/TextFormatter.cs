using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextFormatter
{
    public static string GetPlaceSuffix(int Number)
    {
        if(Number == 11 || Number == 12 ||Number == 13)
            return "th";

        int Last = Number % 10;

        switch(Last)
        {
            case 0 : return "th";
            case 1 : return "st";
            case 2 : return "nd";
            case 3 : return "rd";
            case 4 : return "th";
            case 5 : return "th";
            case 6 : return "th";
            case 7 : return "th";
            case 8 : return "th";
            case 9 : return "th";
        }

        return "ERROR";
    }

    public static string GetCountDownFormatted(int Number)
    {
        switch(Number)
        {
            case -1 : return "";
            case 0 : return "GO";
            case 1 : return "1";
            case 2 : return "2";
            case 3 : return "3";
        }

        return "ERROR";
    }

    public static string GetTimeFormatted(float Time)
    {
        string Formatted = "ERROR";

        int Minutes = (int)Mathf.Floor((Time)/60);
        string MinutesSpacer = "";
        if(Minutes < 10)
            MinutesSpacer = "0";

        int Seconds = (int)Mathf.Floor(((Time) % 60));
        string SecondsSpacer = "";
        if(Seconds < 10)
            SecondsSpacer = "0";

        int Milliseconds = (int)Mathf.Floor(((Time) % 1) * 100f);
        string MillisecondsSpacer = "";
        if(Milliseconds < 10)
            MillisecondsSpacer = "0";

        Formatted = MinutesSpacer + Minutes + ":" + SecondsSpacer + Seconds + ":" + MillisecondsSpacer + Milliseconds;

        return Formatted;
    }
}
