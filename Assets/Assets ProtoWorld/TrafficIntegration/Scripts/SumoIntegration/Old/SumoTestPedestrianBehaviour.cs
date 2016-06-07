/*
 * 
 * SUMO COMMUNICATION
 * SumoTestPedestrianBehaviour.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// This class defines the basic controller of a default pedestrian for testing the interaction
/// pedestrian-vehicle in Unity.
/// </summary>
public class SumoTestPedestrianBehaviour : MonoBehaviour
{
    public float speed = 5.0f;

    /// <summary>
    /// Updates the position of the pedestrian according to the arrow keys.
    /// </summary>
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position =
                new Vector3(transform.position.x, 
                    transform.position.y, transform.position.z + speed*Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position =
                new Vector3(transform.position.x,
                    transform.position.y, transform.position.z - speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position =
                new Vector3(transform.position.x + speed * Time.deltaTime, 
                    transform.position.y, transform.position.z);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position =
                new Vector3(transform.position.x - speed * Time.deltaTime, 
                    transform.position.y, transform.position.z);
        }
    }
}
