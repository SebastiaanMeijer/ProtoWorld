/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Implements the awareness of a pedestrian.
/// </summary>
public class PedestrianObjectAwareness : MonoBehaviour
{
    public float timeBetweenCollisions = 1.0f;
    public bool tellMeWhatYouAreDoing = false;
    public float collisionTimer = 0.0f;
    public bool collided = false;
    private bool onRoad = false;

    /// <summary>
    /// Awake the script.
    /// </summary>
    void Awake()
    {
        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();
        if (GetComponent<Rigidbody>() == null)
            gameObject.AddComponent<Rigidbody>();
        if (!GetComponent<Rigidbody>().isKinematic)
        {
            Debug.LogWarning("Object awareness only work if the rigidbody is kinematic");
            GetComponent<Rigidbody>().isKinematic = true;
        }
        if (!GetComponent<Collider>().isTrigger)
        {
            Debug.LogWarning("Object awareness only work if the collider is trigger");
            GetComponent<Collider>().isTrigger = true;
        }
    }

    /// <summary>
    /// Updates.
    /// </summary>
    void Update()
    {
        if (collisionTimer > 0.0f)
            collisionTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Handles the collision when a pedestrian enters in a traffic road. 
    /// </summary>
    /// <param name="col">Collider with the information of the collision</param>
    private void OnTriggerEnter(Collider col)
    {
        onRoad = true; 
        if (tellMeWhatYouAreDoing)
            Debug.Log(this.name + ": I am on road ");
    }

    /// <summary>
    /// Handles the collision when a pedestrian enters in a traffic road. This method checks for vehicles
    /// around and informs them about the position of the pedestrian. 
    /// </summary>
    /// <param name="col">Collider with the information of the collision</param>
    private void OnTriggerStay(Collider col)
    {
        if (collisionTimer <= 0.0f)
        {
            if (tellMeWhatYouAreDoing)
                Debug.Log(this.name + ": I got a collision with " + col.name);

            collisionTimer = timeBetweenCollisions;

            Collider[] vehiclesAround = Physics.OverlapSphere(this.transform.position, 20.0f, LayerMask.GetMask("Vehicle"));

            if (tellMeWhatYouAreDoing)
                Debug.Log(this.name + ": There are " + vehiclesAround.Length + " vehicles inside the sphere");

            collided = (vehiclesAround.Length != 0);

            foreach (var v in vehiclesAround)
            {
                if (tellMeWhatYouAreDoing)
                    Debug.Log("Sending message to vehicle " + v.name);

                v.SendMessage("PedestrianCrossingRoad", this.transform.position);
            }
        }
    }

    /// <summary>
    /// Handles the collision when a pedestrian exits a traffic road. 
    /// </summary>
    /// <param name="col">Collider with the information of the collision</param>
    private void OnTriggerExit(Collider col)
    {
        onRoad = false;
        if (tellMeWhatYouAreDoing)
            Debug.Log(this.name + ": I am not on road anymore ");
    }

    /// <summary>
    /// Draw a gizmo in the pedestrian informing for cars around it. 
    /// </summary>
    void OnDrawGizmos()
    {
        if (onRoad && collided)
            Gizmos.DrawWireSphere(this.transform.position, 20.0f);
    }
}