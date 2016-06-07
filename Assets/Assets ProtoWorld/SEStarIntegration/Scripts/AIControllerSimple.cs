using UnityEngine;
using System.Collections;

public class AIControllerSimple : MonoBehaviour
{
    private Camera mcamera;
    public static float VicinityDistance = 15f;
    private bool _objectInVicinity;
    private bool _OldobjectInVicinity;
    public delegate void VicinityEventHandler(VicinityChangeEventArgs e);
    public event VicinityEventHandler VicinityChanged;
    public bool ObjectInVicinity
    { get { return _objectInVicinity; } }
    // Use this for initialization
    void Start()
    {
        mcamera = Camera.main;
        _OldobjectInVicinity=_objectInVicinity;
    }


    void Update()
    {
        var distance = Vector3.Distance(transform.position, mcamera.transform.position);
        if (distance < VicinityDistance)
        {
            _objectInVicinity = true;
            if (_OldobjectInVicinity!= _objectInVicinity)
            {
                _OldobjectInVicinity= _objectInVicinity;
                if (VicinityChanged!=null)
                VicinityChanged(new VicinityChangeEventArgs(){ ObjectInVicinity=_objectInVicinity, DetectedDistance=distance, VicinityDistanceThreshold=VicinityDistance});
            }
        }
        else
        {
            _objectInVicinity = false;
            if (_OldobjectInVicinity!= _objectInVicinity)
            {
                _OldobjectInVicinity= _objectInVicinity;
                if (VicinityChanged != null)
                    VicinityChanged(new VicinityChangeEventArgs() { ObjectInVicinity = _objectInVicinity, DetectedDistance = distance, VicinityDistanceThreshold = VicinityDistance });
            }
        }

    }

    public class VicinityChangeEventArgs : System.EventArgs
    {
        public bool ObjectInVicinity { get; set; }
        public float DetectedDistance { get; set; }
        public float VicinityDistanceThreshold { get; set; }
    }
}
