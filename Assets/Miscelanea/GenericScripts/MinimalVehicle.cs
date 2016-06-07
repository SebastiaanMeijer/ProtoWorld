using UnityEngine;
using System.Collections;

public class MinimalVehicle
{
    public string Id;
    public float Latitude;
    public float Longitude;
    public bool found = false;
    public Vector3 TempVec;
    public MinimalVehicle() { }
    public MinimalVehicle(string id, float lat, float lon)
    {
        this.Id = id;
        this.Latitude = lat;
        this.Longitude = lon;
    }
}

