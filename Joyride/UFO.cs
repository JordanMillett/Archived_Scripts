using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UFO : MonoBehaviour
{
    public MyLever l_throttle_mult;
    public MyLever l_throttle;
    public MyLever l_yaw;
    public MyLever l_pitch;
    public MyLever l_roll;

    public MyButton b_insideLight;
    public GameObject insideLight;

    public AudioSource engine;

    public Transform World;
    float Speed = 25f;
    float TurnSpeed = 50f;

    float CurrentSpeed = 0f;
    float movementAlpha = 0f;

    void Start()
    {
        b_insideLight.OnActivate.AddListener(ToggleInsideLights);
    }

    void FixedUpdate()
    {
        Movement();
    }

    void Update()
    {
        float ShakeAmount = Mathf.Lerp(0f, 0.02f, movementAlpha);
        Vector3 ShakeVec = new Vector3(Random.Range(-ShakeAmount, ShakeAmount),Random.Range(-ShakeAmount, ShakeAmount),Random.Range(-ShakeAmount, ShakeAmount));

        this.transform.localPosition = ShakeVec;
    }
    
    void Movement()
    {
        World.transform.RotateAround(Vector3.zero, Vector3.right, Mathf.Lerp(1f, -1f, l_pitch.alpha) * TurnSpeed * Time.fixedDeltaTime);
        World.transform.RotateAround(Vector3.zero, Vector3.up, Mathf.Lerp(1f, -1f, l_yaw.alpha) * TurnSpeed * Time.fixedDeltaTime);
        World.transform.RotateAround(Vector3.zero, Vector3.forward, Mathf.Lerp(-1f, 1f, l_roll.alpha) * TurnSpeed * Time.fixedDeltaTime);

        float speedMult = Mathf.Lerp(1f, 5f, l_throttle_mult.alpha);
        CurrentSpeed = Mathf.Lerp(-Speed * speedMult, Speed * speedMult, l_throttle.alpha);

        movementAlpha = Mathf.Max(
            Mathf.Max(Mathf.Abs((l_pitch.alpha - 0.5f) * 2f), Mathf.Abs((l_yaw.alpha - 0.5f) * 2f)),
            Mathf.Max(Mathf.Abs((l_roll.alpha - 0.5f) * 2f), Mathf.Abs((l_throttle.alpha - 0.5f) * 2f) * ((l_throttle_mult.alpha + 1))));
        movementAlpha /= 2f;

        engine.volume = Mathf.Lerp(0.5f, 0.1f, movementAlpha);
        engine.pitch = Mathf.Lerp(0.25f, 2f, movementAlpha);


        World.transform.Translate((-Vector3.forward * CurrentSpeed) * Time.fixedDeltaTime, Space.World);

        RenderSettings.skybox.SetVector("_Up", Vector3.up);
        RenderSettings.skybox.SetVector("_Right", Vector3.right);
        RenderSettings.skybox.SetVector("_Forward", Vector3.forward);

        RenderSettings.skybox.SetVector("_Rotation", new Vector3(-World.transform.eulerAngles.y, -World.transform.eulerAngles.x, -World.transform.eulerAngles.z));
    }
    
    void ToggleInsideLights()
    {
        insideLight.SetActive(!insideLight.activeSelf);
    }
}
