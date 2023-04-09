using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Transform Projectiles;
    public MeshRenderer MR;

    int Damage = 0;
    DamageBonus Bonus = DamageBonus.None;

    bool FromPlayer = false;

    public void Configure(int _damage, ProjectileSize _size, DamageBonus _bonus, bool _fromPlayer)
    {
        
        Color _color = Game.CommonColors[_bonus];

        float Size = Game.ProjectileSizes[_size];

        Damage = _damage;
        MR.material.SetColor("_EmissionColor", _color * Mathf.Pow(2, 3)); //3 is intensity
        MR.transform.localScale = new Vector3(Size, Size, Size);
        GetComponent<SphereCollider>().radius = Size / 2f;
        Bonus = _bonus;

        FromPlayer = _fromPlayer;
    }

    void OnTriggerEnter(Collider Col)
    {
        if (!Col.isTrigger)
        {
            Life L = Col.GetComponent<Life>();
            if(L)
            {
                if (L.isPlayer != FromPlayer)
                {
                    L.Hurt(Damage, Bonus);
                    this.transform.SetParent(Projectiles);
                    this.gameObject.SetActive(false);
                }
            }else
            {
                this.transform.SetParent(Projectiles);
                this.gameObject.SetActive(false);
            }
        }
    }
}
