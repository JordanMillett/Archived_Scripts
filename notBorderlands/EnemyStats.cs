using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{

    public float Health;
    public float MaxHealth;
    public int Level;
    public string Name;

    public float KillXPAmount;

    HudSync HS;
    PlayerStats PS;

    public DropTable DT;

    void Start()
    {
        HS = GameObject.FindWithTag("Hud").GetComponent<HudSync>();
        PS = GameObject.FindWithTag("Player").GetComponent<PlayerStats>();

        float Magnitude = 0.05f;

        Health += (Level - 1) * (Health * Magnitude);  //0.05 = difficulty
        MaxHealth = Health;

        KillXPAmount += (Level - 1) * (KillXPAmount * (Magnitude/2f));
    }

    public void SendInfoToHud()
    {
        HS.EnableEnemyIndicator(MaxHealth, Health, Name, Level);
    }

    public void Damage(float Amount)
    {
        if(Health - Amount <= 0f)
        {

            Health = 0f;
            
            PS.AddXP(KillXPAmount);

            Die();

        }

        Health -= Amount;
    }

    void Die()
    {

        for(int i = 0; i < DT.Items.Count; i++)
        {

            float SpawnChance = Random.Range(0f,100f);
            if(SpawnChance < DT.ItemChances[i])
            {

                Instantiate(DT.Items[i], this.transform.position + new Vector3(0f,1f,0f), Quaternion.identity);

            }

        }

        Destroy(this.gameObject);
    }
}
