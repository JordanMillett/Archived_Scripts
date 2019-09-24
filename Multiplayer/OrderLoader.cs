using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OrderLoader : MonoBehaviour
{

    public Order O;
    public TextMeshProUGUI OrderNum;
    public GameObject Items;

    float y_loc = 2;

    void Start()
    {
        OrderNum.text = O.OrderNum.ToString();

        for(int i = 0; i < O.Contents.Count;i++)
            SpawnPicture(i);

    }

    void SpawnPicture(int i)
    {
        string Object = "";

        if(O.Contents[i] == Order.Ingredients.Top_Bun)
            Object = "Top Bun";
        if(O.Contents[i] == Order.Ingredients.Cheese)
            Object = "Cheese";
        if(O.Contents[i] == Order.Ingredients.Patty)
            Object = "Patty";
        if(O.Contents[i] == Order.Ingredients.Bottom_Bun)
            Object = "Bottom Bun";
            
        GameObject Item = Instantiate(Resources.Load<GameObject>("Menu/" + Object),Vector3.zero,Quaternion.identity);
        Item.transform.SetParent (Items.transform);
        RectTransform RT = Item.GetComponent<RectTransform>();
        RT.anchoredPosition = new Vector3(0f,y_loc,0f);
        RT.localScale = Vector3.one;
        RT.localEulerAngles = Vector3.zero;
        RT.localPosition = new Vector3(RT.localPosition.x,RT.localPosition.y,0f);
        y_loc--;
        Item.name = Object;




    }


}
