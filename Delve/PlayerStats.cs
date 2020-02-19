using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public Bar healthBar;
    public Bar magicBar;
    public Bar staminaBar;

    public float Health = 100f;
    public float Magic = 100f;
    public float Stamina = 100f;

    float maxHealth = 100f;
    float maxMagic = 100f;
    float maxStamina = 100f;

    //Custom Stat info here, one handed etc.

    void UpdateBars()
    {

       healthBar.Amount = Health/maxHealth;
       magicBar.Amount = Magic/maxMagic;
       staminaBar.Amount = Stamina/maxStamina;

       healthBar.UpdateBar();
       magicBar.UpdateBar();
       staminaBar.UpdateBar();

    }

    public void AddValue(int Index, float Amount)
    {

        switch(Index)
        {
            case 0 : 
            
                Health += Amount; 
                if(Health < 0f)
                    Health = 0f;
                if(Health > maxHealth)
                    Health = maxHealth;
            
            break;
            case 1 : 
            
                Magic += Amount;
                if(Magic < 0f)
                    Magic = 0f;
                if(Magic > maxMagic)
                    Magic = maxMagic;
            
            break;
            case 2 : 
            
                Stamina += Amount;
                if(Stamina < 0f)
                    Stamina = 0f;
                if(Stamina > maxStamina)
                    Stamina = maxStamina;
            
            break;
        }

        UpdateBars();

    }

    public void SetValue(int Index, float Amount)
    {

        switch(Index)
        {
            case 0 : 
            
                Health = Amount; 
                if(Health < 0f)
                    Health = 0f;
                if(Health > maxHealth)
                    Health = maxHealth;
            
            break;
            case 1 : 
            
                Magic = Amount;
                if(Magic < 0f)
                    Magic = 0f;
                if(Magic > maxMagic)
                    Magic = maxMagic;
            
            break;
            case 2 : 
            
                Stamina = Amount;
                if(Stamina < 0f)
                    Stamina = 0f;
                if(Stamina > maxStamina)
                    Stamina = maxStamina;
            
            break;
        }

        UpdateBars();

    }
}
