using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parachute : MonoBehaviour
{
    public AnimationCurve SizeCurve;
    public MeshRenderer MR;
    public Faction Team;

    void Start()
    {
        MR.materials[0].SetColor("Team", Team == Game.TeamOne ? Game.FriendlyColor : Game.EnemyColor);

        this.transform.localScale = Vector3.zero;

        StartCoroutine(Open());
    }
    
    IEnumerator Open()
    {
        float alpha = 0f;
        while(alpha < 1f)
        {
            yield return null;
            this.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, SizeCurve.Evaluate(alpha));
            alpha += 0.02f;
            
        }

        Destroy(this);
    }
}
