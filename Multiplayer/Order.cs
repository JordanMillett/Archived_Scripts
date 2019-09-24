using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Order", menuName = "Order")]
public class Order : ScriptableObject
{
    public enum Ingredients
	{
		Top_Bun,
        Cheese,
        Patty,
        Bottom_Bun
	};

    public int OrderNum;
    public List<Ingredients> Contents = new List<Ingredients>();

}
