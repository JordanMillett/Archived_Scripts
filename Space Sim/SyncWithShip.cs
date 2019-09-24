using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class SyncWithShip : MonoBehaviour
{

    Ship S;
    Simple_Bar B;
    TextMeshProUGUI T;

	public enum type
	{
		Bar,
		Number,
		Notification
	};

	public enum ship_data
	{
		Max_Hull,
		Hull,
		Max_Shields,
		Shields,
		Max_Fuel,
		Fuel,
		Max_Power,
		Power,
		Max_Oxygen,
		Oxygen,
		Max_Speed,
		Speed,
	};

	public type Selector;
	public ship_data Data;
	public ship_data Data_Max;
    public string Suffix;

    void Start()
    {
        S = GameObject.FindWithTag("Ship").GetComponent<Ship>();
        if(Selector == type.Bar)
            B = this.gameObject.GetComponent<Simple_Bar>();

		if(Selector == type.Notification)
        {
            T = this.gameObject.GetComponent<TextMeshProUGUI>();
            S.Note.TextBoxes.Add(T);
        }

		if(Selector == type.Number)
            T = this.gameObject.GetComponent<TextMeshProUGUI>();
          
    }

    void Update()
    {

		if(Selector == type.Bar)
        {
            B.Current = ReturnData(Data);
            B.Max = ReturnData(Data_Max);
        }

		if(Selector == type.Number)
        {

           T.text = ReturnData(Data) + " " + Suffix;

        }

    }

    public float ReturnData(ship_data D)
    {

        switch(D)
        {
            case ship_data.Max_Hull:    return S.Max_Hull;
            case ship_data.Hull:        return S.Hull;
            case ship_data.Max_Shields: return S.Max_Shields; 
			case ship_data.Shields:     return S.Shields; 
			case ship_data.Max_Fuel:    return S.Max_Fuel;
			case ship_data.Fuel:        return S.Fuel; 
			case ship_data.Max_Power:   return S.Max_Power;
			case ship_data.Power:      	return S.Power;
			case ship_data.Max_Oxygen:	return S.Max_Oxygen;
            case ship_data.Oxygen:		return S.Oxygen;
			case ship_data.Max_Speed:   return S.Max_Speed;
			case ship_data.Speed:       return S.Speed;
			default :                   return 0f;
        }

    }
}
