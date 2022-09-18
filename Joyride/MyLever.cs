using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyLever : MonoBehaviour
{
    public float alpha = 0.5f;
    public float centeringSpeed = 1.5f;
    public bool startAtZero = false;
    public HingeJoint _hingeJoint;
    public AudioSource _audioSource;
    public AudioClip med;
    public AudioClip high;
    public bool centering = false;


    float limit = 50f;
    float deadZone = 10f;
    float lastClick = 0.5f;

    Quaternion defaultRot;

    void Start()
    {
        limit = _hingeJoint.limits.max;
        if (startAtZero)
        {
            _hingeJoint.transform.localEulerAngles = new Vector3(-limit, 0f, 0f);
            defaultRot = _hingeJoint.transform.localRotation;
        }else
        {
            defaultRot = Quaternion.identity;
        }


    }
    
    void FixedUpdate()
    {
        float angle = _hingeJoint.transform.localEulerAngles.x;

        if (angle >= 360f - (limit + deadZone))
        {
            if(angle <= 360f - limit)
                alpha = 0f;
            else
                alpha = 0.5f - (((360f - angle) / limit) / 2f);
        }
        else
        {
            if(angle > limit)
                alpha = 1f;
            else
                alpha = ((angle / limit) / 2f) + 0.5f;
        }
        
        if(Mathf.Abs(alpha - 0.5f) < 0.02)
            alpha = 0.5f;

        if (Mathf.Abs(lastClick - alpha) >= 0.05f)
        {
            int val = Mathf.RoundToInt(alpha * 10f);

            if (lastClick - alpha > 0f)
            {
                switch (val)
                {
                    case 1: _audioSource.PlayOneShot(high, 1f); break;
                    case 4: _audioSource.PlayOneShot(med, 1f); break;
                }
            }else
            {
                switch (val)
                {
                    case 6 : _audioSource.PlayOneShot(med, 1f); break;
                    case 9 : _audioSource.PlayOneShot(high, 1f); break;
                }
            }

            lastClick = (val/10f);
        }
        
        
        _hingeJoint.transform.localRotation = Quaternion.Lerp(_hingeJoint.transform.localRotation, defaultRot, Time.fixedDeltaTime * (centering ? 8f : centeringSpeed));

        if (centering)
        {
            if (startAtZero)
            {
                if (alpha < 0.05)
                    centering = false;
            }
            else
            {
                if (Mathf.Abs(alpha - 0.5f) < 0.02)
                    centering = false;
            }
        }

    }
    
    public void Push(bool forward)
    {
        _hingeJoint.GetComponent<Rigidbody>().AddForceAtPosition(_hingeJoint.transform.forward * (forward ? 50f : -50f),_hingeJoint.GetComponent<Rigidbody>().centerOfMass);
    }
}
