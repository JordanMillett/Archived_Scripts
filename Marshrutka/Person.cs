using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Person", menuName = "Person")]
public class Person : ScriptableObject
{
    public Voice V;
    public GameObject Model;
}
