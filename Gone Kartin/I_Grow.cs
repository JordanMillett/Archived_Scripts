using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class I_Grow : MonoBehaviour
{
    public float Scale = 2f;
    public float Duration = 8f;
    public AnimationCurve GrowCurve;
    
    bool Used = false;

    public void Use()
    {
        if(!Used)
        {
            Used = true;
            this.transform.GetChild(0).transform.gameObject.SetActive(false);
            StartCoroutine(Grow());
        }
    }

    IEnumerator Grow()
    {
        float TransitionSpeed = 0.1f;
        KartController KC = this.transform.GetComponent<ItemInvoker>().KC;
        float StartingMass = KC.GetComponent<Rigidbody>().mass;

        KC.GetComponent<Rigidbody>().mass = StartingMass * Scale;
        Vector3 StartingScale = Vector3.one;
        Vector3 BigScale = new Vector3(Scale, Scale, Scale);

        float Alpha = 0f;
        while(Alpha < 1f)
        {
            Alpha += (Time.timeScale * TransitionSpeed);
            KC.transform.localScale = Vector3.Lerp(StartingScale, BigScale, GrowCurve.Evaluate(Alpha));
            yield return null;
        }
        KC.transform.localScale = BigScale;

        yield return new WaitForSeconds(Duration);

        KC.GetComponent<Rigidbody>().mass /= Scale;

        Alpha = 0f;
        while(Alpha < 1f)
        {
            Alpha += (Time.timeScale * TransitionSpeed);
            KC.transform.localScale = Vector3.Lerp(BigScale, StartingScale, GrowCurve.Evaluate(Alpha));
            yield return null;
        }
        KC.transform.localScale = StartingScale;

        KC.ItemReference = null;
        Destroy(this.gameObject);
        
    }
}
