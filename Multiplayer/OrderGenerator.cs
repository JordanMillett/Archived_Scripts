using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderGenerator : MonoBehaviour
{   
    public int CurrentOrder = 0;
    
    public int MakeAmount = 1;
    public int MaxOrderSize = 4;

    public float DelayTime = 2f;

    IEnumerator Start()
    {
        for(int i = 0;i < MakeAmount;i++)
        {
            MakeTicket();
            yield return new WaitForSeconds(DelayTime);
        }

    }

    public void MakeTicket()
    {
        CurrentOrder++;
        Order O = ScriptableObject.CreateInstance<Order>();
        O.OrderNum = CurrentOrder;

        int ContentsSize = Random.Range(3,MaxOrderSize + 1);

        O.Contents.Add(Order.Ingredients.Top_Bun);

        for(int i = 0; i < ContentsSize - 2; i++)
        {

            AddRandomIngredient(O);

        }

        O.Contents.Add(Order.Ingredients.Bottom_Bun);

        SpawnTicket(O);

    }

    void SpawnTicket(Order O)
    {

        GameObject Ticket = Instantiate(Resources.Load<GameObject>("Blank Order"),new Vector3(0f,5f,0f),Quaternion.identity);
        Ticket.GetComponent<OrderLoader>().O = O;

    }

    void AddRandomIngredient(Order O)
    {

        int Index = Random.Range(0,2);

        switch(Index)
        {

            case 0 :
                O.Contents.Add(Order.Ingredients.Patty);
                break;
            case 1:
                O.Contents.Add(Order.Ingredients.Cheese);
                break;
            default:
                O.Contents.Add(Order.Ingredients.Patty);
                break;

        }

    }
}
