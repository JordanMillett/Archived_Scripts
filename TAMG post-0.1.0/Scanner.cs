using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scanner : MonoBehaviour
{
    public AudioClip SoundEffect;
    AudioSource AS;
    bool isOn = false;
    GameObject Cam;
    public TextMeshProUGUI Contents;

    public GameObject Effect;

    bool Scanning = false;

    void Start()
    {
        AS = GetComponent<AudioSource>();
    }

    public void ToggleOn()
    {
        isOn = true;
        Cam = GameObject.FindWithTag("Camera").gameObject;
    }

    IEnumerator Scan(Package P)
    {
        Scanning = true;
        Effect.SetActive(true);

        P.Scanned();
        AS.volume = (.5f * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
        AS.clip = SoundEffect;
        AS.Play();

        yield return new WaitForSeconds(0.25f);

        Effect.SetActive(false);
        Scanning = false;
    }
    
    public void StartScanEffect()
    {
        StartCoroutine(ScanEffect());
    }
    
    IEnumerator ScanEffect()
    {
        Scanning = true;
        Effect.SetActive(true);

        AS.volume = (.5f * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
        AS.clip = SoundEffect;
        AS.Play();

        yield return new WaitForSeconds(0.25f);

        Effect.SetActive(false);
        Scanning = false;
    }

    void Update()
    {
        if(isOn)
        {
            RaycastHit hit;

            if(Physics.Raycast(Cam.transform.position,Cam.transform.forward, out hit, 2.5f))
            {
                try 
                {
                            
                    Package P = hit.collider.transform.gameObject.GetComponent<Package>();

                    if(P != null)
                    {
                        if(Input.GetMouseButtonDown(0) && !Scanning && !UI.Instance.Busy())
                        {
                            StartCoroutine(Scan(P));
                        }
                        Contents.text = P.Contents;
                        
                    }else
                    {
                        if(Input.GetMouseButtonDown(0) && !Scanning && !UI.Instance.Busy())
                        {
                            StartScanEffect();
                        }
                        Contents.text = "NA";
                    }
                    
                }
                catch{}
            }else
            {
                if(Input.GetMouseButtonDown(0) && !Scanning && !UI.Instance.Busy())
                {
                    StartScanEffect();
                }
                Contents.text = "NA";
            }
        }
    }
}
