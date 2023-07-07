using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UserScore : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Money;
    public TextMeshProUGUI Deliveries;
    public TextMeshProUGUI Score;
    public TextMeshProUGUI Grade;
    
    public void Set(PlayerInfo PI)
    {
        Name.text = PI.Name;
        Money.text = PI.Balance.ToString();
        Deliveries.text = PI.TotalDeliveries.ToString();
        Score.text = PI.TotalScore.ToString();
        Grade.text = PI.CalculateGrade();
    }
}
