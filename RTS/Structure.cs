using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyScripts.TeamColors;

public class Structure : MonoBehaviour
{
    public float Health;
    public int Team;

    public Renderer TeamRenderer;
    public Simple_Bar HealthBar;

    void Start()
    {
        UpdateColor();
        HealthBar.Max = Health;
    }

    void Update()
    {
        
    }

    public void Damage(float Amount)
    {
        if(Health > Amount)
            Health -= Amount;
        else
        {
            Health = 0;

            Destroy(this.gameObject);

        }

        HealthBar.Current = Health;

    }
    
    void UpdateColor()
    {
        Color NewColor;
        if (ColorUtility.TryParseHtmlString(TeamColors.Colors[Team], out NewColor))
            TeamRenderer.material.SetColor("_BaseColor", NewColor);
    }
}
