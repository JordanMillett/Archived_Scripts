using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuneMenu : MonoBehaviour
{
    public UIBar AccelerationBar;
    public UIBar MaxSpeedBar;

    CarMenu CM;

    List<EngineKit> EKs = new List<EngineKit>();

    public int UpgradeIndex = 0;

    public void SetIndex(int Index)
    {
        if(PlayerInfo.Balance >= EKs[Index].Cost)
        {
            PlayerInfo.Balance -= EKs[Index].Cost;
            UpgradeIndex = Index;
            CM.ApplyEngine();
            CM.SetScreen(0);
        }
    }

    public void Init(CarMenu _CM, List<EngineKit> _eks, int Index)
    {
        CM = _CM;
        UpgradeIndex = Index;
        EKs = _eks;

        UpdateBars(UpgradeIndex);
    }

    public void UpdateBars(int Index)
    {
        float TopAcc = 0f;
        float TopSpeed = 0f;

        foreach(EngineKit EK in EKs)
        {
            if(EK.Acceleration > TopAcc)
                TopAcc = EK.Acceleration;

            if(EK.TopSpeed > TopSpeed)
                TopSpeed = EK.TopSpeed;
        }

        AccelerationBar.Max = TopAcc;
        MaxSpeedBar.Max = TopSpeed;

        AccelerationBar.Current = EKs[Index].Acceleration;
        MaxSpeedBar.Current = EKs[Index].TopSpeed;
    }

    public void Hovered(int Index)
    {
        UpdateBars(Index);
    }
}
