using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopIn : MonoBehaviour
{   
    float AppearTime = 0.35f;
    public List<MeshRenderer> MeshRends;

    public AnimationCurve AC;

    void Awake()
    {
        foreach(MeshRenderer MR in MeshRends)
        {
            foreach(Material M in MR.materials)
            {
                M.SetFloat("_PopIn", 0f);
            }
        }
    }

    void Start()
    {
        StartCoroutine(Pop());
    }

    IEnumerator Pop()
    {
        float alpha = 0f;
        int CurrentFrames = 0;
        int TotalFrames = Mathf.RoundToInt(AppearTime * 60f);

        while (CurrentFrames <= TotalFrames)
        {
            yield return null;
            alpha = (float)CurrentFrames / (float)TotalFrames;

            foreach (MeshRenderer MR in MeshRends)
            {
                foreach (Material M in MR.materials)
                {
                    M.SetFloat("_PopIn", AC.Evaluate(alpha));
                }
            }

            CurrentFrames++;
        }

        foreach(MeshRenderer MR in MeshRends)
        {
            foreach(Material M in MR.materials)
            {
                M.SetFloat("_PopIn", 1f);
            }
        }
    }
}
