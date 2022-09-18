using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    bool Hit = false;

    void OnCollisionEnter(Collision col)
    {
        if (!Hit)
        {
            Hit = true;
            try
            {
                Enemy E = col.transform.gameObject.GetComponent<Enemy>();

                if (E != null)
                    E.Hurt();
            }
            catch {}

            Destroy(this.GetComponent<SphereCollider>());
            StartCoroutine(Shrink());
        }
    }
    
    IEnumerator Shrink()
    {
        float start = Time.time;

        float scale = 0.5f;
        while (Time.time < start + 0.5f)
        {
            yield return null;
            scale = Mathf.Lerp(0.5f, 0.1f, (Time.time - start) * 2f);
            this.transform.localScale = new Vector3(scale, scale, scale);
        }

        Destroy(this.gameObject);
    }
}
