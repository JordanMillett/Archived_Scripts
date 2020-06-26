using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float MaxHealth;
    public float Health;

    public GameObject HealthDrop;
    public GameObject AmmoDrop;
    public GameObject ShieldDrop;

    float LootForce = 300f;
    float LootOffsetAngle = .3f;

    void Start()
    {

        Health = MaxHealth;

    }

    public void Damage(float Amount)
    {
        if(Health != 0)
        {
            if(Amount < Health)
            {

                Health -= Amount;

            }else
            {

                Health = 0f;

                DropLoot();

                Destroy(this.gameObject);
            }
        }

    }

    void DropLoot()
    {

        for(int i = 0; i < 2; i++)
        {
            int ShieldAmount = Random.Range(0, 2);
            if(ShieldAmount == 1)
                SpawnLoot(ShieldDrop);

            int AmmoAmount = Random.Range(0, 2);
            if(AmmoAmount == 1)
                SpawnLoot(AmmoDrop);

            int HealthAmount = Random.Range(0, 2);
            if(HealthAmount == 1)
                SpawnLoot(HealthDrop);
        }

    }

    void SpawnLoot(GameObject Loot)
    {
        Vector3 OffsetVector = Random.insideUnitCircle.normalized * LootOffsetAngle;
        OffsetVector = new Vector3(OffsetVector.x, 0f, OffsetVector.y);

        GameObject L = Instantiate(Loot, this.transform.position, Quaternion.identity);
        L.GetComponent<Rigidbody>().AddForce((Vector3.up + OffsetVector) * LootForce);

    }

}
