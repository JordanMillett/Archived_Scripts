using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hinge : MonoBehaviour
{
    public bool SingleUse = true;
    public float Speed = 0.05f;
    public float AngleOffset = 120f;

    bool Activated = false;

    public enum Axis
	{
		X_Axis,
		Y_Axis,
		Z_Axis
	};

    public Axis HingeAxis = Axis.X_Axis;

    public void Activate()
    {

        if(!Activated)
        {

            Activated = true;

            StartCoroutine(Open());

        }

    }

    IEnumerator Open()
    {

        Vector3 Offset = Vector3.zero;

        if(HingeAxis == Axis.X_Axis)
            Offset = new Vector3(AngleOffset, 0f, 0f);
        if(HingeAxis == Axis.Y_Axis)
            Offset = new Vector3(0f, AngleOffset, 0f);
        if(HingeAxis == Axis.Z_Axis)
            Offset = new Vector3(0f, 0f, AngleOffset);

        float RotationAlpha = 0f;
        Vector3 StartRotation = this.transform.localEulerAngles;

        while(RotationAlpha < 1f)
        {

            this.transform.localEulerAngles = Vector3.Lerp(StartRotation, Offset, RotationAlpha);
            RotationAlpha += Speed;
            yield return null;

        }

    }
}
