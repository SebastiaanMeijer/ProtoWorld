/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;

public class CoordinateConversionGUI : MonoBehaviour
{

	public UnityEngine.UI.Text SourceLat;
	public UnityEngine.UI.Text SourceLon;
	public UnityEngine.UI.Text TargetDisplay;
	public UnityEngine.UI.Toggle GoToLocation;
	public float CameraDistanceFromThePoint = 3;
	// Use this for initialization
	void Start()
	{
		ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);
		var go = GameObject.Find("AramGISBoundaries");
		var mapboundaries = go.GetComponent<MapBoundaries>();
		CoordinateConvertor.Initialize(client, mapboundaries);
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void OnCoordinateConversionButtonClick()
	{
		if (!string.IsNullOrEmpty(SourceLat.text) && !string.IsNullOrEmpty(SourceLon.text))
		{
			string latstring = SourceLat.text.Trim();
			string lonstring = SourceLon.text.Trim();
			double lat = 0;
			double lon = 0;
			if (double.TryParse(latstring, out lat) && double.TryParse(lonstring, out lon))
			{
				var vector3 = CoordinateConvertor.LatLonToVector3(lat, lon);
				TargetDisplay.text = vector3.ToString();
				if (GoToLocation.isOn)
				{
					// Transform the camera
					//Camera.main.transform.position
					Camera.main.transform.position = vector3 + new Vector3(CameraDistanceFromThePoint, 1, CameraDistanceFromThePoint);
					Camera.main.transform.LookAt(vector3);
					Camera.main.GetComponent<CameraControl>().targetCameraPosition = new Vector3(vector3.x + CameraDistanceFromThePoint, 2, vector3.z + CameraDistanceFromThePoint);
				}
			}
			else
				TargetDisplay.text = "Invalid input!";
		}
	}

}
