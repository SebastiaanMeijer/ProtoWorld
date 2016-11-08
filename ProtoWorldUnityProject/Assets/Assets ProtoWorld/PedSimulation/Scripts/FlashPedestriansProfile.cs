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

    public FlashPedestriansProfile(float speed, bool englishSpeaker, bool italianSpeaker, 
        float chanceOfSubscription, bool willingToChangeDestination, float chanceOfTakingABike, 
        float chanceOfBelievingRumours, bool carAwareness, TravelPreference preference = TravelPreference.time)
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
}
