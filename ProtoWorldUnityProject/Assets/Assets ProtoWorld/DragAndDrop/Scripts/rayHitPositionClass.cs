/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * DRAG AND DROP SYSTEM 
 * Furkan Sonmez
 * 
 */

using UnityEngine;
using System.Collections;

public class rayHitPositionClass : MonoBehaviour
{
    //This is the vector3 where the ray hit location will be stored into
    public static Vector3 hitLocation;

    //this is a static bool which states if the user is holding the mousebutton or not
    public static bool dragging = false;

    //this bool will be used to check if the game has started or not, this kicks in after 0.5 seconds after the game has been started
    public static bool gameStartedBool = false;
	public Camera mainCam;

    void Start()
    {
		if (mainCam == null) {
			mainCam = Camera.main;
		}
        StartCoroutine(GameStarted());
    }

    // this method is used to not drag all the objects that had already been dropped into the simulation before the simulation had started
    public IEnumerator GameStarted()
    {
        yield return new WaitForSeconds(0.5f);
        gameStartedBool = true;

    }

    void Update()
    {
        // whenever you click anywhere this will happen and you will be in the state of "dragging"
        if (Input.GetMouseButtonDown(0))
        {
            dragging = true;
        }

        // this will end the state of "dragging"
        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }


        if (dragging == true)
        {
            RaycastHit hit;

			Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

            // only if the ray hits an object(other than the objects with layer 13(the objects that are added)) the rest of the code will follow
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 13))
                if (hit.collider != null)
                {

                    // this will store the point where anything is hit in the static Vector3 hitLocation. So use rayHitPositionClass.hitLocation to use this Vector3
                    hitLocation = hit.point;
                }
        }
    }
}
