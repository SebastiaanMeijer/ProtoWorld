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

/// <summary>
/// Class that defines a destination entry for Flash Pedestrians (composed of a transform, 
/// a float priority and a array of colliders representing the stations nearby). 
/// </summary>
public class FlashPedestriansDestination : MonoBehaviour, LogObject
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
        destinationTransform = this.transform;

        FlashPedestriansGlobalParameters pedGlobalParameters = GetComponent<FlashPedestriansGlobalParameters>();

        // Get the stations near this destination point
        stationsNearThisDestination = Physics.OverlapSphere(destinationTransform.position, radiousToCheckStations, 1 << LayerMask.NameToLayer("Stations"));

        //Debug.Log(this.gameObject.name + " has found " + stationsNearThisDestination.Length 
            //+ " stations nearby");
    }

	public Dictionary<string, Dictionary<string, string>> getLogData(){
		Dictionary<string, Dictionary<string, string>>logData = new Dictionary<string,Dictionary<string,string>> ();
		logData.Add (tag, new Dictionary<string,string> ());
		logData [tag].Add("Name", destinationName);
		logData [tag].Add("PositionX", destinationTransform.position.x.ToString());
		logData [tag].Add("PositionY", destinationTransform.position.y.ToString());
		logData [tag].Add("PositionZ", destinationTransform.position.z.ToString());
		logData [tag].Add("CheckRadius", radiousToCheckStations.ToString());
		logData [tag].Add("Priority", destinationPriority.ToString());
		return logData;
	}

	public void rebuildFromLog(Dictionary<string, Dictionary<string, string>> logData){
		GameObject flashDestination = new GameObject();
		flashDestination.AddComponent<FlashPedestriansDestination>();
		FlashPedestriansDestination destination = flashDestination.GetComponent<FlashPedestriansDestination>();
		Vector3 position = new Vector3();

		position.x = float.Parse(logData[tag]["PositionX"].ToString());
		position.y = float.Parse(logData[tag]["PositionY"].ToString());
		position.z = float.Parse(logData[tag]["PositionZ"].ToString());
		destination.destinationName = logData[tag]["Name"].ToString();

		flashDestination.transform.parent = GameObject.Find("DestinationPoints").transform;
		flashDestination.name = "FlashDestination";
		destination.destinationTransform.position = position;
		destination.transform.position = position;
	}
}
