using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// The ObjectLifeTime class is used along with simulation scripts to control the lifetime of dynamic objects such as cars.
/// <para>Generally, one would use this on simulations that do not specify when to remove the objects.</para>
/// </summary>
public class ObjectLifeTime : MonoBehaviour {

    private DateTime _LastUpdate;
    public DateTime LastUpdate
    {
        get { return _LastUpdate; }
        set 
        { 
            _LastUpdate = value;
            DateInString = _LastUpdate.ToString();
        }
    }
    public String DateInString;
    
}
