using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UINote : MonoBehaviour
{
    public float RiseSpeed = 2f;

    TextMeshProUGUI Text;
    Color32 TextColor;
    
    float AlphaLerp = 0f;
    public float FadeSpeed = .05f;
    float CurrentAlpha = 255f;

    void Start()
    {

        Text = GetComponent<TextMeshProUGUI>();
        TextColor = Text.color;

    }

    void Update()
    {
        this.transform.position += new Vector3(0f, RiseSpeed, 0f);
        CurrentAlpha = Mathf.Lerp(255f, 0f, AlphaLerp);
        AlphaLerp += FadeSpeed;

        Text.color = new Color32(TextColor.r, TextColor.g, TextColor.b, (byte) CurrentAlpha);

        if(AlphaLerp > 1f)
            Destroy(this.gameObject);

    }


}
