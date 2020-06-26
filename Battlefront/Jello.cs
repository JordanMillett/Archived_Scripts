using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

namespace Jello
{
    public class Tools
    {
        static string namesPath = "Assets/Resources/Jello/names.txt";

        public static bool RandomBool()
        {
            int ran = Random.Range(0, 2);

            if(ran == 0)
                return false;
            else
                return true;
        }
        
        public static string RandomName()
        {

            StreamReader reader = new StreamReader(namesPath); 
            
            string[] lines = reader.ReadToEnd().Split("\n"[0]);
            string name = lines[Random.Range(0, lines.Length)];

            reader.Close();

            return name;

        }

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