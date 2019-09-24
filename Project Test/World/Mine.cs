using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour 
{

	public InvInfo Item;
	public int hitAmount;
	public int hitsLeft;
    public float RanScale = 0;

    void Start()
    {
        if(RanScale > 0)
        {

            float RanScaleMax = RanScale;
            RanScale = Random.Range(5f,RanScaleMax);
            this.gameObject.transform.localScale = Vector3.one * RanScale;

            hitsLeft += Mathf.RoundToInt(RanScale - 5f);

        }
    }

	void Update()
	{
		if(hitsLeft <= 0)
		{
			Destroy (this.gameObject);
		}
	}

}
