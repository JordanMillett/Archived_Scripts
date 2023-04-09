using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public List<Room> Owners;
    public Door _Door;
    
    void Start()
    {
        if (_Door)
            _Door.W = this;
        
        UIManager.UI.M_Game.E_Minimap.WriteWall(new Vector2(this.transform.position.x + 9f, this.transform.position.z + 9f), _Door, this.transform.localEulerAngles.y != 0);
    }
}
