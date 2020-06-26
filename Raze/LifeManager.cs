using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LifeManager : MonoBehaviour
{
    public float MaxHealth;
    public float MaxShields;

    public float Health;
    public float Shields;

    public bool AddHealth(float Amount)
    {

        if(Health == MaxHealth)
        {

            return false;

        }else
        {

            Health += Amount;
            if(Health > MaxHealth)
                Health = MaxHealth;

            return true;

        }

    }

    public bool AddShield(float Amount)
    {

        if(Shields == MaxShields)
        {

            return false;

        }else
        {

            Shields += Amount;
            if(Shields > MaxShields)
                Shields = MaxShields;

            return true;

        }

    }

    public void Damage(float Amount)
    {

        if(Shields >= Amount)
        {

            Shields -= Amount;

        }else
        {
            
            Amount -= Shields;
            Shields = 0f;

            if(Health > Amount)
            {

                Health -= Amount;
                
            }else
            {

                Health = 0f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            }
    
        }

    }
}
