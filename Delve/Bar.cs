using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    public float Amount = 0f;

    RectTransform Fill;

    void Start()
    {

        Fill = transform.GetChild(0).GetComponent<RectTransform>();

    }

    public void UpdateBar()
    {
        Fill.localScale = new Vector3(Amount, 1f ,1f);
    }
}
