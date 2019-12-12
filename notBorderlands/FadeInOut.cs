using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FadeInOut : MonoBehaviour
{


    TextMeshProUGUI Text;
    Color32 TextColor;
    
    float AlphaLerp = 0f;
    public float FadeSpeed = .05f;
    public float StayLength = 2f;
    float CurrentAlpha = 255f;

    void Start()
    {

        Text = GetComponent<TextMeshProUGUI>();
        TextColor = Text.color;
        StartCoroutine(Fade());

    }

    IEnumerator Fade()
    {

        while(AlphaLerp < 1f)
        {
            yield return null;
            Refresh();
            AlphaLerp += FadeSpeed;
        }

        yield return new WaitForSeconds(StayLength);

        while(AlphaLerp > 0f)
        {
            yield return null;
            Refresh();
            AlphaLerp -= FadeSpeed;
        }

        Destroy(this.gameObject);

    }

    void Refresh()
    {

        CurrentAlpha = Mathf.Lerp(0f, 255f, AlphaLerp);
        Text.color = new Color32(TextColor.r, TextColor.g, TextColor.b, (byte) CurrentAlpha);

    }
}
