using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
 public class FireLight : MonoBehaviour
 {
     public float minIntensity = 30f;
     public float maxIntensity = 50f;
	 //Light lt;
 
     float random;
 
     void Start()
     {

		// Light lt = this.gameObject.GetComponent<Light>();
         random = Random.Range(0.0f, 65535.0f);
     }
 
     void Update()
     {
//         float noise = Mathf.PerlinNoise(random, Time.time);
         //lt.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
     }
 }