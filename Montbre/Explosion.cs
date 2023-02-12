using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Explosion : MonoBehaviour
{
    public void Explode(float EffectSize, float DamageSize, int DamageAmount, float Volume, bool PlayerCaused)
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, DamageSize * 2f, Game.UnitOnlyMask);
        if(Game.DEBUG_ShowExplosionSizes)
        {
            Debug.DrawRay(this.transform.position, Vector3.up * DamageSize * 2f, Color.red, 2f);
            Debug.DrawRay(this.transform.position, -Vector3.up * DamageSize * 2f, Color.red, 2f);
            Debug.DrawRay(this.transform.position, Vector3.forward * DamageSize * 2f, Color.red, 2f);
            Debug.DrawRay(this.transform.position, -Vector3.forward * DamageSize * 2f, Color.red, 2f);
            Debug.DrawRay(this.transform.position, Vector3.right * DamageSize * 2f, Color.red, 2f);
            Debug.DrawRay(this.transform.position, -Vector3.right * DamageSize * 2f, Color.red, 2f);
        }
        int i = 0;
        while (i < hitColliders.Length)
        {
            if(hitColliders[i].transform.gameObject.GetComponent<Damage>() != null)
            {
                
                try{
                    Infantry Target = hitColliders[i].transform.gameObject.GetComponent<Infantry>();
                    Target.HitDirection = (Target.Chest.position - this.transform.position).normalized * 8f;
                }catch{}
                hitColliders[i].transform.gameObject.GetComponent<Damage>().Hurt(DamageAmount, true, false, PlayerCaused, "");
            }

            i++;
        }
        
        GetComponent<AudioSourceController>().SetVolume(Volume);
        GetComponent<AudioSourceController>().PlayRandom();
        //GetComponent<AudioSource>().volume = (Volume * 0.25f * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
        transform.GetChild(0).GetComponent<VisualEffect>().SetFloat("Size", EffectSize);
        GetComponent<Despawn>().DespawnTime *= EffectSize + 2f;
        transform.SetParent(GameObject.FindWithTag("Trash").transform);

        float dist = Vector3.Distance(this.transform.position, Manager.M.P.transform.position);
        if(dist < ((DamageSize * 2f) * 10f))
            Manager.M.P.Supress(dist/((DamageSize * 2f) * 10f));
    }
}
