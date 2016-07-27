/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
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
