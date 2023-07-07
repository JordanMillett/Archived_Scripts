using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleScreen : MonoBehaviour
{
    public Console C;
    
    void OnEnable()
    {
        C.Activate();
    }
}
