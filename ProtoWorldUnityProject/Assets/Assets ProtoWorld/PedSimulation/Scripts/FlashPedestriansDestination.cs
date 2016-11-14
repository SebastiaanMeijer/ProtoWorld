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
		LoggableManager.subscribe((Loggable)this);
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

	public NTree<KeyValuePair<string,string>> getLogData(){
		NTree<KeyValuePair<string,string>> logData = new NTree<KeyValuePair<string, string>> (new KeyValuePair<string, string>(tag,null));
		logData.AddChild(new KeyValuePair<string, string>("Name",destinationName));
		logData.AddChild(new KeyValuePair<string, string>("PositionX",destinationTransform.position.x.ToString()));
		logData.AddChild(new KeyValuePair<string, string>("PositionY",destinationTransform.position.y.ToString()));
		logData.AddChild(new KeyValuePair<string, string>("PositionZ",destinationTransform.position.z.ToString()));
		logData.AddChild(new KeyValuePair<string, string>("CheckRadius",radiousToCheckStations.ToString()));
		logData.AddChild(new KeyValuePair<string, string>("Priority",destinationPriority.ToString()));
		return logData;
	}

	public void rebuildFromLog(NTree<KeyValuePair<string,string>> logData){
		GameObject flashDestinationObject = GameObject.Instantiate(gameObject) as GameObject;
		FlashPedestriansDestination flashDestinationScript = flashDestinationObject.GetComponent<FlashPedestriansDestination>();
		Vector3 position = new Vector3();

		flashDestinationScript.destinationName = logData.GetChild (1).data.Value;
		position.x = float.Parse(logData.GetChild(2).data.Value);
		position.y = float.Parse(logData.GetChild(3).data.Value);
		position.z = float.Parse(logData.GetChild(4).data.Value);
		flashDestinationScript.radiousToCheckStations = float.Parse(logData.GetChild(5).data.Value);
		flashDestinationScript.destinationPriority = float.Parse(logData.GetChild(6).data.Value);
		flashDestinationObject.transform.parent = GameObject.Find("DestinationPoints").transform;
		flashDestinationObject.name = "FlashDestination";
		flashDestinationScript.destinationTransform.position = position;

		flashDestinationScript.initializeDestination();
		flashDestinationScript.enabled = true;
	}

    public int getPriorityLevel()
    {
        return 1;
    }
}
