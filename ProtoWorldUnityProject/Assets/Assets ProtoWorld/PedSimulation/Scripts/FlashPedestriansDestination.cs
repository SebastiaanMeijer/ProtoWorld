/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
* 
* FLASH PEDESTRIAN SIMULATOR
* FlashPedestriansDestination.cs
* Miguel Ramos Carretero
* 
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Class that defines a destination entry for Flash Pedestrians (composed of a transform, 
/// a float priority and a array of colliders representing the stations nearby). 
/// </summary>
public class FlashPedestriansDestination : MonoBehaviour, Loggable
{
    public string destinationName;

    public bool hideInUI = false;

    [HideInInspector]
    public Transform destinationTransform;

    [Range(0, 10)]
    public float destinationPriority;

    [HideInInspector]
    public Collider[] stationsNearThisDestination;

    [Range(0, 10000)]
    public float radiousToCheckStations = 100;

    void Awake()
    {
        initializeDestination();
		LoggableManager.subscribe(this);
    }

    public void initializeDestination()
    {
        destinationTransform = this.transform;

        FlashPedestriansGlobalParameters pedGlobalParameters = GetComponent<FlashPedestriansGlobalParameters>();

        // Get the stations near this destination point
        stationsNearThisDestination = Physics.OverlapSphere(destinationTransform.position, radiousToCheckStations, 1 << LayerMask.NameToLayer("Stations"));

        //Debug.Log(this.gameObject.name + " has found " + stationsNearThisDestination.Length 
        //+ " stations nearby");
    }

	public LogDataTree getLogData(){
		LogDataTree logData = new LogDataTree (tag,null);
		logData.AddChild(new LogDataTree("Name",destinationName));
		logData.AddChild(new LogDataTree("PositionX",destinationTransform.position.x.ToString()));
		logData.AddChild(new LogDataTree("PositionY",destinationTransform.position.y.ToString()));
		logData.AddChild(new LogDataTree("PositionZ",destinationTransform.position.z.ToString()));
		logData.AddChild(new LogDataTree("CheckRadius",radiousToCheckStations.ToString()));
		logData.AddChild(new LogDataTree("Priority",destinationPriority.ToString()));
		return logData;
	}

	public void rebuildFromLog(LogDataTree logData){
		GameObject flashDestinationObject = null;
        FlashPedestriansDestination flashDestinationScript = new FlashPedestriansDestination();
        foreach (Loggable destination in LoggableManager.getCurrentSubscribedLoggables())
        {
            if (destination == null)
                continue;

            if (((MonoBehaviour)destination).gameObject.tag == "PedestrianDestination")
            {
                //get the destination that needs to be altered and modify it.
                flashDestinationScript.destinationName = logData.GetChild("Name").Value;
                if (((MonoBehaviour)destination).GetComponent<FlashPedestriansDestination>().destinationName == flashDestinationScript.destinationName)
                {
                    flashDestinationObject = ((MonoBehaviour)destination).gameObject;
                    flashDestinationScript = flashDestinationObject.GetComponent<FlashPedestriansDestination>();

                    Vector3 position = new Vector3();
                    position.x = float.Parse(logData.GetChild("PositionX").Value);
                    position.y = float.Parse(logData.GetChild("PositionY").Value);
                    position.z = float.Parse(logData.GetChild("PositionZ").Value);
                    flashDestinationScript.radiousToCheckStations = float.Parse(logData.GetChild("CheckRadius").Value);
                    flashDestinationScript.destinationPriority = float.Parse(logData.GetChild("Priority").Value);
                    flashDestinationObject.name = "FlashDestination";
                    flashDestinationScript.destinationTransform.position = position;

                    flashDestinationScript.initializeDestination();
                    flashDestinationScript.enabled = true;
                }
            }
        }
    }

	public  LogPriorities getPriorityLevel()
	{
		return LogPriorities.High;
	}

	public bool destroyOnLogLoad(){
		return false;
	}
}
