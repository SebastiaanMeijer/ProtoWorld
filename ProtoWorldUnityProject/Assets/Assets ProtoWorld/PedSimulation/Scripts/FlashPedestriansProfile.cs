/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * FLASH PEDESTRIAN SIMULATOR
 * FlashPedestriansProfile.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlashPedestriansProfile 
{
    public float speed;
    public bool englishSpeaker;
    public bool italianSpeaker;
    public float chanceOfSubscription;
    public float chanceOfBelievingRumours;
    public bool willingToChangeDestination;
    public float chanceOfTakingABike;
    public float weatherFactorOnTakingBikes;
    public bool carAwareness;
    public TravelPreference travelPreference;

    public FlashPedestriansProfile(float speed, bool englishSpeaker, bool italianSpeaker, float chanceOfSubscription, bool willingToChangeDestination, float chanceOfTakingABike, float chanceOfBelievingRumours, bool carAwareness, TravelPreference preference)
    {
        this.speed = speed;
        this.englishSpeaker = englishSpeaker;
        this.italianSpeaker = italianSpeaker;
        this.chanceOfSubscription = chanceOfSubscription;
        this.willingToChangeDestination = willingToChangeDestination;
        this.chanceOfTakingABike = chanceOfTakingABike;
        this.weatherFactorOnTakingBikes = 1.0f;
        this.chanceOfBelievingRumours = chanceOfBelievingRumours;
        this.carAwareness = carAwareness;
        this.travelPreference = preference;
    }

	public Dictionary<string,string> getLogData()
	{
		Dictionary<string,string> structuredData = new Dictionary<string,string> ();
		structuredData.Add ("speed", speed.ToString());
		structuredData.Add ("englishSpeaker", englishSpeaker.ToString());
		structuredData.Add ("italianSpeaker", italianSpeaker.ToString());
		structuredData.Add ("chanceOfSubscription", chanceOfSubscription.ToString());
		structuredData.Add ("willingToChangeDestination", willingToChangeDestination.ToString());
		structuredData.Add ("chanceOfTakingABike", chanceOfTakingABike.ToString());
		structuredData.Add ("weatherFactorOnTakingBikes", weatherFactorOnTakingBikes.ToString());
		structuredData.Add ("chanceOfBelievingRumours", chanceOfBelievingRumours.ToString());
		structuredData.Add ("carAwareness", carAwareness.ToString());
		structuredData.Add ("travelPreference", travelPreference.ToString());
		return structuredData;
	}

	public void rebuildFromLog(Dictionary<string, string> logData){
		float speed = float.Parse(logData["speed"].ToString());
		bool englishSpeaker = bool.Parse(logData["englishSpeaker"].ToString());
		bool italianSpeaker = bool.Parse(logData["italianSpeaker"].ToString());
		float chanceOfSubscription = float.Parse(logData["chanceOfSubscription"].ToString());
		bool willingToChangeDestination = bool.Parse(logData["willingToChangeDestination"].ToString());
		float chanceOfTakingABike = float.Parse(logData["chanceOfTakingABike"].ToString());
		float weatherFactorOnTakingBikes = float.Parse(logData["weatherFactorOnTakingBikes"].ToString());
		float chanceOfBelievingRumours = float.Parse(logData["chanceOfBelievingRumours"].ToString());
		bool carAwareness = bool.Parse(logData["carAwareness"].ToString());
		TravelPreference travelPreference = new TravelPreference();

		FlashPedestriansProfile profile = new FlashPedestriansProfile(speed, englishSpeaker, italianSpeaker, chanceOfSubscription, willingToChangeDestination, chanceOfTakingABike,
			chanceOfBelievingRumours, carAwareness, travelPreference);
	}
}
