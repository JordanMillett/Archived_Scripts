using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public Texture2D Symbol;
    public bool Influenceable = true;
    public int Influence = 0;
    public bool UseRing = false;
    public GameObject Effect;
    public GameObject Status;
    public GameObject Model;

    public int Owner = 0;
    public bool Contested = false;
    int FlagShown = 0;

    List<Unit> Friendlies = new List<Unit>();
    List<Unit> Enemies = new List<Unit>();

    bool Hidden = false;

    public void HideRing()
    {
        Effect.SetActive(false);
    }

    public void HideEffect()
    {
        Effect.SetActive(false);
        Status.SetActive(false);
        Model.SetActive(false);

        Hidden = true;
    }

    public void ShowEffect()
    {
        if(UseRing)
            Effect.SetActive(true);
        
        Status.SetActive(true);
        Model.SetActive(true);

        Hidden = false;
    }

    void Start()
    {
        Effect.transform.GetComponent<MeshRenderer>().material.SetFloat("Influence", (Influence + 100f)/200f);
        Status.transform.GetComponent<MeshRenderer>().material.SetFloat("Influence", (Influence + 100f)/200f);

        if(!Symbol)
            Symbol = Influence == 100 ? Manager.M.Factions[(int) Game.TeamOne].Symbol : Manager.M.Factions[(int) Game.TeamTwo].Symbol;

        Status.transform.GetComponent<MeshRenderer>().material.SetTexture("Symbol", Symbol);

        InvokeRepeating("Change", 0f, 0.1f);
    }

    public void Change()
    {
        if(Hidden)
            return;
        
        if(!Influenceable)
        {
            if(Influence != 0)
                SetFlag(Influence > 0 ? 1 : -1);
            
            Model.transform.GetComponent<MeshRenderer>().materials[1].SetFloat("Raise", Mathf.Abs((Influence)/200f) * 2f);
            Status.transform.GetComponent<MeshRenderer>().material.SetFloat("Influence", (Influence + 100f)/200f);
            return;
        }

        for(int i = Friendlies.Count - 1; i >= 0; i--)
            if(!Friendlies[i])
                Friendlies.RemoveAt(i);

        for(int i = Enemies.Count - 1; i >= 0; i--)
            if(!Enemies[i])
                Enemies.RemoveAt(i);

        //int Difference = Mathf.Clamp((Friendlies.Count - Enemies.Count), -1, 1);
        //Influence = Mathf.Clamp(Influence + Difference, -100, 100);

        if (Owner == 0)
        {
            if(Friendlies.Count > 0 && Enemies.Count == 0)
                Influence = Mathf.Clamp(Influence + 1, -100, 100);
            if(Friendlies.Count == 0 && Enemies.Count > 0)
                Influence = Mathf.Clamp(Influence - 1, -100, 100);
                
            if(Friendlies.Count == 0 && Enemies.Count == 0)
                if(Influence != 0)
                    Influence = Mathf.Clamp(Influence + (Influence > 0 ? -1 : 1), -100, 100);
            
            if (Influence == 100)
                SetOwner(1);
            else if (Influence == -100)
                SetOwner(-1);
        }else
        {
            if(Friendlies.Count > 0 && Enemies.Count == 0)
                Influence = Mathf.Clamp(Influence + 2, -100, 100);
            if(Friendlies.Count == 0 && Enemies.Count > 0)
                Influence = Mathf.Clamp(Influence - 2, -100, 100);
            
            if(Friendlies.Count == 0 && Enemies.Count == 0)
                Influence = Mathf.Clamp(Influence + (Owner == 1 ? 2 : -2), -100, 100);
            
            if (Influence > 0)
                SetOwner(1);
            else if (Influence < 0)
                SetOwner(-1);
        }

        Contested = false;
        if(Owner == 1)
            if(Enemies.Count > 0)
                Contested = true;
        if(Owner == -1)
            if(Friendlies.Count > 0)
                Contested = true;

        if(Influence != 0)
            SetFlag(Influence > 0 ? 1 : -1);

        Model.transform.GetComponent<MeshRenderer>().materials[1].SetFloat("Raise", Mathf.Abs((Influence)/200f) * 2f);
        Status.transform.GetComponent<MeshRenderer>().material.SetFloat("Influence", (Influence + 100f)/200f);
        
    }
    
    void SetOwner(int Index)
    {
        if(Owner != Index)
        {
            Owner = Index;
            if(Game.Started)
                Debug.Log((Index == 1 ? Game.TeamOne.ToString() : Game.TeamTwo.ToString()) + " Captured " + this.gameObject.name);
        }
    }
    
    void SetFlag(int Value)
    {
        if(FlagShown != Value)
        {
            FlagShown = Value;

            if(FlagShown == 1)
                Model.transform.GetComponent<MeshRenderer>().materials[1].SetTexture("Symbol", Manager.M.Factions[(int) Game.TeamOne].Flag);
            if(FlagShown == -1)
                Model.transform.GetComponent<MeshRenderer>().materials[1].SetTexture("Symbol", Manager.M.Factions[(int) Game.TeamTwo].Flag);
        } 
    }

    public void UpdateFlag(int Value)
    {
        FlagShown = Value;

        if(FlagShown == 1)
            Model.transform.GetComponent<MeshRenderer>().materials[1].SetTexture("Symbol", Manager.M.Factions[(int) Game.TeamOne].Flag);
        if(FlagShown == -1)
            Model.transform.GetComponent<MeshRenderer>().materials[1].SetTexture("Symbol", Manager.M.Factions[(int) Game.TeamTwo].Flag);

        Symbol = Influence == 100 ? Manager.M.Factions[(int) Game.TeamOne].Symbol : Manager.M.Factions[(int) Game.TeamTwo].Symbol;
        Status.transform.GetComponent<MeshRenderer>().material.SetTexture("Symbol", Symbol);
    }

    void OnTriggerEnter(Collider col)
    {
        //STORE LIST AND REMOVE NULLS
        if(Influenceable)
        {
            try{
                Unit U = col.transform.root.transform.gameObject.GetComponent<Unit>();
                if(U.Team == Game.TeamOne)
                    Friendlies.Add(U);
                else
                    Enemies.Add(U);
            }catch{}
        }
    }

    void OnTriggerExit(Collider col)
    {
        if(Influenceable)
        {
            try{
                Unit U = col.transform.root.transform.gameObject.GetComponent<Unit>();
                if(U.Team == Game.TeamOne)
                    Friendlies.Remove(U);
                else
                    Enemies.Remove(U);
            }catch{}
        }
    }
    
    void OnDrawGizmos()
    {   
        Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
        Gizmos.DrawSphere(this.transform.position, 0.25f);
    }  
}
