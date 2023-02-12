using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
[System.Serializable]
public class ActorData
{
    public string _name = "Uninitialized";
    public int _level = 1;
    public Faction _faction = Faction.RAID;
    public GameObject _model = null;
} 

public class Actor : MonoBehaviour
{
    public ActorData _actorData;
    public GameObject Model;

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        if(!_actorData._model)
            return;

        this.gameObject.name = _actorData._faction.ToString() + "_" + _actorData._level + "_" + _actorData._name;
        
        Model = GameObject.Instantiate(_actorData._model, this.transform.position, Quaternion.identity);
        Model.transform.SetParent(this.transform);
    }
}*/
