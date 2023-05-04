using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Car : MonoBehaviour
{
    public List<WheelCollider> SteerWheels;
    public List<WheelCollider> ForceWheels;
    public List<WheelCollider> BrakeWheels;

    float maxSteeringAngle = 35f;

    public EngineData Engine;
    public TransmissionData Transmission;
    public float BrakeForce = 2000f;
    public AnimationCurve MaxTurn;

    public float Power = 0f;
    public float TorqueDelivered = 0f;
    public float EngineRPM = 0f;
    public float MPH = 0f;

    float throttle = 0f;
    float steer = 0f;
    float throttleReactSpeed = 5f;
    float rpmDropSpeed = 1f;
    float steerTurnSpeed = 5f;

    public int Gear = 1;

    public MeshRenderer Body;
    Cubemap DamageTexture;
    public Transform CenterOfMass;
    public AudioSource EngineSound;
    public AudioSource Oneshots;

    public AudioClip ShiftUpSound;
    public AudioClip ShiftDownSound;

    const int CubemapFaceSize = 16;

    bool Shifting = false;

    public virtual void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = CenterOfMass.position;
        
        /*
        DamageTexture = new Cubemap(CubemapFaceSize, TextureFormat.RGBA32, false);

        Color[] blank = new Color[CubemapFaceSize*CubemapFaceSize];

        DamageTexture.SetPixels(blank, CubemapFace.PositiveX);
        DamageTexture.SetPixels(blank, CubemapFace.NegativeX);
        DamageTexture.SetPixels(blank, CubemapFace.PositiveY);
        DamageTexture.SetPixels(blank, CubemapFace.NegativeY);
        DamageTexture.SetPixels(blank, CubemapFace.PositiveZ);
        DamageTexture.SetPixels(blank, CubemapFace.NegativeZ);
        DamageTexture.Apply();

        Body.material.SetTexture("_Damage", DamageTexture);
        */

    }
    
    void OnCollisionEnter(Collision Col)
    {
        /*
        Color[] face = DamageTexture.GetPixels(CubemapFace.PositiveZ);

        for (int i = 0; i < face.Length; i++)
        {
            if(Random.Range(0, 2) == 1)
                face[i] = Color.white;
        }

        DamageTexture.SetPixels(face, CubemapFace.PositiveZ);
        DamageTexture.Apply();

        Body.material.SetTexture("_Damage", DamageTexture);
        */
    }
    
    //engine rpm is equal to some ratio of the wheel rpm, torque is horsepower

    public virtual void Update()
    {
        MPH = Mathf.Abs(this.transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).z) * 2.23694f;
        //float waver = ((Mathf.PerlinNoise(Time.time * 0.1f, 0f) * 2f) - 1f) * Engine.IdleRPMWaver;

        //EngineRPM = Mathf.Lerp(Engine.IdleRPM + waver, Engine.MaxRPM, Throttle) * ;
        //TransmissionRPM = EngineRPM * Transmission.Gears[Gear];
        /*
        foreach (WheelCollider Wheel in DriveWheels)
        {
            //Torque = (Engine.Peak_Horsepower / (TransmissionRPM/5252f));
            //Wheel.motorTorque = Torque;
        }*/

        EngineSound.pitch = EngineRPM/3000f;
    }

    public void ShiftGear(bool Up)
    {
        if(Shifting)
            return;

        Gear += Up ? 1 : -1;
        
        if(Gear == Mathf.Clamp(Gear, 0, Transmission.Gears.Count - 1))
        {
            Shifting = true;
            Oneshots.PlayOneShot(Up ? ShiftUpSound : ShiftDownSound);
            Invoke("Shift", 1f);
        }
        
        Gear = Mathf.Clamp(Gear, 0, Transmission.Gears.Count - 1);

        
    }
    
    void Shift()
    {
        Shifting = false;
        
        //float GearTopSpeed = Mathf.Pow(Engine.Horsepower / Transmission.Gears[Gear], 1f / 3f) * (20f / Transmission.Gears[Gear]); //((92 / 3)^(1/3)) * (20 / 3)
        //EngineRPM = Mathf.Lerp(Engine.IdleRPM, Engine.MaxRPM, (MPH / GearTopSpeed));
    }

    public void Control(bool Gas, bool Brake, float Turn)
    {
        float turnAlpha = MaxTurn.Evaluate(MPH / (Mathf.Pow(Engine.Horsepower, 1f / 3f) * 20f));
        steer = Mathf.Lerp(steer, Turn * maxSteeringAngle, Time.deltaTime * steerTurnSpeed) * turnAlpha;
        foreach (WheelCollider Wheel in SteerWheels)
            Wheel.steerAngle = steer;
        
        foreach (WheelCollider Wheel in BrakeWheels)
            Wheel.brakeTorque = Brake ? BrakeForce : 0f;
        
        if(Shifting)
        {
            foreach (WheelCollider Wheel in ForceWheels)
                Wheel.motorTorque = 0f;
            
            EngineRPM = Mathf.Lerp(EngineRPM, Engine.IdleRPM, Time.deltaTime * rpmDropSpeed);
            throttle = Mathf.Lerp(throttle, Gas ? 1f : 0f, Time.deltaTime * throttleReactSpeed);
            return;
        }

        float GearTopSpeed = Mathf.Pow(Engine.Horsepower / Transmission.Gears[Gear], 1f / 3f) * (20f / Transmission.Gears[Gear]); //((92 / 3)^(1/3)) * (20 / 3)
        float RPMGoal = Gas ? Mathf.Lerp(Engine.IdleRPM, Engine.MaxRPM, (MPH / GearTopSpeed)) : Engine.IdleRPM;
        EngineRPM = Mathf.Lerp(EngineRPM, RPMGoal, Time.deltaTime * rpmDropSpeed);
        float RPMAlpha = EngineRPM / Engine.MaxRPM;

        throttle = Mathf.Lerp(throttle, Gas ? 1f : 0f, Time.deltaTime * throttleReactSpeed);
        Power = Engine.Horsepower * Engine.PowerCurve.Evaluate(RPMAlpha) * throttle;

        TorqueDelivered = ((Power * 5252.08f)/(EngineRPM)) * Transmission.Gears[Gear];//for force multiplier
        TorqueDelivered *= Engine.TorqueCurve.Evaluate(RPMAlpha);
        TorqueDelivered *= Gear == 0 ? -1f : 1f; //Reverse gear

        if(Brake)
            TorqueDelivered = 0f;
        
        foreach (WheelCollider Wheel in ForceWheels)
            Wheel.motorTorque = TorqueDelivered * 1.3558f;
        
        /*

        
        EngineRPM = Mathf.Lerp(EngineRPM, Mathf.Lerp(Engine.IdleRPM, Engine.MaxRPM, Throttle), Time.deltaTime * rpmReactSpeed);
        Power = Engine.Horsepower * Engine.PowerCurve.Evaluate(EngineRPM/Engine.MaxRPM);
        
        


        
        TorqueDelivered = Power * 3f;
        //TorqueDelivered = (63025 * Power) / EngineRPM;

 
            //TorqueDelivered = (63025f * Power) / EngineRPM;
            //PoundsTorque = (Engine.Peak_Horsepower * 5252.08f) / EngineRPM;
            //PoundsTorque *= Transmission.Gears[Gear];
        
        
        

        
        */


    }
}
