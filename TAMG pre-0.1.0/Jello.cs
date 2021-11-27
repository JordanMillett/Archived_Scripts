  
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Jello
{
    public class Tools
    {
        public static Vector3 GetOffsetVector(float accuracy)
        {

            float inaccuracy = 100f - accuracy;

            Vector3 offsetVector = new Vector3
            (
                Random.Range(-inaccuracy, inaccuracy), 
                Random.Range(-inaccuracy, inaccuracy), 
                Random.Range(-inaccuracy, inaccuracy)
            );

            offsetVector /= 100f;

            return offsetVector;
        }
    }
}