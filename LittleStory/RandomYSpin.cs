using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomYSpin : MonoBehaviour
{
    public void Spin()
    {

        this.transform.localEulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);

    }
}
