/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;

public class ShowQueNumbers : MonoBehaviour {

	public UnityEngine.UI.Text queTrainToArnhem;
	public UnityEngine.UI.Text queRepToUtrecht1;
	public UnityEngine.UI.Text queRepToUtrecht2;
	public UnityEngine.UI.Text queRegToUtrecht;

	public StationStatistics stationStatistics;

	public int queToArnhem;
	public int queToUtrechtReg;
	public int queToUtrechtRep1;
	public int queToUtrechtRep2;


	// Use this for initialization
	void Awake () {
		stationStatistics = GameObject.Find("Stations").GetComponent<StationStatistics>();
	}
	
	// Update is called once per frame
	void Update () {

		queToArnhem = stationStatistics.QuePerStation[0];
		queToUtrechtReg = stationStatistics.QuePerStation[6];
		queToUtrechtRep1 = stationStatistics.QuePerStation[7];
		queToUtrechtRep2 = stationStatistics.QuePerStation[2];

		queTrainToArnhem.text = "" +queToArnhem;
		queRegToUtrecht.text = ""+queToUtrechtReg;
		queRepToUtrecht1.text = ""+(queToUtrechtRep1 + queToUtrechtRep2);
		queRepToUtrecht2.text = ""+(queToUtrechtRep1 + queToUtrechtRep2);

	
	}
}
