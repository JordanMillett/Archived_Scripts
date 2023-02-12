using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfantryEventHandler : MonoBehaviour
{
    public AudioSourceController Feet;
    public SoundGroup GrassSteps;
    public SoundGroup SolidSteps;

    public void PlayStep()
    {
        if(!Feet.enabled)
            return;
            
        RaycastHit hit;
        if(Physics.Raycast(this.transform.position + new Vector3(0f, 0.5f, 0f), -Vector3.up, out hit, 5f, Game.IgnoreSelectMask))
        {
            if(hit.transform.gameObject.CompareTag("Landable"))
            {
                Feet.Sounds = GrassSteps;
                Feet.PlayRandom();
            }else if(hit.transform.gameObject.CompareTag("Unmoving"))
            {
                Feet.Sounds = SolidSteps;
                Feet.PlayRandom();
            }
        }
    }
}
