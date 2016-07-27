/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;

public class AnimateTiledTexture : MonoBehaviour
{
    private float iX=0;
    private float iY=1;
    public int _uvTieX = 1;
    public int _uvTieY = 1;
    public int _fps = 10;
    private Vector2 _size;
    private Renderer _myRenderer;
    private int _lastIndex = -1;
 
    void Start ()
    {
        _size = new Vector2 (1.0f / _uvTieX ,
                             1.0f / _uvTieY);
 
        _myRenderer = GetComponent<Renderer>();
 
        if(_myRenderer == null) enabled = false;
 
        _myRenderer.material.SetTextureScale("_MainTex", _size);
    }
 
    void Update()
    {
        int index = (int)(Time.timeSinceLevelLoad * _fps) % (_uvTieX * _uvTieY);
 
        if(index != _lastIndex)
        {
            Vector2 offset = new Vector2(iX*_size.x,
                                         1-(_size.y*iY));
            iX++;
            if(iX / _uvTieX == 1)
            {
                if(_uvTieY!=1)    iY++;
                iX=0;
                if(iY / _uvTieY == 1)
                {
                    iY=1;
                }
            }
 
            _myRenderer.material.SetTextureOffset("_MainTex", offset);
 
 
            _lastIndex = index;
        }
    }
}