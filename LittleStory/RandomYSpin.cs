using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomYSpin : MonoBehaviour
{
    public Vector2 ScaleRange;

    public void Spin()
    {

        this.transform.localEulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);

    }

    public void Scale()
    {
        float newScale = Random.Range(ScaleRange.x, ScaleRange.y);
        this.transform.localScale = new Vector3(newScale, newScale, newScale);
    }
}
