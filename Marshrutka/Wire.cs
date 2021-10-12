using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    LineRenderer LR;
    public int Resolution = 2;
    public float Sag = 1f;
    Vector3[] Original = new Vector3[2];

    public void Initialize(Vector3 Start, Vector3 End)
    {
        Original = new Vector3[2];
        Original[0] = Start;
        Original[1] = End;

        MakeLines();
    }

    void MakeLines()
    {
        LR = GetComponent<LineRenderer>();

        if(Resolution > 2)
        {
            Vector3[] NewPoints = new Vector3[Resolution];
            NewPoints[0] = Original[0];
            NewPoints[Resolution - 1] = Original[1];
            LR.positionCount = Resolution;

            for(int i = 1; i < Resolution - 1; i++)
            {
                NewPoints[i] = Vector3.Lerp(Original[0], Original[1], (float) i/(Resolution - 1)) + GetSag(i);
            }

            LR.SetPositions(NewPoints);

        }else
        {
            LR.positionCount = 2;
            LR.SetPositions(Original);
        }
    }

    Vector3 GetSag(int Index)
    {
        float Drop = Mathf.Sin(Mathf.Lerp(0f, 180f, (float) Index/(Resolution - 1)) * Mathf.Deg2Rad);

        return new Vector3(0f, -Sag * Drop, 0f);
    }
}
