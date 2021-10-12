using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettings
{
    public static SaveController SC;
    public static SettingsManager SM;

    public static readonly int StartingMoney = 50;
    public static readonly int Bills = 20;
    public static readonly int BillMultiplier = 2;
    public static readonly int PassengerIncome = 10;
    public static readonly int FineCost = 5;
    public static readonly int FullRefillCost = 20;
    public static readonly int NightDuration = 300;     //400
    public static readonly float PercentFake = 0.4f;    //0.20f
    public static readonly float ClosedChance = 0.25f;   //0.25f

    public static readonly Vector2Int WeightRange = new Vector2Int(40, 91);  //always add 1 to ranges
    public static readonly Vector2Int HeightFootRange = new Vector2Int(4, 7);
    public static readonly Vector2Int HeightInchRange = new Vector2Int(0, 12);

    public static readonly Vector2 HeightLerp = new Vector2(0.95f, 1.05f);
    public static readonly Vector2 WeightLerp = new Vector2(0.8f, 1.20f);

    public static readonly Vector2Int ExpirationOffsetRange = new Vector2Int(1, 15);

    public static readonly string TowCompanyAnswerText = "Ugh, I imagine you're stuck somewhere? we will, be there soon.";

    public static readonly List<string> Republics = new List<string>()
    {
        "Rosinsk",
        "Korovrov",
        "Kumevat",
        "Mupeysk",
        "Nakovo",
        "Sivkar"
    };

    public static readonly List<string> FakeRepublics = new List<string>()
    {
        "Not Sure?",
        "REDACTED",
        "Unavailable"
    };

    public static readonly List<string> MaleFirstNames = new List<string>()
    {
        "Aleksander",
        "Aleksei",
        "Boris",
        "Dmitri",
        "Igor",
        "Oleg",
        "Vladimir",
        "Yuri",
        "Artyom",
        "Mikhail",
        "Ivan",
        "George",
    };

    public static readonly List<string> FemaleFirstNames = new List<string>()
    {
        "Ira",
        "Sofia",
        "Anya",
        "Maria",
        "Alina",
        "Alyona",
        "Mila",
        "Polina",
        "Alisa",
        "Kira",
        "Yulia",
        "Sasha",
    };

    public static readonly List<string> MaleLastNames = new List<string>()
    {
        "Petrov",
        "Volkov",
        "Ivanov",
        "Smirnov",
        "Popov",
        "Sokolov",
        "Lebedev",
        "Orlov",
        "Komarov",
        "Gusev",
        "Titov"
    };

    public static readonly List<string> FemaleLastNames = new List<string>()
    {
        "Petrova",
        "Volkova",
        "Ivanova",
        "Smirnova",
        "Popova",
        "Sokolova",
        "Lebedeva",
        "Orlova",
        "Komarova",
        "Guseva",
        "Titova"
    };

    public static readonly List<int> MonthDays = new List<int>()
    {
        31,
        28,
        31,
        30,
        31,
        30,
        31,
        31,
        30,
        31,
        30,
        31
    };

    public enum Month
    {
        Jan,
        Feb,
        Mar,
        Apr,
        May,
        Jun,
        Jul,
        Aug,
        Sep,
        Oct,
        Nov,
        Dec
    }

    //MOVE LATER
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
}