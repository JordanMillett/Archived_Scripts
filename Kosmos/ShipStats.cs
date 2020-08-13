using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipStats : MonoBehaviour
{
    BattleShipController BSC;
    GameObject Model;

    public Renderer Shield;
    float DefaultShieldIntesity = 0.5f;
    float DamagedShieldIntensity = 5f;
    float ShieldDamageCoolDownAlpha = 0.02f;
    float ShieldDamageAlpha = 0f;
    IEnumerator ShieldDestroyCoroutine;
    bool ShieldDestroyRunning = false;
    float ShieldDestroySpeedAlpha = 0.05f;
    float ShieldDestroyAlpha = 0f;

    IEnumerator ShipShakeCoroutine;
    bool ShipShakeRunning = false;

    float EnergyRechargeDelay = 0.5f;
    bool EnergyRecharging = false;
    IEnumerator EnergyRechargeCoroutine;

    float ShieldsRechargeDelay = 2f;
    bool ShieldsRecharging = false;
    IEnumerator ShieldsRechargeCoroutine;
    public GameObject ExplodeEffect;
    
    public float Hull;
    public float Shields;
    public float Energy;

    public int Team = 0;

    public bool Dead = false;
    public bool Invincible = false;

    void Start()
    {
        Model = transform.GetChild(0).gameObject;
        BSC = GetComponent<BattleShipController>();
        Hull = BSC.ShipInfo.Hull;
        Shields = BSC.ShipInfo.Shields;
        Energy = BSC.ShipInfo.Energy;
    }

    void Update()
    {
        if(!Dead)
        {
            if(ShieldDamageAlpha > 0f)
            {
                Shield.material.SetFloat("_Alpha", Mathf.Lerp(DefaultShieldIntesity, DamagedShieldIntensity, ShieldDamageAlpha));
                ShieldDamageAlpha -= ShieldDamageCoolDownAlpha;
                if(ShieldDamageAlpha < 0f)
                    ShieldDamageAlpha = 0f;
            }

            if(ShipShakeRunning)
            {
                float Magnitude = 0.025f;
                Model.transform.localPosition = new Vector3(Random.Range(-Magnitude, Magnitude), Random.Range(-Magnitude, Magnitude), Random.Range(-Magnitude, Magnitude));
            }else
            {
                Model.transform.localPosition = Vector3.zero;
            }
        }
    }

    public void Damage(float Amount)
    {
        if(Invincible)
            Amount = 0f;

        if(Shields > Amount)
        {
            Shields -= Amount;
            ShieldDamageAlpha = 1f;
        }else if (Amount >= Shields)
        {
            Amount -= Shields;
            if(Shields > 0f)
                DestroyShields(true);
            Shields = 0f;
            

            if(Hull > Amount)
            {
                Hull -= Amount;
                ShakeShip();
            }else if(Amount >= Hull)
            {
                Hull = 0f;
                ShakeShip();
                Explode();
            }
        }

        if(ShieldsRecharging)
            StopCoroutine(ShieldsRechargeCoroutine);

        ShieldsRechargeCoroutine = RechargeShields();
        StartCoroutine(ShieldsRechargeCoroutine);

        
    }

    public void Drain(float Amount)
    {
        Energy -= Amount;
        
        if(EnergyRecharging)
            StopCoroutine(EnergyRechargeCoroutine);

        EnergyRechargeCoroutine = Recharge();
        StartCoroutine(EnergyRechargeCoroutine);
    }

    IEnumerator RechargeShields()
    {
        ShieldsRecharging = true;
        yield return new WaitForSeconds(ShieldsRechargeDelay);
        if(Shields == 0f)
            DestroyShields(false);

        while(Shields < BSC.ShipInfo.Shields)
        {
            Shields += BSC.ShipInfo.ShieldsRechargeRate;
            yield return null;
        }

        ShieldsRecharging = false;
    }

    IEnumerator Recharge()
    {
        EnergyRecharging = true;
        yield return new WaitForSeconds(EnergyRechargeDelay);

        while(Energy < BSC.ShipInfo.Energy)
        {
            Energy += BSC.ShipInfo.EnergyRechargeRate;
            yield return null;
        }

        EnergyRecharging = false;
    }

    void Explode()
    {
        if(!BSC.PlayerControlled)
        {
            GameObject Exp = Instantiate(ExplodeEffect, this.transform.position, Quaternion.identity);
            Exp.transform.SetParent(this.transform.parent);
            Destroy(this.gameObject);
        }else
        {
            //Stop time and swap players
            if(BSC.BM.FriendlyShips.Count > 1)
            {

                StartCoroutine(SwapPlayer());

            }else
            {
                StartCoroutine(RestartGame());
            }
        }
    }

    IEnumerator RestartGame()
    {
        Dead = true;

        GameObject CamEmpty = transform.GetChild(3).gameObject;         //set cam to world
        CamEmpty.transform.SetParent(this.transform.parent.transform);

        transform.GetChild(0).gameObject.SetActive(false);          //Essentially destroy the ship
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);

        GetComponent<Rigidbody>().velocity = Vector3.zero;
        Destroy(GetComponent<Collider>());

        GameObject Exp = Instantiate(ExplodeEffect, this.transform.position, Quaternion.identity);  //explode with effect
        Exp.transform.SetParent(this.transform.parent);

        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator SwapPlayer()
    {
        Dead = true;
        BSC.BM.FriendlyShips[1].Invincible = true;

        GameObject CamEmpty = transform.GetChild(3).gameObject;         //set cam to world
        CamEmpty.transform.SetParent(this.transform.parent.transform);

        transform.GetChild(0).gameObject.SetActive(false);          //Essentially destroy the ship
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);

        GetComponent<Rigidbody>().velocity = Vector3.zero;
        Destroy(GetComponent<Collider>());

        GameObject Exp = Instantiate(ExplodeEffect, this.transform.position, Quaternion.identity);  //explode with effect
        Exp.transform.SetParent(this.transform.parent);
        //Destroy(this.gameObject);

        yield return new WaitForSeconds(0.5f);  //Watch your ship explode, (Account for if other ship dies in this time)
        
        Time.timeScale = 0f; //Freeze Time

        for(int i = 0; i < 60; i++)
            yield return null;

        BSC.BM.FriendlyShips[1].GetComponent<BattleShipController>().PlayerControlled = true;
        
        
        Vector3 StartPosition = CamEmpty.transform.position;
        Quaternion StartRotation = CamEmpty.transform.rotation;
        Vector3 EndPosition = BSC.BM.FriendlyShips[1].transform.position;
        //Quaternion EndRotation = BSC.BM.FriendlyShips[1].transform.rotation;

        //Quaternion StartCamera = CamEmpty.transform.GetChild(0).rotation;
        float alpha = 0f;
        while(alpha < 1f)
        {
            CamEmpty.transform.position = Vector3.Lerp(StartPosition, EndPosition, alpha);
            //CamEmpty.transform.rotation = Quaternion.Lerp(StartRotation, EndRotation, alpha);
            CamEmpty.transform.rotation = Quaternion.Lerp(StartRotation, Quaternion.Euler(BSC.pitch, BSC.yaw, 0f), alpha);
            alpha += 0.01f;
            yield return null;
        }

        //Cursor.lockState = CursorLockMode.Locked;
        //yield return null;
        //Cursor.lockState = CursorLockMode.None;

        CamEmpty.transform.SetParent(BSC.BM.FriendlyShips[1].transform);
        CamEmpty.transform.localPosition = Vector3.zero;
        //CamEmpty.transform.GetChild(0).transform.localRotation = Quaternion.identity;


        //CamEmpty.transform.eulerAngles = new Vector3(pitch, yaw, 0f);

        BSC.BM.FriendlyShips[1].BSC.yaw = BSC.yaw;
        BSC.BM.FriendlyShips[1].BSC.pitch = BSC.pitch;

        GetComponent<BattleShipController>().PlayerControlled = false;
        
        CamEmpty.transform.GetChild(0).GetComponent<DynamicHUD>().enabled = true;
        CamEmpty.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);

        BSC.BM.FriendlyShips[1].Invincible = false;
        Time.timeScale = 1f;
        Destroy(this.gameObject);
    }

    void DestroyShields(bool Destroy)
    {
        if(ShieldDestroyRunning)
            StopCoroutine(ShieldDestroyCoroutine);
        
        ShieldDestroyCoroutine = ShieldsAnimation(Destroy);
        StartCoroutine(ShieldDestroyCoroutine);
    }

    void ShakeShip()
    {
        if(ShipShakeRunning)
            StopCoroutine(ShipShakeCoroutine);
        
        ShipShakeCoroutine = ShakeShipAnimation();
        StartCoroutine(ShipShakeCoroutine);
    }

    IEnumerator ShakeShipAnimation()
    {
        ShipShakeRunning = true;

        yield return new WaitForSeconds(0.25f);

        ShipShakeRunning = false;
    }

    IEnumerator ShieldsAnimation(bool Destroy)     
    {   
        ShieldDestroyRunning = true;
        if(Destroy)
        {
            //ShieldDestroyAlpha = 0f;
            while(ShieldDestroyAlpha < 1f)
            {
                Shield.material.SetFloat("_ExplodeAlpha", ShieldDestroyAlpha);
                ShieldDestroyAlpha += ShieldDestroySpeedAlpha;
                yield return null;
            }
            Shield.material.SetFloat("_ExplodeAlpha", 1f);
        }else
        {
            //ShieldDestroyAlpha = 1f;
            while(ShieldDestroyAlpha > 0f)
            {
                Shield.material.SetFloat("_ExplodeAlpha", ShieldDestroyAlpha);
                ShieldDestroyAlpha -= ShieldDestroySpeedAlpha;
                yield return null;
            }
            Shield.material.SetFloat("_ExplodeAlpha", 0f);
        }
        ShieldDestroyRunning = false;
    }
}
