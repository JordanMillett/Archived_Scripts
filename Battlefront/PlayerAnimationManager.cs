using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    PlayerController PC;
    public Animator An;

    bool Arms = false;

    float totalTime = 0f;

    float recoilOffset = 0f;
    float maxRecoil = .2f;
    float recoilAlpha = 0f;

    void Start()
    {
        PC = GetComponent<PlayerController>();    
    }

    void Update()
    {
        totalTime += Time.deltaTime;

        if(Input.GetMouseButtonDown(0))
            recoilAlpha = 1f;

        recoilOffset = Mathf.Lerp(0f, maxRecoil, recoilAlpha);
        if(recoilAlpha > 0f)
            recoilAlpha -= 0.1f;

        if(Input.GetKeyDown("q"))
            Arms = !Arms;

        An.SetBool("Walking", PC.Moving);
        An.SetBool("ArmsActive", Arms);

        float xOffset = Mathf.PerlinNoise(totalTime, 0f) - 0.5f;
        xOffset *= 0.025f;

        float yOffset = Mathf.PerlinNoise(0f, totalTime) - 0.5f;
        yOffset *= 0.05f;
        
        An.SetFloat("Arms_X_Dir", xOffset);

        float yAim = PC.pitch/-50f;
        An.SetFloat("Arms_Y_Dir", yAim + yOffset + recoilOffset);
        An.SetBool("Sprinting", PC.Sprinting);
    }
}
