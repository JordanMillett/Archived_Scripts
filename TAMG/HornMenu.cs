using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornMenu : MonoBehaviour
{
    CarMenu CM;

    List<Horn> Horns = new List<Horn>();

    public void SetIndex(int Index)
    {
        if(PlayerInfo.Balance >= Horns[Index].Cost)
        {
            PlayerInfo.Balance -= Horns[Index].Cost;
            CM.SetHorn(Index);
            CM.SetScreen(0);
        }
    }

    public void Init(CarMenu _CM, List<Horn> _Horns)
    {
        CM = _CM;
        Horns = _Horns;
    }

    public void Hovered(int Index)
    {
        CM.PlayHorn(Index);
    }
}
